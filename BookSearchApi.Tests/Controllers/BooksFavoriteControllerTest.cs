// Take home project for Matthew Maffett

using BookSearchApi.Controllers;
using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookSearchApi.Tests;

public class BooksFavoriteControllerTests
{
    private readonly Mock<IFavoriteBooksService> _favoriteBooksServiceMock;
    // Our unit under test
    private readonly BooksFavoriteController _uut;
    private readonly Guid _testGuidOne = Guid.NewGuid();
    private readonly string _testKey = "testKey1";
    private readonly string _testTitle = "testTitle1";
    private readonly string _testAuthor = "testAuthor1";
    private readonly int _testCoverId = 87654321;

    public BooksFavoriteControllerTests()
    {
        _favoriteBooksServiceMock = new Mock<IFavoriteBooksService>();
        _uut = new BooksFavoriteController(_favoriteBooksServiceMock.Object);
    }

    // GetAll tests
    [Fact]
    public async Task GetAll_ReturnsOk_WhenUserExists()
    {
        var books = new List<OpenLibraryBook>
    {
        new(_testKey, _testTitle, [_testAuthor], null, [], [], _testCoverId)
    };
        _favoriteBooksServiceMock.Setup(s => s.GetUserWithBooksAsync(_testGuidOne, It.IsAny<CancellationToken>())).ReturnsAsync(books);

        var result = await _uut.GetAll(_testGuidOne, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooks = Assert.IsType<List<OpenLibraryBook>>(okResult.Value);
        Assert.Single(returnedBooks);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithEmptyList_WhenUserHasNoFavorites()
    {
        _favoriteBooksServiceMock.Setup(s => s.GetUserWithBooksAsync(_testGuidOne, It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _uut.GetAll(_testGuidOne, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooks = Assert.IsType<List<OpenLibraryBook>>(okResult.Value);
        Assert.Empty(returnedBooks);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenUserIdIsEmpty()
    {
        var result = await _uut.GetAll(Guid.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // Add tests
    [Fact]
    public async Task Add_ReturnsCreated_WhenBookIsAdded()
    {
        var favoriteBook = new FavoriteBook { UserId = _testGuidOne, Key = _testKey };
        _favoriteBooksServiceMock.Setup(s => s.AddAsync(_testGuidOne, _testKey, It.IsAny<CancellationToken>())).ReturnsAsync(favoriteBook);

        var result = await _uut.Add(_testGuidOne, _testKey, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedBook = Assert.IsType<FavoriteBook>(createdResult.Value);
        Assert.Equal(_testKey, returnedBook.Key);
    }

    [Fact]
    public async Task Add_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _favoriteBooksServiceMock.Setup(s => s.AddAsync(_testGuidOne, _testKey, It.IsAny<CancellationToken>())).ReturnsAsync((FavoriteBook?)null);

        var result = await _uut.Add(_testGuidOne, _testKey, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_WhenUserIdIsEmpty()
    {
        var result = await _uut.Add(Guid.Empty, _testKey, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_WhenKeyIsEmpty()
    {
        var result = await _uut.Add(_testGuidOne, string.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_WhenKeyIsWhitespace()
    {
        var result = await _uut.Add(_testGuidOne, "   ", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // Remove tests
    [Fact]
    public async Task Remove_ReturnsNoContent_WhenBookIsRemoved()
    {
        _favoriteBooksServiceMock.Setup(s => s.RemoveAsync(_testGuidOne, _testKey, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _uut.Remove(_testGuidOne, _testKey, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Remove_ReturnsNotFound_WhenBookDoesNotExist()
    {
        _favoriteBooksServiceMock.Setup(s => s.RemoveAsync(_testGuidOne, _testKey, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _uut.Remove(_testGuidOne, _testKey, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Remove_ReturnsBadRequest_WhenUserIdIsEmpty()
    {
        var result = await _uut.Remove(Guid.Empty, _testKey, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Remove_ReturnsBadRequest_WhenKeyIsEmpty()
    {
        var result = await _uut.Remove(_testGuidOne, string.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Remove_ReturnsBadRequest_WhenKeyIsWhitespace()
    {
        var result = await _uut.Remove(_testGuidOne, "   ", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
