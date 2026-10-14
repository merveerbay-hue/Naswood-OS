import { ResourcePage } from './ResourcePage';

export function ProductionDashboardPage() {
  return (
    <ResourcePage
      title="Production Dashboard"
      description="TASK-065 · Production MVP"
      route="production/dashboard"
      kind="dashboard"
      fields={[
        { key: 'OpenProductionOrders', label: 'OpenProductionOrders', type: 'number' as const },
        { key: 'ActiveWorkOrders', label: 'ActiveWorkOrders', type: 'number' as const },
        { key: 'WipQuantity', label: 'WipQuantity', type: 'number' as const },
        { key: 'ScrapRate', label: 'ScrapRate', type: 'number' as const },
      ]}
    />
  );
}
