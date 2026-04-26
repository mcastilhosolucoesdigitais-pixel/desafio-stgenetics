namespace GoodHamburger.Api.DTOs;

public sealed record OrderItemResponse(int Id, string Name, string Category, decimal Price);

public sealed record OrderResponse(
    int Id,
    IReadOnlyList<OrderItemResponse> Items,
    decimal Subtotal,
    decimal DiscountPercent,
    decimal Discount,
    decimal Total);
