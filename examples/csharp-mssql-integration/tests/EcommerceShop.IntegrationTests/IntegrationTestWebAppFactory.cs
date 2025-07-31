using EcommerceShop.Api.Data;
using EcommerceShop.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;

namespace EcommerceShop.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("YourStrong!Passw0rd")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<EcommerceDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add DbContext using test container connection
            services.AddDbContext<EcommerceDbContext>(options =>
            {
                options.UseSqlServer(_dbContainer.GetConnectionString());
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        // Initialize and seed the database
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EcommerceDbContext>();
        
        await context.Database.EnsureCreatedAsync();
        await SeedDatabaseAsync(context);
    }

    private static async Task SeedDatabaseAsync(EcommerceDbContext context)
    {
        // Clear existing data
        context.ShoppingCartItems.RemoveRange(context.ShoppingCartItems);
        context.Products.RemoveRange(context.Products);
        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();

        // Add test users
        var users = new[]
        {
            new User
            {
                Username = "john_doe",
                Email = "john.doe@example.com",
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new User
            {
                Username = "jane_smith",
                Email = "jane.smith@example.com",
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        // Add test products
        var products = new[]
        {
            new Product
            {
                Name = "Laptop Computer",
                Description = "High-performance laptop for professional use",
                Price = 1299.99m,
                StockQuantity = 50,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new Product
            {
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse with long battery life",
                Price = 29.99m,
                StockQuantity = 200,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Product
            {
                Name = "USB-C Hub",
                Description = "Multi-port USB-C hub with HDMI and USB 3.0 ports",
                Price = 49.99m,
                StockQuantity = 75,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // Get the actual user and product IDs after saving
        var johnDoe = await context.Users.FirstAsync(u => u.Username == "john_doe");
        var laptop = await context.Products.FirstAsync(p => p.Name == "Laptop Computer");
        var mouse = await context.Products.FirstAsync(p => p.Name == "Wireless Mouse");

        // Add shopping cart items for john_doe
        var cartItems = new[]
        {
            new ShoppingCartItem
            {
                UserId = johnDoe.Id,
                ProductId = laptop.Id,
                Quantity = 1,
                AddedAt = DateTime.UtcNow.AddDays(-2)
            },
            new ShoppingCartItem
            {
                UserId = johnDoe.Id,
                ProductId = mouse.Id,
                Quantity = 2,
                AddedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        context.ShoppingCartItems.AddRange(cartItems);
        await context.SaveChangesAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}