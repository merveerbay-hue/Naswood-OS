import { ResourcePage } from './ResourcePage';

export function StockTransferPage() {
  return (
    <ResourcePage
      title="StockTransfer"
      description="TASK-023 · Inventory MVP"
      route="transfers"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'FromWarehouseCode', label: 'FromWarehouseCode', type: 'string' as const },
    { key: 'ToWarehouseCode', label: 'ToWarehouseCode', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'Notes', label: 'Notes', type: 'string' as const }
      ]}
    />
  );
}
