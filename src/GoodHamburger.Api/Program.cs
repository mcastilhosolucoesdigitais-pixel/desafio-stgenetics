using GoodHamburger.Api.Endpoints;
using GoodHamburger.Api.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddSingleton<IMenuService, MenuService>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                builder.Configuration["AllowedOrigins"] ?? "http://localhost:5002")
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.MapDefaultEndpoints();
app.MapOpenApi();
app.MapScalarApiReference("/docs");
app.MapGet("/", () => Results.Redirect("/docs/v1")).ExcludeFromDescription();
app.MapMenuEndpoints();
app.MapOrderEndpoints();

app.Run();

public partial class Program { }
