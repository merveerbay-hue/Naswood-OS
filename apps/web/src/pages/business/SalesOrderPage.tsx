import { ResourcePage } from './ResourcePage';

export function SalesOrderPage() {
  return (
    <ResourcePage
      title="SalesOrder"
      description="TASK-040 · Sales MVP"
      route="sales-orders"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'CustomerCode', label: 'CustomerCode', type: 'string' as const },
    { key: 'OrderDate', label: 'OrderDate', type: 'date' as const },
    { key: 'TotalAmount', label: 'TotalAmount', type: 'number' as const },
    { key: 'Currency', label: 'Currency', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
