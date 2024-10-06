using MediatR;
using UserManagementAPI.Core.Domain.Entities;
using UserManagementAPI.Core.Domain.Interfaces;

namespace UserManagementAPI.Core.Application.Queries;

public class GetUsersQuery : IRequest<IEnumerable<User>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int? Age { get; set; }
    public string? Country { get; set; }
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<User>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUsersAsync(request.Page, request.PageSize, request.Age, request.Country);
    }
}