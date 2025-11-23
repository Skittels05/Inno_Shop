using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.CQRS.Queries;
using Users.Application.DTOs;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.CQRS.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null) throw new Exception("User not found");

            return _mapper.Map<UserDto>(user);
        }
    }
}
