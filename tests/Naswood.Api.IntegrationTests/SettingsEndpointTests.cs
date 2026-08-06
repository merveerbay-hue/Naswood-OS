using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class SettingsEndpointTests
{
    private readonly NaswoodApiFactory _factory;

    public SettingsEndpointTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Create_update_reset_and_categories()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var categories = await client.GetAsync("/api/v1/settings/categories");
        Assert.Equal(HttpStatusCode.OK, categories.StatusCode);

        var create = await client.PostAsJsonAsync("/api/v1/settings", new
        {
            category = "Inventory",
            key = "inventory.negativeStockAllowed",
            name = "Allow Negative Stock",
            value = "false",
            dataType = "Boolean",
            defaultValue = "false",
            scope = "Global",
            isRequired = true
        });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        using var created = await JsonDocument.ParseAsync(await create.Content.ReadAsStreamAsync());
        var id = created.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        var update = await client.PutAsJsonAsync($"/api/v1/settings/{id}", new { value = "true" });
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);

        var reset = await client.PostAsJsonAsync("/api/v1/settings/reset", new { id });
        Assert.Equal(HttpStatusCode.OK, reset.StatusCode);
        using var resetDoc = await JsonDocument.ParseAsync(await reset.Content.ReadAsStreamAsync());
        Assert.Equal("false", resetDoc.RootElement.GetProperty("data").GetProperty("value").GetString());

        var list = await client.GetAsync("/api/v1/settings?category=Localization");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        using var listDoc = await JsonDocument.ParseAsync(await list.Content.ReadAsStreamAsync());
        Assert.True(listDoc.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32() >= 1);
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
