import { ResourcePage } from './ResourcePage';

export function ShiftPage() {
  return (
    <ResourcePage
      title="Shift"
      description="TASK-051 · Production MVP"
      route="shifts"
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
