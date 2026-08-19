/**
 * Client-side gates mirroring StructuralProductionLot create rules (EN 14081 foundation).
 * Backend remains authoritative for plant/source 403 and persistence.
 */

import type { ComplianceScope, GradingMethod } from '@/modules/inventory/materials/materialCompliance';

export type StructuralProductionLotCreateInput = {
  plantId: string;
  materialId: string;
  complianceScope?: ComplianceScope | string;
  supportedGradingMethods?: GradingMethod[] | string[];
  actualGradingMethod?: string;
  productionDate?: string;
  workCenterCode?: string;
  shiftCode?: string;
  notes?: string;
  inputs?: Array<{
    sourceBatchId?: string;
    sourceInventoryBalanceId?: string;
    sourcePackageId?: string;
    quantity: number;
    unit?: string;
    sourcePlantId?: string;
    sourceMaterialCode?: string;
    lotMaterialCode?: string;
  }>;
};

export type CreateGate =
  | { ok: true }
  | { ok: false; code: string; message: string; httpStatus?: number };

export function canCreateStructuralProductionLot(input: StructuralProductionLotCreateInput): CreateGate {
  if (!String(input.plantId || '').trim()) {
    return { ok: false, code: 'PRD-SPL-004', message: 'Plant / factory context is required.', httpStatus: 400 };
  }
  if (!String(input.materialId || '').trim()) {
    return { ok: false, code: 'BUS-001', message: 'Material is required.', httpStatus: 400 };
  }

  const scope = String(input.complianceScope || 'NORMAL_STOCK').toUpperCase();
  if (scope !== 'STRUCTURAL_TIMBER') {
    return {
      ok: false,
      code: 'PRD-SPL-SCOPE',
      message: 'Production Lot yalnızca STRUCTURAL_TIMBER kapsamındaki malzemeler için oluşturulabilir.',
      httpStatus: 400,
    };
  }

  const method = String(input.actualGradingMethod || '').trim().toUpperCase();
  if (method !== 'VISUAL' && method !== 'MACHINE') {
    return {
      ok: false,
      code: 'PRD-SPL-METHOD',
      message: 'ActualGradingMethod must be VISUAL or MACHINE.',
      httpStatus: 400,
    };
  }

  const supported = (input.supportedGradingMethods || []).map((m) => String(m).toUpperCase());
  if (!supported.includes(method)) {
    return {
      ok: false,
      code: 'PRD-SPL-METHOD',
      message: `Malzeme bu yöntemi desteklemiyor: ${method}. Desteklenen: ${supported.join(', ') || '—'}.`,
      httpStatus: 400,
    };
  }

  for (const line of input.inputs || []) {
    if (!line.sourceBatchId && !line.sourceInventoryBalanceId && !line.sourcePackageId) {
      return {
        ok: false,
        code: 'PRD-SPL-011',
        message: 'Each source line needs Batch, Balance, or Package id.',
        httpStatus: 400,
      };
    }
    if (!(line.quantity > 0)) {
      return { ok: false, code: 'PRD-SPL-012', message: 'Source quantity must be positive.', httpStatus: 400 };
    }
    if (
      line.sourcePlantId &&
      input.plantId &&
      line.sourcePlantId.trim().toUpperCase() !== input.plantId.trim().toUpperCase()
    ) {
      return {
        ok: false,
        code: 'PRD-SPL-403',
        message: 'Kaynak kayıt başka tesise ait.',
        httpStatus: 403,
      };
    }
    if (
      line.sourceMaterialCode &&
      line.lotMaterialCode &&
      line.sourceMaterialCode.trim().toUpperCase() !== line.lotMaterialCode.trim().toUpperCase()
    ) {
      return {
        ok: false,
        code: 'PRD-SPL-014',
        message: 'Kaynak malzeme kodu Production Lot malzemesi ile uyuşmuyor.',
        httpStatus: 400,
      };
    }
  }

  return { ok: true };
}

export function canEditCriticalFields(status: string): boolean {
  return String(status || '').toUpperCase() === 'DRAFT';
}

export function mintProductionLotNumberPreview(plantCode: string, sequence: number): string {
  const code = String(plantCode || '').trim().toUpperCase() || 'PLANT';
  return `PLOT-${code}-${String(sequence).padStart(6, '0')}`;
}

export const STRUCTURAL_PRODUCTION_LOT_STATUSES = [
  'DRAFT',
  'IN_PROGRESS',
  'PENDING_CLASSIFICATION',
  'PENDING_QUALITY',
  'RELEASED',
  'QUARANTINED',
  'CANCELLED',
] as const;
