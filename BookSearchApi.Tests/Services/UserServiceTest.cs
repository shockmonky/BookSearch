// Take home project for Matthew Maffett

using BookSearchApi.Data;
using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookSearchApi.Tests.Services;

public class UserServiceTests : IDisposable
{
    private readonly FavoriteBooksContext _db;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    // Our Unit under test
    private readonly UserService _uut;
    private readonly string _testUserOne = "testUser1";
    private readonly string _testUserTwo = "testUser2";
    private readonly Guid _testGuidOne = Guid.NewGuid();
    private readonly Guid _testGuidTwo = Guid.NewGuid();

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<FavoriteBooksContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new FavoriteBooksContext(options);
        _loggerMock = new Mock<ILogger<UserService>>();
        _uut = new UserService(_db, _loggerMock.Object);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    // GetAllAsync tests
    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoUsersExist()
    {
        // no users have been added to the db 
        var result = await _uut.GetAllAsync(CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers_WhenUsersExist()
    {
        var userOne = new User { Id = _testGuidOne, Name = _testUserOne };
        var userTwo = new User { Id = _testGuidTwo, Name = _testUserTwo };

        _db.Users.Add(userOne);
        _db.Users.Add(userTwo);

        await _db.SaveChangesAsync();

        var result = await _uut.GetAllAsync(CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(userOne, result);
        Assert.Contains(userTwo, result);
    }

    // CreateAsync tests
    [Fact]
    public async Task CreateAsync_ReturnsUser_WithCorrectName()
    {
        var result = await _uut.CreateAsync(_testUserOne, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(_testUserOne, result.Name);
    }

    [Fact]
    public async Task CreateAsync_ReturnsUser_WithGeneratedId()
    {
        var result = await _uut.CreateAsync(_testUserOne, CancellationToken.None);

        // Make sure the new user guid isn't all 0's
        Assert.NotEqual(Guid.Empty, result.Id);
    }
}
