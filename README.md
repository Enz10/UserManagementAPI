# User Management API

This project is a User Management API built with ASP.NET Core 6.0, implementing Clean Architecture principles and using MediatR for CQRS pattern.

## Prerequisites

- .NET 6.0 SDK
- SQL Server (LocalDB or a full instance)
- Visual Studio 2022 or Visual Studio Code

## Project Structure

The solution consists of four projects:

1. UserManagementAPI (Web API)
2. UserManagementAPI.Application (Application logic)
3. UserManagementAPI.Domain (Domain models and interfaces)
4. UserManagementAPI.Infrastructure (Data access and external services)

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio or Visual Studio Code
3. Update the connection string in appsettings.json if necessary:
  ```
  "ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mylocaldb;Database=UserManagementDB;Trusted_Connection=True;"
  }
  ```


4. Set up the database:
   - Navigate to the `.scripts` folder at the root of the project
   - Execute the SQL scripts in the following order:
     1. Create the Users table
     2. Create any necessary types
     3. Create the stored procedures

   You can do this by:
   - Using SQL Server Management Studio (SSMS) and executing the scripts manually
   - Using the `sqlcmd` utility from the command line
   - Using Visual Studio's SQL Server Object Explorer

5. Build and run the project

## Features

- User registration and authentication
- JWT-based authentication
- CRUD operations for users
- Pagination and filtering for user listing
- Bulk user creation

## API Endpoints

- POST /api/auth/login - User login
- GET /api/user - Get all users (with pagination and filtering)
- GET /api/user/{id} - Get user by ID
- POST /api/user - Create a new user
- POST /api/user/bulk-create - Create multiple users (up to 1000)
- PUT /api/user/{id} - Update an existing user
- DELETE /api/user/{id} - Delete a user

## Authentication

The API uses JWT for authentication. To access protected endpoints, include the JWT token in the Authorization header:
```
Authorization: Bearer <your_token_here>
```



## Testing

The project includes unit tests for controllers. To run the tests:

1. Open Test Explorer in Visual Studio
2. Click "Run All Tests" or run individual test classes

## Configuration

JWT settings can be configured in appsettings.json:
```
"JwtSettings": {
"Secret": "YourSecretKeyHere",
"ExpirationInMinutes": 60
}
```
