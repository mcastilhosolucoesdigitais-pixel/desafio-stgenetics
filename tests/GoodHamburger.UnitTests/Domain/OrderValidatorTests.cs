using FluentAssertions;
using GoodHamburger.Api.Domain;

namespace GoodHamburger.UnitTests.Domain;

public class OrderValidatorTests
{
    private static readonly IReadOnlyList<MenuItem> Menu =
    [
        new() { Id = 1, Name = "X Burger",     Category = MenuCategory.Sandwich, Price = 5.00m },
        new() { Id = 2, Name = "X Egg",        Category = MenuCategory.Sandwich, Price = 4.50m },
        new() { Id = 3, Name = "X Bacon",      Category = MenuCategory.Sandwich, Price = 7.00m },
        new() { Id = 4, Name = "Batata Frita", Category = MenuCategory.Side,     Price = 2.00m },
        new() { Id = 5, Name = "Refrigerante", Category = MenuCategory.Side,     Price = 2.50m },
    ];

    [Fact]
    public void Validate_ValidOrder_SandwichOnly_ShouldReturn_NoErrors()
    {
        var errors = OrderValidator.Validate([1], Menu);
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ValidOrder_SandwichAndFries_ShouldReturn_NoErrors()
    {
        var errors = OrderValidator.Validate([1, 4], Menu);
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ValidOrder_SandwichAndDrink_ShouldReturn_NoErrors()
    {
        var errors = OrderValidator.Validate([1, 5], Menu);
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ValidOrder_FullCombo_ShouldReturn_NoErrors()
    {
        var errors = OrderValidator.Validate([3, 4, 5], Menu);
        errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyOrder_ShouldReturn_Error()
    {
        var errors = OrderValidator.Validate([], Menu);
        errors.Should().Contain(e => e.Contains("sandwich"));
    }

    [Fact]
    public void Validate_NoSandwich_ShouldReturn_Error()
    {
        var errors = OrderValidator.Validate([4, 5], Menu);
        errors.Should().Contain(e => e.Contains("sandwich"));
    }

    [Fact]
    public void Validate_TwoSandwiches_ShouldReturn_Error()
    {
        var errors = OrderValidator.Validate([1, 2], Menu);
        errors.Should().Contain(e => e.Contains("Sandwich"));
    }

    [Fact]
    public void Validate_TwoFries_ShouldReturn_Error()
    {
        var errors = OrderValidator.Validate([1, 4, 4], Menu);
        errors.Should().Contain(e => e.Contains("duplicate") || e.Contains("Batata"));
    }

    [Fact]
    public void Validate_TwoDrinks_ShouldReturn_Error()
    {
        var errors = OrderValidator.Validate([1, 5, 5], Menu);
        errors.Should().Contain(e => e.Contains("duplicate") || e.Contains("Refrigerante"));
    }

    [Fact]
    public void Validate_UnknownItemId_ShouldReturn_Error()
    {
        var errors = OrderValidator.Validate([1, 99], Menu);
        errors.Should().Contain(e => e.Contains("99"));
    }

    [Fact]
    public void Validate_MultipleErrors_ShouldReturn_AllErrors()
    {
        var errors = OrderValidator.Validate([99, 100], Menu);
        errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
