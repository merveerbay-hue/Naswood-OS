import { createContext } from 'react';

export interface ShellContextValue {
  collapsed: boolean;
  mobileOpen: boolean;
  toggleCollapsed: () => void;
  setMobileOpen: (open: boolean) => void;
}

export const ShellContext = createContext<ShellContextValue | null>(null);
