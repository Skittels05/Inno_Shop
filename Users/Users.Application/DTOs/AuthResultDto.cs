namespace Users.Application.DTOs
{
    public record AuthResultDto(
        string Token,
        Guid UserId,
        string Email,
        string Role
    );
}
