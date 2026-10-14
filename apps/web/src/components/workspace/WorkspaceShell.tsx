import { Link, useRouterState } from '@tanstack/react-router';
import { cn } from '@naswood/ui';
import { useI18n } from '@/i18n';

export interface WorkspaceNavItem {
  id: string;
  label: string;
  path: string;
  screenId?: string;
}

export interface WorkspaceDefinition {
  id: string;
  label: string;
  items: WorkspaceNavItem[];
}

interface WorkspaceShellProps {
  moduleLabel: string;
  moduleHomePath: string;
  workspaces: WorkspaceDefinition[];
  children: React.ReactNode;
}

export function WorkspaceShell({ moduleLabel, moduleHomePath, workspaces, children }: WorkspaceShellProps) {
  const { t } = useI18n();
  const pathname = useRouterState({ select: (s) => s.location.pathname });

  const activeWorkspace =
    workspaces.find((ws) => ws.items.some((item) => pathname === item.path || pathname.startsWith(`${item.path}/`))) ??
    workspaces[0];

  return (
    <section className="space-y-4">
      <header className="space-y-3 border-b border-[var(--border-default)] pb-4">
        <div className="flex flex-wrap items-end justify-between gap-3">
          <div>
            <p className="text-xs font-medium uppercase tracking-[0.14em] text-[var(--text-muted)]">{moduleLabel}</p>
            <h1 className="text-2xl font-semibold tracking-tight">{activeWorkspace?.label ?? moduleLabel}</h1>
          </div>
          <Link
            to={moduleHomePath}
            className="text-sm text-[var(--color-primary)] hover:underline"
          >
            {t('moduleHome')}
          </Link>
        </div>

        <nav className="flex flex-wrap gap-2" aria-label={`${moduleLabel} workspaces`}>
          {workspaces.map((ws) => {
            const href = ws.items[0]?.path ?? moduleHomePath;
            const active = ws.id === activeWorkspace?.id;
            return (
              <Link
                key={ws.id}
                to={href}
                className={cn(
                  'rounded-[var(--radius-md)] px-3 py-1.5 text-sm font-medium transition-colors',
                  active
                    ? 'bg-[var(--color-primary)] text-[var(--text-inverse)]'
                    : 'bg-[var(--color-surface)] text-[var(--text-secondary)] hover:bg-[var(--color-surface-hover)]',
                )}
              >
                {ws.label}
              </Link>
            );
          })}
        </nav>

        {activeWorkspace ? (
          <nav className="flex flex-wrap gap-1" aria-label={`${activeWorkspace.label} screens`}>
            {activeWorkspace.items.map((item) => {
              const active = pathname === item.path || pathname.startsWith(`${item.path}/`);
              return (
                <Link
                  key={item.id}
                  to={item.path}
                  className={cn(
                    'rounded-full px-3 py-1 text-xs font-medium transition-colors',
                    active
                      ? 'bg-[var(--color-surface-hover)] text-[var(--text-primary)] ring-1 ring-[var(--border-default)]'
                      : 'text-[var(--text-muted)] hover:text-[var(--text-primary)]',
                  )}
                >
                  {item.label}
                  {item.screenId ? (
                    <span className="ml-1 opacity-60">{item.screenId}</span>
                  ) : null}
                </Link>
              );
            })}
          </nav>
        ) : null}
      </header>

      <div>{children}</div>
    </section>
  );
}
