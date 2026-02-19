// Copyright (c) YourCompany. All rights reserved.

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
    /// <returns>A paginated list of books with a matching title.</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(BookSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<BookSearchResponse>> Search(
        [FromQuery] string bookName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bookName))
        {
            return this.BadRequest(new { error = "Parameter 'BookName' is required." });
        }

        try
        {
            var result = await openLibraryService.SearchByTitleAsync(bookName, PageToShow, ResultsPerPage, cancellationToken);
            return this.Ok(result);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to reach Open Library API");
            return this.StatusCode(
                StatusCodes.Status502BadGateway,
                new { error = "Unable to reach the Open Library service. Please try again later." });
        }
        catch (TaskCanceledException)
        {
            logger.LogWarning("Request to Open Library was cancelled or timed out");
            return this.StatusCode(
                StatusCodes.Status504GatewayTimeout,
                new { error = "The request timed out. Please try again." });
        }
    }
}
