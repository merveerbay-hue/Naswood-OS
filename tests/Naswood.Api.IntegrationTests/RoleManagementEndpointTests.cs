using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class RoleManagementEndpointTests
{
    private readonly NaswoodApiFactory _factory;

    public RoleManagementEndpointTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Create_clone_and_protect_system_role()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var create = await client.PostAsJsonAsync("/api/v1/roles", new
        {
            code = "PUR_MANAGER",
            name = "Purchasing Manager",
            company = "COMP-001",
            permissions = new[] { "Purchasing.View", "PurchaseOrder.Approve" }
        });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        using var createdDoc = await JsonDocument.ParseAsync(await create.Content.ReadAsStreamAsync());
        var roleId = createdDoc.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        var clone = await client.PostAsJsonAsync($"/api/v1/roles/{roleId}/clone", new
        {
            code = "PUR_MANAGER_COPY",
            name = "Purchasing Manager Copy"
        });
        Assert.Equal(HttpStatusCode.OK, clone.StatusCode);

        var list = await client.GetAsync("/api/v1/roles?code=PUR");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        using var listDoc = await JsonDocument.ParseAsync(await list.Content.ReadAsStreamAsync());
        Assert.True(listDoc.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32() >= 2);

        var roles = await client.GetAsync("/api/v1/roles?code=Administrator");
        roles.EnsureSuccessStatusCode();
        using var adminDoc = await JsonDocument.ParseAsync(await roles.Content.ReadAsStreamAsync());
        var adminId = adminDoc.RootElement.GetProperty("data").GetProperty("items")[0].GetProperty("id").GetGuid();

        var deleteAdmin = await client.DeleteAsync($"/api/v1/roles/{adminId}");
        Assert.Equal(HttpStatusCode.Conflict, deleteAdmin.StatusCode);
    }

    [Fact]
    public async Task Assign_permission_and_user()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var createRole = await client.PostAsJsonAsync("/api/v1/roles", new
        {
            code = "WAREHOUSE_OP",
            name = "Warehouse Operator Role",
            permissions = new[] { "Warehouse.View" }
        });
        createRole.EnsureSuccessStatusCode();
        using var roleDoc = await JsonDocument.ParseAsync(await createRole.Content.ReadAsStreamAsync());
        var roleId = roleDoc.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        var assignPerm = await client.PostAsJsonAsync($"/api/v1/roles/{roleId}/assign-permission", new
        {
            permissions = new[] { "GoodsReceipt.Execute" }
        });
        Assert.Equal(HttpStatusCode.OK, assignPerm.StatusCode);

        var createUser = await client.PostAsJsonAsync("/api/v1/users", new
        {
            employeeNumber = "EMP-ROLE-1",
            username = "whop",
            firstName = "Ware",
            lastName = "House",
            email = "whop@naswood.com",
            password = "Naswood!User12",
            companyIds = new[] { "COMP-001" },
            plantIds = new[] { "PLANT-001" },
            roles = new[] { "ReadOnly" }
        });
        createUser.EnsureSuccessStatusCode();
        using var userDoc = await JsonDocument.ParseAsync(await createUser.Content.ReadAsStreamAsync());
        var userId = userDoc.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        var assignUser = await client.PostAsJsonAsync($"/api/v1/roles/{roleId}/assign-user", new { userId });
        Assert.Equal(HttpStatusCode.OK, assignUser.StatusCode);
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
