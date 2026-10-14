import { ResourcePage } from './ResourcePage';

export function SupplierQuotationPage() {
  return (
    <ResourcePage
      title="SupplierQuotation"
      description="TASK-029 · Purchasing MVP"
      route="supplier-quotations"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'SupplierCode', label: 'SupplierCode', type: 'string' as const },
    { key: 'RfqNumber', label: 'RfqNumber', type: 'string' as const },
    { key: 'TotalAmount', label: 'TotalAmount', type: 'number' as const },
    { key: 'Currency', label: 'Currency', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
