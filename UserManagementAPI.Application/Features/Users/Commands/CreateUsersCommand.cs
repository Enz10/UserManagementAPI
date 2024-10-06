using MediatR;
using UserManagementAPI.Application.Models;
using UserManagementAPI.Core.Domain.Entities;
using UserManagementAPI.Core.Domain.Interfaces;

namespace UserManagementAPI.Core.Application.Commands;

public class CreateUsersCommand : IRequest<IEnumerable<User>>
{
    public IEnumerable<CreateUserDto> Users { get; set; }
}

public class CreateUsersCommandHandler : IRequestHandler<CreateUsersCommand, IEnumerable<User>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUsersCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<User>> Handle(CreateUsersCommand request, CancellationToken cancellationToken)
    {
        var users = await Task.WhenAll(request.Users.Select(async u =>
        {
            var passwordHash = await Task.Run(() => _passwordHasher.HashPassword(u.Password));
            return new User(
                u.FirstName,
                u.LastName,
                u.Age,
                DateTime.UtcNow,
                u.Country,
                u.Province,
                u.City,
                u.Email,
                passwordHash
            );
        }));

        return await _userRepository.CreateUsersAsync(users);
    }
}