import { ResourcePage } from './ResourcePage';

export function MaterialConsumptionPage() {
  return (
    <ResourcePage
      title="MaterialConsumption"
      description="TASK-058 · Production MVP"
      route="material-consumptions"
      kind="master"
      fields={[
        { key: 'Code', label: 'Code', type: 'string' as const },
        { key: 'Name', label: 'Name', type: 'string' as const },
        { key: 'Status', label: 'Status', type: 'string' as const },
        { key: 'Notes', label: 'Notes', type: 'string' as const },
      ]}
    />
  );
}
