// Take home project for Matthew Maffett
using BookSearchApi.Models;

namespace BookSearchApi.Services;

public interface IFavoriteBooksService
{
    /// <summary>
    /// Retrieve all of the favorited books for a User.
    /// </summary>
    /// <param name="userId">The user whose favorite books we want to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> A list of the the favorited books for a User.</returns>
    Task<User?> GetUserWithBooksAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a book to the users favorited books.
    /// </summary>
    /// <param name="userId">The user whose favorite book list we want to add the book to.</param>
    /// <param name="key">The Open Library Key for the book to add to favorites.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> The item that was added to the users favorite books list.</returns>
    Task<FavoriteBook> AddAsync(string userId, string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a book from the users favorite books.
    /// </summary>
    /// <param name="userId">The user whose favorite book list we want to remove the book from.</param>
    /// <param name="id">The DbContext id of the book we want to remove from the users list.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> True if the book was removed.</returns>
    Task<bool> RemoveAsync(string userId, int id, CancellationToken cancellationToken = default);
}
