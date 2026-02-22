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

        var response = await geminiService.SummarizeBooksAsync(books, cancellationToken);

        // If our call to Gemini failed let the user know why
        if (!response.IsSuccessStatusCode)
        {
            string errorMsg;

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                logger.LogError("Gemini API rate limit reached");
                errorMsg = "Gemini API limit reached. Please try again later.";
            }
            else
            {
                logger.LogError("Gemini API returned {StatusCode}", response.StatusCode);
                errorMsg = "Unable to reach Gemini API.";
            }

            return StatusCode(StatusCodes.Status502BadGateway, new { error = errorMsg });
        }

        var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken);

        // Get the text from the Gemini response
        var summary = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

        // Assume if the call was successful but the summary is null the input couldn't be summarized
        if (summary is null)
        {
            return StatusCode(StatusCodes.Status400BadRequest, new { error = "Content could not be summarized." });
        }

        return Ok(summary);
    }
}
