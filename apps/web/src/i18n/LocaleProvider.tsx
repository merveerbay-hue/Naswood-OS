import { useEffect, useMemo, useState, type ReactNode } from 'react';
import { getStoredLocale, I18nContext, storeLocale, translate, type Locale } from './index';
import { tr } from './tr';

export function LocaleProvider({ children }: { children: ReactNode }) {
  const [locale, setLocaleState] = useState<Locale>(() => getStoredLocale());

  useEffect(() => {
    storeLocale(locale);
  }, [locale]);

  const value = useMemo(
    () => ({
      locale,
      setLocale: (next: Locale) => {
        setLocaleState(next);
        storeLocale(next);
      },
      t: (path: string) => translate(tr, path),
      dict: tr,
    }),
    [locale],
  );

  return <I18nContext.Provider value={value}>{children}</I18nContext.Provider>;
}
