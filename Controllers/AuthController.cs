using System.Security.Claims;
using mbm.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mbm.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize]
public class AuthController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult Me()
    {
        var dto = new AuthMeDto(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            User.FindFirstValue(ClaimTypes.Email),
            User.FindFirstValue("preferred_username"),
            User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList());

        return Ok(dto);
    }
}
