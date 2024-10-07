using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagementAPI.Application.Features.Users.Queries;
using UserManagementAPI.Application.Models;
using UserManagementAPI.Controllers;
using UserManagementAPI.Core.Domain.Entities;
using UserManagementAPI.Core.Domain.Interfaces;

namespace UserManagementAPI.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_mediatorMock.Object, _authServiceMock.Object);
    }

    [Fact]
    public async Task Login_ReturnsOkResult_WithToken_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "password" };
        var user = new User(
            id: 1,
            firstName: "Test",
            lastName: "User",
            age: 30,
            createdAt: DateTime.UtcNow,
            country: "TestCountry",
            province: "TestProvince",
            city: "TestCity",
            email: "test@example.com",
            passwordHash: "hashedPassword"
        );
        var token = "validToken";

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _authServiceMock.Setup(a => a.ValidatePassword(loginDto.Password, user.PasswordHash))
            .Returns(true);
        _authServiceMock.Setup(a => a.GenerateJwtToken(user))
            .Returns(token);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var tokenResult = okResult.Value.Should().BeAssignableTo<object>().Subject;
        tokenResult.Should().BeEquivalentTo(new { Token = token });
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenUserDoesNotExist()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "password" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().BeEquivalentTo(new { message = "Invalid email or password" });
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenPasswordIsInvalid()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "password" };
        var user = new User(
            id: 1,
            firstName: "Test",
            lastName: "User",
            age: 30,
            createdAt: DateTime.UtcNow,
            country: "TestCountry",
            province: "TestProvince",
            city: "TestCity",
            email: "test@example.com",
            passwordHash: "hashedPassword"
        );

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _authServiceMock.Setup(a => a.ValidatePassword(loginDto.Password, user.PasswordHash))
            .Returns(false);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().BeEquivalentTo(new { message = "Invalid email or password" });
    }
}