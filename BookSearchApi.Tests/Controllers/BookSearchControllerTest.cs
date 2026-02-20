// Take home project for Matthew Maffett

using BookSearchApi.Controllers;
using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookSearchApi.Tests;

public class BookSearchControllerTests
{
    private readonly Mock<IOpenLibraryService> _openLibraryServiceMock;
    private readonly BookSearchController _controller;

    public BookSearchControllerTests()
    {
        _openLibraryServiceMock = new Mock<IOpenLibraryService>();
        _controller = new BookSearchController(_openLibraryServiceMock.Object);
    }

    // SearchByTitle tests
    [Fact]
    public async Task SearchByTitle_ReturnsOk_WhenBooksFound()
    {
        var books = new List<BookSearchResult>
        {
            new("key1", "The Great Gatsby", ["F. Scott Fitzgerald"], 1925, null, [], [], [], null, null, null, "https://openlibrary.org/works/OL468431W")
        };
        _openLibraryServiceMock.Setup(s => s.SearchByTitleAsync("gatsby", It.IsAny<CancellationToken>())).ReturnsAsync(books);

        var result = await _controller.SearchByTitle("gatsby", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooks = Assert.IsType<List<BookSearchResult>>(okResult.Value);
        Assert.Single(returnedBooks);
    }

    [Fact]
    public async Task SearchByTitle_ReturnsOk_WithEmptyList_WhenNoBooksFound()
    {
        _openLibraryServiceMock.Setup(s => s.SearchByTitleAsync("xyzunknown", It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _controller.SearchByTitle("xyzunknown", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooks = Assert.IsType<List<BookSearchResult>>(okResult.Value);
        Assert.Empty(returnedBooks);
    }

    [Fact]
    public async Task SearchByTitle_ReturnsBadRequest_WhenBookNameIsEmpty()
    {
        var result = await _controller.SearchByTitle(string.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchByTitle_ReturnsBadRequest_WhenBookNameIsWhitespace()
    {
        var result = await _controller.SearchByTitle("   ", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchByTitle_ReturnsBadRequest_WhenBookNameIsThe()
    {
        var result = await _controller.SearchByTitle("the", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchByTitle_ReturnsBadRequest_WhenBookNameIsA()
    {
        var result = await _controller.SearchByTitle("a", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchByTitle_Returns502_WhenHttpRequestExceptionThrown()
    {
        _openLibraryServiceMock.Setup(s => s.SearchByTitleAsync("gatsby", It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException());

        var result = await _controller.SearchByTitle("gatsby", CancellationToken.None);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status502BadGateway, statusResult.StatusCode);
    }

    [Fact]
    public async Task SearchByTitle_Returns504_WhenTaskCanceledExceptionThrown()
    {
        _openLibraryServiceMock.Setup(s => s.SearchByTitleAsync("gatsby", It.IsAny<CancellationToken>())).ThrowsAsync(new TaskCanceledException());

        var result = await _controller.SearchByTitle("gatsby", CancellationToken.None);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status504GatewayTimeout, statusResult.StatusCode);
    }

    // SearchBySubject tests
    [Fact]
    public async Task SearchBySubject_ReturnsOk_WhenBooksFound()
    {
        var books = new List<BookSearchResult>
        {
            new("key1", "A Tale of Two Cities", ["Charles Dickens"], 1859, null, [], [], [], null, null, null, "https://openlibrary.org/works/OL1234W")
        };
        _openLibraryServiceMock.Setup(s => s.SearchBySubjectAsync("fiction", It.IsAny<CancellationToken>())).ReturnsAsync(books);

        var result = await _controller.SearchBySubject("fiction", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooks = Assert.IsType<List<BookSearchResult>>(okResult.Value);
        Assert.Single(returnedBooks);
    }

    [Fact]
    public async Task SearchBySubject_ReturnsOk_WithEmptyList_WhenNoBooksFound()
    {
        _openLibraryServiceMock.Setup(s => s.SearchBySubjectAsync("xyzunknown", It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _controller.SearchBySubject("xyzunknown", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooks = Assert.IsType<List<BookSearchResult>>(okResult.Value);
        Assert.Empty(returnedBooks);
    }

    [Fact]
    public async Task SearchBySubject_ReturnsBadRequest_WhenSubjectNameIsEmpty()
    {
        var result = await _controller.SearchBySubject(string.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchBySubject_ReturnsBadRequest_WhenSubjectNameIsWhitespace()
    {
        var result = await _controller.SearchBySubject("   ", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchBySubject_ReturnsBadRequest_WhenSubjectNameIsThe()
    {
        var result = await _controller.SearchBySubject("the", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchBySubject_ReturnsBadRequest_WhenSubjectNameIsA()
    {
        var result = await _controller.SearchBySubject("a", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchBySubject_Returns502_WhenHttpRequestExceptionThrown()
    {
        _openLibraryServiceMock.Setup(s => s.SearchBySubjectAsync("fiction", It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException());

        var result = await _controller.SearchBySubject("fiction", CancellationToken.None);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status502BadGateway, statusResult.StatusCode);
    }

    [Fact]
    public async Task SearchBySubject_Returns504_WhenTaskCanceledExceptionThrown()
    {
        _openLibraryServiceMock.Setup(s => s.SearchBySubjectAsync("fiction", It.IsAny<CancellationToken>())).ThrowsAsync(new TaskCanceledException());

        var result = await _controller.SearchBySubject("fiction", CancellationToken.None);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status504GatewayTimeout, statusResult.StatusCode);
    }
}
