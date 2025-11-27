using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Users.Application.CQRS.Commands;
using Users.Application.CQRS.Queries;
using Users.Application.DTOs;

namespace Users.Api.Controllers;

[Authorize]
[Route("api/profile")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator) => _mediator = mediator;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpGet]
    public async Task<ActionResult<UserDto>> GetProfile()
        => Ok(await _mediator.Send(new GetUserByIdQuery(CurrentUserId)));

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateMyProfileDto dto)
    {
        try
        {
            await _mediator.Send(new UpdateMyProfileCommand(CurrentUserId, dto));
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = true, message = ex.Message, stackTrace = ex.StackTrace });
        }

    }

    [HttpPatch]
    public async Task<IActionResult> PatchProfile([FromBody] UpdateMyProfileDto dto)
        => await UpdateProfile(dto);
}