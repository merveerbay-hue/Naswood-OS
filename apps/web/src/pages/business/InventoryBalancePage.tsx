import { ResourcePage } from './ResourcePage';

export function InventoryBalancePage() {
  return (
    <ResourcePage
      title="Inventory"
      description="TASK-019 · Inventory MVP"
      route="inventory"
      kind="master"
      fields={[
    { key: 'MaterialCode', label: 'MaterialCode', type: 'string' as const },
    { key: 'WarehouseCode', label: 'WarehouseCode', type: 'string' as const },
    { key: 'LocationCode', label: 'LocationCode', type: 'string' as const },
    { key: 'BatchNumber', label: 'BatchNumber', type: 'string' as const },
    { key: 'QuantityOnHand', label: 'QuantityOnHand', type: 'number' as const },
    { key: 'QuantityReserved', label: 'QuantityReserved', type: 'number' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
