using MediatR;
using UserManagementAPI.Core.Domain.Entities;
using UserManagementAPI.Core.Domain.Interfaces;

namespace UserManagementAPI.Application.Features.Users.Queries;

public class GetUserByEmailQuery : IRequest<User>
{
    public string Email { get; set; }
}

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, User>
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        return user?.DeletedAt == null ? user : null;
    }
}