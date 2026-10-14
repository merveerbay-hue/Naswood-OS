import { ResourcePage } from './ResourcePage';

export function ShipmentPage() {
  return (
    <ResourcePage
      title="Shipment"
      description="TASK-041 · Sales MVP"
      route="shipments"
      kind="document"
      createLabel="Sevkiyat planla"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'SalesOrderNumber', label: 'SalesOrderNumber', type: 'string' as const },
    { key: 'WarehouseCode', label: 'WarehouseCode', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'Notes', label: 'Notes', type: 'string' as const }
      ]}
    />
  );
}
