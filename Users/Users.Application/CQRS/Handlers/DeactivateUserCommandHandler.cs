using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.CQRS.Commands;
using Users.Application.DTOs;
using Users.Application.Events;
using Users.Application.Exceptions;
using Users.Application.Interfaces.Messaging;
using Users.Application.Interfaces.Repositories;

namespace Users.Application.CQRS.Handlers
{
    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageBus _messageBus;

        public DeactivateUserCommandHandler(IUserRepository userRepository, IMessageBus messageBus)
        {
            _userRepository = userRepository;
            _messageBus = messageBus;
        }

        public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                throw new NotFoundException("User not found");

            if (user.IsActive)
            {
                user.IsActive = false;
                await _userRepository.UpdateAsync(user);
                var deactivatedEvent = new UserDeactivatedEvent(user.Id, System.DateTime.UtcNow);
                _messageBus.Publish(deactivatedEvent, "user_deactivated");
            }
        }
    }
}