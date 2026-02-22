// Take home project for Matthew Maffett

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BookSearchApi.Models;

namespace BookSearchApi.Services;

public class GeminiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiService> logger)
    : IGeminiService
{
    public async Task<HttpResponseMessage> SummarizeBooksAsync(List<OpenLibraryBook> books, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Summarizing {Count} books using Gemini", books.Count);

        var request = CreateRequest(books);

        var apiKey = configuration["Gemini:ApiKey"];

        var url = $"/v1beta/models/gemini-2.5-flash-lite:generateContent?key={apiKey}";

        var response = await httpClient.PostAsJsonAsync(url, request, cancellationToken);

        return response;
    }

    // Helper function to create the Gemini Request with th elist of Open Library Books
    private GeminiRequest CreateRequest(List<OpenLibraryBook> books)
    {
        var bookList = new StringBuilder();

        foreach (var book in books)
        {
            var authors = book.Authors is not null ? string.Join(", ", book.Authors) : "Unknown";
            bookList.AppendLine($"- {book.Title} by {authors}");
        }

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
