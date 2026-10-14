using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class AuditEndpointTests
{
    private readonly NaswoodApiFactory _factory;

    public AuditEndpointTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task User_create_writes_audit_and_export_works()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var create = await client.PostAsJsonAsync("/api/v1/users", new
        {
            employeeNumber = "EMP-AUD-1",
            username = "auditor",
            firstName = "Aud",
            lastName = "Itor",
            email = "auditor@naswood.com",
            password = "Naswood!User12",
            companyIds = new[] { "COMP-001" },
            plantIds = new[] { "PLANT-001" },
            roles = new[] { "ReadOnly" }
        });
        create.EnsureSuccessStatusCode();
        using var created = await JsonDocument.ParseAsync(await create.Content.ReadAsStreamAsync());
        var userId = created.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        var search = await client.GetAsync($"/api/v1/audit/search?entity=User&entityId={userId:D}");
        Assert.Equal(HttpStatusCode.OK, search.StatusCode);
        using var searchDoc = await JsonDocument.ParseAsync(await search.Content.ReadAsStreamAsync());
        Assert.True(searchDoc.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32() >= 1);

        var byEntity = await client.GetAsync($"/api/v1/audit/entity/{userId:D}?entity=User");
        Assert.Equal(HttpStatusCode.OK, byEntity.StatusCode);

        var export = await client.GetAsync("/api/v1/audit/export?module=Administration");
        Assert.Equal(HttpStatusCode.OK, export.StatusCode);
        Assert.Equal("text/csv", export.Content.Headers.ContentType?.MediaType);
        var csv = await export.Content.ReadAsStringAsync();
        Assert.Contains("UserCreated", csv);
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
