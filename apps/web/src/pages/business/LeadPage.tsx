import { ResourcePage } from './ResourcePage';

export function LeadPage() {
  return (
    <ResourcePage
      title="Lead"
      description="TASK-037 · Sales MVP"
      route="leads"
      kind="master"
      createLabel="Lead kaydet"
      fields={[
    { key: 'Code', label: 'Code', type: 'string' as const },
    { key: 'Name', label: 'Name', type: 'string' as const },
    { key: 'CompanyName', label: 'CompanyName', type: 'string' as const },
    { key: 'Email', label: 'Email', type: 'string' as const },
    { key: 'Source', label: 'Source', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
