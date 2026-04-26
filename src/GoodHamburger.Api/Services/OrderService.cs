using GoodHamburger.Api.Domain;

namespace GoodHamburger.Api.Services;

public sealed class OrderService(IMenuService menuService) : IOrderService
{
    private readonly List<Order> _orders = [];
    private int _nextId = 1;

    public (Order? Order, IReadOnlyList<string> Errors) Create(IReadOnlyList<int> itemIds)
    {
        var menu = menuService.GetAll();
        var errors = OrderValidator.Validate(itemIds, menu);
        if (errors.Count > 0) return (null, errors);

        var items = itemIds.Select(id => menu.First(m => m.Id == id)).ToList();
        var calc = DiscountCalculator.Calculate(items);

        var order = new Order
        {
            Id = _nextId++,
            Items = items,
            Subtotal = calc.Subtotal,
            DiscountPercent = calc.DiscountPercent,
            Discount = calc.Discount,
            Total = calc.Total,
            CreatedAt = DateTime.UtcNow
        };

        _orders.Add(order);
        return (order, []);
    }

    public IReadOnlyList<Order> GetAll() => _orders.OrderBy(o => o.CreatedAt).ToList();

    public Order? GetById(int id) => _orders.FirstOrDefault(o => o.Id == id);

    public (Order? Order, IReadOnlyList<string> Errors) Update(int id, IReadOnlyList<int> itemIds)
    {
        var existing = GetById(id);
        if (existing is null) return (null, []);

        var menu = menuService.GetAll();
        var errors = OrderValidator.Validate(itemIds, menu);
        if (errors.Count > 0) return (existing, errors);

        var items = itemIds.Select(itemId => menu.First(m => m.Id == itemId)).ToList();
        var calc = DiscountCalculator.Calculate(items);

        var updated = new Order
        {
            Id = existing.Id,
            Items = items,
            Subtotal = calc.Subtotal,
            DiscountPercent = calc.DiscountPercent,
            Discount = calc.Discount,
            Total = calc.Total,
            CreatedAt = existing.CreatedAt
        };

        _orders[_orders.IndexOf(existing)] = updated;
        return (updated, []);
    }

    public bool Delete(int id)
    {
        var order = GetById(id);
        if (order is null) return false;
        _orders.Remove(order);
        return true;
    }
}
