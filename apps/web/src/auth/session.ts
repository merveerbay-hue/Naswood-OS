const ACCESS_KEY = 'naswood.accessToken';
const REFRESH_KEY = 'naswood.refreshToken';
const REMEMBER_KEY = 'naswood.rememberMe';

function storageFor(rememberMe: boolean): Storage {
  return rememberMe ? localStorage : sessionStorage;
}

function clearKey(key: string) {
  localStorage.removeItem(key);
  sessionStorage.removeItem(key);
}

export function getRememberMe(): boolean {
  return localStorage.getItem(REMEMBER_KEY) === '1';
}

export function getAccessToken(): string | null {
  return localStorage.getItem(ACCESS_KEY) ?? sessionStorage.getItem(ACCESS_KEY);
}

export function getRefreshToken(): string | null {
  return localStorage.getItem(REFRESH_KEY) ?? sessionStorage.getItem(REFRESH_KEY);
}

export function isAuthenticated(): boolean {
  return Boolean(getAccessToken());
}

export function persistSession(tokens: {
  accessToken: string;
  refreshToken: string;
  rememberMe: boolean;
}) {
  clearSession();
  localStorage.setItem(REMEMBER_KEY, tokens.rememberMe ? '1' : '0');
  const store = storageFor(tokens.rememberMe);
  store.setItem(ACCESS_KEY, tokens.accessToken);
  store.setItem(REFRESH_KEY, tokens.refreshToken);
}

export function updateTokens(tokens: { accessToken: string; refreshToken: string }) {
  const rememberMe = getRememberMe();
  const store = storageFor(rememberMe);
  clearKey(ACCESS_KEY);
  clearKey(REFRESH_KEY);
  store.setItem(ACCESS_KEY, tokens.accessToken);
  store.setItem(REFRESH_KEY, tokens.refreshToken);
}

export function clearSession() {
  clearKey(ACCESS_KEY);
  clearKey(REFRESH_KEY);
  clearKey(REMEMBER_KEY);
}
