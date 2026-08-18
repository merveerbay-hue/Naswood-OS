import { apiRequest } from './client';

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export async function searchResource<T>(
  route: string,
  q?: string,
  opts?: { page?: number; pageSize?: number; plantId?: string; warehouseCode?: string; locationType?: string },
): Promise<PagedResult<T>> {
  const params = new URLSearchParams({
    page: String(opts?.page ?? 1),
    pageSize: String(opts?.pageSize ?? 50),
  });
  if (q) params.set('q', q);
  if (opts?.plantId) params.set('plantId', opts.plantId);
  if (opts?.warehouseCode) params.set('warehouseCode', opts.warehouseCode);
  if (opts?.locationType) params.set('locationType', opts.locationType);
  return apiRequest<PagedResult<T>>(`/api/v1/${route}?${params}`, { method: 'GET', auth: true });
}

/** Page through all results (API caps pageSize at 100). */
export async function searchAllResource<T>(
  route: string,
  q?: string,
  opts?: { plantId?: string },
): Promise<T[]> {
  const pageSize = 100;
  const first = await searchResource<T>(route, q, { page: 1, pageSize, plantId: opts?.plantId });
  const items = [...(first.items ?? [])];
  const totalPages = Math.max(1, first.totalPages || 1);
  for (let page = 2; page <= totalPages; page += 1) {
    const next = await searchResource<T>(route, q, { page, pageSize, plantId: opts?.plantId });
    items.push(...(next.items ?? []));
  }
  return items;
}

export async function createResource<T>(route: string, body: unknown): Promise<T> {
  return apiRequest<T>(`/api/v1/${route}`, { method: 'POST', auth: true, body });
}

export async function deleteResource(route: string, id: string): Promise<null> {
  return apiRequest<null>(`/api/v1/${route}/${id}`, { method: 'DELETE', auth: true });
}

export async function getResource<T>(route: string, id: string): Promise<T> {
  return apiRequest<T>(`/api/v1/${route}/${id}`, { method: 'GET', auth: true });
}

export async function getDashboard<T>(route: string): Promise<T> {
  return apiRequest<T>(`/api/v1/${route}`, { method: 'GET', auth: true });
}

export async function executeStockDocument<T>(
  route: 'goods-receipts/execute' | 'goods-issues/execute' | 'transfers/execute',
  body: unknown,
): Promise<T> {
  return apiRequest<T>(`/api/v1/${route}`, { method: 'POST', auth: true, body });
}
