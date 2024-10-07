using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagementAPI.Application.Models;
using UserManagementAPI.Controllers;
using UserManagementAPI.Core.Application.Commands;
using UserManagementAPI.Core.Application.Queries;
using UserManagementAPI.Core.Domain.Entities;
using UserManagementAPI.Domain.Exceptions;
using UserManagementAPI.Domain.Utils;

namespace UserManagementAPI.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new UserController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetUsers_ReturnsOkResult_WithPaginatedUsers()
    {
        // Arrange
        var user1 = new User();
        var user2 = new User();
        var expectedUsers = new PaginatedResult<User>
        {
            Items = new List<User> { user1, user2 },
            Page = 1,
            PageSize = 10,
            TotalCount = 2,
            TotalPages = 1
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUsersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _controller.GetUsers();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var users = okResult.Value.Should().BeAssignableTo<PaginatedResult<User>>().Subject;
        users.Should().BeEquivalentTo(expectedUsers);
    }

    [Fact]
    public async Task GetUserById_ReturnsOkResult_WhenUserExists()
    {
        // Arrange
        var expectedUser = new User();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.GetUserById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var user = okResult.Value.Should().BeAssignableTo<User>().Subject;
        user.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _controller.GetUserById(1);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedAtActionResult_WhenUserIsCreated()
    {
        // Arrange
        var userDto = new CreateUserDto();
        var createdUser = new User(1, "John", "Doe", 30, DateTime.UtcNow, "USA", "NY", "New York", "john@example.com", "hashedpassword");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _controller.CreateUser(userDto);

        // Assert
        var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(UserController.GetUserById));
        createdAtActionResult.RouteValues["id"].Should().Be(1);
        createdAtActionResult.Value.Should().BeEquivalentTo(createdUser);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOkResult_WhenUserIsUpdated()
    {
        // Arrange
        var command = new UpdateUserCommand { Id = 1 };
        var updatedUser = new User(1, "John", "Doe", 30, DateTime.UtcNow, "USA", "NY", "New York", "john@example.com", "hashedpassword");
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.UpdateUser(1, command);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var user = okResult.Value.Should().BeAssignableTo<User>().Subject;
        user.Should().BeEquivalentTo(updatedUser);
    }

    [Fact]
    public async Task UpdateUser_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var command = new UpdateUserCommand { Id = 2 };

        // Act
        var result = await _controller.UpdateUser(1, command);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new UpdateUserCommand { Id = 1 };
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act
        var result = await _controller.UpdateUser(1, command);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent_WhenUserIsDeleted()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteUser(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}