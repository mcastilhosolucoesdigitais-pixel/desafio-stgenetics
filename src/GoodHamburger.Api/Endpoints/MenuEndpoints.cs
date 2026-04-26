using GoodHamburger.Api.DTOs;
using GoodHamburger.Api.Services;

namespace GoodHamburger.Api.Endpoints;

public static class MenuEndpoints
{
    public static void MapMenuEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/menu", (IMenuService menuService) =>
        {
            var items = menuService.GetAll()
                .Select(i => new MenuItemResponse(i.Id, i.Name, i.Category.ToString(), i.Price));

            return Results.Ok(items);
        })
        .WithName("GetMenu")
        .WithTags("Menu")
        .Produces<IEnumerable<MenuItemResponse>>();
    }
}
