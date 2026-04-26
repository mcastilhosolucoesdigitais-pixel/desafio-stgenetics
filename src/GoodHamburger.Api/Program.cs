using GoodHamburger.Api.Endpoints;
using GoodHamburger.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddSingleton<IMenuService, MenuService>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapOpenApi();
app.MapMenuEndpoints();
app.MapOrderEndpoints();

app.Run();

public partial class Program { }
