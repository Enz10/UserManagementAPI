using MediatR;
using UserManagementAPI.Core.Domain.Entities;
using UserManagementAPI.Core.Domain.Interfaces;
using UserManagementAPI.Domain.Exceptions;

namespace UserManagementAPI.Core.Application.Commands;

public class UpdateUserCommand : IRequest<User>
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? Age { get; set; }
    public string? Country { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? Email { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {request.Id} not found.");
        }

        user.Update(
            request.FirstName ?? user.FirstName,
            request.LastName ?? user.LastName,
            request.Age ?? user.Age,
            request.Country ?? user.Country,
            request.Province ?? user.Province,
            request.City ?? user.City,
            request.Email ?? user.Email
        );

        return await _userRepository.UpdateUserAsync(user);
    }
}