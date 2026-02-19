// Take home project for Matthew Maffett

using BookSearchApi.Data;
using BookSearchApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookSearchApi.Services;

public class FavoriteBooksService(FavoriteBooksContext db, ILogger<FavoriteBooksService> logger) : IFavoriteBooksService
{
    public async Task<User?> GetUserWithBooksAsync(string userId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting favorites for user: {UserId}", userId);
        return await db.Users
            .Include(u => u.FavoriteBooks)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<FavoriteBook> AddAsync(string userId, FavoriteBook book, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Adding favorite for user: {UserId}", userId);

        var user = await db.Users.FindAsync([userId], cancellationToken);
        if (user is null)
        {
            user = new User { Id = userId };
            db.Users.Add(user);
        }

        book.UserId = userId;
        db.FavoriteBooks.Add(book);
        await db.SaveChangesAsync(cancellationToken);
        return book;
    }

    public async Task<bool> RemoveAsync(string userId, int id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Removing favorite for user: {UserId}", userId);

        var book = await db.FavoriteBooks
            .Where(b => b.Id == id && b.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (book is null)
        {
            return false;
        }

        db.FavoriteBooks.Remove(book);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
