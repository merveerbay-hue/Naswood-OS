import { ResourcePage } from './ResourcePage';

export function CustomerInvoicePage() {
  return (
    <ResourcePage
      title="CustomerInvoice"
      description="TASK-043 · Sales MVP"
      route="customer-invoices"
      kind="document"
      createLabel="Fatura kes"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'CustomerCode', label: 'CustomerCode', type: 'string' as const },
    { key: 'InvoiceDate', label: 'InvoiceDate', type: 'date' as const },
    { key: 'TotalAmount', label: 'TotalAmount', type: 'number' as const },
    { key: 'Currency', label: 'Currency', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
