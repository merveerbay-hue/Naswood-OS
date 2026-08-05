import { apiRequest } from './client';

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export async function searchResource<T>(route: string, q?: string): Promise<PagedResult<T>> {
  const params = new URLSearchParams({ page: '1', pageSize: '50' });
  if (q) params.set('q', q);
  return apiRequest<PagedResult<T>>(`/api/v1/${route}?${params}`, { method: 'GET', auth: true });
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
