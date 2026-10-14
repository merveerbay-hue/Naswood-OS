import { useMemo, useState, type ReactNode } from 'react';
import { loadNotifications, saveNotifications } from './notification-store';
import {
  NotificationsContext,
  type NotificationsContextValue,
} from './notifications-context';
import type { AppNotification, NotificationFilter } from './types';

export function NotificationsProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<AppNotification[]>(() => loadNotifications());
  const [filter, setFilter] = useState<NotificationFilter>('all');
  const [panelOpen, setPanelOpen] = useState(false);

  const value = useMemo<NotificationsContextValue>(() => {
    const unreadCount = items.filter((item) => !item.read).length;
    const filteredItems = items
      .filter((item) => {
        if (filter === 'unread') return !item.read;
        if (filter === 'read') return item.read;
        return true;
      })
      .slice()
      .sort((a, b) => Date.parse(b.createdAt) - Date.parse(a.createdAt));

    const persist = (next: AppNotification[]) => {
      setItems(next);
      saveNotifications(next);
    };

    return {
      items,
      filter,
      unreadCount,
      panelOpen,
      setFilter,
      setPanelOpen,
      markRead: (id: string) => {
        persist(items.map((item) => (item.id === id ? { ...item, read: true } : item)));
      },
      markAllRead: () => {
        persist(items.map((item) => ({ ...item, read: true })));
      },
      filteredItems,
    };
  }, [filter, items, panelOpen]);

  return (
    <NotificationsContext.Provider value={value}>{children}</NotificationsContext.Provider>
  );
}
