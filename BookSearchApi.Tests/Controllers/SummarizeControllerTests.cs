// Take home project for Matthew Maffett

using BookSearchApi.Controllers;
using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace BookSearchApi.Tests.Controllers;

public class SummarizeControllerTests
{
    private readonly Mock<IGeminiService> _geminiServiceMock;
    private readonly Mock<ILogger<SummarizeController>> _loggerMock;
    private readonly SummarizeController _controller;
    private readonly string _testKey = "testKey1";
    private readonly string _testTitle = "testTitle1";
    private readonly string _testAuthor = "testAuthor1";
    private readonly string _testGeminiRespText = "testSummaryFromGemini";
    private readonly List<OpenLibraryBook> _testBookList;

    public SummarizeControllerTests()
    {
        _geminiServiceMock = new Mock<IGeminiService>();
        _loggerMock = new Mock<ILogger<SummarizeController>>();
        _controller = new SummarizeController(_geminiServiceMock.Object, _loggerMock.Object);

        _testBookList = new List<OpenLibraryBook> { new OpenLibraryBook(_testKey, _testTitle, [_testAuthor], null, null, null) };
    }

    // Summarize tests
    [Fact]
    public async Task Summarize_ReturnsOk_WhenGeminiReqSuccessful()
    {
        var geminiResp = new GeminiResponse(
        [
            new GeminiCandidate(
                new GeminiContent(
                [
                    new GeminiPart(_testGeminiRespText)
                ]))
        ]);

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(geminiResp), Encoding.UTF8, "application/json")
        };

        _geminiServiceMock.Setup(s => s.SummarizeBooksAsync(_testBookList, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.Summarize(_testBookList, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(_testGeminiRespText, okResult.Value);
    }

    [Fact]
    public async Task Summarize_ReturnsBadRequest_WhenInputIsNull()
    {
        var result = await _controller.Summarize(null!, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Summarize_ReturnsBadRequest_WhenInputIsEmpty()
    {
        var result = await _controller.Summarize([], CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Summarize_Returns502_WhenGeminiReturnsError()
    {
        var errResp = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _geminiServiceMock.Setup(s => s.SummarizeBooksAsync(_testBookList, It.IsAny<CancellationToken>()))
            .ReturnsAsync(errResp);

        var result = await _controller.Summarize(_testBookList, CancellationToken.None);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status502BadGateway, statusResult.StatusCode);
    }

    [Fact]
    public async Task Summarize_Returns502_WhenGeminiReturnsTooManyRequests()
    {
        var errResp = new HttpResponseMessage(HttpStatusCode.TooManyRequests);

        _geminiServiceMock.Setup(s => s.SummarizeBooksAsync(_testBookList, It.IsAny<CancellationToken>()))
            .ReturnsAsync(errResp);

        var result = await _controller.Summarize(_testBookList, CancellationToken.None);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status502BadGateway, statusResult.StatusCode);
    }

    [Fact]
    public async Task Summarize_Returns400_WhenSummaryIsNull()
    {
        var emptyGeminiResponse = new GeminiResponse(null);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(emptyGeminiResponse), Encoding.UTF8, "application/json")
        };
        _geminiServiceMock.Setup(s => s.SummarizeBooksAsync(_testBookList, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.Summarize(_testBookList, CancellationToken.None);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, statusResult.StatusCode);
    }
}
