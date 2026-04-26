using GoodHamburger.Api.Endpoints;
using GoodHamburger.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMenuService, MenuService>();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapMenuEndpoints();

app.Run();

public partial class Program { }
