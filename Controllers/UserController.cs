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
        [FromHeader(Name = "X-User-Id")] string userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { error = "X-User-Id header is required." });
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
        [FromHeader(Name = "X-User-Id")] string userId,
        [FromBody] string name,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { error = "X-User-Id header is required." });
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new { error = "A valid name is required." });
        }

        var user = await userService.CreateAsync(userId, name, cancellationToken);
        return CreatedAtAction(nameof(Get), user);
    }
}
