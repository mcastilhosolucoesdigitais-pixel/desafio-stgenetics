using FluentAssertions;
using GoodHamburger.Api.Domain;

namespace GoodHamburger.UnitTests.Domain;

public class DiscountCalculatorTests
{
    private static readonly MenuItem XBurger     = new() { Id = 1, Name = "X Burger",     Category = MenuCategory.Sandwich, Price = 5.00m };
    private static readonly MenuItem XEgg        = new() { Id = 2, Name = "X Egg",        Category = MenuCategory.Sandwich, Price = 4.50m };
    private static readonly MenuItem XBacon      = new() { Id = 3, Name = "X Bacon",      Category = MenuCategory.Sandwich, Price = 7.00m };
    private static readonly MenuItem BatatFrita  = new() { Id = 4, Name = "Batata Frita", Category = MenuCategory.Side,     Price = 2.00m };
    private static readonly MenuItem Refrigerante = new() { Id = 5, Name = "Refrigerante", Category = MenuCategory.Side,    Price = 2.50m };

    [Fact]
    public void Calculate_SandwichOnly_ShouldApply_NoDiscount()
    {
        var items = new[] { XBurger };
        var result = DiscountCalculator.Calculate(items);

        result.DiscountPercent.Should().Be(0m);
        result.Subtotal.Should().Be(5.00m);
        result.Discount.Should().Be(0m);
        result.Total.Should().Be(5.00m);
    }

    [Fact]
    public void Calculate_SandwichPlusFries_ShouldApply_10PercentDiscount()
    {
        var items = new[] { XBurger, BatatFrita };
        var result = DiscountCalculator.Calculate(items);

        result.DiscountPercent.Should().Be(0.10m);
        result.Subtotal.Should().Be(7.00m);
        result.Discount.Should().Be(0.70m);
        result.Total.Should().Be(6.30m);
    }

    [Fact]
    public void Calculate_SandwichPlusDrink_ShouldApply_15PercentDiscount()
    {
        var items = new[] { XBurger, Refrigerante };
        var result = DiscountCalculator.Calculate(items);

        result.DiscountPercent.Should().Be(0.15m);
        result.Subtotal.Should().Be(7.50m);
        result.Discount.Should().Be(1.125m);
        result.Total.Should().Be(6.375m);
    }

    [Fact]
    public void Calculate_SandwichPlusFriesPlusDrink_ShouldApply_20PercentDiscount()
    {
        var items = new[] { XBurger, BatatFrita, Refrigerante };
        var result = DiscountCalculator.Calculate(items);

        result.DiscountPercent.Should().Be(0.20m);
        result.Subtotal.Should().Be(9.50m);
        result.Discount.Should().Be(1.90m);
        result.Total.Should().Be(7.60m);
    }

    [Fact]
    public void Calculate_XEggPlusFriesPlusDrink_ShouldApply_20PercentDiscount()
    {
        var items = new[] { XEgg, BatatFrita, Refrigerante };
        var result = DiscountCalculator.Calculate(items);

        result.DiscountPercent.Should().Be(0.20m);
        result.Subtotal.Should().Be(9.00m);
        result.Discount.Should().Be(1.80m);
        result.Total.Should().Be(7.20m);
    }

    [Fact]
    public void Calculate_XBaconPlusDrink_ShouldApply_15PercentDiscount()
    {
        var items = new[] { XBacon, Refrigerante };
        var result = DiscountCalculator.Calculate(items);

        result.DiscountPercent.Should().Be(0.15m);
        result.Subtotal.Should().Be(9.50m);
        result.Discount.Should().Be(1.425m);
        result.Total.Should().Be(8.075m);
    }

    [Fact]
    public void Calculate_Total_ShouldEqual_SubtotalMinusDiscount()
    {
        var items = new[] { XBacon, BatatFrita, Refrigerante };
        var result = DiscountCalculator.Calculate(items);

        result.Total.Should().Be(result.Subtotal - result.Discount);
    }
}
