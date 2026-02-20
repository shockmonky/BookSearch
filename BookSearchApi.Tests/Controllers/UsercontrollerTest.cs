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
    // Our unit under test
    private readonly UserController _uut;
    private readonly Guid _testGuidOne = Guid.NewGuid();
    private readonly Guid _testGuidTwo = Guid.NewGuid();
    private readonly string _userOne = "userOne";
    private readonly string _userTwo = "userTwo";

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _uut = new UserController(_userServiceMock.Object);
    }

    // GetAll tests
    [Fact]
    public async Task GetAll_ReturnsOk_With_Existing_Users()
    {
        var users = new List<User>
        {
            new() { Id = _testGuidOne, Name = _userOne },
            new() { Id = _testGuidTwo, Name = _userTwo }
        };
        _userServiceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var result = await _uut.GetAll(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsType<List<User>>(okResult.Value);
        Assert.Equal(2, returnedUsers.Count);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_EmptyList_WhenNoUsers()
    {
        _userServiceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _uut.GetAll(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsType<List<User>>(okResult.Value);
        Assert.Empty(returnedUsers);
    }

    // Get tests
    [Fact]
    public async Task Get_ReturnsOk_WhenUserExists()
    {
        var user = new User { Id = _testGuidOne, Name = _userOne };
        _userServiceMock.Setup(s => s.GetByIdAsync(_testGuidOne, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _uut.Get(_testGuidOne, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal(_testGuidOne, returnedUser.Id);
        Assert.Equal(_userOne, returnedUser.Name);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _userServiceMock.Setup(s => s.GetByIdAsync(_testGuidOne, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await _uut.Get(_testGuidOne, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenUserIdIsGuidEmpty()
    {
        var result = await _uut.Get(Guid.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // Create tests
    [Fact]
    public async Task Create_ReturnsCreated_WhenUserIsCreated()
    {
        var user = new User { Id = _testGuidOne, Name = _userOne };
        _userServiceMock.Setup(s => s.CreateAsync(_userOne, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _uut.Create(_userOne, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedUser = Assert.IsType<User>(createdResult.Value);
        Assert.Equal(_testGuidOne, returnedUser.Id);
        Assert.Equal(_userOne, returnedUser.Name);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameIsEmpty()
    {
        var result = await _uut.Create(string.Empty, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameIsWhitespace()
    {
        var result = await _uut.Create("   ", CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
