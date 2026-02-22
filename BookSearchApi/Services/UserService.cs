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

    public async Task<User?> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var existingUser = await db.Users
        .FirstOrDefaultAsync(u => u.Name.ToLower() == name.ToLower(), cancellationToken);

        if (existingUser is not null)
        {
            logger.LogWarning("User with name {Name} already exists", name);
            return null;
        }

        var dbId = Guid.NewGuid();
        var user = new User { Id = dbId, Name = name };

        logger.LogInformation("User {name} created with id: {Id}", name, dbId);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        return user;
    }
}
