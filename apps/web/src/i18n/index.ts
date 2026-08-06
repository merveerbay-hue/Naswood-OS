import { createContext, useContext } from 'react';
import { tr, type TrDictionary } from './tr';

export type Locale = 'tr' | 'en';

const STORAGE_KEY = 'naswood.locale';

export function getStoredLocale(): Locale {
  try {
    const value = localStorage.getItem(STORAGE_KEY);
    if (value === 'en' || value === 'tr') return value;
  } catch {
    /* ignore */
  }
  return 'tr';
}

export function storeLocale(locale: Locale) {
  try {
    localStorage.setItem(STORAGE_KEY, locale);
    document.documentElement.lang = locale === 'tr' ? 'tr' : 'en';
  } catch {
    /* ignore */
  }
}

/** Nested path lookup: t('inventory.dashTitle') */
export function translate(dict: TrDictionary, path: string): string {
  const parts = path.split('.');
  let cur: unknown = dict;
  for (const part of parts) {
    if (cur && typeof cur === 'object' && part in (cur as object)) {
      cur = (cur as Record<string, unknown>)[part];
    } else {
      return path;
    }
  }
  return typeof cur === 'string' ? cur : path;
}

export function statusLabel(dict: TrDictionary, status?: string | null): string {
  if (!status) return '—';
  return dict.status[status] ?? dict.status[status.replace(/\s+/g, '')] ?? status;
}

export interface I18nContextValue {
  locale: Locale;
  setLocale: (locale: Locale) => void;
  t: (path: string) => string;
  dict: TrDictionary;
}

export const I18nContext = createContext<I18nContextValue | null>(null);

export function useI18n(): I18nContextValue {
  const ctx = useContext(I18nContext);
  if (!ctx) {
    // Fallback when used outside provider (tests / early boot)
    return {
      locale: 'tr',
      setLocale: () => undefined,
      t: (path) => translate(tr, path),
      dict: tr,
    };
  }
  return ctx;
}

export { tr };
