using EcommerceShop.Api.Models;

namespace EcommerceShop.Api.Services;

public interface IShoppingCartService
{
    Task<ShoppingCartResponse?> GetShoppingCartAsync(int userId);
}