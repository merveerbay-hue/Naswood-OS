import { ResourcePage } from './ResourcePage';

export function MachinePage() {
  return (
    <ResourcePage
      title="Machine"
      description="TASK-048 · Production MVP"
      route="machines"
      kind="master"
      fields={[
    { key: 'Code', label: 'Code', type: 'string' as const },
    { key: 'Name', label: 'Name', type: 'string' as const },
    { key: 'WorkCenterCode', label: 'WorkCenterCode', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'OeeTarget', label: 'OeeTarget', type: 'number' as const }
      ]}
    />
  );
}
