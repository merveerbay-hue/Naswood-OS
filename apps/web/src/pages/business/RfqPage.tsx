import { ResourcePage } from './ResourcePage';

export function RfqPage() {
  return (
    <ResourcePage
      title="Rfq"
      description="TASK-028 · Purchasing MVP"
      route="rfqs"
      kind="document"
      createLabel="Teklif iste"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'Title', label: 'Title', type: 'string' as const },
    { key: 'DueDate', label: 'DueDate', type: 'date' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'Notes', label: 'Notes', type: 'string' as const }
      ]}
    />
  );
}
