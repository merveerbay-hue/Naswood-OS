using Microsoft.EntityFrameworkCore;

namespace Naswood.Modules.Business.Infrastructure.Persistence;

/// <summary>
/// Data corrections that run on every startup after migrations. They change rows, never schema.
/// </summary>
public static class BusinessStartupDataFixes
{
    public static async Task ApplyAsync(BusinessDbContext db, CancellationToken cancellationToken = default)
    {
        // Masif panel (MP) materials are stocked in M3; rows saved with M2 are normalized here.
        await db.Database.ExecuteSqlRawAsync(
            """
            UPDATE business.business_inventory_material
            SET
                "UnitOfMeasure" = 'M3',
                "DefinitionJson" = replace(replace(replace(replace(
                    "DefinitionJson",
                    '"stockUom":"M2"', '"stockUom":"M3"'),
                    '"stockUom": "M2"', '"stockUom": "M3"'),
                    '"volumeCalcRequired":false', '"volumeCalcRequired":true'),
                    '"volumeCalcRequired": false', '"volumeCalcRequired": true')
            WHERE "IsDeleted" = false
              AND (
                    "Code" LIKE 'MP-%'
                    OR "Category" ILIKE '%Masif%'
                    OR "Category" = 'MP'
                    OR "DefinitionJson" ILIKE '%"mainCategory":"MP"%'
                    OR "DefinitionJson" ILIKE '%"mainCategory": "MP"%'
              );
            """,
            cancellationToken).ConfigureAwait(false);

        // A partial package move can leave WarehouseId empty when the destination warehouse has no plant;
        // fill it from the warehouse code.
        await db.Database.ExecuteSqlRawAsync(
            """
            UPDATE business.business_inventory_package p
            SET "WarehouseId" = w."Id"
            FROM business.business_inventory_warehouse w
            WHERE p."WarehouseId" IS NULL
              AND p."IsDeleted" = false
              AND w."IsDeleted" = false
              AND lower(p."WarehouseCode") = lower(w."Code")
              AND (p."PlantId" IS NULL OR w."PlantId" IS NULL OR p."PlantId" = w."PlantId");
            """,
            cancellationToken).ConfigureAwait(false);
    }
}
