using BookSearchApi.Data;
using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookSearchApi.Tests.Services;

public class FavoritesServiceTest : IDisposable
{
    private readonly FavoriteBooksContext _db;
    private readonly Mock<IOpenLibraryService> _openLibraryServiceMock;
    private readonly Mock<ILogger<FavoritesService>> _loggerMock;
    // Our Unit under test
    private readonly FavoritesService _uut;
    private readonly string _testKey = "testKey1";
    private readonly string _testKeyWithWorks = "/works/testKey1";
    private readonly string _testUserName = "testUser1";
    private readonly string _testTitle = "testTitle1";
    private readonly string _testAuthor = "testAuthor1";
    private readonly User _testUser;
    private readonly FavoriteBook _testFavoriteBook;

    public FavoritesServiceTest()
    {
        var options = new DbContextOptionsBuilder<FavoriteBooksContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new FavoriteBooksContext(options);
        _openLibraryServiceMock = new Mock<IOpenLibraryService>();
        _loggerMock = new Mock<ILogger<FavoritesService>>();
        _uut = new FavoritesService(_db, _openLibraryServiceMock.Object, _loggerMock.Object);

        _testUser = new User { Id = Guid.NewGuid(), Name = _testUserName };
        _testFavoriteBook = new FavoriteBook { Id = Guid.NewGuid(), UserId = _testUser.Id, Key = _testKey };
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    // GetBooksForUserAsync tests
    [Fact]
    public async Task GetBooksForUserAsync_ReturnsEmptyList_WhenUserDoesNotExist()
    {
        var result = await _uut.GetBooksForUserAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBooksForUserAsync_ReturnsEmptyList_WhenUserHasNoFavorites()
    {
        _db.Users.Add(_testUser);
        await _db.SaveChangesAsync();

        var result = await _uut.GetBooksForUserAsync(_testUser.Id, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBooksForUserAsync_ReturnsBooks_WhenUserHasFavorites()
    {
        _db.Users.Add(_testUser);
        _db.FavoriteBooks.Add(_testFavoriteBook);
        await _db.SaveChangesAsync();

        var openLibraryBook = new OpenLibraryBook(_testKey, _testTitle, [_testAuthor], [], [], []);

        _openLibraryServiceMock.Setup(s => s.GetByKeyAsync(_testKey, It.IsAny<CancellationToken>())).ReturnsAsync(openLibraryBook);

        var result = await _uut.GetBooksForUserAsync(_testUser.Id, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(_testTitle, result[0].Title);
        Assert.Equal(_testKey, result[0].Key);
    }

    [Fact]
    public async Task GetBooksForUserAsync_SkipsBook_WhenOpenLibraryCannotFindIt()
    {
        _db.FavoriteBooks.Add(_testFavoriteBook);
        await _db.SaveChangesAsync();

        _openLibraryServiceMock.Setup(s => s.GetByKeyAsync(_testKey, It.IsAny<CancellationToken>())).ReturnsAsync((OpenLibraryBook?)null);

        var result = await _uut.GetBooksForUserAsync(_testUser.Id, CancellationToken.None);

        Assert.Empty(result);
    }

    // AddAsync tests
    [Fact]
    public async Task AddAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        var result = await _uut.AddAsync(Guid.NewGuid(), _testKeyWithWorks, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ReturnsFavoriteBook_WhenUserExists()
    {
        _db.Users.Add(_testUser);
        await _db.SaveChangesAsync();

        var result = await _uut.AddAsync(_testUser.Id, _testKeyWithWorks, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(_testKey, result.Key);
        Assert.Equal(_testUser.Id, result.UserId);
    }

    [Fact]
    public async Task AddAsync_StripsPrefixFromKey_WhenKeyContainsWorksPrefix()
    {
        _db.Users.Add(_testUser);
        await _db.SaveChangesAsync();

        var result = await _uut.AddAsync(_testUser.Id, _testKeyWithWorks, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(_testKey, result.Key);
    }

    [Fact]
    public async Task AddAsync_SavesBookToDatabase_WhenUserExists()
    {
        _db.Users.Add(_testUser);
        await _db.SaveChangesAsync();

        await _uut.AddAsync(_testUser.Id, _testKeyWithWorks, CancellationToken.None);

        var savedBook = await _db.FavoriteBooks.FirstOrDefaultAsync(b => b.UserId == _testUser.Id);
        Assert.NotNull(savedBook);
        Assert.Equal(_testKey, savedBook.Key);
    }

    // RemoveAsync tests
    [Fact]
    public async Task RemoveAsync_ReturnsFalse_WhenBookDoesNotExist()
    {
        var result = await _uut.RemoveAsync(Guid.NewGuid(), _testKeyWithWorks, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveAsync_ReturnsTrue_WhenBookIsRemoved()
    {
        _db.FavoriteBooks.Add(_testFavoriteBook);
        await _db.SaveChangesAsync();

        var result = await _uut.RemoveAsync(_testUser.Id, _testKeyWithWorks, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveAsync_DeletesBookFromDatabase_WhenBookIsRemoved()
    {
        _db.FavoriteBooks.Add(_testFavoriteBook);
        await _db.SaveChangesAsync();

        await _uut.RemoveAsync(_testUser.Id, _testKeyWithWorks, CancellationToken.None);

        var deletedBook = await _db.FavoriteBooks.FirstOrDefaultAsync(b => b.Id == _testFavoriteBook.Id);
        Assert.Null(deletedBook);
    }

    [Fact]
    public async Task RemoveAsync_ReturnsFalse_WhenKeyBelongsToDifferentUser()
    {
        _db.FavoriteBooks.Add(_testFavoriteBook);
        await _db.SaveChangesAsync();

        var result = await _uut.RemoveAsync(Guid.NewGuid(), _testKeyWithWorks, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveAsync_StripsPrefixFromKey_WhenKeyContainsWorksPrefix()
    {
        _db.FavoriteBooks.Add(_testFavoriteBook);
        await _db.SaveChangesAsync();

        var result = await _uut.RemoveAsync(_testUser.Id, _testKeyWithWorks, CancellationToken.None);

        Assert.True(result);
    }
}
