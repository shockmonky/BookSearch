// Take home project for Matthew Maffett

using BookSearchApi.Data;
using BookSearchApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookSearchApi.Services;

public class UserService(FavoriteBooksContext db, ILogger<UserService> logger) : IUserService
{
    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all users");
        return await db.Users.ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting user: {UserId}", userId);
        return await db.Users.FindAsync([userId], cancellationToken);
    }

    public async Task<User> CreateAsync(string userId, string name, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating user: {UserId}", userId);
        var user = new User { Id = userId, Name = name };
        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        return user;
    }
}
