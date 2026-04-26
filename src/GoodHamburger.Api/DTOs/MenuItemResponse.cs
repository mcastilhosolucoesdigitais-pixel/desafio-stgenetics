namespace GoodHamburger.Api.DTOs;

public sealed record MenuItemResponse(
    int Id,
    string Name,
    string Category,
    decimal Price);
