import { ResourcePage } from './ResourcePage';

export function DeliveryPage() {
  return (
    <ResourcePage
      title="Delivery"
      description="TASK-042 · Sales MVP"
      route="deliveries"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'ShipmentNumber', label: 'ShipmentNumber', type: 'string' as const },
    { key: 'CustomerCode', label: 'CustomerCode', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'Notes', label: 'Notes', type: 'string' as const }
      ]}
    />
  );
}
