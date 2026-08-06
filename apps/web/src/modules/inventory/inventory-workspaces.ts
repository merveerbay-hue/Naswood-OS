import type { WorkspaceDefinition } from '@/components/workspace/WorkspaceShell';

/** Envanter çalışma alanları — Türkçe arayüz */
export const inventoryWorkspaces: WorkspaceDefinition[] = [
  {
    id: 'dashboard',
    label: 'Gösterge Paneli',
    items: [{ id: 'inv-001', label: 'Özet', path: '/inventory/dashboard', screenId: 'INV-001' }],
  },
  {
    id: 'operations',
    label: 'Operasyonlar',
    items: [
      { id: 'inv-015', label: 'Mal Kabul', path: '/inventory/operations/goods-receipts', screenId: 'INV-015' },
      { id: 'inv-iss-001', label: 'Mal çıkışı başlat', path: '/inventory/operations/issue', screenId: 'INV-ISS-001' },
      { id: 'inv-017', label: 'Çıkış kayıtları', path: '/inventory/operations/goods-issues', screenId: 'INV-017' },
      { id: 'inv-019', label: 'Transfer', path: '/inventory/operations/transfers', screenId: 'INV-019' },
    ],
  },
  {
    id: 'stock',
    label: 'Stok',
    items: [
      { id: 'inv-014', label: 'Stok Bakiyesi', path: '/inventory/stock/balances', screenId: 'INV-014' },
      { id: 'inv-010', label: 'Lotlar', path: '/inventory/stock/lots', screenId: 'INV-010' },
    ],
  },
  {
    id: 'counts',
    label: 'Sayım & Düzeltme',
    items: [
      { id: 'inv-021', label: 'Çevrim Sayımı', path: '/inventory/counts/cycle-counts', screenId: 'INV-021' },
      { id: 'inv-024', label: 'Düzeltme', path: '/inventory/counts/adjustments', screenId: 'INV-024' },
    ],
  },
  {
    id: 'master-data',
    label: 'Ana Veri',
    items: [
      { id: 'inv-004', label: 'Malzemeler', path: '/inventory/master-data/materials', screenId: 'INV-004' },
      { id: 'inv-006', label: 'Depolar', path: '/inventory/master-data/warehouses', screenId: 'INV-006' },
      { id: 'inv-008', label: 'Lokasyonlar', path: '/inventory/master-data/locations', screenId: 'INV-008' },
    ],
  },
  {
    id: 'reports',
    label: 'Raporlar',
    items: [{ id: 'inv-025', label: 'Raporlar', path: '/inventory/reports', screenId: 'INV-025' }],
  },
];
