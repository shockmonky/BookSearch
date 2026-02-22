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

    public OpenLibraryServiceTests()
    {
        _loggerMock = new Mock<ILogger<OpenLibraryService>>();
    }

    private OpenLibraryService CreateService(HttpResponseMessage response)
    {
        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://openlibrary.org")
        };
        return new OpenLibraryService(httpClient, _loggerMock.Object);
    }

    private static HttpResponseMessage CreateJsonResponse(object content)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(content), System.Text.Encoding.UTF8, "application/json")
        };
    }

    private static OpenLibrarySearchResponse CreateSearchResponse(List<OpenLibraryBook> docs)
    {
        return new OpenLibrarySearchResponse(docs.Count, 0, docs);
    }

    private static OpenLibraryBook CreateOpenLibraryBook(string key = "/works/OL468431W", string title = "The Great Gatsby")
    {
        return new OpenLibraryBook(
            Key: key,
            Title: title,
            AuthorNames: ["F. Scott Fitzgerald"],
            Isbn: ["9780743273565"],
            Languages: ["eng"],
            Subjects: ["Fiction"]);
    }

    // SearchByTitleAsync tests
    [Fact]
    public async Task SearchByTitleAsync_ReturnsBooks_WhenBooksFound()
    {
        var books = new List<OpenLibraryBook> { CreateOpenLibraryBook() };
        var service = CreateService(CreateJsonResponse(CreateSearchResponse(books)));

        var result = await service.SearchByTitleAsync("gatsby", 1, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("The Great Gatsby", result[0].Title);
    }

    [Fact]
    public async Task SearchByTitleAsync_ReturnsEmptyList_WhenNoBooksFound()
    {
        var service = CreateService(CreateJsonResponse(CreateSearchResponse([])));

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

    [Fact]
    public async Task SearchByTitleAsync_MapsOpenLibraryUrl_Correctly()
    {
        var books = new List<OpenLibraryBook> { CreateOpenLibraryBook() };
        var service = CreateService(CreateJsonResponse(CreateSearchResponse(books)));

        var result = await service.SearchByTitleAsync("gatsby", 1, CancellationToken.None);

        Assert.Equal("https://openlibrary.org/works/OL468431W", result[0].OpenLibraryUrl);
    }

    // SearchBySubjectAsync tests
    [Fact]
    public async Task SearchBySubjectAsync_ReturnsBooks_WhenBooksFound()
    {
        var books = new List<OpenLibraryBook> { CreateOpenLibraryBook() };
        var service = CreateService(CreateJsonResponse(CreateSearchResponse(books)));

        var result = await service.SearchByTitleAsync("gatsby", 1, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("The Great Gatsby", result[0].Title);
    }

    [Fact]
    public async Task SearchBySubjectAsync_ReturnsEmptyList_WhenNoBooksFound()
    {
        var service = CreateService(CreateJsonResponse(CreateSearchResponse([])));

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
        var books = new List<OpenLibraryBook> { CreateOpenLibraryBook() };
        var service = CreateService(CreateJsonResponse(CreateSearchResponse(books)));

        var result = await service.GetByKeyAsync("/works/OL468431W", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("/works/OL468431W", result.Key);
    }

    [Fact]
    public async Task GetByKeyAsync_ReturnsFirstBook_WhenNoExactKeyMatch()
    {
        var books = new List<OpenLibraryBook>
        {
            CreateOpenLibraryBook("/works/OL999999W", "Some Other Book"),
            CreateOpenLibraryBook("/works/OL468431W", "The Great Gatsby")
        };
        var service = CreateService(CreateJsonResponse(CreateSearchResponse(books)));

        var result = await service.GetByKeyAsync("/works/OL111111W", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("/works/OL999999W", result.Key);
    }

    [Fact]
    public async Task GetByKeyAsync_ReturnsNull_WhenResponseIsEmpty()
    {
        var service = CreateService(CreateJsonResponse(CreateSearchResponse([])));

        var result = await service.GetByKeyAsync("/works/OL468431W", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByKeyAsync_ThrowsArgumentException_WhenKeyIsEmpty()
    {
        var service = CreateService(CreateJsonResponse(CreateSearchResponse([])));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetByKeyAsync(string.Empty, CancellationToken.None));
    }

    [Fact]
    public async Task GetByKeyAsync_ThrowsArgumentException_WhenKeyIsWhitespace()
    {
        var service = CreateService(CreateJsonResponse(CreateSearchResponse([])));

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
