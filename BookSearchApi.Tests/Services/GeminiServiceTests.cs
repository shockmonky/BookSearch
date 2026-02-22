using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;

namespace BookSearchApi.Tests.Services;

public class GeminiServiceTests
{
    private readonly Mock<ILogger<GeminiService>> _loggerMock;
    private readonly IConfiguration _configuration;
    private readonly string _testUrl = "https://atestUrl.com";
    private readonly string _testKey = "testKey1";
    private readonly string _testTitle = "testTitle1";
    private readonly string _testAuthor = "testAuthor1";
    private readonly List<OpenLibraryBook> _testBookList;

    public GeminiServiceTests()
    {
        _loggerMock = new Mock<ILogger<GeminiService>>();
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Gemini:ApiKey", "test-api-key" }
            })
            .Build();

        _testBookList = new List<OpenLibraryBook> { new OpenLibraryBook(_testKey, _testTitle, [_testAuthor], null, null, null) };
    }

    // helper function to setup service under test
    private GeminiService CreateService(HttpResponseMessage response)
    {
        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(_testUrl)
        };
        return new GeminiService(httpClient, _configuration, _loggerMock.Object);
    }

    // helper function to create a valid http resp
    private static HttpResponseMessage CreateSuccessResponse()
    {
        var geminiResponse = new GeminiResponse(
        [
            new GeminiCandidate(
                new GeminiContent(
                [
                    new GeminiPart("A summary of the books.")
                ]))
        ]);

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(geminiResponse), System.Text.Encoding.UTF8, "application/json")
        };
    }

    // SummarizeBooksAsync tests
    [Fact]
    public async Task SummarizeBooksAsync_ReturnsSuccessResponse_WhenApiCallSucceeds()
    {
        var service = CreateService(CreateSuccessResponse());

        var result = await service.SummarizeBooksAsync(_testBookList, CancellationToken.None);

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task SummarizeBooksAsync_ReturnsOkStatusCode_WhenApiCallSucceeds()
    {
        var service = CreateService(CreateSuccessResponse());

        var result = await service.SummarizeBooksAsync(_testBookList, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task SummarizeBooksAsync_ReturnsTooManyRequests_WhenRateLimitHit()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.TooManyRequests));

        var result = await service.SummarizeBooksAsync(_testBookList, CancellationToken.None);

        Assert.Equal(HttpStatusCode.TooManyRequests, result.StatusCode);
    }

    [Fact]
    public async Task SummarizeBooksAsync_ReturnsNotFound_WhenModelNotFound()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.NotFound));

        var result = await service.SummarizeBooksAsync(_testBookList, CancellationToken.None);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task SummarizeBooksAsync_ReturnsUnauthorized_WhenApiKeyIsInvalid()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        var result = await service.SummarizeBooksAsync(_testBookList, CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task SummarizeBooksAsync_HandlesMultipleBooks()
    {
        var service = CreateService(CreateSuccessResponse());

        var result = await service.SummarizeBooksAsync(_testBookList, CancellationToken.None);

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task SummarizeBooksAsync_HandlesBooksWithNoAuthors()
    {
        var books = new List<OpenLibraryBook>
        {
            new("/works/OL468431W", "The Great Gatsby", null, null, null, null)
        };
        var service = CreateService(CreateSuccessResponse());

        var result = await service.SummarizeBooksAsync(books, CancellationToken.None);

        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task SummarizeBooksAsync_ReturnsInternalServerError_WhenApiErrors()
    {
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var result = await service.SummarizeBooksAsync(_testBookList, CancellationToken.None);

        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
    }
}
