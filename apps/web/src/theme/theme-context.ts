import { createContext } from 'react';
import type { ResolvedTheme, ThemePreference } from './theme-prefs';

export interface ThemeContextValue {
  preference: ThemePreference;
  resolved: ResolvedTheme;
  setPreference: (preference: ThemePreference) => void;
  cyclePreference: () => void;
}

export const ThemeContext = createContext<ThemeContextValue | null>(null);
