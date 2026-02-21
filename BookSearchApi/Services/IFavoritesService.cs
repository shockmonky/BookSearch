// Take home project for Matthew Maffett
using BookSearchApi.Models;

namespace BookSearchApi.Services;

public interface IFavoritesService
{
    /// <summary>
    /// Retrieve all of the favorited books for a User.
    /// </summary>
    /// <param name="userId">The user whose favorite books we want to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> A list of the the favorited books for a User.</returns>
    Task<List<OpenLibraryBook>> GetBooksForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a book to the users favorited books.
    /// </summary>
    /// <param name="userId">The user whose favorite book list we want to add the book to.</param>
    /// <param name="key">The Open Library Key for the book to add to favorites.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> The item that was added to the users favorite books list.</returns>
    Task<FavoriteBook?> AddAsync(Guid userId, string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a book from the users favorite books.
    /// </summary>
    /// <param name="userId">The user whose favorite book list we want to remove the book from.</param>
    /// <param name="key">The Open Library key of the book to remove from the user's favorites.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> True if the book was removed.</returns>
    Task<bool> RemoveAsync(Guid userId, string key, CancellationToken cancellationToken = default);
}
