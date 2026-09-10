using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Naswood.BuildingBlocks.Infrastructure;
using Naswood.BuildingBlocks.Infrastructure.Storage;
using Naswood.Modules.Platform.Application;
using Naswood.Modules.Platform.Infrastructure;
using Naswood.Modules.Platform.Infrastructure.Persistence;
using Naswood.Modules.Platform.Presentation;
using Naswood.Modules.Business.Application;
using Naswood.Modules.Business.Infrastructure;
using Naswood.Modules.Business.Presentation;
using Naswood.Modules.Business.Infrastructure.Persistence;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddPlatformPresentation()
    .AddBusinessPresentation();

builder.Services.AddPlatformApplication();
builder.Services.AddBusinessApplication();
builder.Services.AddPlatformInfrastructure(builder.Configuration);
builder.Services.AddBusinessInfrastructure(builder.Configuration);
builder.Services.AddBuildingBlocksInfrastructure(
    typeof(Naswood.Modules.Platform.Application.DependencyInjection).Assembly,
    typeof(Naswood.Modules.Business.Application.DependencyInjection).Assembly);
builder.Services.AddFileStorage(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
    await db.Database.EnsureCreatedAsync();
    var businessDb = scope.ServiceProvider.GetRequiredService<BusinessDbContext>();
    // Same PostgreSQL database as Platform — EnsureCreated is a no-op once DB exists.
    // CreateTables also fails once any business table exists, so apply the model script
    // statement-by-statement and ignore "already exists" for incremental entity adds.
    var businessCreator = businessDb.Database.GetService<IRelationalDatabaseCreator>();
    foreach (var stmt in new[]
    {
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "PublicId" character varying(40) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "PhysicalGroupLabel" character varying(200) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "SourcePlantId" character varying(20) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "LabelPrintedAt" timestamp with time zone NULL""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "LabelPrintCount" integer NOT NULL DEFAULT 0""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "MaterialId" uuid NULL""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "BatchId" uuid NULL""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "WarehouseId" uuid NULL""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "LocationId" uuid NULL""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "CurrentPlantId" character varying(20) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_inventory_batch ADD COLUMN IF NOT EXISTS "SourceReferenceNo" character varying(80) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_production_output ADD COLUMN IF NOT EXISTS "CancelledBy" character varying(200) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_production_output ADD COLUMN IF NOT EXISTS "CancelReason" character varying(500) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_production_output ADD COLUMN IF NOT EXISTS "CancelledAt" timestamp with time zone NULL""",
        """ALTER TABLE IF EXISTS business.business_production_output ADD COLUMN IF NOT EXISTS "QcDecision" character varying(40) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_production_output ADD COLUMN IF NOT EXISTS "QcDecidedBy" character varying(200) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_production_output ADD COLUMN IF NOT EXISTS "QcDecidedAt" timestamp with time zone NULL""",
        """ALTER TABLE IF EXISTS business.business_production_output ADD COLUMN IF NOT EXISTS "QcInspectionReference" character varying(120) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_production_output ADD COLUMN IF NOT EXISTS "QcNotes" character varying(500) NOT NULL DEFAULT ''""",
    })
    {
        try { await businessDb.Database.ExecuteSqlRawAsync(stmt).ConfigureAwait(false); }
        catch { /* table may not exist yet */ }
    }
    try
    {
        await businessCreator.CreateTablesAsync();
    }
    catch (Exception ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
    {
        var script = businessDb.Database.GenerateCreateScript();
        foreach (var statement in SplitPostgresStatements(script))
        {
            try
            {
                await businessDb.Database.ExecuteSqlRawAsync(statement).ConfigureAwait(false);
            }
            catch (Exception statementEx) when (
                statementEx.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase)
                || statementEx.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
                || statementEx.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
            {
                // Table/index/schema already provisioned or column patch still pending.
            }
        }
    }

    // Incremental patches for existing databases (EnsureCreated / CreateTables do not ALTER columns).
    await businessDb.Database.ExecuteSqlRawAsync(
        """
        ALTER TABLE IF EXISTS business.business_inventory_material ADD COLUMN IF NOT EXISTS "DefinitionJson" text NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_warehouse ADD COLUMN IF NOT EXISTS "Description" character varying(500) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_location ADD COLUMN IF NOT EXISTS "Description" character varying(500) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_goodsreceipt ALTER COLUMN "Notes" TYPE text;
        ALTER TABLE IF EXISTS business.business_inventory_goodsissue ALTER COLUMN "Notes" TYPE text;
        ALTER TABLE IF EXISTS business.business_inventory_inventorycount ALTER COLUMN "Notes" TYPE text;
        ALTER TABLE IF EXISTS business.business_inventory_inventorycount ADD COLUMN IF NOT EXISTS "LocationCode" character varying(200) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_inventorycount ADD COLUMN IF NOT EXISTS "CountType" character varying(40) NOT NULL DEFAULT 'Normal';
        ALTER TABLE IF EXISTS business.business_inventory_inventorycount ADD COLUMN IF NOT EXISTS "SnapshotAt" timestamp with time zone NULL;
        ALTER TABLE IF EXISTS business.business_inventory_inventorycount ADD COLUMN IF NOT EXISTS "StartedBy" character varying(200) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_inventorycount ADD COLUMN IF NOT EXISTS "StartedAt" timestamp with time zone NOT NULL DEFAULT NOW();
        ALTER TABLE IF EXISTS business.business_inventory_inventorycount ADD COLUMN IF NOT EXISTS "CountedBy" character varying(200) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_inventorycount ADD COLUMN IF NOT EXISTS "CompletedBy" character varying(200) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_inventorycount ADD COLUMN IF NOT EXISTS "CompletedAt" timestamp with time zone NULL;
        ALTER TABLE IF EXISTS business.business_inventory_inventorycount ADD COLUMN IF NOT EXISTS "ApprovedBy" character varying(200) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_inventorycount ADD COLUMN IF NOT EXISTS "ApprovedAt" timestamp with time zone NULL;
        CREATE TABLE IF NOT EXISTS business.business_inventory_inventorycountline (
            "Id" uuid NOT NULL PRIMARY KEY,
            "CompanyId" character varying(20) NOT NULL,
            "PlantId" character varying(20),
            "CreatedAt" timestamp with time zone NOT NULL,
            "UpdatedAt" timestamp with time zone NOT NULL,
            "IsDeleted" boolean NOT NULL,
            "CountId" uuid NOT NULL,
            "LineNo" integer NOT NULL,
            "Role" character varying(40),
            "Source" character varying(40),
            "MaterialId" uuid NULL,
            "MaterialCode" character varying(200),
            "MaterialName" character varying(400),
            "LocationCode" character varying(200),
            "BatchNumber" character varying(200),
            "LotUnknown" boolean NOT NULL DEFAULT FALSE,
            "PackageNumber" character varying(200),
            "ThicknessMm" numeric(18,4) NULL,
            "WidthMm" numeric(18,4) NULL,
            "LengthMm" numeric(18,4) NULL,
            "PieceCount" numeric(18,4) NULL,
            "MeasuredVolumeM3" numeric(18,6) NULL,
            "SystemQuantityAtStart" numeric(18,6) NOT NULL DEFAULT 0,
            "CountedQuantity" numeric(18,6) NOT NULL DEFAULT 0,
            "StockUnit" character varying(40),
            "CountUnit" character varying(40),
            "CalculatedStockQty" numeric(18,6) NOT NULL DEFAULT 0,
            "KeepSeparate" boolean NOT NULL DEFAULT FALSE,
            "Approved" boolean NOT NULL DEFAULT FALSE,
            "Notes" text
        );
        CREATE INDEX IF NOT EXISTS "IX_business_inventory_inventorycountline_CountId"
            ON business.business_inventory_inventorycountline ("CountId");
        ALTER TABLE IF EXISTS business.business_inventory_inventorycountline ADD COLUMN IF NOT EXISTS "PhysicalGroupLabel" character varying(200) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_inventorycountline ADD COLUMN IF NOT EXISTS "Barcode" character varying(200) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_batch ADD COLUMN IF NOT EXISTS "SourceType" character varying(40) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_batch ADD COLUMN IF NOT EXISTS "SourceReferenceNo" character varying(80) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "PublicId" character varying(40) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "PhysicalGroupLabel" character varying(200) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "SourcePlantId" character varying(20) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "LabelPrintedAt" timestamp with time zone NULL;
        ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "LabelPrintCount" integer NOT NULL DEFAULT 0;
        ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "MaterialId" uuid NULL;
        ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "BatchId" uuid NULL;
        ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "WarehouseId" uuid NULL;
        ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "LocationId" uuid NULL;
        ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "CurrentPlantId" character varying(20) NOT NULL DEFAULT '';
        ALTER TABLE IF EXISTS business.business_inventory_movement ADD COLUMN IF NOT EXISTS "PackageId" uuid NULL;
        CREATE INDEX IF NOT EXISTS "IX_movement_packageid" ON business.business_inventory_movement ("PackageId");
        ALTER TABLE IF EXISTS business.business_inventory_location ADD COLUMN IF NOT EXISTS "StockZoneType" character varying(40) NOT NULL DEFAULT '';
        UPDATE business.business_inventory_location
        SET "StockZoneType" = 'QUARANTINE'
        WHERE "StockZoneType" = '' AND UPPER("LocationType") IN ('QUARANTINE', 'QUARANTINE_AREA');
        UPDATE business.business_inventory_location
        SET "StockZoneType" = 'NORMAL'
        WHERE "StockZoneType" = '';
        CREATE TABLE IF NOT EXISTS business.business_inventory_packageoperation (
            "Id" uuid NOT NULL PRIMARY KEY,
            "CompanyId" character varying(20) NOT NULL,
            "PlantId" character varying(20),
            "CreatedAt" timestamp with time zone NOT NULL,
            "UpdatedAt" timestamp with time zone NOT NULL,
            "IsDeleted" boolean NOT NULL,
            "Number" character varying(80) NOT NULL,
            "OperationType" character varying(40) NOT NULL,
            "Status" character varying(40) NOT NULL,
            "WarehouseCode" character varying(200) NOT NULL DEFAULT '',
            "LocationCode" character varying(200) NOT NULL DEFAULT '',
            "Notes" character varying(500) NOT NULL DEFAULT '',
            "PostedBy" character varying(200) NOT NULL DEFAULT '',
            "PostedAt" timestamp with time zone NULL
        );
        CREATE UNIQUE INDEX IF NOT EXISTS "UX_packageoperation_number_alive"
            ON business.business_inventory_packageoperation ("PlantId", "Number")
            WHERE "IsDeleted" = false AND "Number" <> '';
        CREATE TABLE IF NOT EXISTS business.business_inventory_packagerelation (
            "Id" uuid NOT NULL PRIMARY KEY,
            "CompanyId" character varying(20) NOT NULL,
            "PlantId" character varying(20),
            "CreatedAt" timestamp with time zone NOT NULL,
            "UpdatedAt" timestamp with time zone NOT NULL,
            "IsDeleted" boolean NOT NULL,
            "OperationId" uuid NOT NULL,
            "SourcePackageId" uuid NOT NULL,
            "TargetPackageId" uuid NOT NULL,
            "RelationType" character varying(40) NOT NULL,
            "Quantity" numeric(18,4) NOT NULL,
            "Unit" character varying(40) NOT NULL DEFAULT ''
        );
        CREATE INDEX IF NOT EXISTS "IX_packagerelation_source" ON business.business_inventory_packagerelation ("SourcePackageId");
        CREATE INDEX IF NOT EXISTS "IX_packagerelation_target" ON business.business_inventory_packagerelation ("TargetPackageId");
        CREATE UNIQUE INDEX IF NOT EXISTS "UX_packagerelation_edge"
            ON business.business_inventory_packagerelation ("OperationId", "SourcePackageId", "TargetPackageId", "RelationType")
            WHERE "IsDeleted" = false;
        UPDATE business.business_inventory_package
        SET "PublicId" = replace("Id"::text, '-', '')
        WHERE "PublicId" = '';
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
        """).ConfigureAwait(false);

    foreach (var stmt in new[]
    {
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "PublicId" character varying(40) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "PhysicalGroupLabel" character varying(200) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "SourcePlantId" character varying(20) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "LabelPrintedAt" timestamp with time zone NULL""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "LabelPrintCount" integer NOT NULL DEFAULT 0""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "MaterialId" uuid NULL""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "BatchId" uuid NULL""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "WarehouseId" uuid NULL""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "LocationId" uuid NULL""",
        """ALTER TABLE IF EXISTS business.business_inventory_package ADD COLUMN IF NOT EXISTS "CurrentPlantId" character varying(20) NOT NULL DEFAULT ''""",
        """ALTER TABLE IF EXISTS business.business_inventory_batch ADD COLUMN IF NOT EXISTS "SourceReferenceNo" character varying(80) NOT NULL DEFAULT ''""",
        """UPDATE business.business_inventory_package SET "PublicId" = replace("Id"::text, '-', '') WHERE "PublicId" = ''""",
    })
    {
        try { await businessDb.Database.ExecuteSqlRawAsync(stmt).ConfigureAwait(false); }
        catch { /* already applied or legacy table missing */ }
    }

    await EnsurePackageIdentityIntegrityAsync(businessDb).ConfigureAwait(false);
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static IEnumerable<string> SplitPostgresStatements(string script)
{
    var statements = new List<string>();
    var buffer = new StringBuilder();
    var inDollar = false;

    for (var i = 0; i < script.Length; i++)
    {
        if (script[i] == '$' && i + 3 < script.Length && script.AsSpan(i, 4).SequenceEqual("$EF$".AsSpan()))
        {
            inDollar = !inDollar;
            buffer.Append("$EF$");
            i += 3;
            continue;
        }

        if (script[i] == ';' && !inDollar)
        {
            var statement = buffer.ToString().Trim();
            if (statement.Length > 0)
            {
                statements.Add(statement);
            }

            buffer.Clear();
            continue;
        }

        buffer.Append(script[i]);
    }

    var tail = buffer.ToString().Trim();
    if (tail.Length > 0)
    {
        statements.Add(tail);
    }

    return statements;
}

static async Task EnsurePackageIdentityIntegrityAsync(BusinessDbContext businessDb)
{
    try
    {
        await businessDb.Database.ExecuteSqlRawAsync(
            """
            UPDATE business.business_inventory_package p
            SET "MaterialId" = m."Id"
            FROM business.business_inventory_material m
            WHERE p."MaterialId" IS NULL
              AND p."IsDeleted" = false
              AND m."IsDeleted" = false
              AND lower(p."MaterialCode") = lower(m."Code");

            UPDATE business.business_inventory_package p
            SET "BatchId" = b."Id"
            FROM business.business_inventory_batch b
            WHERE p."BatchId" IS NULL
              AND p."IsDeleted" = false
              AND b."IsDeleted" = false
              AND lower(p."LotNumber") = lower(b."BatchNumber")
              AND lower(p."MaterialCode") = lower(b."MaterialCode")
              AND (p."PlantId" IS NULL OR b."PlantId" IS NULL OR p."PlantId" = b."PlantId");

            UPDATE business.business_inventory_package p
            SET "WarehouseId" = w."Id"
            FROM business.business_inventory_warehouse w
            WHERE p."WarehouseId" IS NULL
              AND p."IsDeleted" = false
              AND w."IsDeleted" = false
              AND lower(p."WarehouseCode") = lower(w."Code")
              AND (p."PlantId" IS NULL OR w."PlantId" IS NULL OR p."PlantId" = w."PlantId");

            UPDATE business.business_inventory_package p
            SET "LocationId" = l."Id"
            FROM business.business_inventory_location l
            WHERE p."LocationId" IS NULL
              AND p."IsDeleted" = false
              AND l."IsDeleted" = false
              AND lower(p."LocationCode") = lower(l."Code")
              AND lower(p."WarehouseCode") = lower(l."WarehouseCode")
              AND (p."PlantId" IS NULL OR l."PlantId" IS NULL OR p."PlantId" = l."PlantId");

            UPDATE business.business_inventory_package
            SET "CurrentPlantId" = COALESCE(NULLIF("CurrentPlantId", ''), "PlantId", '')
            WHERE "CurrentPlantId" = '';

            UPDATE business.business_inventory_package
            SET "PublicId" = replace("Id"::text, '-', '')
            WHERE "PublicId" = '';
            """).ConfigureAwait(false);
    }
    catch (Exception ex) when (ex.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
    {
        return;
    }

    var collisions = new List<string>();
    collisions.AddRange(await QueryDuplicatePackageKeysAsync(
        businessDb,
        """SELECT "Barcode" AS k, COUNT(*)::int AS n FROM business.business_inventory_package WHERE "IsDeleted" = false AND "Barcode" <> '' GROUP BY "Barcode" HAVING COUNT(*) > 1""").ConfigureAwait(false));
    collisions.AddRange(await QueryDuplicatePackageKeysAsync(
        businessDb,
        """SELECT "PublicId" AS k, COUNT(*)::int AS n FROM business.business_inventory_package WHERE "IsDeleted" = false AND "PublicId" <> '' GROUP BY "PublicId" HAVING COUNT(*) > 1""").ConfigureAwait(false));
    if (collisions.Count > 0)
        throw new InvalidOperationException(
            "Paket barkod/PublicId unique constraint kurulamadı. Çakışmalar: " + string.Join("; ", collisions));

    await businessDb.Database.ExecuteSqlRawAsync(
        """CREATE UNIQUE INDEX IF NOT EXISTS "UX_package_barcode_alive" ON business.business_inventory_package ("Barcode") WHERE "IsDeleted" = false AND "Barcode" <> ''""").ConfigureAwait(false);
    await businessDb.Database.ExecuteSqlRawAsync(
        """CREATE UNIQUE INDEX IF NOT EXISTS "UX_package_number_alive" ON business.business_inventory_package ("PackageNumber") WHERE "IsDeleted" = false AND "PackageNumber" <> ''""").ConfigureAwait(false);
    await businessDb.Database.ExecuteSqlRawAsync(
        """CREATE UNIQUE INDEX IF NOT EXISTS "UX_package_publicid_alive" ON business.business_inventory_package ("PublicId") WHERE "IsDeleted" = false AND "PublicId" <> ''""").ConfigureAwait(false);
    await businessDb.Database.ExecuteSqlRawAsync(
        """CREATE UNIQUE INDEX IF NOT EXISTS "UX_batch_production_lot_alive" ON business.business_inventory_batch ("BatchNumber") WHERE "IsDeleted" = false AND "SourceType" = 'PRODUCTION' AND "BatchNumber" <> ''""").ConfigureAwait(false);
    try
    {
        await businessDb.Database.ExecuteSqlRawAsync(
            """CREATE UNIQUE INDEX IF NOT EXISTS "UX_production_output_number_alive" ON business.business_production_output ("Number") WHERE "IsDeleted" = false AND "Number" <> ''""").ConfigureAwait(false);
    }
    catch (Exception ex) when (ex.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
    {
        // Table created on next restart after GenerateCreateScript.
    }
}

static async Task<List<string>> QueryDuplicatePackageKeysAsync(BusinessDbContext db, string sql)
{
    var rows = new List<string>();
    var conn = db.Database.GetDbConnection();
    if (conn.State != System.Data.ConnectionState.Open)
        await conn.OpenAsync().ConfigureAwait(false);
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
    while (await reader.ReadAsync().ConfigureAwait(false))
        rows.Add($"{reader.GetString(0)} ×{reader.GetInt32(1)}");
    return rows;
}

public partial class Program;
