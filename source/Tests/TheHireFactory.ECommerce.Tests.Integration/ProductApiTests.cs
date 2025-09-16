using System.Net;
using System.Net.Http.Json;
using TheHireFactory.ECommerce.Api.Contracts;
using Xunit.Abstractions;

namespace TheHireFactory.ECommerce.Tests.Integration;

public class ProductApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public ProductApiTests(TestWebApplicationFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    private async Task DumpIfServerError(HttpResponseMessage res)
    {
        if ((int)res.StatusCode >= 500)
        {
            var body = await res.Content.ReadAsStringAsync();
            _output.WriteLine($"SERVER ERROR {res.StatusCode}: {body}");
        }
    }

    [Fact]
    public async Task Health_Should_Return_Ok()
    {
        var res = await _client.GetAsync("/health");
        await DumpIfServerError(res);
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProducts_Should_Return_Empty_List_When_No_Data()
    {
        var res = await _client.GetAsync("/api/product");
        await DumpIfServerError(res);
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var products = await res.Content.ReadFromJsonAsync<List<ProductReadDto>>();
        products.Should().NotBeNull();
        products!.Should().BeEmpty();
    }

    [Fact]
    public async Task PostProduct_Should_Create_And_Return_Product()
    {
        var dto = new ProductCreateDto("Keyboard", 499.90m, 5, 1);

        var res = await _client.PostAsJsonAsync("/api/product", dto);
        await DumpIfServerError(res);
        res.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await res.Content.ReadFromJsonAsync<ProductReadDto>();
        created.Should().NotBeNull();
        created!.Name.Should().Be("Keyboard");
    }

    [Fact]
    public async Task GetProduct_ById_Should_Return_404_When_NotFound()
    {
        var res = await _client.GetAsync("/api/product/9999");
        await DumpIfServerError(res);
        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}