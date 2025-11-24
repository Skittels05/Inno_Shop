using MediatR;
using Users.Application.CQRS.Commands;
using Users.Application.DTOs;
using Users.Application.Exceptions;
using Users.Application.Interfaces.Identity;
using Users.Application.Interfaces.Repositories;

namespace Users.Application.CQRS.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResultDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResultDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.LoginDto;
            var user = await _userRepository.GetByEmailAsync(dto.Email, false);
            if (user == null)
                throw new BadRequestException("Invalid credentials");
            if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
                throw new BadRequestException("Invalid credentials");
            if (!user.IsActive)
                throw new Exception("Account is inactive");
            if (!user.EmailConfirmed)
                throw new Exception("Email is not confirmed");
            var token = _jwtTokenGenerator.GenerateToken(
                user.Id,
                user.Email,
                user.Role.ToString()
            );

            return new AuthResultDto(
                token,
                user.Id,
                user.Email,
                user.Role.ToString()
            );
        }
    }
}
