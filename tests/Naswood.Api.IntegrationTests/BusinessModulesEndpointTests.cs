using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class BusinessModulesEndpointTests
{
    private readonly NaswoodApiFactory _factory;
    public BusinessModulesEndpointTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Material_and_supplier_and_customer_and_bom_crud()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var material = await client.PostAsJsonAsync("/api/v1/materials", new
        {
            code = "MAT-001",
            name = "Oak Plank",
            description = "Raw material",
            category = "Wood",
            unitOfMeasure = "PC",
            status = "Active"
        });
        Assert.Equal(HttpStatusCode.OK, material.StatusCode);

        var materials = await client.GetAsync("/api/v1/materials?q=Oak");
        Assert.Equal(HttpStatusCode.OK, materials.StatusCode);

        var supplier = await client.PostAsJsonAsync("/api/v1/suppliers", new
        {
            code = "SUP-001",
            name = "Timber Co",
            taxNumber = "T1",
            email = "a@b.com",
            phone = "123",
            status = "Active"
        });
        Assert.Equal(HttpStatusCode.OK, supplier.StatusCode);

        var customer = await client.PostAsJsonAsync("/api/v1/customers", new
        {
            code = "CUS-001",
            name = "Retail Partner",
            taxNumber = "C1",
            email = "c@d.com",
            phone = "456",
            status = "Active"
        });
        Assert.Equal(HttpStatusCode.OK, customer.StatusCode);

        var bom = await client.PostAsJsonAsync("/api/v1/boms", new
        {
            number = "BOM-001",
            materialCode = "MAT-001",
            version = 1,
            status = "Draft",
            notes = "MVP"
        });
        Assert.Equal(HttpStatusCode.OK, bom.StatusCode);

        var purchasingDash = await client.GetAsync("/api/v1/purchasing/dashboard");
        Assert.Equal(HttpStatusCode.OK, purchasingDash.StatusCode);

        var salesDash = await client.GetAsync("/api/v1/sales/dashboard");
        Assert.Equal(HttpStatusCode.OK, salesDash.StatusCode);
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = _factory.CreateClient();
        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            username = "admin",
            password = "Naswood!Admin1"
        });
        login.EnsureSuccessStatusCode();
        using var document = await JsonDocument.ParseAsync(await login.Content.ReadAsStreamAsync());
        var token = document.RootElement.GetProperty("data").GetProperty("accessToken").GetString()!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
