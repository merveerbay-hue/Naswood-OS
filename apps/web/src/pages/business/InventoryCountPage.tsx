import { ResourcePage } from './ResourcePage';

export function InventoryCountPage() {
  return (
    <ResourcePage
      title="InventoryCount"
      description="TASK-024 · Inventory MVP"
      route="inventory-counts"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'WarehouseCode', label: 'WarehouseCode', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'Notes', label: 'Notes', type: 'string' as const }
      ]}
    />
  );
}
