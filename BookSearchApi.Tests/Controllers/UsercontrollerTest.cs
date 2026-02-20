// Take home project for Matthew Maffett

using BookSearchApi.Controllers;
using BookSearchApi.Models;
using BookSearchApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookSearchApi.Tests;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new UserController(_userServiceMock.Object);
    }

    // GetAll tests
    [Fact]
    public async Task GetAll_ReturnsOk_WithListOfUsers()
    {
        var users = new List<User>
        {
            new() { Id = "user-1", Name = "Matthew" },
            new() { Id = "user-2", Name = "John" }
        };
        _userServiceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var result = await _controller.GetAll(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsType<List<User>>(okResult.Value);
        Assert.Equal(2, returnedUsers.Count);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithEmptyList_WhenNoUsers()
    {
        _userServiceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _controller.GetAll(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsType<List<User>>(okResult.Value);
        Assert.Empty(returnedUsers);
    }

    // Get tests
    [Fact]
    public async Task Get_ReturnsOk_WhenUserExists()
    {
        var user = new User { Id = "user-1", Name = "Matthew" };
        _userServiceMock.Setup(s => s.GetByIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _controller.Get("user-1", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal("user-1", returnedUser.Id);
        Assert.Equal("Matthew", returnedUser.Name);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _userServiceMock.Setup(s => s.GetByIdAsync("user-999", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await _controller.Get("user-999", CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenUserIdIsEmpty()
    {
        var result = await _controller.Get(string.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenUserIdIsWhitespace()
    {
        var result = await _controller.Get("   ", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // Create tests
    [Fact]
    public async Task Create_ReturnsCreated_WhenUserIsCreated()
    {
        var user = new User { Id = "user-1", Name = "Matthew" };
        _userServiceMock.Setup(s => s.CreateAsync("user-1", "Matthew", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _controller.Create("user-1", "Matthew", CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedUser = Assert.IsType<User>(createdResult.Value);
        Assert.Equal("user-1", returnedUser.Id);
        Assert.Equal("Matthew", returnedUser.Name);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenUserIdIsEmpty()
    {
        var result = await _controller.Create(string.Empty, "Matthew", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameIsEmpty()
    {
        var result = await _controller.Create("user-1", string.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameIsWhitespace()
    {
        var result = await _controller.Create("user-1", "   ", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
