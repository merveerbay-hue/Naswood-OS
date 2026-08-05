import { createContext } from 'react';
import type { AppNotification, NotificationFilter } from './types';

export interface NotificationsContextValue {
  items: AppNotification[];
  filter: NotificationFilter;
  unreadCount: number;
  panelOpen: boolean;
  setFilter: (filter: NotificationFilter) => void;
  setPanelOpen: (open: boolean) => void;
  markRead: (id: string) => void;
  markAllRead: () => void;
  filteredItems: AppNotification[];
}

export const NotificationsContext = createContext<NotificationsContextValue | null>(null);
