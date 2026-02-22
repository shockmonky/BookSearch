// Take home project for Matthew Maffett

using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;

namespace BookSearchApi.Tests.Services;

public class OpenLibraryServiceTests
{
    private readonly Mock<ILogger<OpenLibraryService>> _loggerMock;
    private readonly string _testKey = "testKey1";
    private readonly string _testTitle = "testTitle1";
    private readonly string _testAuthor = "testAuthor1";
    private readonly List<OpenLibraryBook> _testBookList;
    private readonly OpenLibrarySearchResponse _testOpenLibResp;

    public OpenLibraryServiceTests()
    {
        _loggerMock = new Mock<ILogger<OpenLibraryService>>();
        _testBookList = new List<OpenLibraryBook> { new OpenLibraryBook(_testKey, _testTitle, [_testAuthor], null, null, null) };
        _testOpenLibResp = new OpenLibrarySearchResponse(_testBookList.Count, 0, _testBookList);
    }

    // Creates our service under test by passing in the pieces we needed to mock out
    private OpenLibraryService CreateService(HttpResponseMessage response)
    {
        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://openlibrary.org")
        };
        return new OpenLibraryService(httpClient, _loggerMock.Object);
    }

    // Helper function to create an http resp with a json body
    private static HttpResponseMessage CreateJsonResponse(object content)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(content), System.Text.Encoding.UTF8, "application/json")
        };
    }

    // SearchByTitleAsync tests
    [Fact]
    public async Task SearchByTitleAsync_ReturnsBooks_WhenBooksFound()
    {
        var service = CreateService(CreateJsonResponse(_testOpenLibResp));

        var result = await service.SearchByTitleAsync("gatsby", 1, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(_testTitle, result[0].Title);
    }

    [Fact]
    public async Task SearchByTitleAsync_ReturnsEmptyList_WhenNoBooksFound()
    {
        var service = CreateService(CreateJsonResponse(new OpenLibrarySearchResponse(0, 0, [])));

        var result = await service.SearchByTitleAsync("xyzunknown", 1, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchByTitleAsync_ReturnsEmptyList_WhenResponseIsNull()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json")
        };
        var service = CreateService(response);

        var result = await service.SearchByTitleAsync("gatsby", 1, CancellationToken.None);

        Assert.Empty(result);
    }

    // SearchBySubjectAsync tests
    [Fact]
    public async Task SearchBySubjectAsync_ReturnsBooks_WhenBooksFound()
    {
        var service = CreateService(CreateJsonResponse(_testOpenLibResp));

        var result = await service.SearchByTitleAsync(_testTitle, 1, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(_testTitle, result[0].Title);
    }

    [Fact]
    public async Task SearchBySubjectAsync_ReturnsEmptyList_WhenNoBooksFound()
    {
        var service = CreateService(CreateJsonResponse(new OpenLibrarySearchResponse(0, 0, [])));

        var result = await service.SearchBySubjectAsync("xyzunknown", CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchBySubjectAsync_ReturnsEmptyList_WhenResponseIsNull()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json")
        };
        var service = CreateService(response);

        var result = await service.SearchBySubjectAsync("fiction", CancellationToken.None);

        Assert.Empty(result);
    }

    // GetByKeyAsync tests
    [Fact]
    public async Task GetByKeyAsync_ReturnsBook_WhenExactKeyMatches()
    {
        var service = CreateService(CreateJsonResponse(_testOpenLibResp));

        var result = await service.GetByKeyAsync(_testKey, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(_testKey, result.Key);
    }

    [Fact]
    public async Task GetByKeyAsync_ReturnsFirstBook_WhenNoExactKeyMatch()
    {
        var books = new List<OpenLibraryBook>
        {
            new OpenLibraryBook(_testKey, _testTitle, [_testAuthor], null, null, null),
            new OpenLibraryBook("newWork1", "newTitle1", ["newAuthro1"], null, null, null)
        };

        var libResp = new OpenLibrarySearchResponse(books.Count, 0, books);
        var service = CreateService(CreateJsonResponse(libResp));

        var result = await service.GetByKeyAsync("keyThatsNotPresent", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(_testKey, result.Key);
    }

    [Fact]
    public async Task GetByKeyAsync_ReturnsNull_WhenResponseIsEmpty()
    {
        var service = CreateService(CreateJsonResponse(new OpenLibrarySearchResponse(0, 0, [])));

        var result = await service.GetByKeyAsync("/works/OL468431W", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByKeyAsync_ThrowsArgumentException_WhenKeyIsEmpty()
    {
        var service = CreateService(CreateJsonResponse(new OpenLibrarySearchResponse(0, 0, [])));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetByKeyAsync(string.Empty, CancellationToken.None));
    }

    [Fact]
    public async Task GetByKeyAsync_ThrowsArgumentException_WhenKeyIsWhitespace()
    {
        var service = CreateService(CreateJsonResponse(new OpenLibrarySearchResponse(0, 0, [])));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetByKeyAsync("   ", CancellationToken.None));
    }

    [Fact]
    public async Task GetByKeyAsync_ReturnsNull_WhenResponseIsNull()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json")
        };
        var service = CreateService(response);

        var result = await service.GetByKeyAsync("/works/OL468431W", CancellationToken.None);

        Assert.Null(result);
    }
}

public class MockHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(response);
    }
}
