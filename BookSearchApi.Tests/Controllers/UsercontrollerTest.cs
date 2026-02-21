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
        var testUserOne = new User() { Id = _testGuidOne, Name = _userOne };
        var testUserTwo = new User() { Id = _testGuidTwo, Name = _userTwo };

        var users = new List<User>
        {
            testUserOne,
            testUserTwo
        };
        _userServiceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var result = await _uut.GetAll(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsType<List<User>>(okResult.Value);
        Assert.Equal(2, returnedUsers.Count);
        Assert.Contains(testUserOne, returnedUsers);
        Assert.Contains(testUserTwo, returnedUsers);
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

    [Fact]
    public async Task Create_ReturnsConflict_WhenUserAlreadyExists()
    {
        // tell our user service to return null, indicating the username already exists
        _userServiceMock.Setup(s => s.CreateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var result = await _uut.Create(_userOne, CancellationToken.None);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }
}
