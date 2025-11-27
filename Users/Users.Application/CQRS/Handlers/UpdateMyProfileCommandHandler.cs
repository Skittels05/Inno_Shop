using AutoMapper;
using MediatR;
using Users.Application.CQRS.Commands;
using Users.Application.DTOs;
using Users.Application.Exceptions;
using Users.Application.Interfaces.Repositories;
using Users.Application.Interfaces.Services;
using Users.Domain.Entities;

namespace Users.Application.CQRS.Handlers;

public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public UpdateMyProfileCommandHandler(
        IUserRepository userRepository,
        IEmailService emailService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException("User not found");

        if (!string.IsNullOrEmpty(request.Dto.Name) &&
            !request.Dto.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase))
        {
            var existingUserByName = await _userRepository.GetByNameAsync(request.Dto.Name);
            if (existingUserByName != null && existingUserByName.Id != user.Id)
                throw new BadRequestException("Name already in use");

            user.Name = request.Dto.Name;
        }

        if (!string.IsNullOrEmpty(request.Dto.Email) &&
            !request.Dto.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Dto.Email, false);
            if (existingUserByEmail != null && existingUserByEmail.Id != user.Id)
                throw new BadRequestException("Email already in use");

            user.Email = request.Dto.Email;
            user.EmailConfirmed = false;
            user.EmailConfirmationToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            user.EmailConfirmationTokenExpiration = DateTime.UtcNow.AddHours(24);

            await _emailService.SendEmailConfirmationEmail(user.Email, user.EmailConfirmationToken);
        }
        await _userRepository.UpdateAsync(user);

        return Unit.Value;
    }
}
