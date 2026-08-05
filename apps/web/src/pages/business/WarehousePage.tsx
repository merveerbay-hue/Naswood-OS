import { ResourcePage } from './ResourcePage';

export function WarehousePage() {
  return (
    <ResourcePage
      title="Warehouse"
      description="TASK-017 · Inventory MVP"
      route="warehouses"
      kind="master"
      fields={[
    { key: 'Code', label: 'Code', type: 'string' as const },
    { key: 'Name', label: 'Name', type: 'string' as const },
    { key: 'WarehouseType', label: 'WarehouseType', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
