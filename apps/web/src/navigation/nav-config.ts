export interface NavItem {
  id: string;
  label: string;
  path?: string;
  icon?: string;
  children?: NavItem[];
  /** If set, user must have at least one of these roles. */
  roles?: string[];
}

/**
 * Naswood default navigation (TASK-007 / TASK-008).
 * Module pages are placeholders until business modules ship.
 */
export const navigationTree: NavItem[] = [
  {
    id: 'dashboard',
    label: 'Dashboard',
    path: '/',
    icon: 'LayoutDashboard',
  },
  {
    id: 'inventory',
    label: 'Inventory',
    path: '/inventory',
    icon: 'Package',
    children: [
      { id: 'inventory-materials', label: 'Materials', path: '/inventory/materials' },
      { id: 'inventory-warehouses', label: 'Warehouses', path: '/inventory/warehouses' },
      { id: 'inventory-locations', label: 'Locations', path: '/inventory/locations' },
      { id: 'inventory-stock', label: 'Inventory', path: '/inventory/stock' },
      { id: 'inventory-reports', label: 'Reports', path: '/inventory/reports' },
    ],
  },
  {
    id: 'purchasing',
    label: 'Purchasing',
    path: '/purchasing',
    icon: 'ShoppingCart',
    children: [
      { id: 'purchasing-suppliers', label: 'Suppliers', path: '/purchasing/suppliers' },
      { id: 'purchasing-requests', label: 'Purchase Requests', path: '/purchasing/requests' },
      { id: 'purchasing-rfq', label: 'RFQ', path: '/purchasing/rfq' },
      { id: 'purchasing-orders', label: 'Purchase Orders', path: '/purchasing/orders' },
      { id: 'purchasing-receipts', label: 'Goods Receipt', path: '/purchasing/receipts' },
      { id: 'purchasing-returns', label: 'Purchase Returns', path: '/purchasing/returns' },
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
  },
  {
    id: 'quality',
    label: 'Quality',
    path: '/quality',
    icon: 'BadgeCheck',
  },
  {
    id: 'maintenance',
    label: 'Maintenance',
    path: '/maintenance',
    icon: 'Wrench',
  },
  {
    id: 'finance',
    label: 'Finance',
    path: '/finance',
    icon: 'Wallet',
  },
  {
    id: 'analytics',
    label: 'Analytics',
    path: '/analytics',
    icon: 'BarChart3',
  },
  {
    id: 'ai',
    label: 'AI',
    path: '/ai',
    icon: 'Sparkles',
  },
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
      { id: 'admin-settings', label: 'Settings', path: '/administration/settings', roles: ['Administrator'] },
      { id: 'admin-audit', label: 'Audit Logs', path: '/administration/audit', roles: ['Administrator'] },
      { id: 'admin-health', label: 'System Health', path: '/administration/health', roles: ['Administrator'] },
    ],
  },
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
    if (item.path === normalized) {
      return [item];
    }
    if (item.children?.length) {
      const childTrail = findNavTrail(normalized, item.children);
      if (childTrail.length > 0) {
        return [item, ...childTrail];
      }
    }
  }

  return [];
}

export function collectNavPaths(items: NavItem[] = navigationTree): string[] {
  const paths: string[] = [];
  for (const item of items) {
    if (item.path) {
      paths.push(item.path);
    }
    if (item.children) {
      paths.push(...collectNavPaths(item.children));
    }
  }
  return paths;
}

export function isPathActive(pathname: string, itemPath?: string): boolean {
  if (!itemPath) {
    return false;
  }
  if (itemPath === '/') {
    return pathname === '/';
  }
  return pathname === itemPath || pathname.startsWith(`${itemPath}/`);
}
