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
    private readonly BooksFavoriteController _controller;

    public BooksFavoriteControllerTests()
    {
        _favoriteBooksServiceMock = new Mock<IFavoriteBooksService>();
        _controller = new BooksFavoriteController(_favoriteBooksServiceMock.Object);
    }

    // GetAll tests
    [Fact]
    public async Task GetAll_ReturnsOk_WhenUserExists()
    {
        var user = new User { Id = "user-1", Name = "Matthew" };
        _favoriteBooksServiceMock.Setup(s => s.GetUserWithBooksAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _controller.GetAll("user-1", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal("user-1", returnedUser.Id);
    }

    [Fact]
    public async Task GetAll_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _favoriteBooksServiceMock.Setup(s => s.GetUserWithBooksAsync("user-999", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await _controller.GetAll("user-999", CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenUserIdIsEmpty()
    {
        var result = await _controller.GetAll(string.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenUserIdIsWhitespace()
    {
        var result = await _controller.GetAll("   ", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // Add tests
    [Fact]
    public async Task Add_ReturnsCreated_WhenBookIsAdded()
    {
        var favoriteBook = new FavoriteBook { Id = 1, UserId = "user-1", Key = "/works/OL468431W" };
        _favoriteBooksServiceMock.Setup(s => s.AddAsync("user-1", "/works/OL468431W", It.IsAny<CancellationToken>())).ReturnsAsync(favoriteBook);

        var result = await _controller.Add("user-1", "/works/OL468431W", CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedBook = Assert.IsType<FavoriteBook>(createdResult.Value);
        Assert.Equal("/works/OL468431W", returnedBook.Key);
    }

    [Fact]
    public async Task Add_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _favoriteBooksServiceMock.Setup(s => s.AddAsync("user-999", "/works/OL468431W", It.IsAny<CancellationToken>())).ReturnsAsync((FavoriteBook?)null);

        var result = await _controller.Add("user-999", "/works/OL468431W", CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_WhenUserIdIsEmpty()
    {
        var result = await _controller.Add(string.Empty, "/works/OL468431W", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_WhenUserIdIsWhitespace()
    {
        var result = await _controller.Add("   ", "/works/OL468431W", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_WhenKeyIsEmpty()
    {
        var result = await _controller.Add("user-1", string.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_WhenKeyIsWhitespace()
    {
        var result = await _controller.Add("user-1", "   ", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // Remove tests
    [Fact]
    public async Task Remove_ReturnsNoContent_WhenBookIsRemoved()
    {
        _favoriteBooksServiceMock.Setup(s => s.RemoveAsync("user-1", "/works/OL468431W", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _controller.Remove("user-1", "/works/OL468431W", CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Remove_ReturnsNotFound_WhenBookDoesNotExist()
    {
        _favoriteBooksServiceMock.Setup(s => s.RemoveAsync("user-1", "/works/OL468431W", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _controller.Remove("user-1", "/works/OL468431W", CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Remove_ReturnsBadRequest_WhenUserIdIsEmpty()
    {
        var result = await _controller.Remove(string.Empty, "/works/OL468431W", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Remove_ReturnsBadRequest_WhenUserIdIsWhitespace()
    {
        var result = await _controller.Remove("   ", "/works/OL468431W", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Remove_ReturnsBadRequest_WhenKeyIsEmpty()
    {
        var result = await _controller.Remove("user-1", string.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Remove_ReturnsBadRequest_WhenKeyIsWhitespace()
    {
        var result = await _controller.Remove("user-1", "   ", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
