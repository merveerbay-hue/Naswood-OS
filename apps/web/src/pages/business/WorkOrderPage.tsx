import { ResourcePage } from './ResourcePage';

export function WorkOrderPage() {
  return (
    <ResourcePage
      title="WorkOrder"
      description="TASK-057 · Production MVP"
      route="work-orders"
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
