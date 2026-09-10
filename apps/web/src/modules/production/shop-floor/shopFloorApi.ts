import { apiRequest } from '@/api/client';

export type ShopFloorWorkCenter = {
  id: string;
  code: string;
  name: string;
  pendingCount: number;
  runningCount: number;
  completedTodayCount: number;
};

export type ShopFloorQueueItem = {
  productionOrderId: string;
  productionOrderNumber: string;
  productionOrderName: string;
  productionOperationId: string;
  sequence: number;
  operationName: string;
  status: string;
  executionId?: string | null;
  executionNumber: string;
};

export type ShopFloorQueue = {
  workCenterId: string;
  workCenterCode: string;
  workCenterName: string;
  pending: ShopFloorQueueItem[];
  running: ShopFloorQueueItem[];
  completedToday: ShopFloorQueueItem[];
};

export type ProductionOrderProgress = {
  id: string;
  number: string;
  name: string;
  status: string;
  plantId: string;
  completedOperations: number;
  totalOperations: number;
  operations: {
    id: string;
    sequence: number;
    operationName: string;
    workCenterCode: string;
    status: string;
    canStart: boolean;
    activeExecutionId?: string | null;
    outputType: string;
  }[];
};

export type ExecutionPassport = {
  id: string;
  number: string;
  status: string;
  productionOrderId: string;
  productionOrderNumber: string;
  productionOrderName: string;
  operationName: string;
  workCenterCode: string;
  workCenterName: string;
  startedByUserId: string;
  totalRunMinutes: number;
  totalPauseMinutes: number;
  totalDowntimeMinutes: number;
  inputQuantity: number;
  outputQuantity: number;
  scrapQuantity: number;
  unit: string;
  outputType: string;
  productionLotNumber: string;
  qcStatus: string;
  structuralProductionLotNumber: string;
  allowedActions: string[];
  events: { eventType: string; occurredAt: string; reasonCode: string; note: string; userId: string }[];
  inputs: {
    id: string;
    barcode: string;
    packageNumber: string;
    materialCode: string;
    lotNumber: string;
    physicalMeasure: string;
    consumedQuantity: number;
    remainingPackageQuantity: number;
    unit: string;
  }[];
  scraps: { id: string; quantity: number; unit: string; reasonCode: string; note: string; userId: string; recordedAt: string }[];
  packages: { packageNo: string; barcode: string; quantity: number; unit: string }[];
};

export type ExecutionScan = {
  packageId: string;
  packageNo: string;
  barcode: string;
  materialCode: string;
  materialName: string;
  sourceLotNumber: string;
  warehouseCode: string;
  locationCode: string;
  availableQuantity: number;
  unit: string;
  status: string;
  inactiveReason?: string | null;
  canConsume: boolean;
  existingConsumptionId?: string | null;
  contents: { id: string; measurement: string; quantity: number; pieceCount?: number | null; unit: string }[];
};

export function listShopFloorWorkCenters() {
  return apiRequest<ShopFloorWorkCenter[]>('/api/v1/production-execution/work-centers', { auth: true });
}

export function getShopFloorQueue(id: string) {
  return apiRequest<ShopFloorQueue>(`/api/v1/production-execution/work-centers/${id}/queue`, { auth: true });
}

export function getOrderProgress(id: string) {
  return apiRequest<ProductionOrderProgress>(`/api/v1/production-execution/orders/${id}`, { auth: true });
}

export function startOperation(operationId: string, workCenterId?: string) {
  return apiRequest<ExecutionPassport>(`/api/v1/production-execution/operations/${operationId}/start`, {
    method: 'POST',
    auth: true,
    body: { workCenterId },
  });
}

export function getExecution(id: string) {
  return apiRequest<ExecutionPassport>(`/api/v1/production-execution/executions/${id}`, { auth: true });
}

export function pauseExecution(id: string) {
  return apiRequest<ExecutionPassport>(`/api/v1/production-execution/executions/${id}/pause`, { method: 'POST', auth: true });
}

export function resumeExecution(id: string) {
  return apiRequest<ExecutionPassport>(`/api/v1/production-execution/executions/${id}/resume`, { method: 'POST', auth: true });
}

export function scanExecution(id: string, barcode: string) {
  return apiRequest<ExecutionScan>(`/api/v1/production-execution/executions/${id}/scan/${encodeURIComponent(barcode)}`, { auth: true });
}

export function consumeExecution(id: string, body: { packageId?: string; barcode: string; quantity: number; packageContentId?: string; pieceCount?: number; idempotencyKey?: string }) {
  return apiRequest<ExecutionPassport>(`/api/v1/production-execution/executions/${id}/consume`, {
    method: 'POST',
    auth: true,
    body,
  });
}

export function startDowntime(id: string, reasonCode: string, note = '') {
  return apiRequest<ExecutionPassport>(`/api/v1/production-execution/executions/${id}/downtime/start`, {
    method: 'POST',
    auth: true,
    body: { reasonCode, note },
  });
}

export function endDowntime(id: string) {
  return apiRequest<ExecutionPassport>(`/api/v1/production-execution/executions/${id}/downtime/end`, {
    method: 'POST',
    auth: true,
    body: { reasonCode: 'OTHER' },
  });
}

export function addScrap(id: string, quantity: number, reasonCode: string, unit: string, note = '') {
  return apiRequest<ExecutionPassport>(`/api/v1/production-execution/executions/${id}/scrap`, {
    method: 'POST',
    auth: true,
    body: { quantity, reasonCode, unit, note },
  });
}

export function completeExecution(id: string, body: {
  outputMaterialId?: string;
  warehouseCode?: string;
  locationCode?: string;
  structuralProductionLotId?: string;
  lines?: { physicalGroupLabel?: string; thicknessMm?: number; widthMm?: number; lengthMm?: number; pieceCount?: number }[];
}) {
  return apiRequest<ExecutionPassport>(`/api/v1/production-execution/executions/${id}/complete`, {
    method: 'POST',
    auth: true,
    body,
  });
}
