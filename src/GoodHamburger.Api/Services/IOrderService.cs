using GoodHamburger.Api.Domain;

namespace GoodHamburger.Api.Services;

public interface IOrderService
{
    (Order? Order, IReadOnlyList<string> Errors) Create(IReadOnlyList<int> itemIds);
    IReadOnlyList<Order> GetAll();
    Order? GetById(int id);
    (Order? Order, IReadOnlyList<string> Errors) Update(int id, IReadOnlyList<int> itemIds);
    bool Delete(int id);
}
