// Take home project for Matthew Maffett

using BookSearchApi.Models;

namespace BookSearchApi.Services;

public interface IOpenLibraryService
{
    /// <summary>
    /// Search Open Library API for books by title.
    /// </summary>
    /// <param name="bookName">The book title or keyword to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> A list of books Open Library could find that matched the given book name.</returns>
    Task<List<BookSearchResult>> SearchByTitleAsync(
        string bookName,
        CancellationToken cancellationToken = default);
}
