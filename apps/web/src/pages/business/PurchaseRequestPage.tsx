import { ResourcePage } from './ResourcePage';

export function PurchaseRequestPage() {
  return (
    <ResourcePage
      title="PurchaseRequest"
      description="TASK-027 · Purchasing MVP"
      route="purchase-requests"
      kind="document"
      createLabel="Satınalma talebi aç"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'Requester', label: 'Requester', type: 'string' as const },
    { key: 'NeededDate', label: 'NeededDate', type: 'date' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'Notes', label: 'Notes', type: 'string' as const }
      ]}
    />
  );
}
