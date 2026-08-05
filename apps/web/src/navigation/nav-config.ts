export interface NavItem {
  id: string;
  label: string;
  path?: string;
  icon?: string;
  children?: NavItem[];
  roles?: string[];
}

export const navigationTree: NavItem[] = [
  { id: 'dashboard', label: 'Dashboard', path: '/', icon: 'LayoutDashboard' },
  {
    id: 'inventory',
    label: 'Inventory',
    path: '/inventory/dashboard',
    icon: 'Package',
    children: [
      { id: 'inventory-dashboard', label: 'Dashboard', path: '/inventory/dashboard' },
      {
        id: 'inventory-operations',
        label: 'Operations',
        children: [
          { id: 'inventory-goods-receipts', label: 'Goods Receipt', path: '/inventory/operations/goods-receipts' },
          { id: 'inventory-goods-issues', label: 'Goods Issue', path: '/inventory/operations/goods-issues' },
          { id: 'inventory-transfers', label: 'Transfer', path: '/inventory/operations/transfers' },
        ],
      },
      {
        id: 'inventory-stock',
        label: 'Stock',
        children: [
          { id: 'inventory-balances', label: 'Stock Balance', path: '/inventory/stock/balances' },
          { id: 'inventory-lots', label: 'Lots', path: '/inventory/stock/lots' },
        ],
      },
      {
        id: 'inventory-counts-ws',
        label: 'Counts & Adjustments',
        children: [
          { id: 'inventory-counts', label: 'Cycle Count', path: '/inventory/counts/cycle-counts' },
          { id: 'inventory-adjustments', label: 'Adjustment', path: '/inventory/counts/adjustments' },
        ],
      },
      {
        id: 'inventory-master-data',
        label: 'Master Data',
        children: [
          { id: 'inventory-materials', label: 'Materials', path: '/inventory/master-data/materials' },
          { id: 'inventory-warehouses', label: 'Warehouses', path: '/inventory/master-data/warehouses' },
          { id: 'inventory-locations', label: 'Locations', path: '/inventory/master-data/locations' },
        ],
      },
      { id: 'inventory-reports', label: 'Reports', path: '/inventory/reports' },
    ],
  },
  {
    id: 'purchasing',
    label: 'Purchasing',
    path: '/purchasing',
    icon: 'ShoppingCart',
    children: [
      { id: 'purchasing-suppliers', label: 'Supplier', path: '/purchasing/suppliers' },
      { id: 'purchasing-purchase-requests', label: 'Purchase Requests', path: '/purchasing/purchase-requests' },
      { id: 'purchasing-rfqs', label: 'Rfq', path: '/purchasing/rfqs' },
      { id: 'purchasing-supplier-quotations', label: 'Supplier Quotations', path: '/purchasing/supplier-quotations' },
      { id: 'purchasing-purchase-orders', label: 'Purchase Orders', path: '/purchasing/purchase-orders' },
      { id: 'purchasing-purchase-goods-receipts', label: 'PurchaseGoods Receipt', path: '/purchasing/purchase-goods-receipts' },
      { id: 'purchasing-purchase-returns', label: 'Purchase Returns', path: '/purchasing/purchase-returns' },
      { id: 'purchasing-supplier-invoices', label: 'Supplier Invoices', path: '/purchasing/supplier-invoices' },
      { id: 'purchasing-dashboard', label: 'Dashboard', path: '/purchasing/dashboard' },
      { id: 'purchasing-reports', label: 'Reports', path: '/purchasing/reports' },
    ],
  },
  {
    id: 'sales',
    label: 'Sales',
    path: '/sales',
    icon: 'TrendingUp',
  },
  {
    id: 'production',
    label: 'Production',
    path: '/production',
    icon: 'Factory',
    children: [
      { id: 'production-boms', label: 'Bom', path: '/production/boms' },
      { id: 'production-routings', label: 'Routing', path: '/production/routings' },
      { id: 'production-machines', label: 'Machine', path: '/production/machines' },
      { id: 'production-work-centers', label: 'Work Centers', path: '/production/work-centers' },
      { id: 'production-production-lines', label: 'Production Lines', path: '/production/production-lines' },
      { id: 'production-shifts', label: 'Shifts', path: '/production/shifts' },
      { id: 'production-calendars', label: 'Calendars', path: '/production/calendars' },
      { id: 'production-toolings', label: 'Tooling', path: '/production/toolings' },
      { id: 'production-operations', label: 'Operations', path: '/production/operations' },
      { id: 'production-parameters', label: 'Production Parameters', path: '/production/production-parameters' },
      { id: 'production-orders', label: 'Production Orders', path: '/production/production-orders' },
      { id: 'production-work-orders', label: 'Work Orders', path: '/production/work-orders' },
      { id: 'production-material-consumptions', label: 'Material Consumptions', path: '/production/material-consumptions' },
      { id: 'production-confirmations', label: 'Production Confirmations', path: '/production/production-confirmations' },
      { id: 'production-wips', label: 'WIP', path: '/production/wips' },
      { id: 'production-packagings', label: 'Packaging', path: '/production/packagings' },
      { id: 'production-finished-goods', label: 'Finished Goods', path: '/production/finished-goods' },
      { id: 'production-scraps', label: 'Scrap', path: '/production/scraps' },
      { id: 'production-reworks', label: 'Rework', path: '/production/reworks' },
      { id: 'production-dashboard', label: 'Dashboard', path: '/production/dashboard' },
    ],
  },
  { id: 'quality', label: 'Quality', path: '/quality', icon: 'BadgeCheck' },
  { id: 'maintenance', label: 'Maintenance', path: '/maintenance', icon: 'Wrench' },
  { id: 'finance', label: 'Finance', path: '/finance', icon: 'Wallet' },
  { id: 'analytics', label: 'Analytics', path: '/analytics', icon: 'BarChart3' },
  { id: 'ai', label: 'AI', path: '/ai', icon: 'Sparkles' },
  {
    id: 'administration',
    label: 'Administration',
    path: '/administration',
    icon: 'Settings',
    roles: ['Administrator'],
    children: [
      { id: 'admin-users', label: 'Users', path: '/administration/users', roles: ['Administrator'] },
      { id: 'admin-roles', label: 'Roles', path: '/administration/roles', roles: ['Administrator'] },
      { id: 'admin-permissions', label: 'Permissions', path: '/administration/permissions', roles: ['Administrator'] },
      { id: 'admin-files', label: 'Files', path: '/administration/files', roles: ['Administrator'] },
      { id: 'admin-settings', label: 'Settings', path: '/administration/settings', roles: ['Administrator'] },
      { id: 'admin-audit', label: 'Audit Logs', path: '/administration/audit', roles: ['Administrator'] },
      { id: 'admin-health', label: 'System Health', path: '/administration/health', roles: ['Administrator'] },
    ],
  },
];

