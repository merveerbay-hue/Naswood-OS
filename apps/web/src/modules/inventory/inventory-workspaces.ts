import type { WorkspaceDefinition } from '@/components/workspace/WorkspaceShell';

/** Inventory workspace map — docs/15_UI_Architecture/Inventory + 19_Navigation/Menu */
export const inventoryWorkspaces: WorkspaceDefinition[] = [
  {
    id: 'dashboard',
    label: 'Dashboard',
    items: [{ id: 'inv-001', label: 'Overview', path: '/inventory/dashboard', screenId: 'INV-001' }],
  },
  {
    id: 'operations',
    label: 'Operations',
    items: [
      { id: 'inv-015', label: 'Goods Receipt', path: '/inventory/operations/goods-receipts', screenId: 'INV-015' },
      { id: 'inv-017', label: 'Goods Issue', path: '/inventory/operations/goods-issues', screenId: 'INV-017' },
      { id: 'inv-019', label: 'Transfer', path: '/inventory/operations/transfers', screenId: 'INV-019' },
    ],
  },
  {
    id: 'stock',
    label: 'Stock',
    items: [
      { id: 'inv-014', label: 'Stock Balance', path: '/inventory/stock/balances', screenId: 'INV-014' },
      { id: 'inv-010', label: 'Lots', path: '/inventory/stock/lots', screenId: 'INV-010' },
    ],
  },
  {
    id: 'counts',
    label: 'Counts & Adjustments',
    items: [
      { id: 'inv-021', label: 'Cycle Count', path: '/inventory/counts/cycle-counts', screenId: 'INV-021' },
      { id: 'inv-024', label: 'Adjustment', path: '/inventory/counts/adjustments', screenId: 'INV-024' },
    ],
  },
  {
    id: 'master-data',
    label: 'Master Data',
    items: [
      { id: 'inv-004', label: 'Materials', path: '/inventory/master-data/materials', screenId: 'INV-004' },
      { id: 'inv-006', label: 'Warehouses', path: '/inventory/master-data/warehouses', screenId: 'INV-006' },
      { id: 'inv-008', label: 'Locations', path: '/inventory/master-data/locations', screenId: 'INV-008' },
    ],
  },
  {
    id: 'reports',
    label: 'Reports',
    items: [{ id: 'inv-025', label: 'Reports', path: '/inventory/reports', screenId: 'INV-025' }],
  },
];
