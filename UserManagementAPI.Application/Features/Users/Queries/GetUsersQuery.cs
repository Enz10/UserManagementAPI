using MediatR;
using UserManagementAPI.Core.Domain.Entities;
using UserManagementAPI.Core.Domain.Interfaces;
using UserManagementAPI.Domain.Utils;

namespace UserManagementAPI.Core.Application.Queries;

public class GetUsersQuery : IRequest<PaginatedResult<User>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int? Age { get; set; }
    public string? Country { get; set; }
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResult<User>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PaginatedResult<User>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUsersAsync(request.Page, request.PageSize, request.Age, request.Country);
    }
}