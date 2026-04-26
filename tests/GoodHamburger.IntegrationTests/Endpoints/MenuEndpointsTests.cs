using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GoodHamburger.IntegrationTests.Endpoints;

public class MenuEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public MenuEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMenu_ShouldReturn_200Ok()
    {
        var response = await _client.GetAsync("/menu");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMenu_ShouldReturn_FiveItems()
    {
        var response = await _client.GetAsync("/menu");
        var body = await response.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<List<JsonElement>>(body, JsonOptions);

        items.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetMenu_ShouldReturn_JsonContentType()
    {
        var response = await _client.GetAsync("/menu");

        response.Content.Headers.ContentType!.MediaType
            .Should().Be("application/json");
    }

    [Fact]
    public async Task GetMenu_EachItem_ShouldHaveRequiredFields()
    {
        var response = await _client.GetAsync("/menu");
        var body = await response.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<List<JsonElement>>(body, JsonOptions)!;

        foreach (var item in items)
        {
            item.TryGetProperty("id", out _).Should().BeTrue(because: "each item must have 'id'");
            item.TryGetProperty("name", out _).Should().BeTrue(because: "each item must have 'name'");
            item.TryGetProperty("category", out _).Should().BeTrue(because: "each item must have 'category'");
            item.TryGetProperty("price", out _).Should().BeTrue(because: "each item must have 'price'");
        }
    }

    [Fact]
    public async Task GetMenu_SandwichItems_ShouldHaveCategory_Sandwich()
    {
        var response = await _client.GetAsync("/menu");
        var body = await response.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<List<JsonElement>>(body, JsonOptions)!;

        var sandwiches = items.Where(i =>
            i.GetProperty("category").GetString() == "Sandwich").ToList();

        sandwiches.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetMenu_SideItems_ShouldHaveCategory_Side()
    {
        var response = await _client.GetAsync("/menu");
        var body = await response.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<List<JsonElement>>(body, JsonOptions)!;

        var sides = items.Where(i =>
            i.GetProperty("category").GetString() == "Side").ToList();

        sides.Should().HaveCount(2);
    }

    [Theory]
    [InlineData("X Burger", "Sandwich", 5.00)]
    [InlineData("X Egg", "Sandwich", 4.50)]
    [InlineData("X Bacon", "Sandwich", 7.00)]
    [InlineData("Batata Frita", "Side", 2.00)]
    [InlineData("Refrigerante", "Side", 2.50)]
    public async Task GetMenu_ShouldContain_AllExpectedItems(string name, string category, decimal expectedPrice)
    {
        var response = await _client.GetAsync("/menu");
        var body = await response.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<List<JsonElement>>(body, JsonOptions)!;

        var match = items.FirstOrDefault(i =>
            i.GetProperty("name").GetString() == name &&
            i.GetProperty("category").GetString() == category);

        match.ValueKind.Should().NotBe(JsonValueKind.Undefined,
            because: $"item '{name}' with category '{category}' must exist");

        match.GetProperty("price").GetDecimal().Should().Be(expectedPrice);
    }
}
