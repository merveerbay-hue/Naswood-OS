import { useContext } from 'react';
import { ShellContext, type ShellContextValue } from './shell-context';

export type { ShellContextValue };

export function useShell(): ShellContextValue {
  const context = useContext(ShellContext);
  if (!context) {
    throw new Error('useShell must be used within ShellProvider');
  }
  return context;
}
