using GoodHamburger.Api.Domain;

namespace GoodHamburger.Api.Services;

public interface IMenuService
{
    IReadOnlyList<MenuItem> GetAll();
}
