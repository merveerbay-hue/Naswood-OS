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
import { MaterialPage } from './pages/business/MaterialPage';
import { WarehousePage } from './pages/business/WarehousePage';
import { LocationPage } from './pages/business/LocationPage';
import { InventoryBalancePage } from './pages/business/InventoryBalancePage';
import { BatchPage } from './pages/business/BatchPage';
import { GoodsReceiptPage } from './pages/business/GoodsReceiptPage';
import { GoodsIssuePage } from './pages/business/GoodsIssuePage';
import { StockTransferPage } from './pages/business/StockTransferPage';
import { InventoryCountPage } from './pages/business/InventoryCountPage';
import { InventoryAdjustmentPage } from './pages/business/InventoryAdjustmentPage';
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

const route_0 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/inventory/materials', component: MaterialPage });
const route_1 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/inventory/warehouses', component: WarehousePage });
const route_2 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/inventory/locations', component: LocationPage });
const route_3 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/inventory/balances', component: InventoryBalancePage });
const route_4 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/inventory/batches', component: BatchPage });
const route_5 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/inventory/goods-receipts', component: GoodsReceiptPage });
const route_6 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/inventory/goods-issues', component: GoodsIssuePage });
const route_7 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/inventory/transfers', component: StockTransferPage });
const route_8 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/inventory/counts', component: InventoryCountPage });
const route_9 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/inventory/adjustments', component: InventoryAdjustmentPage });
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

const implemented = new Set(['/', '/administration/files', '/inventory/materials', '/inventory/warehouses', '/inventory/locations', '/inventory/balances', '/inventory/batches', '/inventory/goods-receipts', '/inventory/goods-issues', '/inventory/transfers', '/inventory/counts', '/inventory/adjustments', '/purchasing/suppliers', '/purchasing/purchase-requests', '/purchasing/rfqs', '/purchasing/supplier-quotations', '/purchasing/purchase-orders', '/purchasing/purchase-goods-receipts', '/purchasing/purchase-returns', '/purchasing/supplier-invoices', '/purchasing/dashboard', '/purchasing/reports', '/sales/customers', '/sales/leads', '/sales/opportunities', '/sales/quotations', '/sales/sales-orders', '/sales/shipments', '/sales/deliveries', '/sales/customer-invoices', '/sales/dashboard', '/sales/reports', '/production/boms', '/production/routings', '/production/machines', '/production/work-centers', '/production/production-lines']);
const modulePaths = collectNavPaths().filter((path) => !implemented.has(path));
const moduleRoutes = modulePaths.map((path) =>
  createRoute({ getParentRoute: () => authenticatedRoute, path, component: ModulePlaceholderPage }),
);

export const routeTree = rootRoute.addChildren([
  loginRoute,
  authenticatedRoute.addChildren([
    dashboardRoute,
    filesRoute,
    route_0,
    route_1,
    route_2,
    route_3,
    route_4,
    route_5,
    route_6,
    route_7,
    route_8,
    route_9,
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
