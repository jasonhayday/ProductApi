namespace ProductApi.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        await _auth.RegisterAsync(user.Username, user.Password);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(User req)
    {
        var token = await _auth.LoginAsync(req.Username, req.Password);

        if (token == null) return Unauthorized();

        return Ok(new { token });
    }
}