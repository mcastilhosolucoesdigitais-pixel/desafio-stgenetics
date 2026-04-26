namespace GoodHamburger.Api.Domain;

public static class OrderValidator
{
    public static IReadOnlyList<string> Validate(IEnumerable<int> itemIds, IReadOnlyList<MenuItem> menu)
    {
        var ids = itemIds.ToList();
        var errors = new List<string>();

        var resolvedItems = new List<MenuItem>();
        foreach (var id in ids)
        {
            var item = menu.FirstOrDefault(m => m.Id == id);
            if (item is null)
                errors.Add($"Item with id {id} not found in menu.");
            else
                resolvedItems.Add(item);
        }

        var sandwiches   = resolvedItems.Where(i => i.Category == MenuCategory.Sandwich).ToList();
        var friesDups    = resolvedItems.Where(i => i.Name == "Batata Frita").ToList();
        var drinkDups    = resolvedItems.Where(i => i.Name == "Refrigerante").ToList();

        if (sandwiches.Count == 0)
            errors.Add("Order must contain exactly one sandwich.");
        else if (sandwiches.Count > 1)
            errors.Add($"Duplicate Sandwich items are not allowed.");

        if (friesDups.Count > 1)
            errors.Add($"Duplicate Batata Frita items are not allowed.");

        if (drinkDups.Count > 1)
            errors.Add($"Duplicate Refrigerante items are not allowed.");

        return errors;
    }
}
