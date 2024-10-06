using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using UserManagementAPI.Core.Domain.Entities;
using UserManagementAPI.Core.Domain.Interfaces;
using UserManagementAPI.Domain.Utils;

namespace UserManagementAPI.Infrastructure.Data;

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<User> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Id = @Id", new { Id = id });
    }

    public async Task<PaginatedResult<User>> GetUsersAsync(int page, int pageSize, int? age, string country)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var parameters = new DynamicParameters();
        parameters.Add("@Page", page);
        parameters.Add("@PageSize", pageSize);
        parameters.Add("@Age", age, DbType.Int32, direction: ParameterDirection.Input);
        parameters.Add("@Country", country);
        parameters.Add("@TotalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@TotalPages", dbType: DbType.Int32, direction: ParameterDirection.Output);

        var users = await connection.QueryAsync<User>(
            "sp_GetUsers",
            parameters,
            commandType: CommandType.StoredProcedure);

        var totalCount = parameters.Get<int>("@TotalCount");
        var totalPages = parameters.Get<int>("@TotalPages");

        return new PaginatedResult<User>
        {
            Items = users,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalCount = totalCount
        };
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email", new { Email = email });
    }

    public async Task<User> CreateUserAsync(User user)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var id = await connection.QuerySingleAsync<int>(
            @"INSERT INTO Users (FirstName, LastName, Age, CreatedAt, Country, Province, City, Email, PasswordHash) 
                      VALUES (@FirstName, @LastName, @Age, @CreatedAt, @Country, @Province, @City, @Email, @PasswordHash);
                      SELECT CAST(SCOPE_IDENTITY() as int)",
            new { user.FirstName, user.LastName, user.Age, user.CreatedAt, user.Country, user.Province, user.City, user.Email, user.PasswordHash });

        return new User(id, user.FirstName, user.LastName, user.Age, user.CreatedAt, user.Country, user.Province, user.City, user.Email, user.PasswordHash);
    }

    public async Task<IEnumerable<User>> CreateUsersAsync(IEnumerable<User> users)
    {
        var usersList = users.ToList();
        if (usersList.Count == 0 || usersList.Count > 1000)
        {
            throw new ArgumentException("Number of users must be between 1 and 1000.");
        }

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var dataTable = new DataTable();
        dataTable.Columns.Add("FirstName", typeof(string));
        dataTable.Columns.Add("LastName", typeof(string));
        dataTable.Columns.Add("Age", typeof(int));
        dataTable.Columns.Add("CreatedAt", typeof(DateTime));
        dataTable.Columns.Add("Country", typeof(string));
        dataTable.Columns.Add("Province", typeof(string));
        dataTable.Columns.Add("City", typeof(string));
        dataTable.Columns.Add("Email", typeof(string));
        dataTable.Columns.Add("PasswordHash", typeof(string));

        foreach (var user in usersList)
        {
            dataTable.Rows.Add(user.FirstName, user.LastName, user.Age, user.CreatedAt, user.Country,
                user.Province, user.City, user.Email, user.PasswordHash);
        }

        using (var bulkCopy = new SqlBulkCopy(connection,
            SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.UseInternalTransaction, null))
        {
            bulkCopy.DestinationTableName = "Users";
            bulkCopy.BatchSize = usersList.Count;
            bulkCopy.BulkCopyTimeout = 60;

            bulkCopy.ColumnMappings.Add("FirstName", "FirstName");
            bulkCopy.ColumnMappings.Add("LastName", "LastName");
            bulkCopy.ColumnMappings.Add("Age", "Age");
            bulkCopy.ColumnMappings.Add("CreatedAt", "CreatedAt");
            bulkCopy.ColumnMappings.Add("Country", "Country");
            bulkCopy.ColumnMappings.Add("Province", "Province");
            bulkCopy.ColumnMappings.Add("City", "City");
            bulkCopy.ColumnMappings.Add("Email", "Email");
            bulkCopy.ColumnMappings.Add("PasswordHash", "PasswordHash");

            await bulkCopy.WriteToServerAsync(dataTable);
        }

        // Retrieve the inserted users
        var insertedUsers = await connection.QueryAsync<User>(
            @"SELECT TOP (@Count) Id, FirstName, LastName, Age, CreatedAt, Country, Province, City, Email, PasswordHash 
          FROM Users 
          ORDER BY Id DESC",
            new { Count = usersList.Count }
        );

        return insertedUsers;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync(
            @"UPDATE Users 
                      SET FirstName = @FirstName, LastName = @LastName, Age = @Age, CreatedAt = @CreatedAt, 
                          Country = @Country, Province = @Province, City = @City, Email = @Email, PasswordHash = @PasswordHash 
                      WHERE Id = @Id",
            user);
        return user;
    }

    public async Task DeleteUserAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { Id = id });
    }
}