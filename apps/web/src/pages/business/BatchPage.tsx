import { ResourcePage } from './ResourcePage';

export function BatchPage() {
  return (
    <ResourcePage
      title="Batch"
      description="TASK-020 · Inventory MVP"
      route="batches"
      kind="master"
      fields={[
    { key: 'BatchNumber', label: 'BatchNumber', type: 'string' as const },
    { key: 'MaterialCode', label: 'MaterialCode', type: 'string' as const },
    { key: 'Quantity', label: 'Quantity', type: 'number' as const },
    { key: 'ExpiryDate', label: 'ExpiryDate', type: 'date' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
