using MediatR;
using Users.Application.CQRS.Commands;
using Users.Application.Interfaces.Repositories;
using Users.Application.Interfaces.Services;

namespace Users.Application.CQRS.Handlers
{
    public class SendEmailConfirmationCommandHandler : IRequestHandler<SendEmailConfirmationCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public SendEmailConfirmationCommandHandler(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(SendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, false);
            if (user == null || user.EmailConfirmed)
                return Unit.Value;

            user.EmailConfirmationToken = GenerateEmailConfirmationToken();
            user.EmailConfirmationTokenExpiration = DateTime.UtcNow.AddHours(24);

            await _userRepository.UpdateAsync(user);
            await _emailService.SendEmailConfirmationEmail(user.Email, user.EmailConfirmationToken);

            return Unit.Value;
        }

        private string GenerateEmailConfirmationToken()
        {
            return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        }
    }
}