using EcommerceShop.Api.Data;
using EcommerceShop.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace EcommerceShop.IntegrationTests;

public class ShoppingCartControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;

    public ShoppingCartControllerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private async Task<int> GetUserIdAsync(string username)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EcommerceDbContext>();
        var user = await context.Users.FirstAsync(u => u.Username == username);
        return user.Id;
    }

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
        Assert.Equal(userId, cart.UserId);
        Assert.Equal("john_doe", cart.Username);
        Assert.Equal(2, cart.Items.Count);
        Assert.Equal(3, cart.TotalItems); // 1 laptop + 2 mice
        Assert.Equal(1359.97m, cart.TotalAmount); // 1299.99 + (2 * 29.99)
    }

    [Fact]
    public async Task GetShoppingCart_WithValidUserId_ReturnsCorrectItemDetails()
    {
        // Arrange
        var userId = await GetUserIdAsync("john_doe");

        // Act
        var response = await _client.GetAsync($"/api/shoppingcart/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        
        var cart = await response.Content.ReadFromJsonAsync<ShoppingCartResponse>();
        
        Assert.NotNull(cart);
        
        // Check laptop item
        var laptopItem = cart.Items.FirstOrDefault(i => i.ProductName == "Laptop Computer");
        Assert.NotNull(laptopItem);
        Assert.Equal("High-performance laptop for professional use", laptopItem.ProductDescription);
        Assert.Equal(1299.99m, laptopItem.UnitPrice);
        Assert.Equal(1, laptopItem.Quantity);
        Assert.Equal(1299.99m, laptopItem.TotalPrice);

        // Check mouse item
        var mouseItem = cart.Items.FirstOrDefault(i => i.ProductName == "Wireless Mouse");
        Assert.NotNull(mouseItem);
        Assert.Equal("Ergonomic wireless mouse with long battery life", mouseItem.ProductDescription);
        Assert.Equal(29.99m, mouseItem.UnitPrice);
        Assert.Equal(2, mouseItem.Quantity);
        Assert.Equal(59.98m, mouseItem.TotalPrice);
    }

    [Fact]
    public async Task GetShoppingCart_WithUserHavingEmptyCart_ReturnsEmptyCart()
    {
        // Arrange
        var userId = await GetUserIdAsync("jane_smith"); // jane_smith has no items in cart

        // Act
        var response = await _client.GetAsync($"/api/shoppingcart/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        
        var cart = await response.Content.ReadFromJsonAsync<ShoppingCartResponse>();
        
        Assert.NotNull(cart);
        Assert.Equal(userId, cart.UserId);
        Assert.Equal("jane_smith", cart.Username);
        Assert.Empty(cart.Items);
        Assert.Equal(0, cart.TotalItems);
        Assert.Equal(0m, cart.TotalAmount);
    }

    [Fact]
    public async Task GetShoppingCart_WithNonExistentUserId_ReturnsNotFound()
    {
        // Arrange
        const int nonExistentUserId = 999;

        // Act
        var response = await _client.GetAsync($"/api/shoppingcart/{nonExistentUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("User with ID 999 not found", content);
    }

    [Fact]
    public async Task GetShoppingCart_WithInvalidUserId_ReturnsBadRequest()
    {
        // Arrange
        const int invalidUserId = 0;

        // Act
        var response = await _client.GetAsync($"/api/shoppingcart/{invalidUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("User ID must be greater than 0", content);
    }

    [Fact]
    public async Task GetShoppingCart_WithNegativeUserId_ReturnsBadRequest()
    {
        // Arrange
        const int negativeUserId = -1;

        // Act
        var response = await _client.GetAsync($"/api/shoppingcart/{negativeUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("User ID must be greater than 0", content);
    }
}