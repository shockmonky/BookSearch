// Take home project for Matthew Maffett

using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<List<User>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet]
    public async Task<ActionResult<User>> Get(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest(new { error = "userId is required." });
        }

        var user = await userService.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return NotFound(new { error = $"User {userId} not found." });
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Create(
    [FromQuery] string name,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new { error = "name is required." });
        }

        var user = await userService.CreateAsync(name, cancellationToken);
        return CreatedAtAction(nameof(Get), user);
    }
}
