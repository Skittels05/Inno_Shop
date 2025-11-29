using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Application.CQRS.Commands;
using Users.Application.DTOs;

namespace Users.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register([FromBody] CreateUserDto dto)
    {
        var command = new CreateUserCommand(dto.Name, dto.Email, dto.Password);
        var userId = await _mediator.Send(command);

        return CreatedAtAction(nameof(Register), new { id = userId }, userId);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await _mediator.Send(new ResetPasswordCommand(dto));
        return Ok(new { message = "Password successfully reset" });
    }

    [HttpGet("login")]
    public async Task<ActionResult<AuthResultDto>> Login([FromQuery] string email, [FromQuery] string password)
    {
        var dto = new UserLoginDto(email, password);
        var result = await _mediator.Send(new LoginUserCommand(dto));
        return Ok(result);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        var command = new ConfirmEmailCommand(email, token);
        var success = await _mediator.Send(command);

        return success
            ? Ok(new { message = "Email successfully confirmed" })
            : BadRequest(new { message = "Invalid or expired token" });
    }

    [HttpGet("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromQuery] string email)
    {
        var dto = new PasswordRecoveryDto(email);
        await _mediator.Send(new PasswordRecoveryCommand(dto));
        return Ok(new { message = "If email exists, recovery link has been sent" });
    }

    [HttpGet("send-confirmation")]
    public async Task<IActionResult> SendConfirmation([FromQuery] string email)
    {
        var command = new SendEmailConfirmationCommand(email);
        await _mediator.Send(command);
        return Ok(new { message = "Confirmation email sent" });
    }
}