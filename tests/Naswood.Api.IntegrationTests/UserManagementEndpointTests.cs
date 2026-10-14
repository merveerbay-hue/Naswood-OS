using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class UserManagementEndpointTests
{
    private readonly NaswoodApiFactory _factory;

    public UserManagementEndpointTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Create_activate_and_search_user()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var create = await client.PostAsJsonAsync("/api/v1/users", new
        {
            employeeNumber = "EMP001",
            username = "jdoe",
            firstName = "John",
            lastName = "Doe",
            email = "john.doe@naswood.com",
            password = "Naswood!User12",
            company = "COMP-001",
            plant = "PLANT-001",
            departmentCode = "PURCHASING",
            positionCode = "BUYER",
            roles = new[] { "ReadOnly" }
        });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        using var createdDoc = await JsonDocument.ParseAsync(await create.Content.ReadAsStreamAsync());
        var userId = createdDoc.RootElement.GetProperty("data").GetProperty("id").GetGuid();
        Assert.Equal("PendingActivation", createdDoc.RootElement.GetProperty("data").GetProperty("status").GetString());

        var activate = await client.PostAsync($"/api/v1/users/{userId}/activate", null);
        Assert.Equal(HttpStatusCode.OK, activate.StatusCode);

        var list = await client.GetAsync("/api/v1/users?username=jdoe");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        using var listDoc = await JsonDocument.ParseAsync(await list.Content.ReadAsStreamAsync());
        Assert.Equal(1, listDoc.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32());

        var detail = await client.GetAsync($"/api/v1/users/{userId}");
        Assert.Equal(HttpStatusCode.OK, detail.StatusCode);
        using var detailDoc = await JsonDocument.ParseAsync(await detail.Content.ReadAsStreamAsync());
        Assert.Equal("Active", detailDoc.RootElement.GetProperty("data").GetProperty("status").GetString());
        Assert.Equal("EMP001", detailDoc.RootElement.GetProperty("data").GetProperty("employeeNumber").GetString());
    }

    [Fact]
    public async Task Deactivated_user_cannot_login()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var create = await client.PostAsJsonAsync("/api/v1/users", new
        {
            employeeNumber = "EMP002",
            username = "ada",
            firstName = "Ada",
            lastName = "Lovelace",
            email = "ada@naswood.com",
            password = "Naswood!User12",
            companyIds = new[] { "COMP-001" },
            plantIds = new[] { "PLANT-001" },
            roles = new[] { "ReadOnly" }
        });
        create.EnsureSuccessStatusCode();
        using var createdDoc = await JsonDocument.ParseAsync(await create.Content.ReadAsStreamAsync());
        var userId = createdDoc.RootElement.GetProperty("data").GetProperty("id").GetGuid();

        (await client.PostAsync($"/api/v1/users/{userId}/activate", null)).EnsureSuccessStatusCode();
        (await client.PostAsync($"/api/v1/users/{userId}/deactivate", null)).EnsureSuccessStatusCode();

        var anon = _factory.CreateClient();
        var login = await anon.PostAsJsonAsync("/api/v1/auth/login", new
        {
            username = "ada",
            password = "Naswood!User12"
        });
        Assert.Equal(HttpStatusCode.Forbidden, login.StatusCode);
    }

    [Fact]
    public async Task Export_returns_csv()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/v1/users/export");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);
        var csv = await response.Content.ReadAsStringAsync();
        Assert.Contains("employeeNumber", csv);
        Assert.Contains("admin", csv);
    }

    [Fact]
    public async Task Import_creates_users_from_csv()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        var csv = """
                  employeeNumber,username,firstName,lastName,email,password,company,plant,department,position,roles
                  EMP010,importer,Imp,Orter,importer@naswood.com,Naswood!User12,COMP-001,PLANT-001,SALES,BUYER,ReadOnly
                  """;
        using var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes(csv)), "file", "users.csv");

        var response = await client.PostAsync("/api/v1/users/import", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        Assert.Equal(1, doc.RootElement.GetProperty("data").GetProperty("createdCount").GetInt32());
        Assert.Equal(0, doc.RootElement.GetProperty("data").GetProperty("failedCount").GetInt32());
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
