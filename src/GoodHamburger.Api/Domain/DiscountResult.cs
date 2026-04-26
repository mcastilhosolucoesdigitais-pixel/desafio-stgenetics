namespace GoodHamburger.Api.Domain;

public sealed record DiscountResult(
    decimal Subtotal,
    decimal DiscountPercent,
    decimal Discount,
    decimal Total);
