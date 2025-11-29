using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Users.Application.Interfaces.Services;
using Users.Infrastructure.Exceptions;
using Users.Infrastructure.Models;
using Users.Infrastructure.Validators;

namespace Users.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ApplicationSettings _appSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly IValidator<SmtpSettings> _smtpValidator;
        private readonly IValidator<ApplicationSettings> _appSettingsValidator;

        public EmailService(
            IOptions<SmtpSettings> smtpSettings,
            IOptions<ApplicationSettings> appSettings,
            ILogger<EmailService> logger,
            IValidator<SmtpSettings> smtpValidator = null,
            IValidator<ApplicationSettings> appSettingsValidator = null)
        {
            _smtpSettings = smtpSettings.Value;
            _appSettings = appSettings.Value;
            _logger = logger;
            _smtpValidator = smtpValidator;
            _appSettingsValidator = appSettingsValidator;

            ValidateConfiguration();
        }

        private void ValidateConfiguration()
        {
            try
            {
                // Валидация SMTP настроек
                if (_smtpValidator != null)
                {
                    var smtpValidationResult = _smtpValidator.Validate(_smtpSettings);
                    if (!smtpValidationResult.IsValid)
                    {
                        var errors = string.Join(", ", smtpValidationResult.Errors.Select(e => e.ErrorMessage));
                        throw new EmailConfigurationException($"Invalid SMTP configuration: {errors}");
                    }
                }

                // Валидация настроек приложения
                if (_appSettingsValidator != null)
                {
                    var appSettingsValidationResult = _appSettingsValidator.Validate(_appSettings);
                    if (!appSettingsValidationResult.IsValid)
                    {
                        var errors = string.Join(", ", appSettingsValidationResult.Errors.Select(e => e.ErrorMessage));
                        throw new EmailConfigurationException($"Invalid application configuration: {errors}");
                    }
                }

                if (!IsValidEmail(_smtpSettings.SenderEmail))
                {
                    throw new EmailConfigurationException("Invalid sender email format");
                }

                _logger.LogInformation("Email service configuration validated successfully");
            }
            catch (EmailConfigurationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new EmailConfigurationException("Failed to validate email configuration", ex);
            }
        }

        public async Task SendPasswordRecoveryEmail(string email, string token)
        {
            ValidateEmail(email);

            var resetLink = $"{_appSettings.BaseUrl}/{_appSettings.ResetPasswordPath}?token={WebUtility.UrlEncode(token)}&email={WebUtility.UrlEncode(email)}";

            var tokenLifetimeHours = 1;
            var expiryTime = DateTime.UtcNow.AddHours(tokenLifetimeHours);

            var subject = "Password Recovery";
            var body = CreatePasswordRecoveryEmailBody(resetLink, expiryTime);

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendEmailConfirmationEmail(string email, string token)
        {
            ValidateEmail(email);

            var confirmationLink = $"{_appSettings.BaseUrl}/{_appSettings.ConfirmEmailPath}?token={WebUtility.UrlEncode(token)}&email={WebUtility.UrlEncode(email)}";

            var tokenLifetimeHours = 24;
            var expiryTime = DateTime.UtcNow.AddHours(tokenLifetimeHours);

            var subject = "Email Confirmation";
            var body = CreateEmailConfirmationBody(confirmationLink, expiryTime);

            await SendEmailAsync(email, subject, body);
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }

            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Invalid email format", nameof(email));
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
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
                _logger.LogInformation("Attempting to send email to {Email} with subject {Subject}", toEmail, subject);

                using var client = CreateSmtpClient();
                using var message = CreateMailMessage(toEmail, subject, body);

                await client.SendMailAsync(message);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (SmtpException ex)
            {
                var errorMessage = $"SMTP error while sending email to {toEmail}: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                throw new EmailSendingException(toEmail, subject, errorMessage, ex);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Unexpected error while sending email to {toEmail}: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                throw new EmailSendingException(toEmail, subject, errorMessage, ex);
            }
        }

        private SmtpClient CreateSmtpClient()
        {
            try
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
            catch (Exception ex)
            {
                throw new EmailConfigurationException("Failed to create SMTP client", ex);
            }
        }

        private MailMessage CreateMailMessage(string toEmail, string subject, string body)
        {
            try
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
            catch (Exception ex)
            {
                throw new EmailSendingException(toEmail, subject, "Failed to create email message", ex);
            }
        }
    }
}