import type { WorkspaceDefinition } from '@/components/workspace/WorkspaceShell';

/** Kalite çalışma alanları — Quality Foundation / Screens */
export const qualityWorkspaces: WorkspaceDefinition[] = [
  {
    id: 'dashboard',
    label: 'Gösterge Paneli',
    items: [{ id: 'qlt-001', label: 'Komuta', path: '/quality/dashboard', screenId: 'QLT-001' }],
  },
  {
    id: 'operations',
    label: 'Operasyonlar',
    items: [
      { id: 'qlt-insp', label: 'Muayene başlat', path: '/quality/operations/inspect', screenId: 'QLT-INSP-001' },
      { id: 'qlt-hold', label: 'Hold masası', path: '/quality/operations/hold-desk', screenId: 'QLT-HOLD' },
      { id: 'qlt-ncr', label: 'NCR aç', path: '/quality/operations/ncr', screenId: 'QLT-NCR-001' },
      { id: 'qlt-ncr-lib', label: 'NCR kayıtları', path: '/quality/operations/ncrs', screenId: 'QLT-NCR-LIB' },
      { id: 'qlt-capa', label: 'CAPA aç', path: '/quality/operations/capa', screenId: 'QLT-CAPA-001' },
    ],
  },
  {
    id: 'laboratory',
    label: 'Laboratuvar',
    items: [{ id: 'qlt-moisture', label: 'Nem / Lab', path: '/quality/laboratory/moisture', screenId: 'QLT-LAB' }],
  },
  {
    id: 'compliance',
    label: 'Uyumluluk',
    items: [
      { id: 'qlt-trace', label: 'İzlenebilirlik', path: '/quality/compliance/traceability', screenId: 'QLT-TRACE' },
      { id: 'qlt-coc', label: 'CoC / FSC-PEFC', path: '/quality/compliance/coc', screenId: 'QLT-COC' },
      { id: 'qlt-cert', label: 'Sertifikalar', path: '/quality/compliance/certificates', screenId: 'QLT-CERT' },
    ],
  },
  {
    id: 'plans',
    label: 'Plan & Spek',
    items: [
      { id: 'qlt-plan', label: 'Muayene planları', path: '/quality/plans/inspection-plans', screenId: 'QLT-PLAN' },
      { id: 'qlt-spec', label: 'Spek / AQL', path: '/quality/plans/specs', screenId: 'QLT-SPEC' },
    ],
  },
  {
    id: 'insights',
    label: 'Rapor & Analitik',
    items: [
      { id: 'qlt-rpt', label: 'Raporlar', path: '/quality/reports', screenId: 'QLT-RPT' },
      { id: 'qlt-set', label: 'Ayarlar', path: '/quality/settings', screenId: 'QLT-SET' },
    ],
  },
];
