import type { WorkspaceDefinition } from '@/components/workspace/WorkspaceShell';

/** Production workspace map — docs/15_UI_Architecture/Production/Workspaces.md */
export const productionWorkspaces: WorkspaceDefinition[] = [
  {
    id: 'dashboard',
    label: 'Dashboard',
    items: [{ id: 'prd-001', label: 'Overview', path: '/production/dashboard', screenId: 'PRD-001' }],
  },
  {
    id: 'planning',
    label: 'Planning',
    items: [
      { id: 'prd-010', label: 'Production Orders', path: '/production/planning/orders', screenId: 'PRD-010' },
      { id: 'prd-012', label: 'Work Orders', path: '/production/planning/work-orders', screenId: 'PRD-012' },
      { id: 'prd-021', label: 'Scheduling', path: '/production/planning/scheduling', screenId: 'PRD-021' },
      { id: 'prd-022', label: 'Capacity', path: '/production/planning/capacity', screenId: 'PRD-022' },
      { id: 'prd-023', label: 'Dispatch', path: '/production/planning/dispatch', screenId: 'PRD-023' },
    ],
  },
  {
    id: 'execution',
    label: 'Execution',
    items: [
      { id: 'prd-013', label: 'Operator Terminal', path: '/production/execution/operator-terminal', screenId: 'PRD-013' },
      { id: 'prd-024', label: 'Machine Panel', path: '/production/execution/machine-panel', screenId: 'PRD-024' },
      { id: 'prd-014', label: 'Consumption', path: '/production/execution/consumption', screenId: 'PRD-014' },
      { id: 'prd-015', label: 'Confirmation', path: '/production/execution/confirmation', screenId: 'PRD-015' },
      { id: 'prd-016', label: 'WIP', path: '/production/execution/wip', screenId: 'PRD-016' },
      { id: 'prd-017', label: 'Packaging', path: '/production/execution/packaging', screenId: 'PRD-017' },
      { id: 'prd-018', label: 'Finished Goods', path: '/production/execution/finished-goods', screenId: 'PRD-018' },
      { id: 'prd-027', label: 'Scrap', path: '/production/execution/scrap', screenId: 'PRD-027' },
      { id: 'prd-028', label: 'Rework', path: '/production/execution/rework', screenId: 'PRD-028' },
    ],
  },
  {
    id: 'master-data',
    label: 'Master Data',
    items: [
      { id: 'prd-002', label: 'BOM', path: '/production/master-data/boms', screenId: 'PRD-002' },
      { id: 'prd-004', label: 'Routing', path: '/production/master-data/routings', screenId: 'PRD-004' },
      { id: 'prd-025', label: 'Operations', path: '/production/master-data/operations', screenId: 'PRD-025' },
      { id: 'prd-007', label: 'Machines', path: '/production/master-data/machines', screenId: 'PRD-007' },
      { id: 'prd-006', label: 'Work Centers', path: '/production/master-data/work-centers', screenId: 'PRD-006' },
      { id: 'prd-026', label: 'Lines', path: '/production/master-data/lines', screenId: 'PRD-026' },
      { id: 'prd-008', label: 'Shifts', path: '/production/master-data/shifts', screenId: 'PRD-008' },
      { id: 'prd-009', label: 'Calendars', path: '/production/master-data/calendars', screenId: 'PRD-009' },
      { id: 'prd-053', label: 'Tooling', path: '/production/master-data/toolings', screenId: 'PRD-Tooling' },
    ],
  },
  {
    id: 'insights',
    label: 'Reports & Analytics',
    items: [
      { id: 'prd-020', label: 'Reports', path: '/production/reports', screenId: 'PRD-020' },
      { id: 'prd-019', label: 'Analytics', path: '/production/analytics', screenId: 'PRD-019' },
      { id: 'prd-029', label: 'Settings', path: '/production/settings', screenId: 'PRD-029' },
    ],
  },
];
