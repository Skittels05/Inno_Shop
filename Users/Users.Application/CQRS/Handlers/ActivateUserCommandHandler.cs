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
    public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageBus _messageBus;

        public ActivateUserCommandHandler(IUserRepository userRepository, IMessageBus messageBus)
        {
            _userRepository = userRepository;
            _messageBus = messageBus;
        }

        public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                throw new NotFoundException("User not found");

            if (!user.IsActive)
            {
                user.IsActive = true;
                await _userRepository.UpdateAsync(user);
                var activatedEvent = new UserActivatedEvent(user.Id, System.DateTime.UtcNow);
                _messageBus.Publish(activatedEvent, "user_activated");
            }
        }
    }
}