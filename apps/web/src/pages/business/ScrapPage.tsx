import { ResourcePage } from './ResourcePage';

export function ScrapPage() {
  return (
    <ResourcePage
      title="Scrap"
      description="TASK-063 · Production MVP"
      route="scraps"
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
