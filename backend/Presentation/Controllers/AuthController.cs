using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    [HttpGet("status")]
    [AllowAnonymous]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            Message = "Firebase JWT validation enabled when FIREBASE_PROJECT_ID is configured.",
            GuestMode = true,
            GoogleSignIn = true
        });
    }

    [HttpGet("me")]
    [Authorize(Policy = "GuestOrAuthenticated")]
    public IActionResult GetProfile()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return Ok(new
            {
                IsAuthenticated = false,
                DisplayName = "Guest"
            });
        }

        return Ok(new
        {
            IsAuthenticated = true,
            UserId = User.FindFirst("user_id")?.Value ?? User.FindFirst("sub")?.Value ?? string.Empty,
            Email = User.FindFirst("email")?.Value ?? string.Empty,
            DisplayName = User.FindFirst("name")?.Value ?? User.FindFirst("email")?.Value ?? "Authenticated User"
        });
    }
}
