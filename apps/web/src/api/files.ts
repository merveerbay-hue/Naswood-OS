import { apiRequest } from './client';
import { ApiClientError, type ApiFailure, type ApiResponse } from './types';
import { getAccessToken, clearSession, getRefreshToken, updateTokens } from '@/auth/session';
import type { AuthenticationResult } from './types';

export interface StoredFileDto {
  id: string;
  number: string;
  name: string;
  originalName: string;
  extension: string;
  contentType: string;
  sizeBytes: number;
  checksum: string | null;
  category: string;
  module: string;
  relatedEntityType: string | null;
  relatedEntityId: string | null;
  companyId: string;
  plantId: string | null;
  version: number;
  isCurrentVersion: boolean;
  parentFileId: string | null;
  status: string;
  storageKey: string;
  previewAvailable: boolean;
  tags: string[];
  uploadedAt: string;
  uploadedBy: string | null;
}

export interface PagedFilesDto {
  items: StoredFileDto[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

async function parseJson<T>(response: Response): Promise<ApiResponse<T> | null> {
  const text = await response.text();
  if (!text) return null;
  return JSON.parse(text) as ApiResponse<T>;
}

async function refreshAccessToken(): Promise<boolean> {
  const refreshToken = getRefreshToken();
  if (!refreshToken) return false;
  const response = await fetch('/api/v1/auth/refresh', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
    body: JSON.stringify({ refreshToken }),
  });
  const payload = await parseJson<AuthenticationResult>(response);
  if (!response.ok || !payload || !payload.success) {
    clearSession();
    return false;
  }
  updateTokens({
    accessToken: payload.data.accessToken,
    refreshToken: payload.data.refreshToken,
  });
  return true;
}

export async function searchFiles(params?: {
  name?: string;
  page?: number;
  pageSize?: number;
}): Promise<PagedFilesDto> {
  const query = new URLSearchParams();
  if (params?.name) query.set('name', params.name);
  query.set('page', String(params?.page ?? 1));
  query.set('pageSize', String(params?.pageSize ?? 20));
  query.set('currentOnly', 'true');
  return apiRequest<PagedFilesDto>(`/api/v1/files/search?${query.toString()}`, {
    method: 'GET',
    auth: true,
  });
}

export async function uploadFile(file: File, meta?: { module?: string; category?: string; tags?: string }) {
  const form = new FormData();
  form.append('file', file);
  if (meta?.module) form.append('module', meta.module);
  if (meta?.category) form.append('category', meta.category);
  if (meta?.tags) form.append('tags', meta.tags);

  const headers = new Headers({ Accept: 'application/json' });
  const token = getAccessToken();
  if (token) headers.set('Authorization', `Bearer ${token}`);

  let response = await fetch('/api/v1/files', { method: 'POST', headers, body: form });
  if (response.status === 401) {
    const refreshed = await refreshAccessToken();
    if (refreshed) {
      const retryHeaders = new Headers({ Accept: 'application/json' });
      const next = getAccessToken();
      if (next) retryHeaders.set('Authorization', `Bearer ${next}`);
      response = await fetch('/api/v1/files', { method: 'POST', headers: retryHeaders, body: form });
    }
  }

  const payload = await parseJson<StoredFileDto>(response);
  if (!response.ok || !payload || !payload.success) {
    throw new ApiClientError(
      response.status,
      (payload as ApiFailure | null) ?? null,
      `Upload failed (${response.status})`,
    );
  }
  return payload.data;
}

export async function deleteFile(id: string): Promise<null> {
  return apiRequest<null>(`/api/v1/files/${id}`, { method: 'DELETE', auth: true });
}

export function downloadFileUrl(id: string): string {
  return `/api/v1/files/${id}/download`;
}

export async function downloadFile(id: string, fileName: string): Promise<void> {
  const headers = new Headers();
  const token = getAccessToken();
  if (token) headers.set('Authorization', `Bearer ${token}`);
  const response = await fetch(downloadFileUrl(id), { headers });
  if (!response.ok) {
    throw new Error(`Download failed (${response.status})`);
  }
  const blob = await response.blob();
  const url = URL.createObjectURL(blob);
  const anchor = document.createElement('a');
  anchor.href = url;
  anchor.download = fileName;
  anchor.click();
  URL.revokeObjectURL(url);
}
