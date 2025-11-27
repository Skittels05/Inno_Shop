using MediatR;
using Users.Application.CQRS.Commands;
using Users.Application.Exceptions;
using Users.Application.Interfaces.Repositories;

namespace Users.Application.CQRS.Handlers
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public ConfirmEmailCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, false);

            if (user == null)
                throw new NotFoundException("User not found");

            if (user.EmailConfirmationToken != request.Token ||
                user.EmailConfirmationTokenExpiration < DateTime.UtcNow)
            {
                throw new BadRequestException("Invalid or expired token");
            }

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpiration = null;

            await _userRepository.UpdateAsync(user);

            return true;
        }
    }
}