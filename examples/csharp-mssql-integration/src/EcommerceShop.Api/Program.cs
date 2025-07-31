using EcommerceShop.Api.Data;
using EcommerceShop.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map shopping cart endpoint
app.MapGet("/api/shoppingcart/{userId:int}", async (int userId, EcommerceDbContext context) =>
{
    if (userId <= 0)
    {
        return Results.BadRequest("User ID must be greater than 0");
    }

    var user = await context.Users
        .Where(u => u.Id == userId)
        .Select(u => new { u.Id, u.Username })
        .FirstOrDefaultAsync();

    if (user == null)
    {
        return Results.NotFound($"User with ID {userId} not found");
    }

    var cartItems = await context.ShoppingCartItems
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

    var cart = new ShoppingCartResponse
    {
        UserId = user.Id,
        Username = user.Username,
        Items = cartItems,
        TotalAmount = cartItems.Sum(item => item.TotalPrice),
        TotalItems = cartItems.Sum(item => item.Quantity)
    };

    return Results.Ok(cart);
})
.WithName("GetShoppingCart")
.WithOpenApi(operation => new(operation)
{
    Summary = "Get the shopping cart for a specific user",
    Description = "Retrieves the shopping cart data for a user including all items, quantities, and totals",
    Parameters = new List<Microsoft.OpenApi.Models.OpenApiParameter>
    {
        new()
        {
            Name = "userId",
            In = Microsoft.OpenApi.Models.ParameterLocation.Path,
            Required = true,
            Description = "The ID of the user",
            Schema = new Microsoft.OpenApi.Models.OpenApiSchema { Type = "integer", Format = "int32" }
        }
    }
});

app.Run();

// Make the Program class available for testing
public partial class Program { }
