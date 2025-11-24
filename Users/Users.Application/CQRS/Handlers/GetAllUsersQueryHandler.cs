using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.CQRS.Queries;
using Users.Application.DTOs;
using Users.Application.Interfaces.Repositories;

namespace Users.Application.CQRS.Handlers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _userRepository.FindAll(request.TrackChanges);
            var usersDto = query.ProjectTo<UserDto>(_mapper.ConfigurationProvider);

            return await Task.FromResult(usersDto.ToList());
        }
    }
}
