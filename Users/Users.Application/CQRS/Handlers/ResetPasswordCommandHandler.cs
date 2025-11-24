using MediatR;
using Users.Application.CQRS.Commands;
using Users.Application.Exceptions;
using Users.Application.Interfaces;
using Users.Application.Interfaces.Identity;
using Users.Application.Interfaces.Repositories;

namespace Users.Application.CQRS.Handlers
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public ResetPasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.ResetDto.Email, true); 

            if (user == null)
                throw new BadRequestException("Invalid or expired recovery token");

            if (user.PasswordRecoveryToken != request.ResetDto.Token ||
                user.PasswordRecoveryTokenExpiration < DateTime.UtcNow)
            {
                throw new BadRequestException("Invalid or expired recovery token");
            }

            user.PasswordHash = _passwordHasher.Hash(request.ResetDto.NewPassword);
            user.PasswordRecoveryToken = null;
            user.PasswordRecoveryTokenExpiration = null;

            await _userRepository.UpdateAsync(user);

            return Unit.Value;
        }
    }
}