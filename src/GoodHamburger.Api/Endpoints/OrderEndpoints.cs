using GoodHamburger.Api.DTOs;
using GoodHamburger.Api.Services;

namespace GoodHamburger.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders").WithTags("Orders");

        group.MapPost("/", (OrderRequest req, IOrderService orderService) =>
        {
            var (order, errors) = orderService.Create(req.ItemIds);
            if (errors.Count > 0)
                return Results.UnprocessableEntity(new { errors });

            return Results.Created($"/orders/{order!.Id}", ToResponse(order));
        })
        .Produces<OrderResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status422UnprocessableEntity);

        group.MapGet("/", (IOrderService orderService) =>
            Results.Ok(orderService.GetAll().Select(ToResponse)))
        .Produces<IEnumerable<OrderResponse>>();

        group.MapGet("/{id:int}", (int id, IOrderService orderService) =>
        {
            var order = orderService.GetById(id);
            return order is null
                ? Results.NotFound(new { error = $"Order {id} not found." })
                : Results.Ok(ToResponse(order));
        })
        .Produces<OrderResponse>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:int}", (int id, OrderRequest req, IOrderService orderService) =>
        {
            var existing = orderService.GetById(id);
            if (existing is null)
                return Results.NotFound(new { error = $"Order {id} not found." });

            var (order, errors) = orderService.Update(id, req.ItemIds);
            if (errors.Count > 0)
                return Results.UnprocessableEntity(new { errors });

            return Results.Ok(ToResponse(order!));
        })
        .Produces<OrderResponse>()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status422UnprocessableEntity);

        group.MapDelete("/{id:int}", (int id, IOrderService orderService) =>
        {
            var deleted = orderService.Delete(id);
            return deleted
                ? Results.NoContent()
                : Results.NotFound(new { error = $"Order {id} not found." });
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }

    private static OrderResponse ToResponse(Domain.Order o) => new(
        o.Id,
        o.Items.Select(i => new OrderItemResponse(i.Id, i.Name, i.Category.ToString(), i.Price)).ToList(),
        o.Subtotal,
        o.DiscountPercent,
        o.Discount,
        o.Total);
}
