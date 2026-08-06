import type { WorkspaceDefinition } from '@/components/workspace/WorkspaceShell';

/** Üretim çalışma alanları — Türkçe arayüz */
export const productionWorkspaces: WorkspaceDefinition[] = [
  {
    id: 'dashboard',
    label: 'Gösterge Paneli',
    items: [{ id: 'prd-001', label: 'Özet', path: '/production/dashboard', screenId: 'PRD-001' }],
  },
  {
    id: 'planning',
    label: 'Planlama',
    items: [
      { id: 'prd-010', label: 'Üretim Emirleri', path: '/production/planning/orders', screenId: 'PRD-010' },
      { id: 'prd-012', label: 'İş Emirleri', path: '/production/planning/work-orders', screenId: 'PRD-012' },
      { id: 'prd-021', label: 'Çizelgeleme', path: '/production/planning/scheduling', screenId: 'PRD-021' },
      { id: 'prd-022', label: 'Kapasite', path: '/production/planning/capacity', screenId: 'PRD-022' },
      { id: 'prd-023', label: 'Sevk Listesi', path: '/production/planning/dispatch', screenId: 'PRD-023' },
    ],
  },
  {
    id: 'execution',
    label: 'İcra',
    items: [
      { id: 'prd-013', label: 'Operatör Terminali', path: '/production/execution/operator-terminal', screenId: 'PRD-013' },
      { id: 'prd-024', label: 'Makine Paneli', path: '/production/execution/machine-panel', screenId: 'PRD-024' },
      { id: 'prd-014', label: 'Sarfiyat', path: '/production/execution/consumption', screenId: 'PRD-014' },
      { id: 'prd-015', label: 'Teyit', path: '/production/execution/confirmation', screenId: 'PRD-015' },
      { id: 'prd-016', label: 'Yarı Mamul (WIP)', path: '/production/execution/wip', screenId: 'PRD-016' },
      { id: 'prd-017', label: 'Paketleme', path: '/production/execution/packaging', screenId: 'PRD-017' },
      { id: 'prd-018', label: 'Mamul', path: '/production/execution/finished-goods', screenId: 'PRD-018' },
      { id: 'prd-027', label: 'Hurda', path: '/production/execution/scrap', screenId: 'PRD-027' },
      { id: 'prd-028', label: 'Yeniden İşlem', path: '/production/execution/rework', screenId: 'PRD-028' },
    ],
  },
  {
    id: 'master-data',
    label: 'Ana Veri',
    items: [
      { id: 'prd-002', label: 'Ürün Ağacı (BOM)', path: '/production/master-data/boms', screenId: 'PRD-002' },
      { id: 'prd-004', label: 'Rota', path: '/production/master-data/routings', screenId: 'PRD-004' },
      { id: 'prd-025', label: 'Operasyonlar', path: '/production/master-data/operations', screenId: 'PRD-025' },
      { id: 'prd-007', label: 'Makineler', path: '/production/master-data/machines', screenId: 'PRD-007' },
      { id: 'prd-006', label: 'İş Merkezleri', path: '/production/master-data/work-centers', screenId: 'PRD-006' },
      { id: 'prd-026', label: 'Hatlar', path: '/production/master-data/lines', screenId: 'PRD-026' },
      { id: 'prd-008', label: 'Vardiyalar', path: '/production/master-data/shifts', screenId: 'PRD-008' },
      { id: 'prd-009', label: 'Takvimler', path: '/production/master-data/calendars', screenId: 'PRD-009' },
      { id: 'prd-053', label: 'Takım', path: '/production/master-data/toolings', screenId: 'PRD-Tooling' },
    ],
  },
  {
    id: 'insights',
    label: 'Rapor & Analitik',
    items: [
      { id: 'prd-020', label: 'Raporlar', path: '/production/reports', screenId: 'PRD-020' },
      { id: 'prd-019', label: 'Analitik', path: '/production/analytics', screenId: 'PRD-019' },
      { id: 'prd-029', label: 'Ayarlar', path: '/production/settings', screenId: 'PRD-029' },
    ],
  },
];
