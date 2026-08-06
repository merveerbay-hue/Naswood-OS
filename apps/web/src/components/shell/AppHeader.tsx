import { Link, useNavigate, useRouterState } from '@tanstack/react-router';
import { ChevronDown, LogOut, Menu, PanelLeft, Search, Sparkles } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';
import { Button, cn } from '@naswood/ui';
import { useAuth } from '@/auth/useAuth';
import { useI18n } from '@/i18n';
import { NotificationBell } from './NotificationBell';
import { ThemeToggle } from './ThemeToggle';
import { useShell } from './useShell';

export function AppHeader() {
  const { t } = useI18n();
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
        aria-label={collapsed ? 'Kenar çubuğunu genişlet' : 'Kenar çubuğunu daralt'}
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
            Şirket
          </label>
          <select
            id="header-company"
            className="h-9 rounded-[var(--radius-md)] border border-[var(--border-default)] bg-[var(--color-background)] px-2 text-sm"
            value={user?.companyId ?? ''}
            disabled
            title={t('companySwitchSoon')}
          >
            <option value={user?.companyId ?? ''}>{user?.companyId ?? 'Şirket'}</option>
          </select>
          <label className="sr-only" htmlFor="header-plant">
            Tesis
          </label>
          <select
            id="header-plant"
            className="h-9 rounded-[var(--radius-md)] border border-[var(--border-default)] bg-[var(--color-background)] px-2 text-sm"
            value={user?.plantId ?? ''}
            disabled
            title={t('plantSwitchSoon')}
          >
            <option value={user?.plantId ?? ''}>{user?.plantId ?? 'Tesis'}</option>
          </select>
        </div>

        <Button
          type="button"
          variant="outline"
          size="sm"
          className="hidden sm:inline-flex"
          disabled
          title={t('globalSearchSoon')}
        >
          <Search className="size-4" />
          <span className="hidden lg:inline">Ara</span>
          <kbd className="ml-1 hidden rounded border border-[var(--border-default)] px-1 text-[10px] text-[var(--text-muted)] lg:inline">
            ⌘K
          </kbd>
        </Button>

        <NotificationBell />
        <Button type="button" variant="ghost" size="sm" disabled title={t('aiSoon')}>
          <Sparkles className="size-4" />
        </Button>
        <ThemeToggle />

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
                {user?.roles?.[0] ?? 'Kullanıcı'}
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
              title="Profil ayarları yakında"
            >
              Tercihler
            </button>
            <button
              type="button"
              role="menuitem"
              className="flex w-full items-center gap-2 rounded-[var(--radius-sm)] px-3 py-2 text-sm text-[var(--color-danger)] hover:bg-[var(--color-surface-hover)]"
              onClick={onLogout}
              disabled={loggingOut}
            >
              <LogOut className="size-4" />
              {loggingOut ? 'Çıkış yapılıyor…' : 'Çıkış yap'}
            </button>
          </div>
        </div>
      </div>
    </header>
  );
}
