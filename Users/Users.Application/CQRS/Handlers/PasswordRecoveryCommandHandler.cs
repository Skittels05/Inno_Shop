using MediatR;
using Users.Application.CQRS.Commands;
using Users.Application.Interfaces;
using Users.Application.Interfaces.Repositories;

namespace Users.Application.CQRS.Handlers
{
    public class PasswordRecoveryCommandHandler : IRequestHandler<PasswordRecoveryCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public PasswordRecoveryCommandHandler(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(PasswordRecoveryCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.RecoveryDto.Email, false);
            if (user == null)
                return Unit.Value;

            user.PasswordRecoveryToken = Guid.NewGuid().ToString();
            user.PasswordRecoveryTokenExpiration = DateTime.UtcNow.AddHours(1);

            await _userRepository.UpdateAsync(user);
            await _emailService.SendPasswordRecoveryEmail(user.Email, user.PasswordRecoveryToken);

            return Unit.Value;
        }
    }
}