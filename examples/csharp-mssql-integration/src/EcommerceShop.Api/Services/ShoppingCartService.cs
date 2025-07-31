using EcommerceShop.Api.Data;
using EcommerceShop.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceShop.Api.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly EcommerceDbContext _context;

    public ShoppingCartService(EcommerceDbContext context)
    {
        _context = context;
    }

    public async Task<ShoppingCartResponse?> GetShoppingCartAsync(int userId)
    {
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.Id, u.Username })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return null;
        }

        var cartItems = await _context.ShoppingCartItems
            .Include(sci => sci.Product)
            .Where(sci => sci.UserId == userId)
            .Select(sci => new ShoppingCartItemResponse
            {
                Id = sci.Id,
                ProductId = sci.ProductId,
                ProductName = sci.Product.Name,
                ProductDescription = sci.Product.Description,
                UnitPrice = sci.Product.Price,
                Quantity = sci.Quantity,
                TotalPrice = sci.Product.Price * sci.Quantity,
                AddedAt = sci.AddedAt
            })
            .ToListAsync();

        return new ShoppingCartResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Items = cartItems,
            TotalAmount = cartItems.Sum(item => item.TotalPrice),
            TotalItems = cartItems.Sum(item => item.Quantity)
        };
    }
}