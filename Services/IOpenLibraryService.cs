// Take home project for Matthew Maffett

using BookSearchApi.Models;

namespace BookSearchApi.Services;

public interface IOpenLibraryService
{
    /// <summary>
    /// Search Open Library API for a book by title.
    /// </summary>
    /// <param name="bookName">The book title to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> A list of books from Open Library that matched the given book name.</returns>
    Task<List<BookSearchResult>> SearchByTitleAsync(
        string bookName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search Open Library API for books by subject.
    /// </summary>
    /// <param name="subjectName">The subject to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> A list of books from Open Library that fall under the given subject.</returns>
    Task<List<BookSearchResult>> SearchBySubjectAsync(
        string subjectName,
        CancellationToken cancellationToken = default);
}
