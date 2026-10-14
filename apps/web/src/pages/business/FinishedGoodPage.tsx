import { ResourcePage } from './ResourcePage';

export function FinishedGoodPage() {
  return (
    <ResourcePage
      title="FinishedGood"
      description="TASK-062 · Production MVP"
      route="finished-goods"
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
