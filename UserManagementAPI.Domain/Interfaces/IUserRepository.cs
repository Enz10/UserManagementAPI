using UserManagementAPI.Core.Domain.Entities;
using UserManagementAPI.Domain.Utils;

namespace UserManagementAPI.Core.Domain.Interfaces;

public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<PaginatedResult<User>> GetUsersAsync(int page, int pageSize, int? age, string country);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<IEnumerable<User>> CreateUsersAsync(IEnumerable<User> users);
    Task<User> UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
}