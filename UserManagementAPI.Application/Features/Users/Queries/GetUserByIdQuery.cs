using MediatR;
using UserManagementAPI.Core.Domain.Entities;
using UserManagementAPI.Core.Domain.Interfaces;

namespace UserManagementAPI.Core.Application.Queries;

public class GetUserByIdQuery : IRequest<User>
{
    public int Id { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        return user?.DeletedAt == null ? user : null;
    }
}