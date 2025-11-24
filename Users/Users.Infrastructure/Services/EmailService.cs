using Users.Application.Interfaces;
using Users.Application.Interfaces.Services;

namespace Users.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendPasswordRecoveryEmail(string email, string token)
        {
            //реализация...
            Console.WriteLine($"Password recovery token for {email}: {token}");
            await Task.CompletedTask;
        }
    }
}