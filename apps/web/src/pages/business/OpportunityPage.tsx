import { ResourcePage } from './ResourcePage';

export function OpportunityPage() {
  return (
    <ResourcePage
      title="Opportunity"
      description="TASK-038 · Sales MVP"
      route="opportunities"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'CustomerCode', label: 'CustomerCode', type: 'string' as const },
    { key: 'Title', label: 'Title', type: 'string' as const },
    { key: 'Amount', label: 'Amount', type: 'number' as const },
    { key: 'Stage', label: 'Stage', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
