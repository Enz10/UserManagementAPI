using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Application.Features.Users.Queries;
using UserManagementAPI.Application.Models;
using UserManagementAPI.Core.Domain.Interfaces;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuthService _authService;

    public AuthController(IMediator mediator, IAuthService authService)
    {
        _mediator = mediator;
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var query = new GetUserByEmailQuery { Email = model.Email };
        var user = await _mediator.Send(query);

        if (user == null)
            return BadRequest(new { message = "Invalid email or password" });

        if (!_authService.ValidatePassword(model.Password, user.PasswordHash))
            return BadRequest(new { message = "Invalid email or password" });

        var token = _authService.GenerateJwtToken(user);
        return Ok(new { Token = token });
    }
}