import {
  Outlet,
  createRootRouteWithContext,
  createRoute,
  createRouter,
  redirect,
} from '@tanstack/react-router';
import type { QueryClient } from '@tanstack/react-query';
import { AuthenticatedLayout } from './layouts/AuthenticatedLayout';
import { DashboardPage } from './pages/DashboardPage';
import { LoginPage } from './pages/LoginPage';
import { ModulePlaceholderPage } from './pages/ModulePlaceholderPage';
import { isAuthenticated } from './auth/session';
import { collectNavPaths } from './navigation/nav-config';
import { FilesPage } from './pages/FilesPage';
import { InventoryWorkspaceLayout } from './modules/inventory/InventoryWorkspaceLayout';
import { InventoryDashboardPage } from './modules/inventory/overview/InventoryDashboardPage';
import {
  AdjustmentListPage,
  CycleCountListPage,
  GoodsIssueListPage,
  GoodsReceiptDetailPage,
  GoodsReceiptListPage,
  InventoryReportsPage,
  LocationListPage,
  LotListPage,
  MaterialDetailPage,
  MaterialListPage,
  StockBalancePage,
  TransferListPage,
  WarehouseDetailPage,
  WarehouseListPage,
} from './modules/inventory/screens';
import { SupplierPage } from './pages/business/SupplierPage';
import { PurchaseRequestPage } from './pages/business/PurchaseRequestPage';
import { RfqPage } from './pages/business/RfqPage';
import { SupplierQuotationPage } from './pages/business/SupplierQuotationPage';
import { PurchaseOrderPage } from './pages/business/PurchaseOrderPage';
import { PurchaseGoodsReceiptPage } from './pages/business/PurchaseGoodsReceiptPage';
import { PurchaseReturnPage } from './pages/business/PurchaseReturnPage';
import { SupplierInvoicePage } from './pages/business/SupplierInvoicePage';
import { PurchasingDashboardPage } from './pages/business/PurchasingDashboardPage';
import { PurchasingReportPage } from './pages/business/PurchasingReportPage';
import { CustomerPage } from './pages/business/CustomerPage';
import { LeadPage } from './pages/business/LeadPage';
import { OpportunityPage } from './pages/business/OpportunityPage';
import { SalesQuotationPage } from './pages/business/SalesQuotationPage';
import { SalesOrderPage } from './pages/business/SalesOrderPage';
import { ShipmentPage } from './pages/business/ShipmentPage';
import { DeliveryPage } from './pages/business/DeliveryPage';
import { CustomerInvoicePage } from './pages/business/CustomerInvoicePage';
import { SalesDashboardPage } from './pages/business/SalesDashboardPage';
import { SalesReportPage } from './pages/business/SalesReportPage';
import { BomPage } from './pages/business/BomPage';
import { RoutingPage } from './pages/business/RoutingPage';
import { MachinePage } from './pages/business/MachinePage';
import { WorkCenterPage } from './pages/business/WorkCenterPage';
import { ProductionLinePage } from './pages/business/ProductionLinePage';
import { ShiftPage } from './pages/business/ShiftPage';
import { CalendarPage } from './pages/business/CalendarPage';
import { ToolingPage } from './pages/business/ToolingPage';
import { OperationPage } from './pages/business/OperationPage';
import { ProductionParameterPage } from './pages/business/ProductionParameterPage';
import { ProductionOrderPage } from './pages/business/ProductionOrderPage';
import { WorkOrderPage } from './pages/business/WorkOrderPage';
import { MaterialConsumptionPage } from './pages/business/MaterialConsumptionPage';
import { ProductionConfirmationPage } from './pages/business/ProductionConfirmationPage';
import { WipPage } from './pages/business/WipPage';
import { PackagingPage } from './pages/business/PackagingPage';
import { FinishedGoodPage } from './pages/business/FinishedGoodPage';
import { ScrapPage } from './pages/business/ScrapPage';
import { ReworkPage } from './pages/business/ReworkPage';
import { ProductionDashboardPage } from './pages/business/ProductionDashboardPage';

export interface RouterContext {
  queryClient: QueryClient;
}

const rootRoute = createRootRouteWithContext<RouterContext>()({
  component: () => <Outlet />,
});

const loginRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/login',
  beforeLoad: () => {
    if (isAuthenticated()) throw redirect({ to: '/' });
  },
  component: LoginPage,
});

const authenticatedRoute = createRoute({
  getParentRoute: () => rootRoute,
  id: 'authenticated',
  beforeLoad: () => {
    if (!isAuthenticated()) throw redirect({ to: '/login' });
  },
  component: AuthenticatedLayout,
});

