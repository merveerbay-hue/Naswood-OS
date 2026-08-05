import { ResourcePage } from './ResourcePage';

export function WorkCenterPage() {
  return (
    <ResourcePage
      title="WorkCenter"
      description="TASK-049 · Production MVP"
      route="work-centers"
      kind="master"
      fields={[
    { key: 'Code', label: 'Code', type: 'string' as const },
    { key: 'Name', label: 'Name', type: 'string' as const },
    { key: 'CapacityPerHour', label: 'CapacityPerHour', type: 'number' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
