// Take home project for Matthew Maffett

using System.Text;
using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BookSearchController(IOpenLibraryService openLibraryService)
    : ControllerBase
{
    /// <summary>
    /// Search for books by title.
    /// </summary>
    /// <param name="bookName">The book title to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of the first 100 books with a matching title.</returns>
    [HttpGet("searchtitle")]
    [ProducesResponseType(typeof(List<BookSearchResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<List<BookSearchResult>>> SearchByTitle(
        [FromQuery] string bookName,
        CancellationToken cancellationToken = default)
    {
        var isBadBookName = string.Equals(bookName.Trim(), "the", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(bookName.Trim(), "a", StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(bookName) || isBadBookName)
        {
            return this.BadRequest(new { error = "Valid BookName is required." });
        }

        try
        {
            var result = await openLibraryService.SearchByTitleAsync(bookName, cancellationToken);
            return this.Ok(result);
        }
        catch (HttpRequestException)
        {
            var errMessage = new StringBuilder("Open Library API could not be reachded");

            errMessage.Append(". Please try again later.");
            return this.StatusCode(
                StatusCodes.Status502BadGateway,
                new { error = errMessage.ToString() });
        }
        catch (TaskCanceledException)
        {
            var errMessage = new StringBuilder("Open Library Request was cancelled or timed out");

            errMessage.Append(". Please try again later.");
            return this.StatusCode(
                StatusCodes.Status504GatewayTimeout,
                new { error = errMessage.ToString() });
        }
    }

    /// <summary>
    /// Search for books by subject.
    /// </summary>
    /// <param name="subjectName">The subject to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of books with that fall under the provided subject.</returns>
    [HttpGet("searchsubject")]
    [ProducesResponseType(typeof(List<BookSearchResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<List<BookSearchResult>>> SearchBySubject(
        [FromQuery] string subjectName,
        CancellationToken cancellationToken = default)
    {
        var isBadSubjectName = string.Equals(subjectName.Trim(), "the", StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(subjectName.Trim(), "a", StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(subjectName) || isBadSubjectName)
        {
            return this.BadRequest(new { error = "Valid subject is required." });
        }

        try
        {
            var result = await openLibraryService.SearchBySubjectAsync(subjectName, cancellationToken);
            return this.Ok(result);
        }
        catch (HttpRequestException)
        {
            var errMessage = new StringBuilder("Open Library API could not be reachded");

            errMessage.Append(". Please try again later.");
            return this.StatusCode(
                StatusCodes.Status502BadGateway,
                new { error = errMessage.ToString() });
        }
        catch (TaskCanceledException)
        {
            var errMessage = new StringBuilder("Open Library Request was cancelled or timed out");

            errMessage.Append(". Please try again later.");
            return this.StatusCode(
                StatusCodes.Status504GatewayTimeout,
                new { error = errMessage.ToString() });
        }
    }
}
