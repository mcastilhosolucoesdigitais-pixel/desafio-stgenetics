using System.Net.Http.Json;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public sealed class ApiClient(HttpClient http)
{
    public Task<List<MenuItemModel>?> GetMenuAsync() =>
        http.GetFromJsonAsync<List<MenuItemModel>>("menu");

    public Task<List<OrderModel>?> GetOrdersAsync() =>
        http.GetFromJsonAsync<List<OrderModel>>("orders");

    public async Task<(OrderModel? Order, List<string>? Errors)> CreateOrderAsync(List<int> itemIds)
    {
        var response = await http.PostAsJsonAsync("orders", new { itemIds });
        if (response.IsSuccessStatusCode)
            return (await response.Content.ReadFromJsonAsync<OrderModel>(), null);

        var err = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        return (null, err?.Errors ?? ["Erro desconhecido."]);
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var response = await http.DeleteAsync($"orders/{id}");
        return response.IsSuccessStatusCode;
    }

    private sealed record ErrorResponse(List<string> Errors);
}
