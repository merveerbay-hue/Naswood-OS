import { Link, useNavigate, useRouterState } from '@tanstack/react-router';
import { Bell, ChevronDown, LogOut, Menu, Moon, PanelLeft, Search, Sparkles } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';
import { Button, cn } from '@naswood/ui';
import { useAuth } from '@/auth/useAuth';
import { useShell } from './useShell';

export function AppHeader() {
  const { user, logout } = useAuth();
  const { collapsed, toggleCollapsed } = useShell();
  const navigate = useNavigate();
  const pathname = useRouterState({ select: (s) => s.location.pathname });
  const [menuOpen, setMenuOpen] = useState(false);
  const [loggingOut, setLoggingOut] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    setMenuOpen(false);
  }, [pathname]);

  useEffect(() => {
    const onPointerDown = (event: MouseEvent) => {
      if (!menuRef.current?.contains(event.target as Node)) {
        setMenuOpen(false);
      }
    };
    document.addEventListener('mousedown', onPointerDown);
    return () => document.removeEventListener('mousedown', onPointerDown);
  }, []);

  const onLogout = async () => {
    setLoggingOut(true);
    try {
      await logout();
      await navigate({ to: '/login' });
    } finally {
      setLoggingOut(false);
    }
  };

  return (
    <header className="sticky top-0 z-20 flex h-16 items-center gap-3 border-b border-[var(--border-default)] bg-[var(--color-background)]/95 px-4 backdrop-blur">
      <Button
        type="button"
        variant="ghost"
        size="sm"
        className="shrink-0"
        aria-label={collapsed ? 'Expand sidebar' : 'Collapse sidebar'}
        onClick={toggleCollapsed}
      >
        <span className="lg:hidden">
          <Menu className="size-4" />
        </span>
        <span className="hidden lg:inline">
          <PanelLeft className="size-4" />
        </span>
      </Button>

      <Link to="/" className="hidden min-w-0 sm:block">
        <span className="text-sm font-semibold tracking-tight text-[var(--text-primary)]">
          Naswood OS
        </span>
      </Link>

      <div className="ml-auto flex items-center gap-2">
        <div className="hidden items-center gap-2 md:flex">
          <label className="sr-only" htmlFor="header-company">
            Company
          </label>
          <select
            id="header-company"
            className="h-9 rounded-[var(--radius-md)] border border-[var(--border-default)] bg-[var(--color-background)] px-2 text-sm"
            value={user?.companyId ?? ''}
            disabled
            title="Company switching lands with multi-company UX"
          >
            <option value={user?.companyId ?? ''}>{user?.companyId ?? 'Company'}</option>
          </select>
          <label className="sr-only" htmlFor="header-plant">
            Plant
          </label>
          <select
            id="header-plant"
            className="h-9 rounded-[var(--radius-md)] border border-[var(--border-default)] bg-[var(--color-background)] px-2 text-sm"
            value={user?.plantId ?? ''}
            disabled
            title="Plant switching lands with multi-plant UX"
          >
            <option value={user?.plantId ?? ''}>{user?.plantId ?? 'Plant'}</option>
          </select>
        </div>

        <Button type="button" variant="outline" size="sm" className="hidden sm:inline-flex" disabled title="Global search coming soon">
          <Search className="size-4" />
          <span className="hidden lg:inline">Search</span>
          <kbd className="ml-1 hidden rounded border border-[var(--border-default)] px-1 text-[10px] text-[var(--text-muted)] lg:inline">
            ⌘K
          </kbd>
        </Button>

        <Button type="button" variant="ghost" size="sm" disabled title="Notifications (TASK-011)">
          <Bell className="size-4" />
        </Button>
        <Button type="button" variant="ghost" size="sm" disabled title="AI Assistant coming soon">
          <Sparkles className="size-4" />
        </Button>
        <Button type="button" variant="ghost" size="sm" disabled title="Theme (TASK-010)">
          <Moon className="size-4" />
        </Button>

        <div className="relative" ref={menuRef}>
          <button
            type="button"
            className="flex items-center gap-2 rounded-[var(--radius-md)] border border-[var(--border-default)] px-2 py-1.5 text-left hover:bg-[var(--color-surface-hover)]"
            onClick={() => setMenuOpen((open) => !open)}
            aria-expanded={menuOpen}
            aria-haspopup="menu"
          >
            <span className="flex size-8 items-center justify-center rounded-full bg-[var(--color-secondary)] text-xs font-semibold text-[var(--text-inverse)]">
              {(user?.name ?? user?.username ?? '?').slice(0, 1).toUpperCase()}
            </span>
            <span className="hidden min-w-0 lg:block">
              <span className="block truncate text-sm font-medium">{user?.name ?? user?.username}</span>
              <span className="block truncate text-xs text-[var(--text-muted)]">
                {user?.roles?.[0] ?? 'User'}
              </span>
            </span>
            <ChevronDown className="hidden size-4 text-[var(--text-muted)] lg:block" />
          </button>

          <div
            role="menu"
            className={cn(
              'absolute right-0 mt-2 w-56 rounded-[var(--radius-md)] border border-[var(--border-default)] bg-[var(--color-background)] p-1 shadow-lg',
              menuOpen ? 'block' : 'hidden',
            )}
          >
            <div className="border-b border-[var(--border-default)] px-3 py-2">
              <p className="truncate text-sm font-medium">{user?.name}</p>
              <p className="truncate text-xs text-[var(--text-muted)]">{user?.email ?? user?.username}</p>
            </div>
            <button
              type="button"
              role="menuitem"
              className="flex w-full items-center gap-2 rounded-[var(--radius-sm)] px-3 py-2 text-sm text-[var(--text-secondary)] hover:bg-[var(--color-surface-hover)] disabled:opacity-50"
              disabled
              title="Profile settings coming soon"
            >
              Preferences
            </button>
            <button
              type="button"
              role="menuitem"
              className="flex w-full items-center gap-2 rounded-[var(--radius-sm)] px-3 py-2 text-sm text-[var(--color-danger)] hover:bg-[var(--color-surface-hover)]"
              onClick={onLogout}
              disabled={loggingOut}
            >
              <LogOut className="size-4" />
              {loggingOut ? 'Signing out…' : 'Sign out'}
            </button>
          </div>
        </div>
      </div>
    </header>
  );
}
