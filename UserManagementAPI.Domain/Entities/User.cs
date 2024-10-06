namespace UserManagementAPI.Core.Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public int Age { get; private set; }
    public string Country { get; private set; }
    public string Province { get; private set; }
    public string City { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User() { }

    public User(string firstName, string lastName, int age, DateTime createdAt, string country, string province, string city, string email, string passwordHash)
        : this(0, firstName, lastName, age, createdAt, country, province, city, email, passwordHash)
    { }

    public User(int id, string firstName, string lastName, int age, DateTime createdAt, string country, string province, string city, string email, string passwordHash)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        CreatedAt = createdAt;
        Country = country;
        Province = province;
        City = city;
        Email = email;
        PasswordHash = passwordHash;
    }
}