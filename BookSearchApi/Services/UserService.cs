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

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting user: {UserId}", userId);
        return await db.Users.FindAsync([userId], cancellationToken);
    }

    public async Task<User> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var dbId = Guid.NewGuid();
        var user = new User { Id = dbId, Name = name };

        logger.LogInformation("User {name} created with id: {Id}", name, dbId);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        return user;
    }
}
