export interface ApiErrorItem {
  code: string;
  category: string;
  field: string | null;
  message: string;
  details?: Record<string, unknown>;
}

export interface ApiSuccess<T> {
  success: true;
  data: T;
  message: string | null;
  metadata?: Record<string, unknown>;
}

export interface ApiFailure {
  success: false;
  data: null;
  message: string | null;
  errors: ApiErrorItem[];
  metadata?: Record<string, unknown>;
}

export type ApiResponse<T> = ApiSuccess<T> | ApiFailure;

export interface AuthenticatedUser {
  id: string;
  username: string;
  name: string;
  email: string | null;
  companyId: string;
  plantId: string;
  roles: string[];
}

export interface AuthenticationResult {
  accessToken: string;
  refreshToken: string;
  tokenType: string;
  expiresIn: number;
  user: AuthenticatedUser;
}

export interface CurrentUser {
  id: string;
  username: string;
  name: string;
  email: string | null;
  companyId: string;
  plantId: string;
  sessionId: string;
  roles: string[];
}

export interface LoginRequest {
  username: string;
  password: string;
  rememberMe: boolean;
  companyId?: string;
  plantId?: string;
  deviceName?: string;
  browser?: string;
  operatingSystem?: string;
}

export class ApiClientError extends Error {
  readonly status: number;
  readonly code: string | null;
  readonly errors: ApiErrorItem[];

  constructor(status: number, body: ApiFailure | null, fallbackMessage: string) {
    const first = body?.errors?.[0];
    super(first?.message ?? body?.message ?? fallbackMessage);
    this.name = 'ApiClientError';
    this.status = status;
    this.code = first?.code ?? null;
    this.errors = body?.errors ?? [];
  }
}