const dashboardRoute = createRoute({
  getParentRoute: () => authenticatedRoute,
  path: '/',
  component: DashboardPage,
});

const filesRoute = createRoute({
  getParentRoute: () => authenticatedRoute,
  path: '/administration/files',
  component: FilesPage,
});

const inventoryRoute = createRoute({
  getParentRoute: () => authenticatedRoute,
  path: '/inventory',
  component: InventoryWorkspaceLayout,
});

const inventoryIndexRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: '/',
  beforeLoad: () => {
    throw redirect({ to: '/inventory/dashboard' });
  },
});

const invDashboardRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'dashboard',
  component: InventoryDashboardPage,
});
const invMaterialsRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'master-data/materials',
  component: MaterialListPage,
});
const invMaterialDetailRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'master-data/materials/$id',
  component: MaterialDetailPage,
});
const invWarehousesRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'master-data/warehouses',
  component: WarehouseListPage,
});
const invWarehouseDetailRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'master-data/warehouses/$id',
  component: WarehouseDetailPage,
});
const invLocationsRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'master-data/locations',
  component: LocationListPage,
});
const invBalancesRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'stock/balances',
  component: StockBalancePage,
});
const invLotsRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'stock/lots',
  component: LotListPage,
});
const invReceiptsRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'operations/goods-receipts',
  component: GoodsReceiptListPage,
});
const invReceiptDetailRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'operations/goods-receipts/$id',
  component: GoodsReceiptDetailPage,
});
const invIssuesRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'operations/goods-issues',
  component: GoodsIssueListPage,
});
const invTransfersRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'operations/transfers',
  component: TransferListPage,
});
const invCountsRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'counts/cycle-counts',
  component: CycleCountListPage,
});
const invAdjustmentsRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'counts/adjustments',
  component: AdjustmentListPage,
});
const invReportsRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'reports',
  component: InventoryReportsPage,
});

const legacyInventoryRedirects = [
  ['/inventory/materials', '/inventory/master-data/materials'],
  ['/inventory/warehouses', '/inventory/master-data/warehouses'],
  ['/inventory/locations', '/inventory/master-data/locations'],
  ['/inventory/balances', '/inventory/stock/balances'],
  ['/inventory/batches', '/inventory/stock/lots'],
  ['/inventory/goods-receipts', '/inventory/operations/goods-receipts'],
  ['/inventory/goods-issues', '/inventory/operations/goods-issues'],
  ['/inventory/transfers', '/inventory/operations/transfers'],
  ['/inventory/counts', '/inventory/counts/cycle-counts'],
  ['/inventory/adjustments', '/inventory/counts/adjustments'],
].map(([from, to]) =>
  createRoute({
    getParentRoute: () => authenticatedRoute,
    path: from,
    beforeLoad: () => {
      throw redirect({ to });
    },
  }),
);

const route_10 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/purchasing/suppliers', component: SupplierPage });
const route_11 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/purchasing/purchase-requests', component: PurchaseRequestPage });
const route_12 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/purchasing/rfqs', component: RfqPage });
const route_13 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/purchasing/supplier-quotations', component: SupplierQuotationPage });
const route_14 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/purchasing/purchase-orders', component: PurchaseOrderPage });
const route_15 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/purchasing/purchase-goods-receipts', component: PurchaseGoodsReceiptPage });
const route_16 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/purchasing/purchase-returns', component: PurchaseReturnPage });
const route_17 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/purchasing/supplier-invoices', component: SupplierInvoicePage });
const route_18 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/purchasing/dashboard', component: PurchasingDashboardPage });
const route_19 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/purchasing/reports', component: PurchasingReportPage });
const route_20 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/customers', component: CustomerPage });
const route_21 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/leads', component: LeadPage });
const route_22 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/opportunities', component: OpportunityPage });
const route_23 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/quotations', component: SalesQuotationPage });
const route_24 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/sales-orders', component: SalesOrderPage });
const route_25 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/shipments', component: ShipmentPage });
const route_26 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/deliveries', component: DeliveryPage });
const route_27 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/customer-invoices', component: CustomerInvoicePage });
const route_28 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/dashboard', component: SalesDashboardPage });
const route_29 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/reports', component: SalesReportPage });
const route_30 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/boms', component: BomPage });
const route_31 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/routings', component: RoutingPage });
const route_32 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/machines', component: MachinePage });
const route_33 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/work-centers', component: WorkCenterPage });
const route_34 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/production-lines', component: ProductionLinePage });
const route_35 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/shifts', component: ShiftPage });
const route_36 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/calendars', component: CalendarPage });
const route_37 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/toolings', component: ToolingPage });
const route_38 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/operations', component: OperationPage });
const route_39 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/production-parameters', component: ProductionParameterPage });
const route_40 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/production-orders', component: ProductionOrderPage });
const route_41 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/work-orders', component: WorkOrderPage });
const route_42 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/material-consumptions', component: MaterialConsumptionPage });
const route_43 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/production-confirmations', component: ProductionConfirmationPage });
const route_44 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/wips', component: WipPage });
const route_45 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/packagings', component: PackagingPage });
const route_46 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/finished-goods', component: FinishedGoodPage });
const route_47 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/scraps', component: ScrapPage });
const route_48 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/reworks', component: ReworkPage });
const route_49 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/production/dashboard', component: ProductionDashboardPage });

