import { apiRequest } from '@/api/client';
import { ApiClientError } from '@/api/types';
import type { InventoryCountSession } from './cycleCountSession';

const GUID = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

export function isCountId(id: string | undefined | null): id is string {
  return Boolean(id && GUID.test(id));
}

export function sessionId(doc: { id?: string; Id?: string } | null | undefined): string {
  return String(doc?.id ?? doc?.Id ?? '').trim();
}

function staleApiMessage(path: string, err: unknown): Error {
  if (err instanceof ApiClientError && err.status === 404) {
    return new Error(
      `Sayım API 404: ${path}. Bu önizlemenin API süreci eski olabilir — API’yi yeniden başlatın (satır kaydı POST /api/v1/inventory-counts/{id}/lines).`,
    );
  }
  return err instanceof Error ? err : new Error(String(err));
}

export async function putCountLines(id: string, lines: unknown[]) {
  if (!isCountId(id)) throw new Error('Sayım oturumu id yok — önce sayımı başlatın.');
  const path = `/api/v1/inventory-counts/${id}/lines`;
  try {
    return await apiRequest<InventoryCountSession>(path, {
      method: 'POST',
      auth: true,
      body: { lines },
    });
  } catch (err) {
    throw staleApiMessage(`POST ${path}`, err);
  }
}

export async function completeCount(id: string) {
  if (!isCountId(id)) throw new Error('Sayım oturumu id yok.');
  const path = `/api/v1/inventory-counts/${id}/complete`;
  try {
    return await apiRequest<InventoryCountSession>(path, {
      method: 'POST',
      auth: true,
      body: {},
    });
  } catch (err) {
    throw staleApiMessage(`POST ${path}`, err);
  }
}

export async function postCount(id: string, reason?: string) {
  if (!isCountId(id)) throw new Error('Sayım oturumu id yok.');
  const path = `/api/v1/inventory-counts/${id}/post`;
  try {
    return await apiRequest<{
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
      lotCount?: number;
      packageCount?: number;
      lots?: string[];
      packages?: Array<{ packageId: string; packageNo: string; barcode: string }>;
    }>(path, {
      method: 'POST',
      auth: true,
      body: { reason: reason || 'Stok sayım düzeltmesi', approveAllVariances: true },
    });
  } catch (err) {
    throw staleApiMessage(`POST ${path}`, err);
  }
}

export async function cancelCount(id: string) {
  if (!isCountId(id)) throw new Error('Sayım oturumu id yok.');
  const path = `/api/v1/inventory-counts/${id}/cancel`;
  try {
    return await apiRequest<InventoryCountSession>(path, {
      method: 'POST',
      auth: true,
      body: {},
    });
  } catch (err) {
    throw staleApiMessage(`POST ${path}`, err);
  }
}
