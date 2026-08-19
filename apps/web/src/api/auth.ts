import { apiRequest } from './client';
import type { AuthenticationResult, CurrentUser, LoginRequest, VisiblePlant } from './types';
import {
  clearSession,
  getAccessToken,
  getRefreshToken,
  persistSession,
  updateAccessToken,
} from '@/auth/session';

export async function login(request: LoginRequest): Promise<AuthenticationResult> {
  const result = await apiRequest<AuthenticationResult>('/api/v1/auth/login', {
    method: 'POST',
    body: {
      username: request.username,
      password: request.password,
      rememberMe: request.rememberMe,
      companyId: request.companyId || undefined,
      plantId: request.plantId || undefined,
      deviceName: request.deviceName ?? 'Naswood Web',
      browser: request.browser ?? navigator.userAgent,
      operatingSystem: request.operatingSystem ?? navigator.platform,
    },
  });

  persistSession({
    accessToken: result.accessToken,
    refreshToken: result.refreshToken,
    rememberMe: request.rememberMe,
  });

  return result;
}

export async function fetchCurrentUser(): Promise<CurrentUser> {
  return apiRequest<CurrentUser>('/api/v1/auth/me', {
    method: 'GET',
    auth: true,
  });
}

export async function fetchVisiblePlants(): Promise<VisiblePlant[]> {
  return apiRequest<VisiblePlant[]>('/api/v1/auth/plants', {
    method: 'GET',
    auth: true,
  });
}

/** Üst Yönetici — switch working plant; Ana Üs unchanged. */
export async function switchWorkingPlant(plantId: string): Promise<AuthenticationResult> {
  const result = await apiRequest<AuthenticationResult>('/api/v1/auth/switch-plant', {
    method: 'POST',
    auth: true,
    body: { plantId },
  });
  if (result.accessToken) {
    updateAccessToken(result.accessToken);
  }
  return result;
}

export async function logout(): Promise<void> {
  const accessToken = getAccessToken();
  const refreshToken = getRefreshToken();

  try {
    if (accessToken) {
      await apiRequest<null>('/api/v1/auth/logout', {
        method: 'POST',
        auth: true,
        retryOnUnauthorized: false,
      });
    } else if (refreshToken) {
      await apiRequest<null>('/api/v1/auth/revoke', {
        method: 'POST',
        body: { refreshToken },
        retryOnUnauthorized: false,
      });
    }
  } finally {
    clearSession();
  }
}
