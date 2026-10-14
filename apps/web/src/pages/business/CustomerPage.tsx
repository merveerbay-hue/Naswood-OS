import { ResourcePage } from './ResourcePage';

export function CustomerPage() {
  return (
    <ResourcePage
      title="Customer"
      description="TASK-036 · Sales MVP"
      route="customers"
      kind="master"
      fields={[
    { key: 'Code', label: 'Code', type: 'string' as const },
    { key: 'Name', label: 'Name', type: 'string' as const },
    { key: 'TaxNumber', label: 'TaxNumber', type: 'string' as const },
    { key: 'Email', label: 'Email', type: 'string' as const },
    { key: 'Phone', label: 'Phone', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
