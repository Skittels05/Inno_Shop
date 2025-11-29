using Microsoft.AspNetCore.Mvc;

namespace Products.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("check-auth")]
        public IActionResult CheckAuth()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                return Ok(new
                {
                    isAuthenticated = true,
                    userId = userId,
                    claims = User.Claims.Select(c => new { c.Type, c.Value })
                });
            }
            return Ok(new { isAuthenticated = false });
        }
    }
}