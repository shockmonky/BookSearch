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
    /// <param name="userId">The userId from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of books the user has favorited.</returns>
    [HttpGet]
    public async Task<ActionResult<User>> GetAll(
        [FromQuery] string userId,
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

    /// <summary>
    /// Addsa book to the users favorites list.
    /// </summary>
    /// <param name="userId">The userId from.</param>
    /// <param name="key">The Open Library Key for the book to add to favorites.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of books the user has favorited. NotFound if User does not exist.</returns>
    [HttpPost]
    public async Task<ActionResult<FavoriteBook>> Add(
        [FromQuery] string userId,
        [FromQuery] string key,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { error = "X-User-Id header is required." });
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            return BadRequest(new { error = "A valid Open Library key is required." });
        }

        var added = await favoriteBooksService.AddAsync(userId, key, cancellationToken);
        if (added is null)
        {
            return NotFound(new { error = $"User {userId} not found." });
        }

        return CreatedAtAction(nameof(GetAll), added);
    }

    /// <summary>
    /// Addsa book to the users favorites list.
    /// </summary>
    /// <param name="userId">The userId from.</param>
    /// <param name="id">The db context of the book to remove from the user's favorites.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of books the user has favorited.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(
        [FromQuery] string userId,
        [FromQuery] int id,
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
