export type ThemePreference = 'light' | 'dark' | 'system';
export type ResolvedTheme = 'light' | 'dark';

const THEME_KEY = 'naswood.theme';

export function readThemePreference(): ThemePreference {
  if (typeof window === 'undefined') {
    return 'light';
  }
  const value = localStorage.getItem(THEME_KEY);
  if (value === 'light' || value === 'dark' || value === 'system') {
    return value;
  }
  // Cursor / embedded previews often report prefers-color-scheme: dark.
  // Prefer light until the user explicitly chooses a theme.
  try {
    if (window.self !== window.top) {
      return 'light';
    }
  } catch {
    return 'light';
  }
  return 'light';
}

export function writeThemePreference(preference: ThemePreference) {
  localStorage.setItem(THEME_KEY, preference);
}

export function getSystemTheme(): ResolvedTheme {
  if (typeof window === 'undefined') {
    return 'light';
  }
  return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}

export function resolveTheme(preference: ThemePreference): ResolvedTheme {
  return preference === 'system' ? getSystemTheme() : preference;
}

export function applyTheme(resolved: ResolvedTheme) {
  document.documentElement.dataset.theme = resolved;
  document.documentElement.style.colorScheme = resolved;
}
