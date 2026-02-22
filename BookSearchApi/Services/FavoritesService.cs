// Take home project for Matthew Maffett

using BookSearchApi.Data;
using BookSearchApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookSearchApi.Services;

public class FavoritesService(FavoriteBooksContext db,
                                  IOpenLibraryService openLibraryService,
                                  ILogger<FavoritesService> logger) : IFavoritesService
{
    public async Task<List<OpenLibraryBook>> GetBooksForUserAsync(Guid userId, CancellationToken cancellationToken = default)
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

    public async Task<FavoriteBook?> AddAsync(Guid userId, string key, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Adding favorite for user: {UserId}", userId);

        // sanitize the key value
        var cleanKey = key.Trim().Replace("/works/", string.Empty);

        var user = await db.Users.FindAsync([userId], cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User {UserId} not found", userId);
            return null;
        }

        // If the book is already favorited just return that book
        var existingBook = await db.FavoriteBooks
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Key == cleanKey, cancellationToken);

        if (existingBook is not null)
        {
            logger.LogWarning("Book {Key} already exists in favorites for user {UserId}", cleanKey, userId);
            return existingBook;
        }

        // if it's a new book create and add it
        var book = new FavoriteBook { Id = Guid.NewGuid(), Key = cleanKey, UserId = userId };
        db.FavoriteBooks.Add(book);
        await db.SaveChangesAsync(cancellationToken);
        return book;
    }

    public async Task<bool> RemoveAsync(Guid userId, string key, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Removing favorite book with key: {Key} for user: {UserId}", key, userId);

        // sanitize our key value
        var cleanKey = key.Trim().Replace("/works/", string.Empty);

        // See if the book is in the db for the user
        var book = await db.FavoriteBooks
            .Where(b => b.UserId == userId && b.Key.Equals(cleanKey))
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
