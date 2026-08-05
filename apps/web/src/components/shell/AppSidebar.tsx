import { Link, useRouterState } from '@tanstack/react-router';
import {
  BadgeCheck,
  BarChart3,
  ChevronDown,
  ChevronRight,
  Factory,
  LayoutDashboard,
  Package,
  Settings,
  ShoppingCart,
  Sparkles,
  TrendingUp,
  Wallet,
  Wrench,
  type LucideIcon,
} from 'lucide-react';
import { useMemo, useState } from 'react';
import { cn } from '@naswood/ui';
import { useAuth } from '@/auth/useAuth';
import {
  filterNavigationByRoles,
  isPathActive,
  navigationTree,
  type NavItem,
} from '@/navigation/nav-config';
import { useShell } from './useShell';

const ICONS: Record<string, LucideIcon> = {
  LayoutDashboard,
  Package,
  ShoppingCart,
  TrendingUp,
  Factory,
  BadgeCheck,
  Wrench,
  Wallet,
  BarChart3,
  Sparkles,
  Settings,
};

function NavNode({
  item,
  depth,
  collapsed,
  onNavigate,
}: {
  item: NavItem;
  depth: number;
  collapsed: boolean;
  onNavigate: () => void;
}) {
  const pathname = useRouterState({ select: (s) => s.location.pathname });
  const active = isPathActive(pathname, item.path);
  const childActive = item.children?.some((child) => isPathActive(pathname, child.path)) ?? false;
  const [open, setOpen] = useState(active || childActive);
  const Icon = item.icon ? ICONS[item.icon] : undefined;
  const hasChildren = Boolean(item.children?.length);

  if (collapsed && depth === 0) {
    return (
      <Link
        to={item.path ?? '#'}
        title={item.label}
        onClick={onNavigate}
        className={cn(
          'flex h-10 w-10 items-center justify-center rounded-[var(--radius-md)] transition-colors',
          active || childActive
            ? 'bg-[var(--color-primary)] text-[var(--text-inverse)]'
            : 'text-[var(--text-secondary)] hover:bg-[var(--color-surface-hover)] hover:text-[var(--text-primary)]',
        )}
      >
        {Icon ? <Icon className="size-5" /> : <span className="text-xs font-semibold">{item.label[0]}</span>}
      </Link>
    );
  }

  return (
    <div className="space-y-1">
      <div className="flex items-center gap-1">
        {item.path ? (
          <Link
            to={item.path}
            onClick={onNavigate}
            className={cn(
              'flex min-w-0 flex-1 items-center gap-3 rounded-[var(--radius-md)] px-3 py-2 text-sm font-medium transition-colors',
              active
                ? 'bg-[var(--color-primary)] text-[var(--text-inverse)]'
                : 'text-[var(--text-secondary)] hover:bg-[var(--color-surface-hover)] hover:text-[var(--text-primary)]',
            )}
            style={{ paddingLeft: `${0.75 + depth * 0.75}rem` }}
          >
            {Icon && depth === 0 ? <Icon className="size-4 shrink-0" /> : null}
            <span className="truncate">{item.label}</span>
          </Link>
        ) : (
          <button
            type="button"
            onClick={() => setOpen((value) => !value)}
            className={cn(
              'flex min-w-0 flex-1 items-center gap-3 rounded-[var(--radius-md)] px-3 py-2 text-left text-sm font-medium text-[var(--text-secondary)] hover:bg-[var(--color-surface-hover)] hover:text-[var(--text-primary)]',
            )}
            style={{ paddingLeft: `${0.75 + depth * 0.75}rem` }}
          >
            {Icon && depth === 0 ? <Icon className="size-4 shrink-0" /> : null}
            <span className="truncate">{item.label}</span>
          </button>
        )}
        {hasChildren ? (
          <button
            type="button"
            aria-label={open ? `Collapse ${item.label}` : `Expand ${item.label}`}
            className="rounded p-1 text-[var(--text-muted)] hover:bg-[var(--color-surface-hover)]"
            onClick={() => setOpen((value) => !value)}
          >
            {open ? <ChevronDown className="size-4" /> : <ChevronRight className="size-4" />}
          </button>
        ) : null}
      </div>
      {hasChildren && open ? (
        <div className="space-y-1">
          {item.children!.map((child) => (
            <NavNode
              key={child.id}
              item={child}
              depth={depth + 1}
              collapsed={collapsed}
              onNavigate={onNavigate}
            />
          ))}
        </div>
      ) : null}
    </div>
  );
}

export function AppSidebar() {
  const { user } = useAuth();
  const { collapsed, mobileOpen, setMobileOpen } = useShell();
  const items = useMemo(
    () => filterNavigationByRoles(navigationTree, user?.roles ?? []),
    [user?.roles],
  );

  const closeMobile = () => setMobileOpen(false);

  return (
    <>
      <div
        className={cn(
          'fixed inset-0 z-30 bg-black/40 transition-opacity lg:hidden',
          mobileOpen ? 'opacity-100' : 'pointer-events-none opacity-0',
        )}
        onClick={closeMobile}
        aria-hidden={!mobileOpen}
      />

      <aside
        className={cn(
          'fixed inset-y-0 left-0 z-40 flex flex-col border-r border-[var(--border-default)] bg-[var(--color-surface)] transition-[width,transform] duration-200',
          collapsed ? 'w-[72px]' : 'w-[280px]',
          mobileOpen ? 'translate-x-0' : '-translate-x-full lg:translate-x-0',
        )}
        aria-label="Primary"
      >
        <div
          className={cn(
            'flex h-16 items-center border-b border-[var(--border-default)] px-4',
            collapsed ? 'justify-center' : 'gap-3',
          )}
        >
          <div className="flex size-9 items-center justify-center rounded-[var(--radius-md)] bg-[var(--color-primary)] text-sm font-bold text-[var(--text-inverse)]">
            N
          </div>
          {!collapsed ? (
            <div className="min-w-0">
              <p className="truncate text-sm font-semibold tracking-tight">Naswood OS</p>
              <p className="truncate text-xs text-[var(--text-muted)]">Platform</p>
            </div>
          ) : null}
        </div>

        <nav className={cn('flex-1 space-y-1 overflow-y-auto p-3', collapsed && 'flex flex-col items-center')}>
          {items.map((item) => (
            <NavNode
              key={item.id}
              item={item}
              depth={0}
              collapsed={collapsed}
              onNavigate={closeMobile}
            />
          ))}
        </nav>

        {!collapsed ? (
          <div className="border-t border-[var(--border-default)] p-3 text-xs text-[var(--text-muted)]">
            <p className="font-medium text-[var(--text-secondary)]">Favorites</p>
            <p className="mt-1">Coming soon</p>
            <p className="mt-3 font-medium text-[var(--text-secondary)]">Recent</p>
            <p className="mt-1">Coming soon</p>
          </div>
        ) : null}
      </aside>
    </>
  );
}
