import { Link, useRouterState } from '@tanstack/react-router';
import { ChevronRight } from 'lucide-react';
import { useI18n } from '@/i18n';
import { findNavTrail } from '@/navigation/nav-config';

export function AppBreadcrumb() {
  const { t } = useI18n();
  const pathname = useRouterState({ select: (s) => s.location.pathname });
  const trail = findNavTrail(pathname);

  if (trail.length === 0) {
    return null;
  }

  return (
    <nav aria-label={t('breadcrumb')} className="flex flex-wrap items-center gap-1 text-sm text-[var(--text-secondary)]">
      {trail.map((item, index) => {
        const isLast = index === trail.length - 1;
        return (
          <span key={item.id} className="inline-flex items-center gap-1">
            {index > 0 ? <ChevronRight className="size-3.5 text-[var(--text-muted)]" /> : null}
            {isLast || !item.path ? (
              <span className="font-medium text-[var(--text-primary)]" aria-current="page">
                {item.label}
              </span>
            ) : (
              <Link to={item.path} className="hover:text-[var(--text-primary)] hover:underline">
                {item.label}
              </Link>
            )}
          </span>
        );
      })}
    </nav>
  );
}
