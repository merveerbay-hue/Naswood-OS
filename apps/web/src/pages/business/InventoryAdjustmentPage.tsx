import { ResourcePage } from './ResourcePage';

export function InventoryAdjustmentPage() {
  return (
    <ResourcePage
      title="InventoryAdjustment"
      description="TASK-025 · Inventory MVP"
      route="inventory-adjustments"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'WarehouseCode', label: 'WarehouseCode', type: 'string' as const },
    { key: 'Reason', label: 'Reason', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'Notes', label: 'Notes', type: 'string' as const }
      ]}
    />
  );
}
