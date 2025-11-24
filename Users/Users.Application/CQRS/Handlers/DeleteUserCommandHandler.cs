using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.CQRS.Commands;
using Users.Application.Exceptions;
using Users.Application.Interfaces.Repositories;

namespace Users.Application.CQRS.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null) throw new NotFoundException("User not found");

            await _userRepository.DeleteAsync(user);
            return Unit.Value;
        }
    }
}
