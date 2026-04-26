namespace GoodHamburger.Api.Domain;

public sealed class Order
{
    public int Id { get; init; }
    public IReadOnlyList<MenuItem> Items { get; init; } = [];
    public decimal Subtotal { get; init; }
    public decimal DiscountPercent { get; init; }
    public decimal Discount { get; init; }
    public decimal Total { get; init; }
    public DateTime CreatedAt { get; init; }
}
