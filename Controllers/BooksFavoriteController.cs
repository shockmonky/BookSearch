// Take home project for Matthew Maffett

using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BooksFavoriteController(IFavoriteBooksService favoriteBooksService) : ControllerBase
{
    /// <summary>
    /// Retrieve all of a users favorite books.
    /// </summary>
    /// <param name="subjectName">The subject to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of books with that fall under the provided subject.</returns>
    [HttpGet]
    public async Task<ActionResult<User>> GetAll(
        [FromHeader(Name = "X-User-Id")] string userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { error = "X-User-Id header is required." });
        }

        var user = await favoriteBooksService.GetUserWithBooksAsync(userId, cancellationToken);
        if (user is null)
        {
            return NotFound(new { error = "User not found." });
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<FavoriteBook>> Add(
        [FromHeader(Name = "X-User-Id")] string userId,
        FavoriteBook book,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { error = "X-User-Id header is required." });
        }

        var added = await favoriteBooksService.AddAsync(userId, book, cancellationToken);
        return CreatedAtAction(nameof(GetAll), added);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(
        [FromHeader(Name = "X-User-Id")] string userId,
        int id,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { error = "X-User-Id header is required." });
        }

        var removed = await favoriteBooksService.RemoveAsync(userId, id, cancellationToken);
        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}
