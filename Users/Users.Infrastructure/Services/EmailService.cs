using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Users.Application.Interfaces.Services;
using Users.Infrastructure.Models;

namespace Users.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ApplicationSettings _appSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<SmtpSettings> smtpSettings,
            IOptions<ApplicationSettings> appSettings,
            ILogger<EmailService> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _appSettings = appSettings.Value;
            _logger = logger;

            ValidateSmtpSettings();
        }

        private void ValidateSmtpSettings()
        {
            if (string.IsNullOrEmpty(_smtpSettings.Server))
                throw new ArgumentException("SMTP server is not configured");

            if (string.IsNullOrEmpty(_smtpSettings.Username))
                throw new ArgumentException("SMTP username is not configured");

            if (string.IsNullOrEmpty(_smtpSettings.Password))
                throw new ArgumentException("SMTP password is not configured");
        }

        public async Task SendPasswordRecoveryEmail(string email, string token)
        {
            var resetLink = $"{_appSettings.BaseUrl}/{_appSettings.ResetPasswordPath}?token={WebUtility.UrlEncode(token)}&email={WebUtility.UrlEncode(email)}";

            var tokenLifetimeHours = 1;
            var expiryTime = DateTime.UtcNow.AddHours(tokenLifetimeHours);

            var subject = "Password Recovery";
            var body = CreatePasswordRecoveryEmailBody(resetLink, expiryTime);

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailConfirmationEmail(string email, string token)
        {
            var confirmationLink = $"{_appSettings.BaseUrl}/{_appSettings.ConfirmEmailPath}?token={WebUtility.UrlEncode(token)}&email={WebUtility.UrlEncode(email)}";

            var tokenLifetimeHours = 24;
            var expiryTime = DateTime.UtcNow.AddHours(tokenLifetimeHours);

            var subject = "Email Confirmation";
            var body = CreateEmailConfirmationBody(confirmationLink, expiryTime);

            await SendEmailAsync(email, subject, body);
        }

        private string CreatePasswordRecoveryEmailBody(string resetLink, DateTime expiryTime)
        {
            return $@"
Password Recovery

You received this email because a password reset was requested for your account.

To reset your password, click the link below:
{resetLink}

This link is valid until: {expiryTime:yyyy-MM-dd HH:mm:ss} UTC

If you did not request a password reset, please ignore this email.

Best regards,
Support Team";
        }

        private string CreateEmailConfirmationBody(string confirmationLink, DateTime expiryTime)
        {
            return $@"
Email Confirmation

Thank you for registering! To complete your registration, please confirm your email address.

To confirm your email, click the link below:
{confirmationLink}

This link is valid until: {expiryTime:yyyy-MM-dd HH:mm:ss} UTC

Best regards,
Support Team";
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                _logger.LogInformation("Attempting to send email to {Email}", toEmail);

                using var client = CreateSmtpClient();
                using var message = CreateMailMessage(toEmail, subject, body);

                await client.SendMailAsync(message);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
            {
                EnableSsl = _smtpSettings.EnableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000
            };
        }

        private MailMessage CreateMailMessage(string toEmail, string subject, string body)
        {
            return new MailMessage
            {
                From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                To = { toEmail },
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
                BodyEncoding = System.Text.Encoding.UTF8,
                SubjectEncoding = System.Text.Encoding.UTF8
            };
        }
    }
}