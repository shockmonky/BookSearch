// Take home project for Matthew Maffett

using BookSearchApi.Models;

namespace BookSearchApi.Services;

public interface IGeminiService
{
    Task<string?> SummarizeBooksAsync(List<OpenLibraryBook> books, CancellationToken cancellationToken = default);
}
