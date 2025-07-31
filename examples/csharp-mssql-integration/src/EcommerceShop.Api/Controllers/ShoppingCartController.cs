using EcommerceShop.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController : ControllerBase
{
    private readonly IShoppingCartService _shoppingCartService;

    public ShoppingCartController(IShoppingCartService shoppingCartService)
    {
        _shoppingCartService = shoppingCartService;
    }

    /// <summary>
    /// Get the shopping cart for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <returns>The user's shopping cart data</returns>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetShoppingCart(int userId)
    {
        if (userId <= 0)
        {
            return BadRequest("User ID must be greater than 0");
        }

        var cart = await _shoppingCartService.GetShoppingCartAsync(userId);
        
        if (cart == null)
        {
            return NotFound($"User with ID {userId} not found");
        }

        return Ok(cart);
    }
}