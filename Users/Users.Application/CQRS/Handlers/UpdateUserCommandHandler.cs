using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.CQRS.Commands;
using Users.Application.DTOs;
using Users.Application.Interfaces.Repositories;

namespace Users.Application.CQRS.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null) throw new Exception("User not found");

            _mapper.Map(request.UserDto, user);

            await _userRepository.UpdateAsync(user);
            return Unit.Value;
        }
    }
}
