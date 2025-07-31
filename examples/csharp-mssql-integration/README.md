# Example 1: Integration Testing against MSSQL Database using C#

This example demonstrates how to implement effective integration testing for a .NET application using a real SQL Server database running in a container via TestContainers.NET.

## Overview

The example implements a simple ecommerce shopping cart API that showcases:

- **Modern C# and .NET 8** technology stack
- **Entity Framework Core** for data access with SQL Server
- **TestContainers.NET** for integration testing with a real MSSQL database in containers
- **xUnit.net** as the test framework
- **GitHub Actions** for CI/CD pipeline

## Architecture

```
src/EcommerceShop.Api/
├── Controllers/         # API Controllers
├── Models/             # Domain models and DTOs
├── Services/           # Business logic services
└── Data/               # Entity Framework DbContext

tests/EcommerceShop.IntegrationTests/
├── IntegrationTestWebAppFactory.cs    # Test infrastructure
└── ShoppingCartControllerTests.cs     # Integration tests
```

## Domain Model

The application models a simple ecommerce scenario:

- **User**: Represents a customer with username and email
- **Product**: Represents items available for purchase
- **ShoppingCartItem**: Links users to products with quantities

## API Endpoints

### GET /api/shoppingcart/{userId}

Returns the shopping cart data for a specific user including:
- User information
- List of cart items with product details
- Total amount and item count

**Example Response:**
```json
{
  "userId": 1,
  "username": "john_doe",
  "items": [
    {
      "id": 1,
      "productId": 1,
      "productName": "Laptop Computer",
      "productDescription": "High-performance laptop for professional use",
      "unitPrice": 1299.99,
      "quantity": 1,
      "totalPrice": 1299.99,
      "addedAt": "2024-01-29T10:30:00Z"
    }
  ],
  "totalAmount": 1299.99,
  "totalItems": 1
}
```

## Integration Testing Approach

The integration tests use **TestContainers.NET** to:

1. **Spin up a real SQL Server container** for each test run
2. **Apply Entity Framework migrations** to create the database schema
3. **Seed test data** with known users, products, and shopping cart items
4. **Execute HTTP requests** against the API running with the test database
5. **Verify responses** match expected business logic

### Test Scenarios Covered

1. ✅ **Valid user with items in cart** - Returns complete cart data
2. ✅ **Valid user with empty cart** - Returns empty cart structure
3. ✅ **Non-existent user** - Returns 404 Not Found
4. ✅ **Invalid user ID (0 or negative)** - Returns 400 Bad Request
5. ✅ **Correct item calculations** - Verifies pricing and totals

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://docs.docker.com/get-docker/) or [Podman](https://podman.io/getting-started/installation) (for TestContainers)

## Running the Example

### 1. Clone and Navigate

```bash
git clone <repository-url>
cd examples/csharp-mssql-integration
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run Integration Tests

```bash
dotnet test --verbosity normal
```

The tests will:
- Automatically pull the SQL Server Docker image (if not already present)
- Start a containerized SQL Server instance
- Run all integration tests
- Clean up the container when finished

### 5. Run the API Locally (Optional)

To run the API with a local SQL Server:

```bash
# Update connection string in appsettings.json
# Then run:
cd src/EcommerceShop.Api
dotnet run
```

Access the Swagger UI at: `https://localhost:5001/swagger`

## Key Integration Testing Patterns

### 1. Test Container Setup

```csharp
private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
    .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
    .WithPassword("YourStrong!Passw0rd")
    .Build();
```

### 2. Database Seeding

```csharp
private static async Task SeedDatabaseAsync(EcommerceDbContext context)
{
    // Clear existing data
    context.Users.RemoveRange(context.Users);
    await context.SaveChangesAsync();
    
    // Add test data
    var users = new[] { /* test users */ };
    context.Users.AddRange(users);
    await context.SaveChangesAsync();
}
```

### 3. HTTP Client Testing

```csharp
[Fact]
public async Task GetShoppingCart_WithValidUserId_ReturnsShoppingCartData()
{
    // Arrange
    var userId = await GetUserIdAsync("john_doe");

    // Act
    var response = await _client.GetAsync($"/api/shoppingcart/{userId}");

    // Assert
    response.EnsureSuccessStatusCode();
    var cart = await response.Content.ReadFromJsonAsync<ShoppingCartResponse>();
    Assert.NotNull(cart);
    Assert.Equal("john_doe", cart.Username);
}
```

## CI/CD Pipeline

The GitHub Actions workflow (`.github/workflows/ci.yml`) automatically:

1. **Checks out code** on push/PR
2. **Sets up .NET 8** environment
3. **Restores NuGet packages**
4. **Builds the solution**
5. **Runs integration tests** (including Docker containers)
6. **Uploads test results** as artifacts

## Benefits of This Approach

✅ **Real Database Testing**: Tests run against actual SQL Server, catching database-specific issues

✅ **Isolated Test Environment**: Each test run gets a fresh database container

✅ **Fast Feedback**: Tests complete in seconds, suitable for CI/CD

✅ **No External Dependencies**: Everything needed is defined in code and containers

✅ **Cross-Platform**: Runs on any system with Docker or Podman support

✅ **Comprehensive Coverage**: Tests the full stack from HTTP to database

## Technology Stack

- **Framework**: .NET 8
- **Web Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core 9
- **Database**: SQL Server 2022
- **Testing Framework**: xUnit.net
- **Test Containers**: TestContainers.NET
- **CI/CD**: GitHub Actions

## Next Steps

This example can be extended with:

- Authentication and authorization
- Additional CRUD operations
- Database migrations testing
- Performance testing
- API versioning
- Health checks
- Logging and monitoring

---

## Related Examples

- [Example 2: Node.js with PostgreSQL](../nodejs-postgresql-integration/)
- [Example 3: Python with MongoDB](../python-mongodb-integration/)

For more integration testing patterns and best practices, see the [main presentation](../../README.md).