// Sales children appended
navigationTree.find(i => i.id === 'sales')!.children = [
      { id: 'sales-customers', label: 'Customer', path: '/sales/customers' },
      { id: 'sales-leads', label: 'Lead', path: '/sales/leads' },
      { id: 'sales-opportunities', label: 'Opportunity', path: '/sales/opportunities' },
      { id: 'sales-quotations', label: 'Quotations', path: '/sales/quotations' },
      { id: 'sales-sales-orders', label: 'Sales Orders', path: '/sales/sales-orders' },
      { id: 'sales-shipments', label: 'Shipment', path: '/sales/shipments' },
      { id: 'sales-deliveries', label: 'Delivery', path: '/sales/deliveries' },
      { id: 'sales-customer-invoices', label: 'Customer Invoices', path: '/sales/customer-invoices' },
      { id: 'sales-dashboard', label: 'Dashboard', path: '/sales/dashboard' },
      { id: 'sales-reports', label: 'Reports', path: '/sales/reports' },
];

export function filterNavigationByRoles(items: NavItem[], roles: string[]): NavItem[] {
  return items
    .filter((item) => !item.roles || item.roles.some((role) => roles.includes(role)))
    .map((item) => ({
      ...item,
      children: item.children ? filterNavigationByRoles(item.children, roles) : undefined,
    }));
}

export function findNavTrail(pathname: string, items: NavItem[] = navigationTree): NavItem[] {
  const normalized = pathname === '' ? '/' : pathname;
  for (const item of items) {
    if (item.path === normalized) return [item];
    if (item.children?.length) {
      const childTrail = findNavTrail(normalized, item.children);
      if (childTrail.length > 0) return [item, ...childTrail];
    }
  }
  return [];
}

export function collectNavPaths(items: NavItem[] = navigationTree): string[] {
  const paths: string[] = [];
  for (const item of items) {
    if (item.path) paths.push(item.path);
    if (item.children) paths.push(...collectNavPaths(item.children));
  }
  return paths;
}

export function isPathActive(pathname: string, itemPath?: string): boolean {
  if (!itemPath) return false;
  if (itemPath === '/') return pathname === '/';
  return pathname === itemPath || pathname.startsWith(`${itemPath}/`);
}
