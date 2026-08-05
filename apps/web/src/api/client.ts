import {
  ApiClientError,
  type ApiFailure,
  type ApiResponse,
  type AuthenticationResult,
} from './types';
import {
  clearSession,
  getAccessToken,
  getRefreshToken,
  updateTokens,
} from '@/auth/session';

type RequestOptions = Omit<RequestInit, 'body'> & {
  body?: unknown;
  auth?: boolean;
  retryOnUnauthorized?: boolean;
};

let refreshInFlight: Promise<boolean> | null = null;

async function parseJson<T>(response: Response): Promise<ApiResponse<T> | null> {
  const text = await response.text();
  if (!text) {
    return null;
  }
  return JSON.parse(text) as ApiResponse<T>;
}

async function refreshAccessToken(): Promise<boolean> {
  const refreshToken = getRefreshToken();
  if (!refreshToken) {
    return false;
  }

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

function ensureRefresh(): Promise<boolean> {
  if (!refreshInFlight) {
    refreshInFlight = refreshAccessToken().finally(() => {
      refreshInFlight = null;
    });
  }
  return refreshInFlight;
}

export async function apiRequest<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const {
    body,
    auth = false,
    retryOnUnauthorized = true,
    headers: initHeaders,
    ...rest
  } = options;

  const headers = new Headers(initHeaders);
  headers.set('Accept', 'application/json');
  if (body !== undefined) {
    headers.set('Content-Type', 'application/json');
  }
  if (auth) {
    const token = getAccessToken();
    if (token) {
      headers.set('Authorization', `Bearer ${token}`);
    }
  }

  const response = await fetch(path, {
    ...rest,
    headers,
    body: body === undefined ? undefined : JSON.stringify(body),
  });

  if (response.status === 401 && auth && retryOnUnauthorized) {
    const refreshed = await ensureRefresh();
    if (refreshed) {
      return apiRequest<T>(path, { ...options, retryOnUnauthorized: false });
    }
  }

  const payload = await parseJson<T>(response);

  if (!response.ok || !payload || !payload.success) {
    throw new ApiClientError(
      response.status,
      (payload as ApiFailure | null) ?? null,
      `Request failed (${response.status})`,
    );
  }

  return payload.data;
}
