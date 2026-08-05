import { ResourcePage } from './ResourcePage';

export function PurchaseGoodsReceiptPage() {
  return (
    <ResourcePage
      title="PurchaseGoodsReceipt"
      description="TASK-031 · Purchasing MVP"
      route="purchase-goods-receipts"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'PurchaseOrderNumber', label: 'PurchaseOrderNumber', type: 'string' as const },
    { key: 'WarehouseCode', label: 'WarehouseCode', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'Notes', label: 'Notes', type: 'string' as const }
      ]}
    />
  );
}
