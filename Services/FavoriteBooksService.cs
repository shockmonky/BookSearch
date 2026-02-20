// Take home project for Matthew Maffett

using BookSearchApi.Data;
using BookSearchApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookSearchApi.Services;

public class FavoriteBooksService(FavoriteBooksContext db,
                                  IOpenLibraryService openLibraryService,
                                  ILogger<FavoriteBooksService> logger) : IFavoriteBooksService
{
    public async Task<List<OpenLibraryBook>> GetUserWithBooksAsync(string userId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting favorites for user: {UserId}", userId);

        // Get the user and have FWCore fill out their favorites list
        var user = await db.Users
            .Include(u => u.FavoriteBooks)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || user.FavoriteBooks.Count == 0)
        {
            return [];
        }

        var bookList = new List<OpenLibraryBook>();

        foreach (var favBook in user.FavoriteBooks)
        {
            // If the OpenLibService can find the book by key add it to our list
            if (await openLibraryService.GetByKeyAsync(favBook.Key, cancellationToken) is OpenLibraryBook book)
            {
                bookList.Add(book);
            }
            else
            {
                logger.LogInformation("Open Library could not find a book for work: {key}", favBook.Key);
            }
        }

        return bookList;
    }

    public async Task<FavoriteBook?> AddAsync(string userId, string key, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Adding favorite for user: {UserId}", userId);

        var user = await db.Users.FindAsync([userId], cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User {UserId} not found", userId);
            return null;
        }

        var book = new FavoriteBook { Key = key, UserId = userId };
        db.FavoriteBooks.Add(book);
        await db.SaveChangesAsync(cancellationToken);
        return book;
    }

    public async Task<bool> RemoveAsync(string userId, int id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Removing favorite for user: {UserId}", userId);

        // See if the book is in the db for the user
        var book = await db.FavoriteBooks.Where(b => b.Id == id && b.UserId == userId).FirstOrDefaultAsync(cancellationToken);

        if (book is null)
        {
            return false;
        }

        db.FavoriteBooks.Remove(book);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
