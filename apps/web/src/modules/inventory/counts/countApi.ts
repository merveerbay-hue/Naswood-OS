import { apiRequest } from '@/api/client';
import type { InventoryCountSession } from './cycleCountSession';

export async function putCountLines(id: string, lines: unknown[]) {
  return apiRequest<InventoryCountSession>(`/api/v1/inventory-counts/${id}/lines`, {
    method: 'PUT',
    auth: true,
    body: { lines },
  });
}

export async function completeCount(id: string) {
  return apiRequest<InventoryCountSession>(`/api/v1/inventory-counts/${id}/complete`, {
    method: 'POST',
    auth: true,
    body: {},
  });
}

export async function postCount(id: string, reason?: string) {
  return apiRequest<{
    countId: string;
    countNumber: string;
    status: string;
    adjustmentCount: number;
    adjustments: Array<{
      materialCode: string;
      difference: number;
      movementNumber: string;
      oldQuantity: number;
      countedQuantity: number;
      unit: string;
    }>;
  }>(`/api/v1/inventory-counts/${id}/post`, {
    method: 'POST',
    auth: true,
    body: { reason: reason || 'Stok sayım düzeltmesi', approveAllVariances: true },
  });
}

export async function cancelCount(id: string) {
  return apiRequest<InventoryCountSession>(`/api/v1/inventory-counts/${id}/cancel`, {
    method: 'POST',
    auth: true,
    body: {},
  });
}
