import { ResourcePage } from './ResourcePage';

export function SalesDashboardPage() {
  return (
    <ResourcePage
      title="Sales Dashboard"
      description="TASK-044 · Sales MVP"
      route="sales/dashboard"
      kind="dashboard"
      fields={[
    { key: 'OpenOrders', label: 'OpenOrders', type: 'number' as const },
    { key: 'PipelineAmount', label: 'PipelineAmount', type: 'number' as const },
    { key: 'OverdueDeliveries', label: 'OverdueDeliveries', type: 'number' as const },
    { key: 'RevenueMtd', label: 'RevenueMtd', type: 'number' as const }
      ]}
    />
  );
}
