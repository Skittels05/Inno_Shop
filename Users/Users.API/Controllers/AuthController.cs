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


    [HttpPost("login")]
    public async Task<ActionResult<AuthResultDto>> Login([FromBody] UserLoginDto dto)
    {
        var result = await _mediator.Send(new LoginUserCommand(dto));
        return Ok(result);
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        var success = await _mediator.Send(command);
        return success
            ? Ok(new { message = "Email successfully confirmed" })
            : BadRequest(new { message = "Invalid or expired token" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] PasswordRecoveryDto dto)
    {
        await _mediator.Send(new PasswordRecoveryCommand(dto));
        return Ok(new { message = "If email exists, recovery link has been sent" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await _mediator.Send(new ResetPasswordCommand(dto));
        return Ok(new { message = "Password successfully reset" });
    }
    [HttpPost("send-confirmation")]
    public async Task<IActionResult> SendConfirmation([FromBody] SendEmailConfirmationCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Confirmation email sent" });
    }
}