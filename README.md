# Sculptor

Sculptor is an object-relational mapper (ORM) for .NET, providing an expressive query builder, asynchronous support, and an implementation of the [Active Record Pattern](https://en.wikipedia.org/wiki/Active_record_pattern).

Sculptor's goal is not only to be an ORM but to become a full database toolkit for .NET by providing a schema builder in the future.

Sculptor is still under heavy development and not meant to be used in production for now.

## Usage instructions

```csharp
public class User : Model<User>
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

// Add a new connection
Manager.Connections.Add("default", new MySqlConnection("localhost", "database", "root", "secret"));

// Retrieve a user
User user = User.First();
user.Password = "secret";
user.Save();

// Retrieve a user using the query builder for more complex queries
User user = User.Query.Select("email", "password").Where("email", "john.doe@example.com").First();

// Create a new user
User user = new User();
user.Email = "john.doe@example.com";
user.Password = "secret";
user.Save();

// Asynchronous support
User user = User.FindAsync(42);
user.Password = "secret";
user.SaveAsync();
```

## Features

- [x] Database providers
    - [x] MySQL ([MySqlConnector](https://mysqlconnector.net/))
    - [ ] PostgreSQL
    - [ ] SQLite
    - [ ] SQL Server
- [ ] Relationships
    - [ ] One To One
    - [ ] One To Many
    - [ ] Many To Many
- [ ] Migrations

## License

Sculptor is open-sourced software licensed under the [MIT license](https://opensource.org/licenses/MIT).