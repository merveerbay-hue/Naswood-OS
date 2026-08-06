import { cn } from '@naswood/ui';
import { statusLabel, useI18n } from '@/i18n';

const TONE: Record<string, string> = {
  active: 'bg-emerald-500/15 text-emerald-700 dark:text-emerald-300',
  posted: 'bg-emerald-500/15 text-emerald-700 dark:text-emerald-300',
  draft: 'bg-amber-500/15 text-amber-800 dark:text-amber-200',
  open: 'bg-sky-500/15 text-sky-800 dark:text-sky-200',
  released: 'bg-sky-500/15 text-sky-800 dark:text-sky-200',
  inprogress: 'bg-violet-500/15 text-violet-800 dark:text-violet-200',
  blocked: 'bg-rose-500/15 text-rose-700 dark:text-rose-300',
  cancelled: 'bg-[var(--color-surface-hover)] text-[var(--text-muted)]',
  closed: 'bg-[var(--color-surface-hover)] text-[var(--text-muted)]',
  inactive: 'bg-[var(--color-surface-hover)] text-[var(--text-muted)]',
};

export function StatusBadge({ status }: { status?: string | null }) {
  const { dict } = useI18n();
  const raw = (status ?? '').trim();
  const label = statusLabel(dict, raw || null);
  const toneKey = raw.replace(/\s+/g, '').toLowerCase();
  const tone = TONE[toneKey] ?? 'bg-[var(--color-surface-hover)] text-[var(--text-secondary)]';
  return (
    <span className={cn('inline-flex rounded-full px-2 py-0.5 text-xs font-medium', tone)}>
      {label}
    </span>
  );
}
