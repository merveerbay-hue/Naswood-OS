using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class HealthEndpointTests
{
    private readonly NaswoodApiFactory _factory;

    public HealthEndpointTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Live_returns_healthy_envelope()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var root = document.RootElement;

        Assert.True(root.GetProperty("success").GetBoolean());
        Assert.Equal("Healthy", root.GetProperty("data").GetProperty("status").GetString());
    }

    [Fact]
    public async Task Ready_returns_component_list()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var root = document.RootElement;

        Assert.True(root.GetProperty("success").GetBoolean());
        Assert.True(root.GetProperty("data").GetProperty("components").GetArrayLength() >= 1);
    }

    [Fact]
    public async Task Health_returns_version_and_components()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var root = document.RootElement;
        var data = root.GetProperty("data");

        Assert.True(root.GetProperty("success").GetBoolean());
        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("version").GetString()));
        Assert.True(data.GetProperty("components").GetArrayLength() >= 1);
    }
}
