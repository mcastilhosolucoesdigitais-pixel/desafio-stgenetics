namespace GoodHamburger.Web.Models;

public sealed record OrderItemModel(int Id, string Name, string Category, decimal Price);

public sealed record OrderModel(
    int Id,
    List<OrderItemModel> Items,
    decimal Subtotal,
    decimal DiscountPercent,
    decimal Discount,
    decimal Total);
