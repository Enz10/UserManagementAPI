using MediatR;
using UserManagementAPI.Application.Models;
using UserManagementAPI.Core.Domain.Entities;
using UserManagementAPI.Core.Domain.Interfaces;

namespace UserManagementAPI.Core.Application.Commands;

public class CreateUserCommand : IRequest<User>
{
    public CreateUserDto UserDto { get; set; }

    public CreateUserCommand(CreateUserDto userDto)
    {
        UserDto = userDto;
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.UserDto;
        var passwordHash = _passwordHasher.HashPassword(dto.Password);
        var user = new User(dto.FirstName, dto.LastName, dto.Age, DateTime.UtcNow, dto.Country, dto.Province, dto.City, dto.Email, passwordHash);
        return await _userRepository.CreateUserAsync(user);
    }
}