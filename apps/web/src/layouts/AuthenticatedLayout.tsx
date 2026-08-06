import { Outlet } from '@tanstack/react-router';
import { cn } from '@naswood/ui';
import { useAuth } from '@/auth/useAuth';
import { AppBreadcrumb } from '@/components/shell/AppBreadcrumb';
import { AppHeader } from '@/components/shell/AppHeader';
import { AppSidebar } from '@/components/shell/AppSidebar';
import { ShellProvider } from '@/components/shell/ShellProvider';
import { useShell } from '@/components/shell/useShell';
import { useI18n } from '@/i18n';

function AuthenticatedShellFrame() {
  const { isBootstrapping } = useAuth();
  const { collapsed } = useShell();
  const { t } = useI18n();

  if (isBootstrapping) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-[var(--color-background)]">
        <p className="text-[var(--text-secondary)]">{t('loadingSession')}</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-[var(--color-background)] text-[var(--text-primary)]">
      <AppSidebar />
      <div
        className={cn(
          'flex min-h-screen flex-col transition-[padding] duration-200',
          collapsed ? 'lg:pl-[72px]' : 'lg:pl-[280px]',
        )}
      >
        <AppHeader />
        <div className="mx-auto flex w-full max-w-[1600px] flex-1 flex-col gap-4 px-4 py-4 sm:px-6">
          <AppBreadcrumb />
          <div className="flex-1">
            <Outlet />
          </div>
          <footer className="border-t border-[var(--border-default)] py-3 text-xs text-[var(--text-muted)]">
            {t('shellFooter')}
          </footer>
        </div>
      </div>
    </div>
  );
}

export function AuthenticatedLayout() {
  return (
    <ShellProvider>
      <AuthenticatedShellFrame />
    </ShellProvider>
  );
}
