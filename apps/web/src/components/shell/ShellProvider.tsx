import { useEffect, useMemo, useState, type ReactNode } from 'react';
import {
  readSidebarCollapsed,
  writeMobileSidebarOpen,
  writeSidebarCollapsed,
} from '@/navigation/sidebar-prefs';
import { ShellContext, type ShellContextValue } from './shell-context';

function useIsDesktop() {
  const [isDesktop, setIsDesktop] = useState(() =>
    typeof window !== 'undefined' ? window.matchMedia('(min-width: 1024px)').matches : true,
  );

  useEffect(() => {
    const media = window.matchMedia('(min-width: 1024px)');
    const onChange = () => setIsDesktop(media.matches);
    onChange();
    media.addEventListener('change', onChange);
    return () => media.removeEventListener('change', onChange);
  }, []);

  return isDesktop;
}

export function ShellProvider({ children }: { children: ReactNode }) {
  const isDesktop = useIsDesktop();
  const [collapsed, setCollapsed] = useState(() => readSidebarCollapsed());
  const [mobileOpen, setMobileOpenState] = useState(false);

  useEffect(() => {
    if (isDesktop) {
      setMobileOpenState(false);
      writeMobileSidebarOpen(false);
    }
  }, [isDesktop]);

  const value = useMemo<ShellContextValue>(
    () => ({
      collapsed: isDesktop ? collapsed : false,
      mobileOpen,
      toggleCollapsed: () => {
        if (!isDesktop) {
          setMobileOpenState((open) => {
            const next = !open;
            writeMobileSidebarOpen(next);
            return next;
          });
          return;
        }
        setCollapsed((prev) => {
          const next = !prev;
          writeSidebarCollapsed(next);
          return next;
        });
      },
      setMobileOpen: (open: boolean) => {
        setMobileOpenState(open);
        writeMobileSidebarOpen(open);
      },
    }),
    [collapsed, isDesktop, mobileOpen],
  );

  return <ShellContext.Provider value={value}>{children}</ShellContext.Provider>;
}
