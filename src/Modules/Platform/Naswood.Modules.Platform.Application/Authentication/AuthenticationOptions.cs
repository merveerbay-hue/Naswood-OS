namespace Naswood.Modules.Platform.Application.Authentication;

public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    public string Issuer { get; set; } = "Naswood.OS";

    public string Audience { get; set; } = "Naswood.OS";

    public string SigningKey { get; set; } = string.Empty;

    public int AccessTokenMinutes { get; set; } = 60;

    public int RefreshTokenDays { get; set; } = 30;

    public int IdleTimeoutMinutes { get; set; } = 30;

    public int AbsoluteSessionHours { get; set; } = 12;

    public int BcryptWorkFactor { get; set; } = 12;

    public BootstrapAdminOptions BootstrapAdmin { get; set; } = new();
}

public sealed class BootstrapAdminOptions
{
    public bool Enabled { get; set; } = true;

    public string Username { get; set; } = "admin";

    public string Password { get; set; } = string.Empty;

    public string DisplayName { get; set; } = "Administrator";

    public string Email { get; set; } = "admin@naswood.local";

    public string CompanyId { get; set; } = "COMP-001";

    public string PlantId { get; set; } = "PLANT-001";

    public string Role { get; set; } = "Administrator";
}
