// Take home project for Matthew Maffett

using BookSearchApi.Models;

namespace BookSearchApi.Services;

public interface IOpenLibraryService
{
    /// <summary>
    /// Search for books by title using the Open Library Search API.
    /// </summary>
    /// <param name="query">The book title or keyword to search for.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of results per page (max 100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<BookSearchResponse> SearchByTitleAsync(
        string query,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}
