// Take home project for Matthew Maffett

using BookSearchApi.Controllers;
using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookSearchApi.Tests;

public class BookControllerTests
{
    private readonly Mock<IOpenLibraryService> _openLibraryServiceMock;
    // Our unit under test
    private readonly BookController _uut;
    private readonly string _testKey = "testKey1";
    private readonly string _testTitle = "testTitle1";
    private readonly string _testAuthor = "testAuthor1";
    private readonly string _testSubject = "testSubkect1";
    private readonly string _testOpenLibLink = "testOpenLink.com";

    public BookControllerTests()
    {
        _openLibraryServiceMock = new Mock<IOpenLibraryService>();
        _uut = new BookController(_openLibraryServiceMock.Object);
    }

    // SearchByTitle tests
    [Fact]
    public async Task SearchByTitle_ReturnsOk_WhenBooksFound()
    {
        var books = new List<BookSearchResult>
        {
             new(_testKey, _testTitle, [_testAuthor], null, [], [], _testOpenLibLink)
        };
        _openLibraryServiceMock.Setup(s => s.SearchByTitleAsync(It.IsAny<String>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(books);

        var result = await _uut.SearchByTitle(_testTitle, 1, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooks = Assert.IsType<List<BookSearchResult>>(okResult.Value);
        Assert.Single(returnedBooks);

        var testBook = returnedBooks[0];
        Assert.Equal(testBook.Key, _testKey);
        Assert.Equal(testBook.Title, _testTitle);
        Assert.Equal(testBook.OpenLibraryUrl, _testOpenLibLink);
    }

    [Fact]
    public async Task SearchByTitle_ReturnsOk_WithEmptyList_WhenNoBooksFound()
    {
        _openLibraryServiceMock.Setup(s => s.SearchByTitleAsync(It.IsAny<String>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _uut.SearchByTitle(_testTitle, 1, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooks = Assert.IsType<List<BookSearchResult>>(okResult.Value);
        Assert.Empty(returnedBooks);
    }

    [Fact]
    public async Task SearchByTitle_ReturnsBadRequest_WhenBookNameIsEmpty()
    {
        var result = await _uut.SearchByTitle(string.Empty, 1, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchByTitle_ReturnsBadRequest_WhenBookNameIsWhitespace()
    {
        var result = await _uut.SearchByTitle("   ", 1, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchByTitle_ReturnsBadRequest_WhenBookNameIsThe()
    {
        var result = await _uut.SearchByTitle("the", 1, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchByTitle_ReturnsBadRequest_WhenBookNameIsA()
    {
        var result = await _uut.SearchByTitle("a", 1, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchByTitle_ReturnsBadRequest_WhenLimitIsZero()
    {
        var result = await _uut.SearchByTitle(_testTitle, 0, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchByTitle_ReturnsBadRequest_WhenLimitExcedesHundred()
    {
        var result = await _uut.SearchByTitle(_testTitle, 101, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchByTitle_Returns502_WhenHttpRequestExceptionThrown()
    {
        _openLibraryServiceMock.Setup(s => s.SearchByTitleAsync(It.IsAny<String>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException());

        var result = await _uut.SearchByTitle(_testTitle, 1, CancellationToken.None);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status502BadGateway, statusResult.StatusCode);
    }

    [Fact]
    public async Task SearchByTitle_Returns504_WhenTaskCanceledExceptionThrown()
    {
        _openLibraryServiceMock.Setup(s => s.SearchByTitleAsync(It.IsAny<String>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new TaskCanceledException());

        var result = await _uut.SearchByTitle(_testTitle, 1, CancellationToken.None);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status504GatewayTimeout, statusResult.StatusCode);
    }

    // SearchBySubject tests
    [Fact]
    public async Task SearchBySubject_ReturnsOk_WhenBooksFound()
    {
        var books = new List<BookSearchResult>
        {
            new(_testKey, _testTitle, [_testAuthor], null, [], [], _testOpenLibLink)
        };
        _openLibraryServiceMock.Setup(s => s.SearchBySubjectAsync(It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(books);

        var result = await _uut.SearchBySubject(_testSubject, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooks = Assert.IsType<List<BookSearchResult>>(okResult.Value);
        Assert.Single(returnedBooks);
    }

    [Fact]
    public async Task SearchBySubject_ReturnsOk_WithEmptyList_WhenNoBooksFound()
    {
        _openLibraryServiceMock.Setup(s => s.SearchBySubjectAsync(It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _uut.SearchBySubject(_testSubject, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBooks = Assert.IsType<List<BookSearchResult>>(okResult.Value);
        Assert.Empty(returnedBooks);
    }

    [Fact]
    public async Task SearchBySubject_ReturnsBadRequest_WhenSubjectNameIsEmpty()
    {
        var result = await _uut.SearchBySubject(string.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchBySubject_ReturnsBadRequest_WhenSubjectNameIsWhitespace()
    {
        var result = await _uut.SearchBySubject("   ", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchBySubject_ReturnsBadRequest_WhenSubjectNameIsThe()
    {
        var result = await _uut.SearchBySubject("the", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchBySubject_ReturnsBadRequest_WhenSubjectNameIsA()
    {
        var result = await _uut.SearchBySubject("a", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task SearchBySubject_Returns502_WhenHttpRequestExceptionThrown()
    {
        _openLibraryServiceMock.Setup(s => s.SearchBySubjectAsync(It.IsAny<String>(), It.IsAny<CancellationToken>())).ThrowsAsync(new HttpRequestException());

        var result = await _uut.SearchBySubject(_testSubject, CancellationToken.None);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status502BadGateway, statusResult.StatusCode);
    }

    [Fact]
    public async Task SearchBySubject_Returns504_WhenTaskCanceledExceptionThrown()
    {
        _openLibraryServiceMock.Setup(s => s.SearchBySubjectAsync(It.IsAny<String>(), It.IsAny<CancellationToken>())).ThrowsAsync(new TaskCanceledException());

        var result = await _uut.SearchBySubject(_testSubject, CancellationToken.None);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status504GatewayTimeout, statusResult.StatusCode);
    }
}
