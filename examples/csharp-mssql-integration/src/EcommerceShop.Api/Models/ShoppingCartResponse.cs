namespace EcommerceShop.Api.Models;

public class ShoppingCartResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public List<ShoppingCartItemResponse> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public int TotalItems { get; set; }
}

public class ShoppingCartItemResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductDescription { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime AddedAt { get; set; }
}