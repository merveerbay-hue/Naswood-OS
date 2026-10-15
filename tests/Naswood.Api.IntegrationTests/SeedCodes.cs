using Naswood.Modules.Business.Application.Inventory;

namespace Naswood.Api.IntegrationTests;

/// <summary>
/// Package codes for rows seeded directly into the database. They sit far above anything the
/// sequences mint during a test, so seeded and minted codes never collide.
/// </summary>
internal static class SeedCodes
{
    public static PackageIdentityMint Package(int n) =>
        new(IdentityCodes.Format(IdentityType.Pkg, IdentityCodes.LocalSiteCode, 900_000 + n), Guid.NewGuid().ToString("N"));
}
