namespace GoodHamburger.Api.Domain;

public static class DiscountCalculator
{
    public static DiscountResult Calculate(IEnumerable<MenuItem> items)
    {
        var list = items.ToList();
        bool hasFries = list.Any(i => i.Category == MenuCategory.Side && i.Name == "Batata Frita");
        bool hasDrink = list.Any(i => i.Category == MenuCategory.Side && i.Name == "Refrigerante");

        decimal percent = (hasFries, hasDrink) switch
        {
            (true, true)  => 0.20m,
            (false, true) => 0.15m,
            (true, false) => 0.10m,
            _             => 0m
        };

        decimal subtotal = list.Sum(i => i.Price);
        decimal discount = subtotal * percent;

        return new DiscountResult(subtotal, percent, discount, subtotal - discount);
    }
}
