// Take home project for Matthew Maffett

using System.Text;
using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BooksController(IOpenLibraryService openLibraryService, ILogger<BooksController> logger)
    : ControllerBase
{
    private const int PageToShow = 1;
    private const int ResultsPerPage = 10;

    /// <summary>
    /// Search for books by title.
    /// </summary>
    /// <param name="bookName">The book title to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of books with a matching title.</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<BookSearchResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<List<BookSearchResult>>> Search(
        [FromQuery] string bookName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bookName))
        {
            return this.BadRequest(new { error = "Parameter 'BookName' is required." });
        }

        try
        {
            var result = await openLibraryService.SearchByTitleAsync(bookName, cancellationToken);
            return this.Ok(result);
        }
        catch (HttpRequestException ex)
        {
            var errMessage = new StringBuilder("Open Library API could not be reachded");
            logger.LogError(ex, errMessage.ToString());

            errMessage.Append(". Please try again later.");
            return this.StatusCode(
                StatusCodes.Status502BadGateway,
                new { error = errMessage.ToString() });
        }
        catch (TaskCanceledException)
        {
            var errMessage = new StringBuilder("Open Library Request was cancelled or timed out");
            logger.LogWarning(errMessage.ToString());

            errMessage.Append(". Please try again later.");
            return this.StatusCode(
                StatusCodes.Status504GatewayTimeout,
                new { error = errMessage.ToString() });
        }
    }
}
