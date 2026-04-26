using GoodHamburger.Api.Domain;

namespace GoodHamburger.Api.Services;

public sealed class MenuService : IMenuService
{
    private static readonly IReadOnlyList<MenuItem> Items =
    [
        new() { Id = 1, Name = "X Burger",     Category = MenuCategory.Sandwich, Price = 5.00m },
        new() { Id = 2, Name = "X Egg",        Category = MenuCategory.Sandwich, Price = 4.50m },
        new() { Id = 3, Name = "X Bacon",      Category = MenuCategory.Sandwich, Price = 7.00m },
        new() { Id = 4, Name = "Batata Frita", Category = MenuCategory.Side,     Price = 2.00m },
        new() { Id = 5, Name = "Refrigerante", Category = MenuCategory.Side,     Price = 2.50m },
    ];

    public IReadOnlyList<MenuItem> GetAll() => Items;
}
