using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class FilesEndpointTests
{
    private readonly NaswoodApiFactory _factory;

    public FilesEndpointTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Upload_get_download_search_update_and_delete()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        using var content = new MultipartFormDataContent();
        var bytes = Encoding.UTF8.GetBytes("hello naswood files");
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        content.Add(fileContent, "file", "hello.txt");
        content.Add(new StringContent("Platform"), "module");
        content.Add(new StringContent("General"), "category");
        content.Add(new StringContent("docs,test"), "tags");

        var upload = await client.PostAsync("/api/v1/files", content);
        Assert.Equal(HttpStatusCode.OK, upload.StatusCode);
        using var uploadDoc = await JsonDocument.ParseAsync(await upload.Content.ReadAsStreamAsync());
        Assert.True(uploadDoc.RootElement.GetProperty("success").GetBoolean());
        var data = uploadDoc.RootElement.GetProperty("data");
        var id = data.GetProperty("id").GetGuid();
        Assert.Equal("hello.txt", data.GetProperty("originalName").GetString());
        Assert.Equal(".txt", data.GetProperty("extension").GetString());

        var get = await client.GetAsync($"/api/v1/files/{id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var download = await client.GetAsync($"/api/v1/files/{id}/download");
        Assert.Equal(HttpStatusCode.OK, download.StatusCode);
        Assert.Equal("hello naswood files", await download.Content.ReadAsStringAsync());

        var search = await client.GetAsync("/api/v1/files/search?name=hello");
        Assert.Equal(HttpStatusCode.OK, search.StatusCode);
        using var searchDoc = await JsonDocument.ParseAsync(await search.Content.ReadAsStreamAsync());
        Assert.True(searchDoc.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32() >= 1);

        var update = await client.PutAsJsonAsync($"/api/v1/files/{id}", new
        {
            name = "hello-renamed",
            category = "Documents",
            tags = new[] { "renamed" }
        });
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);

        var delete = await client.DeleteAsync($"/api/v1/files/{id}");
        Assert.Equal(HttpStatusCode.OK, delete.StatusCode);

        var missing = await client.GetAsync($"/api/v1/files/{id}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task Upload_rejects_disallowed_extension()
    {
        await _factory.ResetDatabaseAsync();
        var client = await CreateAuthenticatedClientAsync();

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("binary"));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        content.Add(fileContent, "file", "payload.exe");

        var upload = await client.PostAsync("/api/v1/files", content);
        Assert.Equal(HttpStatusCode.BadRequest, upload.StatusCode);
        using var doc = await JsonDocument.ParseAsync(await upload.Content.ReadAsStreamAsync());
        Assert.Equal("FILE-002", doc.RootElement.GetProperty("errors")[0].GetProperty("code").GetString());
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
