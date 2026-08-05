import { ResourcePage } from './ResourcePage';

export function GoodsReceiptPage() {
  return (
    <ResourcePage
      title="GoodsReceipt"
      description="TASK-021 · Inventory MVP"
      route="goods-receipts"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'WarehouseCode', label: 'WarehouseCode', type: 'string' as const },
    { key: 'Reference', label: 'Reference', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'Notes', label: 'Notes', type: 'string' as const }
      ]}
    />
  );
}
