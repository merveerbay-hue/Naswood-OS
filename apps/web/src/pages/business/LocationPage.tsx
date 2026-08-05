import { ResourcePage } from './ResourcePage';

export function LocationPage() {
  return (
    <ResourcePage
      title="Location"
      description="TASK-018 · Inventory MVP"
      route="locations"
      kind="master"
      fields={[
    { key: 'Code', label: 'Code', type: 'string' as const },
    { key: 'Name', label: 'Name', type: 'string' as const },
    { key: 'WarehouseCode', label: 'WarehouseCode', type: 'string' as const },
    { key: 'LocationType', label: 'LocationType', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
