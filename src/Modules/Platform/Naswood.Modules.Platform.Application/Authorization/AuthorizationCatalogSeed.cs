using Naswood.Modules.Platform.Domain.Authorization;

namespace Naswood.Modules.Platform.Application.Authorization;

/// <summary>
/// Seeds the permission catalog and Administrator role from approved design samples.
/// Mutation APIs remain TASK-004 / TASK-005.
/// </summary>
public static class AuthorizationCatalogSeed
{
    public static IReadOnlyList<PermissionDefinition> CreatePermissions()
    {
        var specs = new (string Code, string Module, string? Entity, string Action, string? Field, string Name)[]
        {
            ("Platform.Dashboard.View", "Platform", "Dashboard", "View", null, "View Dashboard"),
            ("Authorization.View", "Authorization", null, "View", null, "View Authorization"),
            ("Authorization.Configure", "Authorization", null, "Configure", null, "Configure Authorization"),
            ("Authorization.Assign", "Authorization", null, "Assign", null, "Assign Authorization"),
            ("Authorization.Export", "Authorization", null, "Export", null, "Export Authorization"),
            ("Authorization.Audit", "Authorization", null, "Audit", null, "Audit Authorization"),
            ("Administration.Manage", "Administration", null, "Manage", null, "Manage Administration"),
            ("Administration.User.Delete", "Administration", "User", "Delete", null, "Delete User"),
            ("Inventory.View", "Inventory", null, "View", null, "View Inventory"),
            ("Inventory.Create", "Inventory", null, "Create", null, "Create Inventory"),
            ("Inventory.Update", "Inventory", null, "Update", null, "Update Inventory"),
            ("Inventory.Delete", "Inventory", null, "Delete", null, "Delete Inventory"),
            ("Inventory.Export", "Inventory", null, "Export", null, "Export Inventory"),
            ("Inventory.Import", "Inventory", null, "Import", null, "Import Inventory"),
            ("Warehouse.View", "Inventory", "Warehouse", "View", null, "View Warehouse"),
            ("Warehouse.Create", "Inventory", "Warehouse", "Create", null, "Create Warehouse"),
            ("Warehouse.Update", "Inventory", "Warehouse", "Update", null, "Update Warehouse"),
            ("Warehouse.Delete", "Inventory", "Warehouse", "Delete", null, "Delete Warehouse"),
            ("GoodsReceipt.Execute", "Inventory", "GoodsReceipt", "Execute", null, "Execute Goods Receipt"),
            ("GoodsIssue.Execute", "Inventory", "GoodsIssue", "Execute", null, "Execute Goods Issue"),
            ("Purchasing.View", "Purchasing", null, "View", null, "View Purchasing"),
            ("PurchaseOrder.Approve", "Purchasing", "PurchaseOrder", "Approve", null, "Approve Purchase Order"),
            ("PurchaseOrder.Create", "Purchasing", "PurchaseOrder", "Create", null, "Create Purchase Order"),
            ("PurchaseOrder.Price.View", "Purchasing", "PurchaseOrder", "View", "Price", "View Purchase Order Price"),
            ("PurchaseOrder.Own", "Purchasing", "PurchaseOrder", "Own", null, "Own Purchase Order Access"),
            ("Sales.View", "Sales", null, "View", null, "View Sales"),
            ("SalesOrder.Approve", "Sales", "SalesOrder", "Approve", null, "Approve Sales Order"),
            ("Production.View", "Production", null, "View", null, "View Production"),
            ("ProductionOrder.Approve", "Production", "ProductionOrder", "Approve", null, "Approve Production Order"),
            ("Quality.View", "Quality", null, "View", null, "View Quality"),
            ("QualityInspection.Execute", "Quality", "QualityInspection", "Execute", null, "Execute Quality Inspection"),
            ("Maintenance.View", "Maintenance", null, "View", null, "View Maintenance"),
            ("MaintenanceOrder.Execute", "Maintenance", "MaintenanceOrder", "Execute", null, "Execute Maintenance Order"),
            ("Finance.View", "Finance", null, "View", null, "View Finance"),
            ("Finance.Report.View", "Finance", "Report", "View", null, "View Finance Report"),
            ("AI.Chat", "AI", null, "Chat", null, "Use AI Chat")
        };

        return specs
            .Select(s => PermissionDefinition.Create(s.Code, s.Module, s.Action, s.Name, s.Entity, s.Field))
            .ToArray();
    }

    public static RoleDefinition CreateAdministratorRole(IEnumerable<PermissionDefinition> permissions) =>
        RoleDefinition.Create(
            "Administrator",
            "Administrator",
            permissions.Select(p => p.Code));

    public static RoleDefinition CreateReadOnlyRole() =>
        RoleDefinition.Create(
            "ReadOnly",
            "Read Only",
            [
                "Platform.Dashboard.View",
                "Authorization.View",
                "Inventory.View",
                "Warehouse.View",
                "Purchasing.View",
                "Sales.View",
                "Production.View",
                "Quality.View",
                "Maintenance.View",
                "Finance.View",
                "Finance.Report.View"
            ]);
}
