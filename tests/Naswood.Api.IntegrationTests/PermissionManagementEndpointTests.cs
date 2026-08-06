using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class PermissionManagementEndpointTests
{
    private readonly NaswoodApiFactory _factory;

    public PermissionManagementEndpointTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Create_validate_and_protect_reserved_permission()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var validate = await client.PostAsJsonAsync("/api/v1/permissions/validate", new
        {
            code = "PurchaseOrder.Release",
            module = "Purchasing",
            feature = "PurchaseOrder",
            action = "Release"
        });
        Assert.Equal(HttpStatusCode.OK, validate.StatusCode);
        using var validateDoc = await JsonDocument.ParseAsync(await validate.Content.ReadAsStreamAsync());
        Assert.True(validateDoc.RootElement.GetProperty("data").GetProperty("isValid").GetBoolean());

        var create = await client.PostAsJsonAsync("/api/v1/permissions", new
        {
            code = "PurchaseOrder.Release",
            module = "Purchasing",
            feature = "PurchaseOrder",
            action = "Release",
            category = "Transaction"
        });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);

        var templates = await client.GetAsync("/api/v1/permissions/templates");
        Assert.Equal(HttpStatusCode.OK, templates.StatusCode);

        var list = await client.GetAsync("/api/v1/permissions?code=Inventory.View");
        list.EnsureSuccessStatusCode();
        using var listDoc = await JsonDocument.ParseAsync(await list.Content.ReadAsStreamAsync());
        var reservedId = listDoc.RootElement.GetProperty("data").GetProperty("items")[0].GetProperty("id").GetGuid();

        var deleteReserved = await client.DeleteAsync($"/api/v1/permissions/{reservedId}");
        Assert.Equal(HttpStatusCode.Conflict, deleteReserved.StatusCode);
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
