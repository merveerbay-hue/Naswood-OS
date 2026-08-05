import { ResourcePage } from './ResourcePage';

export function PackagingPage() {
  return (
    <ResourcePage
      title="Packaging"
      description="TASK-061 · Production MVP"
      route="packagings"
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
