using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GoodHamburger.IntegrationTests.Endpoints;

public class OrderEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OrderEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // ── POST /orders ────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateOrder_SandwichOnly_ShouldReturn_201WithZeroDiscount()
    {
        var response = await _client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1 } });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        body.GetProperty("discountPercent").GetDecimal().Should().Be(0m);
        body.GetProperty("discount").GetDecimal().Should().Be(0m);
        body.GetProperty("subtotal").GetDecimal().Should().Be(5.00m);
        body.GetProperty("total").GetDecimal().Should().Be(5.00m);
    }

    [Fact]
    public async Task CreateOrder_SandwichPlusFries_ShouldReturn_201With10PercentDiscount()
    {
        var response = await _client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1, 4 } });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        body.GetProperty("discountPercent").GetDecimal().Should().Be(0.10m);
    }

    [Fact]
    public async Task CreateOrder_SandwichPlusDrink_ShouldReturn_201With15PercentDiscount()
    {
        var response = await _client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1, 5 } });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        body.GetProperty("discountPercent").GetDecimal().Should().Be(0.15m);
    }

    [Fact]
    public async Task CreateOrder_FullCombo_ShouldReturn_201With20PercentDiscount()
    {
        var response = await _client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1, 4, 5 } });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        body.GetProperty("discountPercent").GetDecimal().Should().Be(0.20m);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturn_201WithRequiredFields()
    {
        var response = await _client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1 } });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        body.TryGetProperty("id", out _).Should().BeTrue();
        body.TryGetProperty("items", out _).Should().BeTrue();
        body.TryGetProperty("subtotal", out _).Should().BeTrue();
        body.TryGetProperty("discount", out _).Should().BeTrue();
        body.TryGetProperty("total", out _).Should().BeTrue();
    }

    [Fact]
    public async Task CreateOrder_NoSandwich_ShouldReturn_422()
    {
        var response = await _client.PostAsJsonAsync("/orders", new { itemIds = new[] { 4, 5 } });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateOrder_TwoSandwiches_ShouldReturn_422()
    {
        var response = await _client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1, 2 } });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateOrder_DuplicateFries_ShouldReturn_422()
    {
        var response = await _client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1, 4, 4 } });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateOrder_UnknownItem_ShouldReturn_422()
    {
        var response = await _client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1, 99 } });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateOrder_422Response_ShouldContain_ErrorsField()
    {
        var response = await _client.PostAsJsonAsync("/orders", new { itemIds = new[] { 4 } });

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        body.TryGetProperty("errors", out _).Should().BeTrue();
    }

    // ── GET /orders ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrders_ShouldReturn_200()
    {
        var response = await _client.GetAsync("/orders");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOrders_AfterCreate_ShouldContain_CreatedOrder()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        await client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1 } });
        var response = await client.GetAsync("/orders");
        var list = await response.Content.ReadFromJsonAsync<List<JsonElement>>(JsonOptions);

        list.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    // ── GET /orders/{id} ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetOrderById_Existing_ShouldReturn_200()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1 } });
        var createdBody = await created.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        int id = createdBody.GetProperty("id").GetInt32();

        var response = await client.GetAsync($"/orders/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOrderById_NotFound_ShouldReturn_404()
    {
        var response = await _client.GetAsync("/orders/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOrderById_404Response_ShouldContain_ErrorField()
    {
        var response = await _client.GetAsync("/orders/99999");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        body.TryGetProperty("error", out _).Should().BeTrue();
    }

    // ── PUT /orders/{id} ─────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateOrder_Existing_ShouldReturn_200WithRecalculatedValues()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1 } });
        var createdBody = await created.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        int id = createdBody.GetProperty("id").GetInt32();

        var updated = await client.PutAsJsonAsync($"/orders/{id}", new { itemIds = new[] { 1, 4, 5 } });
        updated.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await updated.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        body.GetProperty("discountPercent").GetDecimal().Should().Be(0.20m);
    }

    [Fact]
    public async Task UpdateOrder_NotFound_ShouldReturn_404()
    {
        var response = await _client.PutAsJsonAsync("/orders/99999", new { itemIds = new[] { 1 } });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateOrder_InvalidItems_ShouldReturn_422()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1 } });
        var createdBody = await created.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        int id = createdBody.GetProperty("id").GetInt32();

        var updated = await client.PutAsJsonAsync($"/orders/{id}", new { itemIds = new[] { 4, 5 } });
        updated.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // ── DELETE /orders/{id} ──────────────────────────────────────────────────

    [Fact]
    public async Task DeleteOrder_Existing_ShouldReturn_204()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1 } });
        var createdBody = await created.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        int id = createdBody.GetProperty("id").GetInt32();

        var deleted = await client.DeleteAsync($"/orders/{id}");
        deleted.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteOrder_NotFound_ShouldReturn_404()
    {
        var response = await _client.DeleteAsync("/orders/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteOrder_AfterDelete_GetShouldReturn_404()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/orders", new { itemIds = new[] { 1 } });
        var createdBody = await created.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        int id = createdBody.GetProperty("id").GetInt32();

        await client.DeleteAsync($"/orders/{id}");

        var get = await client.GetAsync($"/orders/{id}");
        get.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
