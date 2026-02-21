// Take home project for Matthew Maffett

using BookSearchApi.Models;

namespace BookSearchApi.Services;

public interface IUserService
{
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<User> CreateAsync(string name, CancellationToken cancellationToken = default);
}
