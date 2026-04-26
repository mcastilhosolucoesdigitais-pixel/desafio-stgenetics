using FluentAssertions;
using GoodHamburger.Api.Domain;
using GoodHamburger.Api.Services;

namespace GoodHamburger.UnitTests.Domain;

public class MenuServiceTests
{
    private readonly IMenuService _sut = new MenuService();

    [Fact]
    public void GetAll_ShouldReturn_FiveItems()
    {
        var items = _sut.GetAll();

        items.Should().HaveCount(5);
    }

    [Fact]
    public void GetAll_ShouldContain_ThreeSandwiches()
    {
        var items = _sut.GetAll();

        items.Where(i => i.Category == MenuCategory.Sandwich)
             .Should().HaveCount(3);
    }

    [Fact]
    public void GetAll_ShouldContain_TwoSides()
    {
        var items = _sut.GetAll();

        items.Where(i => i.Category == MenuCategory.Side)
             .Should().HaveCount(2);
    }

    [Theory]
    [InlineData("X Burger", MenuCategory.Sandwich, 5.00)]
    [InlineData("X Egg", MenuCategory.Sandwich, 4.50)]
    [InlineData("X Bacon", MenuCategory.Sandwich, 7.00)]
    [InlineData("Batata Frita", MenuCategory.Side, 2.00)]
    [InlineData("Refrigerante", MenuCategory.Side, 2.50)]
    public void GetAll_ShouldContain_ExpectedItems(string name, MenuCategory category, decimal price)
    {
        var items = _sut.GetAll();

        items.Should().Contain(i =>
            i.Name == name &&
            i.Category == category &&
            i.Price == price);
    }

    [Fact]
    public void GetAll_EachItem_ShouldHavePositiveId()
    {
        var items = _sut.GetAll();

        items.Should().OnlyContain(i => i.Id > 0);
    }

    [Fact]
    public void GetAll_EachItem_ShouldHaveUniqueId()
    {
        var items = _sut.GetAll();

        items.Select(i => i.Id).Should().OnlyHaveUniqueItems();
    }
}
