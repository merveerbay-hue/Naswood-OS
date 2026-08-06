import { useEffect, useRef } from 'react';
import { Button, cn } from '@naswood/ui';
import { useI18n } from '@/i18n';
import { useNotifications } from '@/notifications/useNotifications';
import type { AppNotification, NotificationFilter } from '@/notifications/types';

function formatRelative(iso: string, t: (path: string) => string): string {
  const deltaMs = Date.now() - Date.parse(iso);
  const minutes = Math.floor(deltaMs / 60_000);
  if (minutes < 1) return t('notify.justNow');
  if (minutes < 60) return t('notify.minutesAgo').replace('{n}', String(minutes));
  const hours = Math.floor(minutes / 60);
  if (hours < 24) return t('notify.hoursAgo').replace('{n}', String(hours));
  const days = Math.floor(hours / 24);
  return t('notify.daysAgo').replace('{n}', String(days));
}

function priorityClass(priority: AppNotification['priority']): string {
  switch (priority) {
    case 'success':
      return 'bg-[var(--color-success)]';
    case 'warning':
      return 'bg-[var(--color-warning)]';
    case 'danger':
      return 'bg-[var(--color-danger)]';
    default:
      return 'bg-[var(--color-info)]';
  }
}

export function NotificationPanel() {
  const { t } = useI18n();
  const {
    panelOpen,
    setPanelOpen,
    filter,
    setFilter,
    filteredItems,
    unreadCount,
    markRead,
    markAllRead,
  } = useNotifications();
  const panelRef = useRef<HTMLDivElement>(null);

  const filters: { id: NotificationFilter; label: string }[] = [
    { id: 'all', label: t('notify.all') },
    { id: 'unread', label: t('notify.unread') },
    { id: 'read', label: t('notify.read') },
  ];

  useEffect(() => {
    if (!panelOpen) {
      return;
    }
    const onPointerDown = (event: MouseEvent) => {
      if (!panelRef.current?.contains(event.target as Node)) {
        setPanelOpen(false);
      }
    };
    const onKeyDown = (event: KeyboardEvent) => {
      if (event.key === 'Escape') {
        setPanelOpen(false);
      }
    };
    document.addEventListener('mousedown', onPointerDown);
    document.addEventListener('keydown', onKeyDown);
    return () => {
      document.removeEventListener('mousedown', onPointerDown);
      document.removeEventListener('keydown', onKeyDown);
    };
  }, [panelOpen, setPanelOpen]);

  if (!panelOpen) {
    return null;
  }

  return (
    <div
      ref={panelRef}
      role="dialog"
      aria-label={t('notificationCenter')}
      className="absolute right-0 z-50 mt-2 w-[min(100vw-2rem,22rem)] overflow-hidden rounded-[var(--radius-lg)] border border-[var(--border-default)] bg-[var(--color-background)] shadow-xl"
    >
      <div className="flex items-center justify-between border-b border-[var(--border-default)] px-4 py-3">
        <div>
          <p className="text-sm font-semibold">{t('notify.title')}</p>
          <p className="text-xs text-[var(--text-muted)]">
            {unreadCount === 0
              ? t('notify.upToDate')
              : t('notify.unreadCount').replace('{count}', String(unreadCount))}
          </p>
        </div>
        <Button
          type="button"
          variant="ghost"
          size="sm"
          disabled={unreadCount === 0}
          onClick={markAllRead}
        >
          {t('notify.markAllRead')}
        </Button>
      </div>

      <div className="flex gap-1 border-b border-[var(--border-default)] px-2 py-2">
        {filters.map((item) => (
          <button
            key={item.id}
            type="button"
            className={cn(
              'rounded-[var(--radius-md)] px-3 py-1.5 text-xs font-medium',
              filter === item.id
                ? 'bg-[var(--color-primary)] text-[var(--text-inverse)]'
                : 'text-[var(--text-secondary)] hover:bg-[var(--color-surface-hover)]',
            )}
            onClick={() => setFilter(item.id)}
          >
            {item.label}
          </button>
        ))}
      </div>

      <ul className="max-h-80 overflow-y-auto">
        {filteredItems.length === 0 ? (
          <li className="px-4 py-8 text-center text-sm text-[var(--text-muted)]">
            {t('notify.empty')}
          </li>
        ) : (
          filteredItems.map((item) => (
            <li key={item.id} className="border-b border-[var(--border-default)] last:border-b-0">
              <button
                type="button"
                className={cn(
                  'flex w-full gap-3 px-4 py-3 text-left hover:bg-[var(--color-surface-hover)]',
                  !item.read && 'bg-[var(--color-surface)]',
                )}
                onClick={() => markRead(item.id)}
              >
                <span
                  className={cn('mt-1 size-2 shrink-0 rounded-full', priorityClass(item.priority))}
                  aria-hidden
                />
                <span className="min-w-0 flex-1">
                  <span className="flex items-start justify-between gap-2">
                    <span className="truncate text-sm font-medium">{item.title}</span>
                    <span className="shrink-0 text-[10px] text-[var(--text-muted)]">
                      {formatRelative(item.createdAt, t)}
                    </span>
                  </span>
                  <span className="mt-0.5 block text-xs text-[var(--text-secondary)]">
                    {item.message}
                  </span>
                  {item.module ? (
                    <span className="mt-1 inline-block text-[10px] uppercase tracking-wide text-[var(--text-muted)]">
                      {item.module}
                    </span>
                  ) : null}
                </span>
              </button>
            </li>
          ))
        )}
      </ul>

      <div className="border-t border-[var(--border-default)] px-4 py-2 text-[10px] text-[var(--text-muted)]">
        {t('comingSoon')}
      </div>
    </div>
  );
}
