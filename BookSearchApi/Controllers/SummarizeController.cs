// Take home project for Matthew Maffett

using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookSearchApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class SummarizeController(
    IGeminiService geminiService,
    ILogger<SummarizeController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> Summarize(
        [FromBody] List<OpenLibraryBook> books,
        CancellationToken cancellationToken)
    {
        if (books is null || books.Count == 0)
        {
            return BadRequest(new { error = "At least one book is required." });
        }

        logger.LogInformation("Summarizing {Count} search results", books.Count);

        var summary = await geminiService.SummarizeBooksAsync(books, cancellationToken);

        if (summary is null)
        {
            return StatusCode(StatusCodes.Status502BadGateway, new { error = "Gemini API rate limit or daily quota reached. Please try again later." });
        }

        return Ok(summary);
    }
}
