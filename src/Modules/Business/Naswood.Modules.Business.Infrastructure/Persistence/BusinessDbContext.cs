using Microsoft.EntityFrameworkCore;

namespace Naswood.Modules.Business.Infrastructure.Persistence;

public sealed class BusinessDbContext : DbContext
{
    public BusinessDbContext(DbContextOptions<BusinessDbContext> options) : base(options) { }

    public DbSet<Naswood.Modules.Business.Domain.Inventory.Material> Materials => Set<Naswood.Modules.Business.Domain.Inventory.Material>();
    public DbSet<Naswood.Modules.Business.Domain.Inventory.Warehouse> Warehouses => Set<Naswood.Modules.Business.Domain.Inventory.Warehouse>();
    public DbSet<Naswood.Modules.Business.Domain.Inventory.Location> Locations => Set<Naswood.Modules.Business.Domain.Inventory.Location>();
    public DbSet<Naswood.Modules.Business.Domain.Inventory.InventoryBalance> InventoryBalances => Set<Naswood.Modules.Business.Domain.Inventory.InventoryBalance>();
    public DbSet<Naswood.Modules.Business.Domain.Inventory.Batch> Batchs => Set<Naswood.Modules.Business.Domain.Inventory.Batch>();
    public DbSet<Naswood.Modules.Business.Domain.Inventory.GoodsReceipt> GoodsReceipts => Set<Naswood.Modules.Business.Domain.Inventory.GoodsReceipt>();
    public DbSet<Naswood.Modules.Business.Domain.Inventory.GoodsIssue> GoodsIssues => Set<Naswood.Modules.Business.Domain.Inventory.GoodsIssue>();
    public DbSet<Naswood.Modules.Business.Domain.Inventory.StockTransfer> StockTransfers => Set<Naswood.Modules.Business.Domain.Inventory.StockTransfer>();
    public DbSet<Naswood.Modules.Business.Domain.Inventory.InventoryCount> InventoryCounts => Set<Naswood.Modules.Business.Domain.Inventory.InventoryCount>();
    public DbSet<Naswood.Modules.Business.Domain.Inventory.InventoryAdjustment> InventoryAdjustments => Set<Naswood.Modules.Business.Domain.Inventory.InventoryAdjustment>();
    public DbSet<Naswood.Modules.Business.Domain.Purchasing.Supplier> Suppliers => Set<Naswood.Modules.Business.Domain.Purchasing.Supplier>();
    public DbSet<Naswood.Modules.Business.Domain.Purchasing.PurchaseRequest> PurchaseRequests => Set<Naswood.Modules.Business.Domain.Purchasing.PurchaseRequest>();
    public DbSet<Naswood.Modules.Business.Domain.Purchasing.Rfq> Rfqs => Set<Naswood.Modules.Business.Domain.Purchasing.Rfq>();
    public DbSet<Naswood.Modules.Business.Domain.Purchasing.SupplierQuotation> SupplierQuotations => Set<Naswood.Modules.Business.Domain.Purchasing.SupplierQuotation>();
    public DbSet<Naswood.Modules.Business.Domain.Purchasing.PurchaseOrder> PurchaseOrders => Set<Naswood.Modules.Business.Domain.Purchasing.PurchaseOrder>();
    public DbSet<Naswood.Modules.Business.Domain.Purchasing.PurchaseGoodsReceipt> PurchaseGoodsReceipts => Set<Naswood.Modules.Business.Domain.Purchasing.PurchaseGoodsReceipt>();
    public DbSet<Naswood.Modules.Business.Domain.Purchasing.PurchaseReturn> PurchaseReturns => Set<Naswood.Modules.Business.Domain.Purchasing.PurchaseReturn>();
    public DbSet<Naswood.Modules.Business.Domain.Purchasing.SupplierInvoice> SupplierInvoices => Set<Naswood.Modules.Business.Domain.Purchasing.SupplierInvoice>();
    public DbSet<Naswood.Modules.Business.Domain.Purchasing.PurchasingReportDefinition> PurchasingReports => Set<Naswood.Modules.Business.Domain.Purchasing.PurchasingReportDefinition>();
    public DbSet<Naswood.Modules.Business.Domain.Sales.Customer> Customers => Set<Naswood.Modules.Business.Domain.Sales.Customer>();
    public DbSet<Naswood.Modules.Business.Domain.Sales.Lead> Leads => Set<Naswood.Modules.Business.Domain.Sales.Lead>();
    public DbSet<Naswood.Modules.Business.Domain.Sales.Opportunity> Opportunitys => Set<Naswood.Modules.Business.Domain.Sales.Opportunity>();
    public DbSet<Naswood.Modules.Business.Domain.Sales.SalesQuotation> SalesQuotations => Set<Naswood.Modules.Business.Domain.Sales.SalesQuotation>();
    public DbSet<Naswood.Modules.Business.Domain.Sales.SalesOrder> SalesOrders => Set<Naswood.Modules.Business.Domain.Sales.SalesOrder>();
    public DbSet<Naswood.Modules.Business.Domain.Sales.Shipment> Shipments => Set<Naswood.Modules.Business.Domain.Sales.Shipment>();
    public DbSet<Naswood.Modules.Business.Domain.Sales.Delivery> Deliverys => Set<Naswood.Modules.Business.Domain.Sales.Delivery>();
    public DbSet<Naswood.Modules.Business.Domain.Sales.CustomerInvoice> CustomerInvoices => Set<Naswood.Modules.Business.Domain.Sales.CustomerInvoice>();
    public DbSet<Naswood.Modules.Business.Domain.Sales.SalesReportDefinition> SalesReports => Set<Naswood.Modules.Business.Domain.Sales.SalesReportDefinition>();
    public DbSet<Naswood.Modules.Business.Domain.Production.Bom> Boms => Set<Naswood.Modules.Business.Domain.Production.Bom>();
    public DbSet<Naswood.Modules.Business.Domain.Production.Routing> Routings => Set<Naswood.Modules.Business.Domain.Production.Routing>();
    public DbSet<Naswood.Modules.Business.Domain.Production.Machine> Machines => Set<Naswood.Modules.Business.Domain.Production.Machine>();
    public DbSet<Naswood.Modules.Business.Domain.Production.WorkCenter> WorkCenters => Set<Naswood.Modules.Business.Domain.Production.WorkCenter>();
    public DbSet<Naswood.Modules.Business.Domain.Production.ProductionLine> ProductionLines => Set<Naswood.Modules.Business.Domain.Production.ProductionLine>();
    public DbSet<Naswood.Modules.Business.Domain.Production.Shift> Shifts => Set<Naswood.Modules.Business.Domain.Production.Shift>();
    public DbSet<Naswood.Modules.Business.Domain.Production.Calendar> Calendars => Set<Naswood.Modules.Business.Domain.Production.Calendar>();
    public DbSet<Naswood.Modules.Business.Domain.Production.Tooling> Toolings => Set<Naswood.Modules.Business.Domain.Production.Tooling>();
    public DbSet<Naswood.Modules.Business.Domain.Production.Operation> Operations => Set<Naswood.Modules.Business.Domain.Production.Operation>();
    public DbSet<Naswood.Modules.Business.Domain.Production.ProductionParameter> ProductionParameters => Set<Naswood.Modules.Business.Domain.Production.ProductionParameter>();
    public DbSet<Naswood.Modules.Business.Domain.Production.ProductionOrder> ProductionOrders => Set<Naswood.Modules.Business.Domain.Production.ProductionOrder>();
    public DbSet<Naswood.Modules.Business.Domain.Production.WorkOrder> WorkOrders => Set<Naswood.Modules.Business.Domain.Production.WorkOrder>();
    public DbSet<Naswood.Modules.Business.Domain.Production.MaterialConsumption> MaterialConsumptions => Set<Naswood.Modules.Business.Domain.Production.MaterialConsumption>();
    public DbSet<Naswood.Modules.Business.Domain.Production.ProductionConfirmation> ProductionConfirmations => Set<Naswood.Modules.Business.Domain.Production.ProductionConfirmation>();
    public DbSet<Naswood.Modules.Business.Domain.Production.Wip> Wips => Set<Naswood.Modules.Business.Domain.Production.Wip>();
    public DbSet<Naswood.Modules.Business.Domain.Production.Packaging> Packagings => Set<Naswood.Modules.Business.Domain.Production.Packaging>();
    public DbSet<Naswood.Modules.Business.Domain.Production.FinishedGood> FinishedGoods => Set<Naswood.Modules.Business.Domain.Production.FinishedGood>();
    public DbSet<Naswood.Modules.Business.Domain.Production.Scrap> Scraps => Set<Naswood.Modules.Business.Domain.Production.Scrap>();
    public DbSet<Naswood.Modules.Business.Domain.Production.Rework> Reworks => Set<Naswood.Modules.Business.Domain.Production.Rework>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("business");

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Inventory.Material>(entity =>
        {
            entity.ToTable("business_inventory_material");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(200);
            entity.Property(x => x.Category).HasMaxLength(200);
            entity.Property(x => x.UnitOfMeasure).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Inventory.Warehouse>(entity =>
        {
            entity.ToTable("business_inventory_warehouse");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.WarehouseType).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Inventory.Location>(entity =>
        {
            entity.ToTable("business_inventory_location");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.WarehouseCode).HasMaxLength(200);
            entity.Property(x => x.LocationType).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Inventory.InventoryBalance>(entity =>
        {
            entity.ToTable("business_inventory_inventorybalance");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.MaterialCode).HasMaxLength(200);
            entity.Property(x => x.WarehouseCode).HasMaxLength(200);
            entity.Property(x => x.LocationCode).HasMaxLength(200);
            entity.Property(x => x.BatchNumber).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Inventory.Batch>(entity =>
        {
            entity.ToTable("business_inventory_batch");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.BatchNumber).HasMaxLength(200);
            entity.Property(x => x.MaterialCode).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Inventory.GoodsReceipt>(entity =>
        {
            entity.ToTable("business_inventory_goodsreceipt");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.WarehouseCode).HasMaxLength(200);
            entity.Property(x => x.Reference).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Inventory.GoodsIssue>(entity =>
        {
            entity.ToTable("business_inventory_goodsissue");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.WarehouseCode).HasMaxLength(200);
            entity.Property(x => x.Reference).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Inventory.StockTransfer>(entity =>
        {
            entity.ToTable("business_inventory_stocktransfer");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.FromWarehouseCode).HasMaxLength(200);
            entity.Property(x => x.ToWarehouseCode).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Inventory.InventoryCount>(entity =>
        {
            entity.ToTable("business_inventory_inventorycount");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.WarehouseCode).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Inventory.InventoryAdjustment>(entity =>
        {
            entity.ToTable("business_inventory_inventoryadjustment");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.WarehouseCode).HasMaxLength(200);
            entity.Property(x => x.Reason).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Purchasing.Supplier>(entity =>
        {
            entity.ToTable("business_purchasing_supplier");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.TaxNumber).HasMaxLength(200);
            entity.Property(x => x.Email).HasMaxLength(200);
            entity.Property(x => x.Phone).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Purchasing.PurchaseRequest>(entity =>
        {
            entity.ToTable("business_purchasing_purchaserequest");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.Requester).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Purchasing.Rfq>(entity =>
        {
            entity.ToTable("business_purchasing_rfq");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.Title).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Purchasing.SupplierQuotation>(entity =>
        {
            entity.ToTable("business_purchasing_supplierquotation");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.SupplierCode).HasMaxLength(200);
            entity.Property(x => x.RfqNumber).HasMaxLength(200);
            entity.Property(x => x.Currency).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Purchasing.PurchaseOrder>(entity =>
        {
            entity.ToTable("business_purchasing_purchaseorder");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.SupplierCode).HasMaxLength(200);
            entity.Property(x => x.Currency).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Purchasing.PurchaseGoodsReceipt>(entity =>
        {
            entity.ToTable("business_purchasing_purchasegoodsreceipt");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.PurchaseOrderNumber).HasMaxLength(200);
            entity.Property(x => x.WarehouseCode).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Purchasing.PurchaseReturn>(entity =>
        {
            entity.ToTable("business_purchasing_purchasereturn");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.SupplierCode).HasMaxLength(200);
            entity.Property(x => x.PurchaseOrderNumber).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Purchasing.SupplierInvoice>(entity =>
        {
            entity.ToTable("business_purchasing_supplierinvoice");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.SupplierCode).HasMaxLength(200);
            entity.Property(x => x.Currency).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Purchasing.PurchasingReportDefinition>(entity =>
        {
            entity.ToTable("business_purchasing_purchasingreport");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.ReportCode).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.Category).HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Sales.Customer>(entity =>
        {
            entity.ToTable("business_sales_customer");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.TaxNumber).HasMaxLength(200);
            entity.Property(x => x.Email).HasMaxLength(200);
            entity.Property(x => x.Phone).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Sales.Lead>(entity =>
        {
            entity.ToTable("business_sales_lead");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.CompanyName).HasMaxLength(200);
            entity.Property(x => x.Email).HasMaxLength(200);
            entity.Property(x => x.Source).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Sales.Opportunity>(entity =>
        {
            entity.ToTable("business_sales_opportunity");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.CustomerCode).HasMaxLength(200);
            entity.Property(x => x.Title).HasMaxLength(200);
            entity.Property(x => x.Stage).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Sales.SalesQuotation>(entity =>
        {
            entity.ToTable("business_sales_salesquotation");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.CustomerCode).HasMaxLength(200);
            entity.Property(x => x.Currency).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Sales.SalesOrder>(entity =>
        {
            entity.ToTable("business_sales_salesorder");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.CustomerCode).HasMaxLength(200);
            entity.Property(x => x.Currency).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Sales.Shipment>(entity =>
        {
            entity.ToTable("business_sales_shipment");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.SalesOrderNumber).HasMaxLength(200);
            entity.Property(x => x.WarehouseCode).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Sales.Delivery>(entity =>
        {
            entity.ToTable("business_sales_delivery");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.ShipmentNumber).HasMaxLength(200);
            entity.Property(x => x.CustomerCode).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Sales.CustomerInvoice>(entity =>
        {
            entity.ToTable("business_sales_customerinvoice");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.CustomerCode).HasMaxLength(200);
            entity.Property(x => x.Currency).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Sales.SalesReportDefinition>(entity =>
        {
            entity.ToTable("business_sales_salesreport");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.ReportCode).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.Category).HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.Bom>(entity =>
        {
            entity.ToTable("business_production_bom");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.MaterialCode).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.Routing>(entity =>
        {
            entity.ToTable("business_production_routing");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Number).HasMaxLength(200);
            entity.Property(x => x.MaterialCode).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.Machine>(entity =>
        {
            entity.ToTable("business_production_machine");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.WorkCenterCode).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.WorkCenter>(entity =>
        {
            entity.ToTable("business_production_workcenter");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.ProductionLine>(entity =>
        {
            entity.ToTable("business_production_productionline");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.Shift>(entity =>
        {
            entity.ToTable("business_production_shift");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.Calendar>(entity =>
        {
            entity.ToTable("business_production_calendar");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.Tooling>(entity =>
        {
            entity.ToTable("business_production_tooling");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.Operation>(entity =>
        {
            entity.ToTable("business_production_operation");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.ProductionParameter>(entity =>
        {
            entity.ToTable("business_production_productionparameter");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.ProductionOrder>(entity =>
        {
            entity.ToTable("business_production_productionorder");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.WorkOrder>(entity =>
        {
            entity.ToTable("business_production_workorder");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.MaterialConsumption>(entity =>
        {
            entity.ToTable("business_production_materialconsumption");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.ProductionConfirmation>(entity =>
        {
            entity.ToTable("business_production_productionconfirmation");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.Wip>(entity =>
        {
            entity.ToTable("business_production_wip");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.Packaging>(entity =>
        {
            entity.ToTable("business_production_packaging");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.FinishedGood>(entity =>
        {
            entity.ToTable("business_production_finishedgood");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.Scrap>(entity =>
        {
            entity.ToTable("business_production_scrap");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });

        modelBuilder.Entity<Naswood.Modules.Business.Domain.Production.Rework>(entity =>
        {
            entity.ToTable("business_production_rework");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyId).HasMaxLength(20).IsRequired();
            entity.Property(x => x.PlantId).HasMaxLength(20);
            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Code).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(200);
            entity.Property(x => x.PlantId).HasMaxLength(200);
            entity.Property(x => x.Status).HasMaxLength(200);
            entity.Property(x => x.Notes).HasMaxLength(200);
        });
    }
}
