using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Naswood.Modules.Business.Infrastructure.Persistence;

namespace Naswood.Modules.Business.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("Platform")
            ?? throw new InvalidOperationException("Connection string 'Platform' is required.");
        services.AddDbContext<BusinessDbContext>(o => o.UseNpgsql(cs));
        services.AddScoped<Naswood.Modules.Business.Application.Common.IBusinessUnitOfWork, Persistence.BusinessUnitOfWork>();
        services.AddScoped<Naswood.Modules.Business.Application.Inventory.IMaterialRepository, Naswood.Modules.Business.Infrastructure.Inventory.MaterialRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Inventory.IWarehouseRepository, Naswood.Modules.Business.Infrastructure.Inventory.WarehouseRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Inventory.ILocationRepository, Naswood.Modules.Business.Infrastructure.Inventory.LocationRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Inventory.IInventoryBalanceRepository, Naswood.Modules.Business.Infrastructure.Inventory.InventoryBalanceRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Inventory.IBatchRepository, Naswood.Modules.Business.Infrastructure.Inventory.BatchRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Inventory.IGoodsReceiptRepository, Naswood.Modules.Business.Infrastructure.Inventory.GoodsReceiptRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Inventory.IGoodsIssueRepository, Naswood.Modules.Business.Infrastructure.Inventory.GoodsIssueRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Inventory.IStockTransferRepository, Naswood.Modules.Business.Infrastructure.Inventory.StockTransferRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Inventory.IInventoryCountRepository, Naswood.Modules.Business.Infrastructure.Inventory.InventoryCountRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Inventory.IInventoryAdjustmentRepository, Naswood.Modules.Business.Infrastructure.Inventory.InventoryAdjustmentRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Purchasing.ISupplierRepository, Naswood.Modules.Business.Infrastructure.Purchasing.SupplierRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Purchasing.IPurchaseRequestRepository, Naswood.Modules.Business.Infrastructure.Purchasing.PurchaseRequestRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Purchasing.IRfqRepository, Naswood.Modules.Business.Infrastructure.Purchasing.RfqRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Purchasing.ISupplierQuotationRepository, Naswood.Modules.Business.Infrastructure.Purchasing.SupplierQuotationRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Purchasing.IPurchaseOrderRepository, Naswood.Modules.Business.Infrastructure.Purchasing.PurchaseOrderRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Purchasing.IPurchaseGoodsReceiptRepository, Naswood.Modules.Business.Infrastructure.Purchasing.PurchaseGoodsReceiptRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Purchasing.IPurchaseReturnRepository, Naswood.Modules.Business.Infrastructure.Purchasing.PurchaseReturnRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Purchasing.ISupplierInvoiceRepository, Naswood.Modules.Business.Infrastructure.Purchasing.SupplierInvoiceRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Purchasing.IPurchasingReportRepository, Naswood.Modules.Business.Infrastructure.Purchasing.PurchasingReportRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Sales.ICustomerRepository, Naswood.Modules.Business.Infrastructure.Sales.CustomerRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Sales.ILeadRepository, Naswood.Modules.Business.Infrastructure.Sales.LeadRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Sales.IOpportunityRepository, Naswood.Modules.Business.Infrastructure.Sales.OpportunityRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Sales.ISalesQuotationRepository, Naswood.Modules.Business.Infrastructure.Sales.SalesQuotationRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Sales.ISalesOrderRepository, Naswood.Modules.Business.Infrastructure.Sales.SalesOrderRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Sales.IShipmentRepository, Naswood.Modules.Business.Infrastructure.Sales.ShipmentRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Sales.IDeliveryRepository, Naswood.Modules.Business.Infrastructure.Sales.DeliveryRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Sales.ICustomerInvoiceRepository, Naswood.Modules.Business.Infrastructure.Sales.CustomerInvoiceRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Sales.ISalesReportRepository, Naswood.Modules.Business.Infrastructure.Sales.SalesReportRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Production.IBomRepository, Naswood.Modules.Business.Infrastructure.Production.BomRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Production.IRoutingRepository, Naswood.Modules.Business.Infrastructure.Production.RoutingRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Production.IMachineRepository, Naswood.Modules.Business.Infrastructure.Production.MachineRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Production.IWorkCenterRepository, Naswood.Modules.Business.Infrastructure.Production.WorkCenterRepository>();
        services.AddScoped<Naswood.Modules.Business.Application.Production.IProductionLineRepository, Naswood.Modules.Business.Infrastructure.Production.ProductionLineRepository>();
        return services;
    }
}
