import { ResourcePage } from './ResourcePage';

export function SupplierInvoicePage() {
  return (
    <ResourcePage
      title="SupplierInvoice"
      description="TASK-033 · Purchasing MVP"
      route="supplier-invoices"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'SupplierCode', label: 'SupplierCode', type: 'string' as const },
    { key: 'InvoiceDate', label: 'InvoiceDate', type: 'date' as const },
    { key: 'TotalAmount', label: 'TotalAmount', type: 'number' as const },
    { key: 'Currency', label: 'Currency', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
