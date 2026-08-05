import { ResourcePage } from './ResourcePage';

export function PurchaseOrderPage() {
  return (
    <ResourcePage
      title="PurchaseOrder"
      description="TASK-030 · Purchasing MVP"
      route="purchase-orders"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'SupplierCode', label: 'SupplierCode', type: 'string' as const },
    { key: 'OrderDate', label: 'OrderDate', type: 'date' as const },
    { key: 'TotalAmount', label: 'TotalAmount', type: 'number' as const },
    { key: 'Currency', label: 'Currency', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
