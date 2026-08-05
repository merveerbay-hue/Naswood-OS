import { Bell } from 'lucide-react';
import { Button, cn } from '@naswood/ui';
import { useNotifications } from '@/notifications/useNotifications';
import { NotificationPanel } from './NotificationPanel';

export function NotificationBell() {
  const { unreadCount, panelOpen, setPanelOpen } = useNotifications();

  return (
    <div className="relative">
      <Button
        type="button"
        variant="ghost"
        size="sm"
        aria-label={unreadCount > 0 ? `Notifications (${unreadCount} unread)` : 'Notifications'}
        aria-expanded={panelOpen}
        aria-haspopup="dialog"
        onClick={() => setPanelOpen(!panelOpen)}
      >
        <Bell className="size-4" />
        {unreadCount > 0 ? (
          <span
            className={cn(
              'absolute -right-0.5 -top-0.5 flex h-4 min-w-4 items-center justify-center rounded-full bg-[var(--color-danger)] px-1 text-[10px] font-semibold text-white',
            )}
          >
            {unreadCount > 9 ? '9+' : unreadCount}
          </span>
        ) : null}
      </Button>
      <NotificationPanel />
    </div>
  );
}
