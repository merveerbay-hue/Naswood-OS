import { ResourcePage } from './ResourcePage';

export function ToolingPage() {
  return (
    <ResourcePage
      title="Tooling"
      description="TASK-053 · Production MVP"
      route="toolings"
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
