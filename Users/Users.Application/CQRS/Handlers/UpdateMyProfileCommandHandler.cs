using AutoMapper;
using MediatR;
using Users.Application.CQRS.Commands;
using Users.Application.Exceptions;
using Users.Application.Interfaces.Repositories;
using Users.Application.Interfaces.Services;

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

        var newEmail = request.Dto.Email;
        bool emailChanged = !string.IsNullOrEmpty(newEmail) &&
                            !newEmail.Equals(user.Email, StringComparison.OrdinalIgnoreCase);

        if (emailChanged)
        {
            var existingUserByEmail = await _userRepository.GetByEmailAsync(newEmail!, false);
            if (existingUserByEmail != null && existingUserByEmail.Id != user.Id)
                throw new BadRequestException("Email already in use");

            user.EmailConfirmed = false;
            user.EmailConfirmationToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            user.EmailConfirmationTokenExpiration = DateTime.UtcNow.AddHours(24);

            await _emailService.SendEmailConfirmationEmail(newEmail!, user.EmailConfirmationToken);
        }

        var newName = request.Dto.Name;
        bool nameChanged = !string.IsNullOrEmpty(newName) &&
                           !newName.Equals(user.Name, StringComparison.OrdinalIgnoreCase);

        if (nameChanged)
        {
            var existingUserByName = await _userRepository.GetByNameAsync(newName!);
            if (existingUserByName != null && existingUserByName.Id != user.Id)
                throw new BadRequestException("Name already in use");
        }

        _mapper.Map(request.Dto, user);

        if (emailChanged)
            user.Email = newEmail!;
        if (nameChanged)
            user.Name = newName!;

        await _userRepository.UpdateAsync(user);

        return Unit.Value;
    }
}