const implemented = new Set([
  '/',
  '/administration/files',
  '/inventory',
  '/inventory/dashboard',
  '/inventory/master-data/materials',
  '/inventory/master-data/warehouses',
  '/inventory/master-data/locations',
  '/inventory/stock/balances',
  '/inventory/stock/lots',
  '/inventory/operations/goods-receipts',
  '/inventory/operations/goods-issues',
  '/inventory/operations/transfers',
  '/inventory/counts/cycle-counts',
  '/inventory/counts/adjustments',
  '/inventory/reports',
  '/inventory/materials',
  '/inventory/warehouses',
  '/inventory/locations',
  '/inventory/balances',
  '/inventory/batches',
  '/inventory/goods-receipts',
  '/inventory/goods-issues',
  '/inventory/transfers',
  '/inventory/counts',
  '/inventory/adjustments',
  '/purchasing/suppliers',
  '/purchasing/purchase-requests',
  '/purchasing/rfqs',
  '/purchasing/supplier-quotations',
  '/purchasing/purchase-orders',
  '/purchasing/purchase-goods-receipts',
  '/purchasing/purchase-returns',
  '/purchasing/supplier-invoices',
  '/purchasing/dashboard',
  '/purchasing/reports',
  '/sales/customers',
  '/sales/leads',
  '/sales/opportunities',
  '/sales/quotations',
  '/sales/sales-orders',
  '/sales/shipments',
  '/sales/deliveries',
  '/sales/customer-invoices',
  '/sales/dashboard',
  '/sales/reports',
  '/production/boms',
  '/production/routings',
  '/production/machines',
  '/production/work-centers',
  '/production/production-lines',
  '/production/shifts',
  '/production/calendars',
  '/production/toolings',
  '/production/operations',
  '/production/production-parameters',
  '/production/production-orders',
  '/production/work-orders',
  '/production/material-consumptions',
  '/production/production-confirmations',
  '/production/wips',
  '/production/packagings',
  '/production/finished-goods',
  '/production/scraps',
  '/production/reworks',
  '/production/dashboard',
]);
const modulePaths = collectNavPaths().filter((path) => !implemented.has(path));
const moduleRoutes = modulePaths.map((path) =>
  createRoute({ getParentRoute: () => authenticatedRoute, path, component: ModulePlaceholderPage }),
);

export const routeTree = rootRoute.addChildren([
  loginRoute,
  authenticatedRoute.addChildren([
    dashboardRoute,
    filesRoute,
    inventoryRoute.addChildren([
      inventoryIndexRoute,
      invDashboardRoute,
      invMaterialsRoute,
      invMaterialDetailRoute,
      invWarehousesRoute,
      invWarehouseDetailRoute,
      invLocationsRoute,
      invBalancesRoute,
      invLotsRoute,
      invReceiptsRoute,
      invReceiptDetailRoute,
      invIssuesRoute,
      invTransfersRoute,
      invCountsRoute,
      invAdjustmentsRoute,
      invReportsRoute,
    ]),
    ...legacyInventoryRedirects,
    route_10,
    route_11,
    route_12,
    route_13,
    route_14,
    route_15,
    route_16,
    route_17,
    route_18,
    route_19,
    route_20,
    route_21,
    route_22,
    route_23,
    route_24,
    route_25,
    route_26,
    route_27,
    route_28,
    route_29,
    route_30,
    route_31,
    route_32,
    route_33,
    route_34,
    route_35,
    route_36,
    route_37,
    route_38,
    route_39,
    route_40,
    route_41,
    route_42,
    route_43,
    route_44,
    route_45,
    route_46,
    route_47,
    route_48,
    route_49,
    ...moduleRoutes,
  ]),
]);

export function createAppRouter(queryClient: QueryClient) {
  return createRouter({ routeTree, context: { queryClient }, defaultPreload: 'intent' });
}

declare module '@tanstack/react-router' {
  interface Register {
    router: ReturnType<typeof createAppRouter>;
  }
}
