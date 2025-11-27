using MediatR;
using Users.Application.DTOs;

namespace Users.Application.CQRS.Commands;

public record UpdateMyProfileCommand(
    Guid UserId,
    UpdateMyProfileDto Dto
) : IRequest<Unit>;