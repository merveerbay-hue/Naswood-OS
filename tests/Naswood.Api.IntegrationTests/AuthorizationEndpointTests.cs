using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class AuthorizationEndpointTests
{
    private readonly NaswoodApiFactory _factory;

    public AuthorizationEndpointTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Me_permissions_returns_administrator_grants()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/v1/me/permissions");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = document.RootElement.GetProperty("data");
        Assert.Contains(data.GetProperty("roles").EnumerateArray(), r => r.GetString() == "Administrator");
        Assert.True(data.GetProperty("permissions").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Check_allows_granted_permission_and_denies_unknown()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var allowed = await client.PostAsJsonAsync("/api/v1/authorization/check", new
        {
            permission = "Inventory.View",
            companyId = "COMP-001",
            plantId = "PLANT-001"
        });
        Assert.Equal(HttpStatusCode.OK, allowed.StatusCode);
        using var allowedDoc = await JsonDocument.ParseAsync(await allowed.Content.ReadAsStreamAsync());
        Assert.True(allowedDoc.RootElement.GetProperty("data").GetProperty("allowed").GetBoolean());

        var denied = await client.PostAsJsonAsync("/api/v1/authorization/check", new
        {
            permission = "DoesNotExist.Permission",
            companyId = "COMP-001",
            plantId = "PLANT-001"
        });
        Assert.Equal(HttpStatusCode.OK, denied.StatusCode);
        using var deniedDoc = await JsonDocument.ParseAsync(await denied.Content.ReadAsStreamAsync());
        Assert.False(deniedDoc.RootElement.GetProperty("data").GetProperty("allowed").GetBoolean());
        Assert.Equal("AUTHZ-001", deniedDoc.RootElement.GetProperty("data").GetProperty("denialCode").GetString());
    }

    [Fact]
    public async Task Check_denies_foreign_company()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/v1/authorization/check", new
        {
            permission = "Inventory.View",
            companyId = "COMP-999",
            plantId = "PLANT-001"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        Assert.False(document.RootElement.GetProperty("data").GetProperty("allowed").GetBoolean());
        Assert.Equal("AUTHZ-003", document.RootElement.GetProperty("data").GetProperty("denialCode").GetString());
    }

    [Fact]
    public async Task Permissions_and_roles_require_authorization_view()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var permissions = await client.GetAsync("/api/v1/permissions");
        Assert.Equal(HttpStatusCode.OK, permissions.StatusCode);
        using var permissionsDoc = await JsonDocument.ParseAsync(await permissions.Content.ReadAsStreamAsync());
        Assert.True(permissionsDoc.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32() > 0);

        var roles = await client.GetAsync("/api/v1/roles");
        Assert.Equal(HttpStatusCode.OK, roles.StatusCode);
        using var rolesDoc = await JsonDocument.ParseAsync(await roles.Content.ReadAsStreamAsync());
        Assert.True(rolesDoc.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32() > 0);

        var menu = await client.GetAsync("/api/v1/authorization/menu");
        Assert.Equal(HttpStatusCode.OK, menu.StatusCode);
        using var menuDoc = await JsonDocument.ParseAsync(await menu.Content.ReadAsStreamAsync());
        Assert.True(menuDoc.RootElement.GetProperty("data").GetArrayLength() > 0);
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
