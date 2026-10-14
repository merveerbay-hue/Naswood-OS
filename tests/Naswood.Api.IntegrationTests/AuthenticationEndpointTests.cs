using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Naswood.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public class AuthenticationEndpointTests
{
    private readonly NaswoodApiFactory _factory;

    public AuthenticationEndpointTests(NaswoodApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Login_with_valid_credentials_returns_tokens()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            username = "admin",
            password = "Naswood!Admin1",
            deviceName = "integration-test"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var data = document.RootElement.GetProperty("data");
        Assert.True(document.RootElement.GetProperty("success").GetBoolean());
        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("accessToken").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("refreshToken").GetString()));
        Assert.Equal(3600, data.GetProperty("expiresIn").GetInt32());
        Assert.Equal("admin", data.GetProperty("user").GetProperty("username").GetString());
    }

    [Fact]
    public async Task Login_with_invalid_password_returns_unauthorized()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            username = "admin",
            password = "WrongPassword!1"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        Assert.False(document.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal("AUTH-001", document.RootElement.GetProperty("errors")[0].GetProperty("code").GetString());
    }

    [Fact]
    public async Task Five_failed_logins_lock_account()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();

        for (var i = 0; i < 5; i++)
        {
            await client.PostAsJsonAsync("/api/v1/auth/login", new
            {
                username = "admin",
                password = "WrongPassword!1"
            });
        }

        var locked = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            username = "admin",
            password = "Naswood!Admin1"
        });

        Assert.Equal(HttpStatusCode.Forbidden, locked.StatusCode);
        using var document = await JsonDocument.ParseAsync(await locked.Content.ReadAsStreamAsync());
        Assert.Equal("AUTH-003", document.RootElement.GetProperty("errors")[0].GetProperty("code").GetString());
    }

    [Fact]
    public async Task Refresh_rotates_refresh_token_and_me_works()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();

        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            username = "admin",
            password = "Naswood!Admin1"
        });
        using var loginDoc = await JsonDocument.ParseAsync(await login.Content.ReadAsStreamAsync());
        var loginData = loginDoc.RootElement.GetProperty("data");
        var accessToken = loginData.GetProperty("accessToken").GetString()!;
        var refreshToken = loginData.GetProperty("refreshToken").GetString()!;

        var refresh = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken });
        Assert.Equal(HttpStatusCode.OK, refresh.StatusCode);
        using var refreshDoc = await JsonDocument.ParseAsync(await refresh.Content.ReadAsStreamAsync());
        var newRefresh = refreshDoc.RootElement.GetProperty("data").GetProperty("refreshToken").GetString();
        var newAccess = refreshDoc.RootElement.GetProperty("data").GetProperty("accessToken").GetString()!;
        Assert.NotEqual(refreshToken, newRefresh);

        var oldRefresh = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, oldRefresh.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newAccess);
        var me = await client.GetAsync("/api/v1/auth/me");
        var meBody = await me.Content.ReadAsStringAsync();
        Assert.True(me.StatusCode == HttpStatusCode.OK, $"me failed: {(int)me.StatusCode} {meBody}");

        var session = await client.GetAsync("/api/v1/auth/session");
        var sessionBody = await session.Content.ReadAsStringAsync();
        Assert.True(session.StatusCode == HttpStatusCode.OK, $"session failed: {(int)session.StatusCode} {sessionBody}");

        var logout = await client.PostAsync("/api/v1/auth/logout", null);
        var logoutBody = await logout.Content.ReadAsStringAsync();
        Assert.True(logout.StatusCode == HttpStatusCode.OK, $"logout failed: {(int)logout.StatusCode} {logoutBody}");
    }

    [Fact]
    public async Task Revoke_invalidates_refresh_token()
    {
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();

        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            username = "admin",
            password = "Naswood!Admin1"
        });
        using var loginDoc = await JsonDocument.ParseAsync(await login.Content.ReadAsStreamAsync());
        var refreshToken = loginDoc.RootElement.GetProperty("data").GetProperty("refreshToken").GetString()!;

        var revoke = await client.PostAsJsonAsync("/api/v1/auth/revoke", new { refreshToken });
        Assert.Equal(HttpStatusCode.OK, revoke.StatusCode);

        var refresh = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
    }
}
