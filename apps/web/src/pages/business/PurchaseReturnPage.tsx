import { ResourcePage } from './ResourcePage';

export function PurchaseReturnPage() {
  return (
    <ResourcePage
      title="PurchaseReturn"
      description="TASK-032 · Purchasing MVP"
      route="purchase-returns"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'SupplierCode', label: 'SupplierCode', type: 'string' as const },
    { key: 'PurchaseOrderNumber', label: 'PurchaseOrderNumber', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'Notes', label: 'Notes', type: 'string' as const }
      ]}
    />
  );
}
