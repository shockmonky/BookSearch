// Take home project for Matthew Maffett
using BookSearchApi.Models;

namespace BookSearchApi.Services;

public interface IFavoriteBooksService
{
    /// <summary>
    /// Retrieve all of the favorited books for a User.
    /// </summary>
    /// <param name="userId">The book title to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> A list of the the favorited books for a User.</returns>
    Task<List<FavoriteBook>> GetFavorites(
        int userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a book to the users favorited books.
    /// </summary>
    /// <param name="userId">The book title to search for.</param>
    /// <param name="worksKey">The key of the object to be added to the users favorites.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> The item that was added to the users favorites list.</returns>
    Task<List<FavoriteBook>> AddFavorite(
        int userId,
        string worksKey,
        CancellationToken cancellationToken = default);
}
