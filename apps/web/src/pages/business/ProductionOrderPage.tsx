import { ResourcePage } from './ResourcePage';

export function ProductionOrderPage() {
  return (
    <ResourcePage
      title="ProductionOrder"
      description="TASK-056 · Production MVP"
      route="production-orders"
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
