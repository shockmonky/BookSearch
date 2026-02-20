// Take home project for Matthew Maffett

using BookSearchApi.Models;

namespace BookSearchApi.Services;

public interface IUserService
{
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);

    Task<User> CreateAsync(string userId, string name, CancellationToken cancellationToken = default);
}
