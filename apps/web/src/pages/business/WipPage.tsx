import { ResourcePage } from './ResourcePage';

export function WipPage() {
  return (
    <ResourcePage
      title="Wip"
      description="TASK-060 · Production MVP"
      route="wips"
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
