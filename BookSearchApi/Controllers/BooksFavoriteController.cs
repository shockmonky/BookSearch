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
    public async Task<ActionResult<List<OpenLibraryBook>>> GetAll(
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { error = "userId is required." });
        }

        var bookList = await favoriteBooksService.GetUserWithBooksAsync(userId, cancellationToken);
        if (bookList is null)
        {
            return NotFound(new { error = "No favorites found for User." });
        }

        return Ok(bookList);
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
            return BadRequest(new { error = "userId is required." });
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

        return CreatedAtAction(nameof(Add), added);
    }

    /// <summary>
    /// REmvoes a book fromthe users favorites list.
    /// </summary>
    /// <param name="userId">The userId from.</param>
    /// <param name="key">The Open Library key of the book to remove from the user's favorites.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of books the user has favorited.</returns>
    [HttpDelete]
    public async Task<IActionResult> Remove(
    [FromQuery] string userId,
    [FromQuery] string key,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new { error = "userId is required." });
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            return BadRequest(new { error = "A valid Open Library key is required." });
        }

        var removed = await favoriteBooksService.RemoveAsync(userId, key, cancellationToken);
        if (!removed)
        {
            return NotFound(new { error = $"Book {key} not found in favorites for user {userId}." });
        }

        return NoContent();
    }
}
