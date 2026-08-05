import { ResourcePage } from './ResourcePage';

export function MaterialPage() {
  return (
    <ResourcePage
      title="Material"
      description="TASK-016 · Inventory MVP"
      route="materials"
      kind="master"
      fields={[
    { key: 'Code', label: 'Code', type: 'string' as const },
    { key: 'Name', label: 'Name', type: 'string' as const },
    { key: 'Description', label: 'Description', type: 'string' as const },
    { key: 'Category', label: 'Category', type: 'string' as const },
    { key: 'UnitOfMeasure', label: 'UnitOfMeasure', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
