using UserManagementAPI.Core.Domain.Entities;

namespace UserManagementAPI.Core.Domain.Interfaces;

public interface IAuthService
{
    string GenerateJwtToken(User user);
    bool ValidatePassword(string password, string hashedPassword);
}