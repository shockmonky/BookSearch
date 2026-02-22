// Take home project for Matthew Maffett

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BookSearchApi.Models;

namespace BookSearchApi.Services;

public class GeminiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiService> logger)
    : IGeminiService
{
    public async Task<string?> SummarizeBooksAsync(List<OpenLibraryBook> books, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Summarizing {Count} books using Gemini", books.Count);

        var apiKey = configuration["Gemini:ApiKey"];
        var bookList = new StringBuilder();

        foreach (var book in books)
        {
            var authors = book.AuthorNames is not null ? string.Join(", ", book.AuthorNames) : "Unknown";
            bookList.AppendLine($"- {book.Title} by {authors}");
        }

        var sb = new StringBuilder($"I am interested in these books: {bookList}");
        sb.Append("Please provide a brief summary of each book and what kind of reader would enjoy them.");

        var request = new GeminiRequest(
        [
            new GeminiContent(
            [
                new GeminiPart(sb.ToString())
            ])
        ]);

        var url = $"/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

        var response = await httpClient.PostAsJsonAsync(url, request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Gemini API returned {StatusCode}", response.StatusCode);
            return null;
        }

        var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken);

        return geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
    }

    // Helper function to create the Gemini Request
    private GeminiRequest CreateRequest()
    {
		var sb = new StringBuilder($"I am interested in these books: {bookList}");
		sb.Append("Please provide a brief summary of each book and what kind of reader would enjoy them.");

		return new GeminiRequest(
		[
			new GeminiContent(
			[
				new GeminiPart(sb.ToString())
			])
		]);
	}
}
