import { ResourcePage } from './ResourcePage';

export function OperationPage() {
  return (
    <ResourcePage
      title="Operation"
      description="TASK-054 · Production MVP"
      route="operations"
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
