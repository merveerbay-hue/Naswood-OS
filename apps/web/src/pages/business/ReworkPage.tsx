import { ResourcePage } from './ResourcePage';

export function ReworkPage() {
  return (
    <ResourcePage
      title="Rework"
      description="TASK-064 · Production MVP"
      route="reworks"
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
