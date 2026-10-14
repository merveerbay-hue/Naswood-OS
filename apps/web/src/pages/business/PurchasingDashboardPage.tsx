import { ResourcePage } from './ResourcePage';

export function PurchasingDashboardPage() {
  return (
    <ResourcePage
      title="Purchasing Dashboard"
      description="TASK-034 · Purchasing MVP"
      route="purchasing/dashboard"
      kind="dashboard"
      fields={[
    { key: 'OpenPurchaseOrders', label: 'OpenPurchaseOrders', type: 'number' as const },
    { key: 'PendingApprovals', label: 'PendingApprovals', type: 'number' as const },
    { key: 'OverdueReceipts', label: 'OverdueReceipts', type: 'number' as const },
    { key: 'SpendMtd', label: 'SpendMtd', type: 'number' as const }
      ]}
    />
  );
}
