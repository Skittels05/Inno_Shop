namespace Users.Application.DTOs;

public record UpdateMyProfileDto(
    string? Name = null,
    string? Email = null
);