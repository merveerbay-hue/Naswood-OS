using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Naswood.Modules.Business.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "business");

            migrationBuilder.CreateTable(
                name: "business_inventory_batch",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaterialCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SourceType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    SourceReferenceNo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_batch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_goodsissue",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_goodsissue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_goodsreceipt",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_goodsreceipt", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_inventoryadjustment",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_inventoryadjustment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_inventorybalance",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BatchNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    QuantityOnHand = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    QuantityReserved = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_inventorybalance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_inventorycount",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CountType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    SnapshotAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StartedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CountedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompletedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ApprovedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ApprovedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_inventorycount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_inventorycountline",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CountId = table.Column<Guid>(type: "uuid", nullable: false),
                    LineNo = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Source = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    MaterialCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaterialName = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BatchNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LotUnknown = table.Column<bool>(type: "boolean", nullable: false),
                    PackageNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PhysicalGroupLabel = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Barcode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ThicknessMm = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    WidthMm = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    LengthMm = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    PieceCount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    MeasuredVolumeM3 = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    SystemQuantityAtStart = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    CountedQuantity = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    StockUnit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CountUnit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CalculatedStockQty = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    KeepSeparate = table.Column<bool>(type: "boolean", nullable: false),
                    Approved = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_inventorycountline", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_location",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocationType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StockZoneType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_material",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DefinitionJson = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_material", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_materialidentity",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentityNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaterialCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LotNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RootGoodsReceiptNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_materialidentity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_movement",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MovementNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MovementType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Direction = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaterialCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaterialIdentityNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PackageNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LotNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    PackageId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_movement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_package",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaterialIdentityNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaterialCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LotNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Barcode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PublicId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    PhysicalGroupLabel = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SourcePlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CurrentPlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    BatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: true),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    LabelPrintedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LabelPrintCount = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_package", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_packagecontent",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    LineNo = table.Column<int>(type: "integer", nullable: false),
                    ThicknessMm = table.Column<decimal>(type: "numeric", nullable: true),
                    WidthMm = table.Column<decimal>(type: "numeric", nullable: true),
                    LengthMm = table.Column<decimal>(type: "numeric", nullable: true),
                    PieceCount = table.Column<decimal>(type: "numeric", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_packagecontent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_packageoperation",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    OperationType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PostedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PostedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_packageoperation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_packagerelation",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourcePackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetPackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelationType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_packagerelation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_stocktransfer",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FromWarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ToWarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_stocktransfer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_inventory_warehouse",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_inventory_warehouse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_bom",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaterialCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_bom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_calendar",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_calendar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_execution_consumption",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExecutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourcePackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceLotId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceMaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceMaterialCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PackageNumber = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Barcode = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    SourceWarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SourceLocationCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PackageContentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PhysicalMeasure = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ConsumedQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    RemainingPackageQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    PayloadHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ConsumedPieceCount = table.Column<decimal>(type: "numeric", nullable: true),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_execution_consumption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_execution_event",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExecutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ReasonCode = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_execution_event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_execution_scrap",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExecutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ReasonCode = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RecordedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_execution_scrap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_finishedgood",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_finishedgood", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_lot_source",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionLotId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceLotId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceMaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceMaterialCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ConsumedQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOutputId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourcePackageId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductionOperationExecutionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_lot_source", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_machine",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WorkCenterCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OeeTarget = table.Column<decimal>(type: "numeric", nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_machine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_materialconsumption",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_materialconsumption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_operation",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_operation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_operation_execution",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkCenterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StartedByUserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PausedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedByUserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TotalRunMinutes = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPauseMinutes = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalDowntimeMinutes = table.Column<decimal>(type: "numeric", nullable: false),
                    InputQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    OutputQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    ScrapQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    DestinationWarehouseId = table.Column<Guid>(type: "uuid", nullable: true),
                    DestinationLocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    StructuralProductionLotId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductionOutputId = table.Column<Guid>(type: "uuid", nullable: true),
                    IntervalStartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CancelReason = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CancelNote = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CancelledByUserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompleteIdempotencyKey = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    CompletePayloadHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CancelIdempotencyKey = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    CancelPayloadHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_operation_execution", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_order_operation",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    OperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkCenterId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpectedMaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutputType = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    StructuralProductionLotId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_order_operation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_output",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    OutputMaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    OutputMaterialCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: true),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkCenterCode = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    StockStatus = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    OutputBatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutputLotNumber = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    InputQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    OutputQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    ScrapQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    PostedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PostedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CancelledBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CancelReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CancelledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    QcDecision = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    QcDecidedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    QcDecidedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    QcInspectionReference = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    QcNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProductionOperationExecutionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_output", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_packaging",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_packaging", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_productionconfirmation",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_productionconfirmation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_productionline",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_productionline", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_productionorder",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_productionorder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_productionparameter",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_productionparameter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_rework",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_rework", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_routing",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaterialCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_routing", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_scrap",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_scrap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_shift",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_shift", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_shopfloor_feedback",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Topic = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Screen = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WorkCenterId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkCenterCode = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ExecutionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExecutionNumber = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ProductionOrderNumber = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Impact = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AppVersion = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    GitSha = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_shopfloor_feedback", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_structural_production_lot",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionLotNumber = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ActualGradingMethod = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    WorkCenterCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProductionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ShiftCode = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReleasedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReleasedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CancelledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CancelledBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_structural_production_lot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_tooling",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_tooling", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_wip",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_wip", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_workcenter",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CapacityPerHour = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_workcenter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_workorder",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_workorder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_purchasing_purchasegoodsreceipt",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PurchaseOrderNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_purchasing_purchasegoodsreceipt", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_purchasing_purchaseorder",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SupplierCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_purchasing_purchaseorder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_purchasing_purchaserequest",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Requester = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NeededDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_purchasing_purchaserequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_purchasing_purchasereturn",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SupplierCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PurchaseOrderNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_purchasing_purchasereturn", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_purchasing_purchasingreport",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_purchasing_purchasingreport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_purchasing_rfq",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_purchasing_rfq", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_purchasing_supplier",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TaxNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_purchasing_supplier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_purchasing_supplierinvoice",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SupplierCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    InvoiceDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_purchasing_supplierinvoice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_purchasing_supplierquotation",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SupplierCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RfqNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_purchasing_supplierquotation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_sales_customer",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TaxNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_sales_customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_sales_customerinvoice",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CustomerCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    InvoiceDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_sales_customerinvoice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_sales_delivery",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ShipmentNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CustomerCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_sales_delivery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_sales_lead",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Source = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_sales_lead", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_sales_opportunity",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CustomerCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Stage = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_sales_opportunity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_sales_salesorder",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CustomerCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_sales_salesorder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_sales_salesquotation",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CustomerCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ValidUntil = table.Column<DateOnly>(type: "date", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_sales_salesquotation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_sales_salesreport",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_sales_salesreport", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_sales_shipment",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SalesOrderNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlantId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_sales_shipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "business_production_structural_production_lot_input",
                schema: "business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionLotId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceBatchId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourceInventoryBalanceId = table.Column<Guid>(type: "uuid", nullable: true),
                    SourcePackageId = table.Column<Guid>(type: "uuid", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_production_structural_production_lot_input", x => x.Id);
                    table.ForeignKey(
                        name: "FK_business_production_structural_production_lot_input_busines~",
                        column: x => x.ProductionLotId,
                        principalSchema: "business",
                        principalTable: "business_production_structural_production_lot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_inventorycountline_CountId",
                schema: "business",
                table: "business_inventory_inventorycountline",
                column: "CountId");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_location_PlantId_WarehouseCode_Code",
                schema: "business",
                table: "business_inventory_location",
                columns: new[] { "PlantId", "WarehouseCode", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_material_Code",
                schema: "business",
                table: "business_inventory_material",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_materialidentity_IdentityNumber",
                schema: "business",
                table: "business_inventory_materialidentity",
                column: "IdentityNumber");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_movement_DocumentNumber",
                schema: "business",
                table: "business_inventory_movement",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_movement_MovementNumber",
                schema: "business",
                table: "business_inventory_movement",
                column: "MovementNumber");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_movement_PackageId",
                schema: "business",
                table: "business_inventory_movement",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_package_Barcode",
                schema: "business",
                table: "business_inventory_package",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_package_BatchId",
                schema: "business",
                table: "business_inventory_package",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_package_MaterialId",
                schema: "business",
                table: "business_inventory_package",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_package_PackageNumber",
                schema: "business",
                table: "business_inventory_package",
                column: "PackageNumber");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_package_PublicId",
                schema: "business",
                table: "business_inventory_package",
                column: "PublicId");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_packagecontent_PackageId",
                schema: "business",
                table: "business_inventory_packagecontent",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_packageoperation_PlantId_Number",
                schema: "business",
                table: "business_inventory_packageoperation",
                columns: new[] { "PlantId", "Number" });

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_packagerelation_OperationId",
                schema: "business",
                table: "business_inventory_packagerelation",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_packagerelation_OperationId_SourcePackag~",
                schema: "business",
                table: "business_inventory_packagerelation",
                columns: new[] { "OperationId", "SourcePackageId", "TargetPackageId", "RelationType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_packagerelation_SourcePackageId",
                schema: "business",
                table: "business_inventory_packagerelation",
                column: "SourcePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_packagerelation_TargetPackageId",
                schema: "business",
                table: "business_inventory_packagerelation",
                column: "TargetPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_business_inventory_warehouse_PlantId_Code",
                schema: "business",
                table: "business_inventory_warehouse",
                columns: new[] { "PlantId", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_business_production_execution_consumption_ExecutionId",
                schema: "business",
                table: "business_production_execution_consumption",
                column: "ExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_execution_consumption_SourcePackageId",
                schema: "business",
                table: "business_production_execution_consumption",
                column: "SourcePackageId");

            migrationBuilder.CreateIndex(
                name: "UX_prd_exec_consume_idem",
                schema: "business",
                table: "business_production_execution_consumption",
                columns: new[] { "ExecutionId", "IdempotencyKey" },
                unique: true,
                filter: "\"IsDeleted\" = false AND \"IdempotencyKey\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_execution_event_ExecutionId",
                schema: "business",
                table: "business_production_execution_event",
                column: "ExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_execution_scrap_ExecutionId",
                schema: "business",
                table: "business_production_execution_scrap",
                column: "ExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_lot_source_ProductionLotId",
                schema: "business",
                table: "business_production_lot_source",
                column: "ProductionLotId");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_lot_source_ProductionOperationExecution~",
                schema: "business",
                table: "business_production_lot_source",
                column: "ProductionOperationExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_lot_source_ProductionOutputId",
                schema: "business",
                table: "business_production_lot_source",
                column: "ProductionOutputId");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_lot_source_SourceLotId",
                schema: "business",
                table: "business_production_lot_source",
                column: "SourceLotId");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_operation_execution_PlantId_Number",
                schema: "business",
                table: "business_production_operation_execution",
                columns: new[] { "PlantId", "Number" });

            migrationBuilder.CreateIndex(
                name: "IX_business_production_operation_execution_WorkCenterId",
                schema: "business",
                table: "business_production_operation_execution",
                column: "WorkCenterId");

            migrationBuilder.CreateIndex(
                name: "UX_prd_exec_open_operation",
                schema: "business",
                table: "business_production_operation_execution",
                column: "ProductionOperationId",
                unique: true,
                filter: "\"IsDeleted\" = false AND \"Status\" IN ('RUNNING','PAUSED','NOT_STARTED')");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_order_operation_ProductionOrderId",
                schema: "business",
                table: "business_production_order_operation",
                column: "ProductionOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_order_operation_WorkCenterId",
                schema: "business",
                table: "business_production_order_operation",
                column: "WorkCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_output_Number",
                schema: "business",
                table: "business_production_output",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_output_OutputBatchId",
                schema: "business",
                table: "business_production_output",
                column: "OutputBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_output_ProductionOrderId",
                schema: "business",
                table: "business_production_output",
                column: "ProductionOrderId");

            migrationBuilder.CreateIndex(
                name: "UX_prd_out_execution",
                schema: "business",
                table: "business_production_output",
                column: "ProductionOperationExecutionId",
                unique: true,
                filter: "\"IsDeleted\" = false AND \"ProductionOperationExecutionId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_shopfloor_feedback_CreatedAt",
                schema: "business",
                table: "business_production_shopfloor_feedback",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_shopfloor_feedback_WorkCenterId",
                schema: "business",
                table: "business_production_shopfloor_feedback",
                column: "WorkCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_structural_production_lot_PlantId",
                schema: "business",
                table: "business_production_structural_production_lot",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_business_production_structural_production_lot_ProductionLot~",
                schema: "business",
                table: "business_production_structural_production_lot",
                column: "ProductionLotNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_business_production_structural_production_lot_input_Product~",
                schema: "business",
                table: "business_production_structural_production_lot_input",
                column: "ProductionLotId");

            // Indexes the pre-migration startup SQL created after the model tables, kept last and in that order.
            // PostgreSQL checks unique indexes in creation order, and the production handlers recover from
            // specific constraint names (UX_prd_exec_open_operation must fire before UX_prd_exec_number).

            migrationBuilder.CreateIndex(
                name: "IX_movement_packageid",
                schema: "business",
                table: "business_inventory_movement",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "UX_packageoperation_number_alive",
                schema: "business",
                table: "business_inventory_packageoperation",
                columns: new[] { "PlantId", "Number" },
                unique: true,
                filter: "\"IsDeleted\" = false AND \"Number\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_packagerelation_source",
                schema: "business",
                table: "business_inventory_packagerelation",
                column: "SourcePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_packagerelation_target",
                schema: "business",
                table: "business_inventory_packagerelation",
                column: "TargetPackageId");

            migrationBuilder.CreateIndex(
                name: "UX_packagerelation_edge",
                schema: "business",
                table: "business_inventory_packagerelation",
                columns: new[] { "OperationId", "SourcePackageId", "TargetPackageId", "RelationType" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_prd_op_order",
                schema: "business",
                table: "business_production_order_operation",
                column: "ProductionOrderId");

            migrationBuilder.CreateIndex(
                name: "UX_prd_exec_number",
                schema: "business",
                table: "business_production_operation_execution",
                columns: new[] { "PlantId", "Number" },
                unique: true,
                filter: "\"IsDeleted\" = false AND \"Number\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_prd_exec_event",
                schema: "business",
                table: "business_production_execution_event",
                column: "ExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_prd_exec_cons",
                schema: "business",
                table: "business_production_execution_consumption",
                column: "ExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_prd_exec_scrap",
                schema: "business",
                table: "business_production_execution_scrap",
                column: "ExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_prd_sf_feedback_created",
                schema: "business",
                table: "business_production_shopfloor_feedback",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "UX_package_barcode_alive",
                schema: "business",
                table: "business_inventory_package",
                column: "Barcode",
                unique: true,
                filter: "\"IsDeleted\" = false AND \"Barcode\" <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_package_number_alive",
                schema: "business",
                table: "business_inventory_package",
                column: "PackageNumber",
                unique: true,
                filter: "\"IsDeleted\" = false AND \"PackageNumber\" <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_package_publicid_alive",
                schema: "business",
                table: "business_inventory_package",
                column: "PublicId",
                unique: true,
                filter: "\"IsDeleted\" = false AND \"PublicId\" <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_batch_production_lot_alive",
                schema: "business",
                table: "business_inventory_batch",
                column: "BatchNumber",
                unique: true,
                filter: "\"IsDeleted\" = false AND \"SourceType\" = 'PRODUCTION' AND \"BatchNumber\" <> ''");

            migrationBuilder.CreateIndex(
                name: "UX_production_output_number_alive",
                schema: "business",
                table: "business_production_output",
                column: "Number",
                unique: true,
                filter: "\"IsDeleted\" = false AND \"Number\" <> ''");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "business_inventory_batch",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_goodsissue",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_goodsreceipt",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_inventoryadjustment",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_inventorybalance",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_inventorycount",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_inventorycountline",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_location",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_material",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_materialidentity",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_movement",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_package",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_packagecontent",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_packageoperation",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_packagerelation",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_stocktransfer",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_inventory_warehouse",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_bom",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_calendar",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_execution_consumption",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_execution_event",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_execution_scrap",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_finishedgood",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_lot_source",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_machine",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_materialconsumption",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_operation",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_operation_execution",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_order_operation",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_output",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_packaging",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_productionconfirmation",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_productionline",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_productionorder",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_productionparameter",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_rework",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_routing",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_scrap",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_shift",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_shopfloor_feedback",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_structural_production_lot_input",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_tooling",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_wip",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_workcenter",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_workorder",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_purchasing_purchasegoodsreceipt",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_purchasing_purchaseorder",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_purchasing_purchaserequest",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_purchasing_purchasereturn",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_purchasing_purchasingreport",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_purchasing_rfq",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_purchasing_supplier",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_purchasing_supplierinvoice",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_purchasing_supplierquotation",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_sales_customer",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_sales_customerinvoice",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_sales_delivery",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_sales_lead",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_sales_opportunity",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_sales_salesorder",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_sales_salesquotation",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_sales_salesreport",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_sales_shipment",
                schema: "business");

            migrationBuilder.DropTable(
                name: "business_production_structural_production_lot",
                schema: "business");
        }
    }
}
