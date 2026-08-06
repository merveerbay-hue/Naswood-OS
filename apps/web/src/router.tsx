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
import { ProductionWorkspaceLayout } from './modules/production/ProductionWorkspaceLayout';
import { ProductionDashboardPage } from './modules/production/overview/ProductionDashboardPage';
import {
  BomDetailPage,
  BomListPage,
  CalendarsPage,
  CapacityPage,
  ConfirmationPage,
  ConsumptionPage,
  DispatchPage,
  FinishedGoodsPage,
  MachinePanelPage,
  MachinesMasterPage,
  OperationsMasterPage,
  OperatorTerminalPage,
  PackagingPage,
  ProductionAnalyticsPage,
  ProductionLinesPage,
  ProductionOrderDetailPage,
  ProductionOrderListPage,
  ProductionReportsPage,
  ProductionSettingsPage,
  ReworkPage,
  RoutingListPage,
  SchedulingPage,
  ScrapPage,
  ShiftsPage,
  ToolingsPage,
  WipPage,
  WorkCentersPage,
  WorkOrderListPage,
} from './modules/production/screens';
import {
  BomBuilderPage,
  CalendarPlannerPage,
  CycleCountWizardPage,
  IssueWizardPage,
  LineDesignerPage,
  MachineStudioPage,
  MaterialDefinePage,
  OperationDesignerPage,
  PlanningWizardPage,
  PurchaseOrderWizardPage,
  ReceivingWizardPage,
  RoutingDesignerPage,
  SalesOrderWizardPage,
  ShiftPlannerPage,
  ToolLibraryManagerPage,
  TransferWizardPage,
  WarehouseDefinePage,
  WorkCenterDesignerPage,
} from './modules/shared/process/processWizardPages';

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
const invDefineMaterialRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'master-data/define-material',
  component: MaterialDefinePage,
});
const invDefineWarehouseRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'master-data/define-warehouse',
  component: WarehouseDefinePage,
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
const invReceiveWizardRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'operations/receive',
  component: ReceivingWizardPage,
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
const invIssueWizardRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'operations/issue',
  component: IssueWizardPage,
});
const invTransfersRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'operations/transfers',
  component: TransferListPage,
});
const invTransferWizardRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'operations/transfer',
  component: TransferWizardPage,
});
const invCountsRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'counts/cycle-counts',
  component: CycleCountListPage,
});
const invCountWizardRoute = createRoute({
  getParentRoute: () => inventoryRoute,
  path: 'counts/start',
  component: CycleCountWizardPage,
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
const route_14b = createRoute({
  getParentRoute: () => authenticatedRoute,
  path: '/purchasing/purchase-orders/place',
  component: PurchaseOrderWizardPage,
});
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
const route_24b = createRoute({
  getParentRoute: () => authenticatedRoute,
  path: '/sales/sales-orders/enter',
  component: SalesOrderWizardPage,
});
const route_25 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/shipments', component: ShipmentPage });
const route_26 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/deliveries', component: DeliveryPage });
const route_27 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/customer-invoices', component: CustomerInvoicePage });
const route_28 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/dashboard', component: SalesDashboardPage });
const route_29 = createRoute({ getParentRoute: () => authenticatedRoute, path: '/sales/reports', component: SalesReportPage });
const productionRoute = createRoute({
  getParentRoute: () => authenticatedRoute,
  path: '/production',
  component: ProductionWorkspaceLayout,
});
const productionIndexRoute = createRoute({
  getParentRoute: () => productionRoute,
  path: '/',
  beforeLoad: () => {
    throw redirect({ to: '/production/dashboard' });
  },
});
const prdDashboard = createRoute({ getParentRoute: () => productionRoute, path: 'dashboard', component: ProductionDashboardPage });
const prdOrders = createRoute({ getParentRoute: () => productionRoute, path: 'planning/orders', component: ProductionOrderListPage });
const prdPlanWizard = createRoute({ getParentRoute: () => productionRoute, path: 'planning/plan', component: PlanningWizardPage });
const prdBomBuilder = createRoute({ getParentRoute: () => productionRoute, path: 'engineering/bom-builder', component: BomBuilderPage });
const prdRoutingDesigner = createRoute({ getParentRoute: () => productionRoute, path: 'engineering/routing-designer', component: RoutingDesignerPage });
const prdMachineStudio = createRoute({ getParentRoute: () => productionRoute, path: 'engineering/machine-studio', component: MachineStudioPage });
const prdWcDesigner = createRoute({ getParentRoute: () => productionRoute, path: 'engineering/work-center-designer', component: WorkCenterDesignerPage });
const prdLineDesigner = createRoute({ getParentRoute: () => productionRoute, path: 'engineering/line-designer', component: LineDesignerPage });
const prdOpDesigner = createRoute({ getParentRoute: () => productionRoute, path: 'engineering/operation-designer', component: OperationDesignerPage });
const prdShiftPlanner = createRoute({ getParentRoute: () => productionRoute, path: 'engineering/shift-planner', component: ShiftPlannerPage });
const prdCalPlanner = createRoute({ getParentRoute: () => productionRoute, path: 'engineering/calendar-planner', component: CalendarPlannerPage });
const prdToolLib = createRoute({ getParentRoute: () => productionRoute, path: 'engineering/tool-library', component: ToolLibraryManagerPage });

const prdOrderDetail = createRoute({ getParentRoute: () => productionRoute, path: 'planning/orders/$id', component: ProductionOrderDetailPage });
const prdWorkOrders = createRoute({ getParentRoute: () => productionRoute, path: 'planning/work-orders', component: WorkOrderListPage });
const prdScheduling = createRoute({ getParentRoute: () => productionRoute, path: 'planning/scheduling', component: SchedulingPage });
const prdCapacity = createRoute({ getParentRoute: () => productionRoute, path: 'planning/capacity', component: CapacityPage });
const prdDispatch = createRoute({ getParentRoute: () => productionRoute, path: 'planning/dispatch', component: DispatchPage });
const prdOperator = createRoute({ getParentRoute: () => productionRoute, path: 'execution/operator-terminal', component: OperatorTerminalPage });
const prdMachinePanel = createRoute({ getParentRoute: () => productionRoute, path: 'execution/machine-panel', component: MachinePanelPage });
const prdConsumption = createRoute({ getParentRoute: () => productionRoute, path: 'execution/consumption', component: ConsumptionPage });
const prdConfirmation = createRoute({ getParentRoute: () => productionRoute, path: 'execution/confirmation', component: ConfirmationPage });
const prdWip = createRoute({ getParentRoute: () => productionRoute, path: 'execution/wip', component: WipPage });
const prdPackaging = createRoute({ getParentRoute: () => productionRoute, path: 'execution/packaging', component: PackagingPage });
const prdFg = createRoute({ getParentRoute: () => productionRoute, path: 'execution/finished-goods', component: FinishedGoodsPage });
const prdScrap = createRoute({ getParentRoute: () => productionRoute, path: 'execution/scrap', component: ScrapPage });
const prdRework = createRoute({ getParentRoute: () => productionRoute, path: 'execution/rework', component: ReworkPage });
const prdBoms = createRoute({ getParentRoute: () => productionRoute, path: 'master-data/boms', component: BomListPage });
const prdBomDetail = createRoute({ getParentRoute: () => productionRoute, path: 'master-data/boms/$id', component: BomDetailPage });
const prdRoutings = createRoute({ getParentRoute: () => productionRoute, path: 'master-data/routings', component: RoutingListPage });
const prdOperations = createRoute({ getParentRoute: () => productionRoute, path: 'master-data/operations', component: OperationsMasterPage });
const prdMachines = createRoute({ getParentRoute: () => productionRoute, path: 'master-data/machines', component: MachinesMasterPage });
const prdWorkCenters = createRoute({ getParentRoute: () => productionRoute, path: 'master-data/work-centers', component: WorkCentersPage });
const prdLines = createRoute({ getParentRoute: () => productionRoute, path: 'master-data/lines', component: ProductionLinesPage });
const prdShifts = createRoute({ getParentRoute: () => productionRoute, path: 'master-data/shifts', component: ShiftsPage });
const prdCalendars = createRoute({ getParentRoute: () => productionRoute, path: 'master-data/calendars', component: CalendarsPage });
const prdToolings = createRoute({ getParentRoute: () => productionRoute, path: 'master-data/toolings', component: ToolingsPage });
const prdReports = createRoute({ getParentRoute: () => productionRoute, path: 'reports', component: ProductionReportsPage });
const prdAnalytics = createRoute({ getParentRoute: () => productionRoute, path: 'analytics', component: ProductionAnalyticsPage });
const prdSettings = createRoute({ getParentRoute: () => productionRoute, path: 'settings', component: ProductionSettingsPage });

const legacyProductionRedirects = [
  ['/production/boms', '/production/master-data/boms'],
  ['/production/routings', '/production/master-data/routings'],
  ['/production/machines', '/production/master-data/machines'],
  ['/production/work-centers', '/production/master-data/work-centers'],
  ['/production/production-lines', '/production/master-data/lines'],
  ['/production/shifts', '/production/master-data/shifts'],
  ['/production/calendars', '/production/master-data/calendars'],
  ['/production/toolings', '/production/master-data/toolings'],
  ['/production/operations', '/production/master-data/operations'],
  ['/production/production-orders', '/production/planning/orders'],
  ['/production/work-orders', '/production/planning/work-orders'],
  ['/production/material-consumptions', '/production/execution/consumption'],
  ['/production/production-confirmations', '/production/execution/confirmation'],
  ['/production/wips', '/production/execution/wip'],
  ['/production/packagings', '/production/execution/packaging'],
  ['/production/finished-goods', '/production/execution/finished-goods'],
  ['/production/scraps', '/production/execution/scrap'],
  ['/production/reworks', '/production/execution/rework'],
].map(([from, to]) =>
  createRoute({
    getParentRoute: () => authenticatedRoute,
    path: from,
    beforeLoad: () => {
      throw redirect({ to });
    },
  }),
);

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
  '/inventory/operations/receive',
  '/inventory/operations/goods-issues',
  '/inventory/operations/issue',
  '/inventory/operations/transfers',
  '/inventory/operations/transfer',
  '/inventory/counts/cycle-counts',
  '/inventory/counts/start',
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
  '/purchasing/purchase-orders/place',
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
  '/sales/sales-orders/enter',
  '/sales/shipments',
  '/sales/deliveries',
  '/sales/customer-invoices',
  '/sales/dashboard',
  '/sales/reports',
  '/production',
  '/production/dashboard',
  '/production/planning/orders',
  '/production/planning/plan',
  '/production/engineering/tool-library',
  '/production/engineering/calendar-planner',
  '/production/engineering/shift-planner',
  '/production/engineering/operation-designer',
  '/production/engineering/line-designer',
  '/production/engineering/work-center-designer',
  '/production/engineering/machine-studio',
  '/production/engineering/routing-designer',
  '/production/engineering/bom-builder',
  '/inventory/master-data/define-warehouse',
  '/inventory/master-data/define-material',
  '/production/planning/work-orders',
  '/production/planning/scheduling',
  '/production/planning/capacity',
  '/production/planning/dispatch',
  '/production/execution/operator-terminal',
  '/production/execution/machine-panel',
  '/production/execution/consumption',
  '/production/execution/confirmation',
  '/production/execution/wip',
  '/production/execution/packaging',
  '/production/execution/finished-goods',
  '/production/execution/scrap',
  '/production/execution/rework',
  '/production/master-data/boms',
  '/production/master-data/routings',
  '/production/master-data/operations',
  '/production/master-data/machines',
  '/production/master-data/work-centers',
  '/production/master-data/lines',
  '/production/master-data/shifts',
  '/production/master-data/calendars',
  '/production/master-data/toolings',
  '/production/reports',
  '/production/analytics',
  '/production/settings',
  '/production/boms',
  '/production/routings',
  '/production/machines',
  '/production/work-centers',
  '/production/production-lines',
  '/production/shifts',
  '/production/calendars',
  '/production/toolings',
  '/production/operations',
  '/production/production-orders',
  '/production/work-orders',
  '/production/material-consumptions',
  '/production/production-confirmations',
  '/production/wips',
  '/production/packagings',
  '/production/finished-goods',
  '/production/scraps',
  '/production/reworks',
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
      invDefineMaterialRoute,
      invDefineWarehouseRoute,
      invBalancesRoute,
      invLotsRoute,
      invReceiptsRoute,
      invReceiveWizardRoute,
      invReceiptDetailRoute,
      invIssuesRoute,
      invIssueWizardRoute,
      invTransfersRoute,
      invTransferWizardRoute,
      invCountsRoute,
      invCountWizardRoute,
      invAdjustmentsRoute,
      invReportsRoute,
    ]),
    ...legacyInventoryRedirects,
    route_10,
    route_11,
    route_12,
    route_13,
    route_14,
    route_14b,
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
    route_24b,
    route_25,
    route_26,
    route_27,
    route_28,
    route_29,
    productionRoute.addChildren([
      productionIndexRoute,
      prdDashboard,
      prdOrders,
      prdPlanWizard,
      prdBomBuilder,
      prdRoutingDesigner,
      prdMachineStudio,
      prdWcDesigner,
      prdLineDesigner,
      prdOpDesigner,
      prdShiftPlanner,
      prdCalPlanner,
      prdToolLib,
      prdOrderDetail,
      prdWorkOrders,
      prdScheduling,
      prdCapacity,
      prdDispatch,
      prdOperator,
      prdMachinePanel,
      prdConsumption,
      prdConfirmation,
      prdWip,
      prdPackaging,
      prdFg,
      prdScrap,
      prdRework,
      prdBoms,
      prdBomDetail,
      prdRoutings,
      prdOperations,
      prdMachines,
      prdWorkCenters,
      prdLines,
      prdShifts,
      prdCalendars,
      prdToolings,
      prdReports,
      prdAnalytics,
      prdSettings,
    ]),
    ...legacyProductionRedirects,
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
