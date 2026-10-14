import { ResourcePage } from './ResourcePage';

export function SalesQuotationPage() {
  return (
    <ResourcePage
      title="SalesQuotation"
      description="TASK-039 · Sales MVP"
      route="quotations"
      kind="document"
      createLabel="Teklif hazırla"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'CustomerCode', label: 'CustomerCode', type: 'string' as const },
    { key: 'ValidUntil', label: 'ValidUntil', type: 'date' as const },
    { key: 'TotalAmount', label: 'TotalAmount', type: 'number' as const },
    { key: 'Currency', label: 'Currency', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
