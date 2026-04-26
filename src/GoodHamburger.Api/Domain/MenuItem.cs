namespace GoodHamburger.Api.Domain;

public sealed class MenuItem
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public MenuCategory Category { get; init; }
    public decimal Price { get; init; }
}
