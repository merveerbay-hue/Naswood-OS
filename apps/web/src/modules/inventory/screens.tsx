import { useParams } from '@tanstack/react-router';
import { Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { EntityDetailScreen } from './components/EntityDetailScreen';
import { EntityListScreen, type EntityField } from './components/EntityListScreen';

const materialFields: EntityField[] = [
  { key: 'Code', label: 'Code' },
  { key: 'Name', label: 'Name' },
  { key: 'Description', label: 'Description' },
  { key: 'Category', label: 'Category' },
  { key: 'UnitOfMeasure', label: 'UoM' },
  { key: 'Status', label: 'Status', status: true },
];

const warehouseFields: EntityField[] = [
  { key: 'Code', label: 'Code' },
  { key: 'Name', label: 'Name' },
  { key: 'WarehouseType', label: 'Type' },
  { key: 'Status', label: 'Status', status: true },
];

const locationFields: EntityField[] = [
  { key: 'WarehouseCode', label: 'Warehouse' },
  { key: 'Code', label: 'Code' },
  { key: 'Name', label: 'Name' },
  { key: 'LocationType', label: 'Type' },
  { key: 'Status', label: 'Status', status: true },
];

const balanceFields: EntityField[] = [
  { key: 'MaterialCode', label: 'Material' },
  { key: 'WarehouseCode', label: 'Warehouse' },
  { key: 'LocationCode', label: 'Location' },
  { key: 'BatchNumber', label: 'Lot' },
  { key: 'QuantityOnHand', label: 'On hand', type: 'number' },
  { key: 'QuantityReserved', label: 'Reserved', type: 'number' },
  { key: 'Status', label: 'Status', status: true },
];

const batchFields: EntityField[] = [
  { key: 'BatchNumber', label: 'Lot' },
  { key: 'MaterialCode', label: 'Material' },
  { key: 'Quantity', label: 'Qty', type: 'number' },
  { key: 'ExpiryDate', label: 'Expiry', type: 'date' },
  { key: 'Status', label: 'Status', status: true },
];

const receiptFields: EntityField[] = [
  { key: 'Number', label: 'Number' },
  { key: 'WarehouseCode', label: 'Warehouse' },
  { key: 'Reference', label: 'Reference' },
  { key: 'Status', label: 'Status', status: true },
  { key: 'Notes', label: 'Notes' },
];

const issueFields: EntityField[] = [
  { key: 'Number', label: 'Number' },
  { key: 'WarehouseCode', label: 'Warehouse' },
  { key: 'Reference', label: 'Reference' },
  { key: 'Status', label: 'Status', status: true },
  { key: 'Notes', label: 'Notes' },
];

const transferFields: EntityField[] = [
  { key: 'Number', label: 'Number' },
  { key: 'FromWarehouseCode', label: 'From WH' },
  { key: 'ToWarehouseCode', label: 'To WH' },
  { key: 'Status', label: 'Status', status: true },
  { key: 'Notes', label: 'Notes' },
];

const countFields: EntityField[] = [
  { key: 'Number', label: 'Number' },
  { key: 'WarehouseCode', label: 'Warehouse' },
  { key: 'Status', label: 'Status', status: true },
  { key: 'Notes', label: 'Notes' },
];

const adjustmentFields: EntityField[] = [
  { key: 'Number', label: 'Number' },
  { key: 'WarehouseCode', label: 'Warehouse' },
  { key: 'Reason', label: 'Reason' },
  { key: 'Status', label: 'Status', status: true },
  { key: 'Notes', label: 'Notes' },
];

export function MaterialListPage() {
  return (
    <EntityListScreen
      screenId="INV-004"
      title="Materials"
      description="Master data · material library for warehouse and planning."
      route="materials"
      fields={materialFields}
      detailPath={(id) => `/inventory/master-data/materials/${id}`}
      createLabel="New material"
    />
  );
}

export function MaterialDetailPage() {
  const { id } = useParams({ strict: false }) as { id: string };
  return (
    <EntityDetailScreen
      screenId="INV-005"
      title="Material Detail"
      route="materials"
      id={id}
      listPath="/inventory/master-data/materials"
      fields={materialFields}
    />
  );
}

export function WarehouseListPage() {
  return (
    <EntityListScreen
      screenId="INV-006"
      title="Warehouses"
      description="Master data · warehouse directory."
      route="warehouses"
      fields={warehouseFields}
      detailPath={(id) => `/inventory/master-data/warehouses/${id}`}
      createLabel="New warehouse"
    />
  );
}

export function WarehouseDetailPage() {
  const { id } = useParams({ strict: false }) as { id: string };
  return (
    <EntityDetailScreen
      screenId="INV-007"
      title="Warehouse Detail"
      route="warehouses"
      id={id}
      listPath="/inventory/master-data/warehouses"
      fields={warehouseFields}
    />
  );
}

export function LocationListPage() {
  return (
    <EntityListScreen
      screenId="INV-008"
      title="Locations"
      description="Master data · bins and zones."
      route="locations"
      fields={locationFields}
      createLabel="New location"
    />
  );
}

export function StockBalancePage() {
  return (
    <EntityListScreen
      screenId="INV-014"
      title="Stock Balance Inquiry"
      description="On-hand, reserved, and available by material / location / lot."
      route="inventory"
      fields={balanceFields}
      createLabel="Add balance row"
    />
  );
}

export function LotListPage() {
  return (
    <EntityListScreen
      screenId="INV-010"
      title="Lots"
      description="Batch / lot directory with status signals."
      route="batches"
      fields={batchFields}
      createLabel="New lot"
    />
  );
}

export function GoodsReceiptListPage() {
  return (
    <EntityListScreen
      screenId="INV-015"
      title="Goods Receipts"
      description="Inbound documents — open Detail to post."
      route="goods-receipts"
      fields={receiptFields}
      detailPath={(id) => `/inventory/operations/goods-receipts/${id}`}
      createLabel="New receipt"
    />
  );
}

export function GoodsReceiptDetailPage() {
  const { id } = useParams({ strict: false }) as { id: string };
  return (
    <EntityDetailScreen
      screenId="INV-016"
      title="Goods Receipt Detail"
      route="goods-receipts"
      id={id}
      listPath="/inventory/operations/goods-receipts"
      fields={receiptFields}
    />
  );
}

export function GoodsIssueListPage() {
  return (
    <EntityListScreen
      screenId="INV-017"
      title="Goods Issues"
      description="Outbound issue documents."
      route="goods-issues"
      fields={issueFields}
      createLabel="New issue"
    />
  );
}

export function TransferListPage() {
  return (
    <EntityListScreen
      screenId="INV-019"
      title="Transfers"
      description="Inter-location / inter-warehouse moves."
      route="transfers"
      fields={transferFields}
      createLabel="New transfer"
    />
  );
}

export function CycleCountListPage() {
  return (
    <EntityListScreen
      screenId="INV-021"
      title="Cycle Counts"
      description="Count sessions and follow-up."
      route="inventory-counts"
      fields={countFields}
      createLabel="New count"
    />
  );
}

export function AdjustmentListPage() {
  return (
    <EntityListScreen
      screenId="INV-024"
      title="Inventory Adjustments"
      description="Approved variance documents — posting is permission gated."
      route="inventory-adjustments"
      fields={adjustmentFields}
      createLabel="New adjustment"
    />
  );
}

export function InventoryReportsPage() {
  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">INV-025</p>
        <h2 className="text-xl font-semibold tracking-tight">Inventory Reports</h2>
        <p className="mt-1 text-sm text-[var(--text-secondary)]">Launcher for operational inventory reports.</p>
      </div>
      <div className="grid gap-4 md:grid-cols-3">
        {[
          ['Stock by warehouse', 'On-hand and available grouped by warehouse'],
          ['Open movements', 'Draft receipts, issues, transfers'],
          ['Count accuracy', 'Cycle count variance summary'],
        ].map(([title, body]) => (
          <Card key={title}>
            <CardHeader>
              <CardTitle className="text-base">{title}</CardTitle>
            </CardHeader>
            <CardContent className="text-sm text-[var(--text-secondary)]">{body}</CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}
