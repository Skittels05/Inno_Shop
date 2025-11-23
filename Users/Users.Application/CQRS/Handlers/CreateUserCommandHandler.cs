using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.CQRS.Commands;
using Users.Application.Interfaces.Security;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.CQRS.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(request);
            user.PasswordHash = _passwordHasher.Hash(request.Password);

            await _userRepository.CreateAsync(user);
            return user.Id;
        }
    }
}
