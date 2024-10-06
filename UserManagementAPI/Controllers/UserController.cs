using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Application.Models;
using UserManagementAPI.Core.Application.Commands;
using UserManagementAPI.Core.Application.Queries;
using UserManagementAPI.Core.Domain.Entities;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("bulk-create")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUsers([FromBody] CreateUsersCommand command)
    {
        if (command.Users == null || !command.Users.Any() || command.Users.Count() > 1000)
        {
            return BadRequest("Invalid number of users. Must be between 1 and 1000.");
        }

        var createdUsers = await _mediator.Send(command);
        return Ok(createdUsers);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? age = null, [FromQuery] string? country = null)
    {
        var query = new GetUsersQuery
        {
            Page = page,
            PageSize = pageSize,
            Age = age,
            Country = country
        };

        var users = await _mediator.Send(query);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var query = new GetUserByIdQuery { Id = id };
        var user = await _mediator.Send(query);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
    {
        var command = new CreateUserCommand(userDto);
        var createdUser = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }
}