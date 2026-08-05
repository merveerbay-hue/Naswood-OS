import { useContext } from 'react';
import {
  NotificationsContext,
  type NotificationsContextValue,
} from './notifications-context';

export type { NotificationsContextValue };

export function useNotifications(): NotificationsContextValue {
  const context = useContext(NotificationsContext);
  if (!context) {
    throw new Error('useNotifications must be used within NotificationsProvider');
  }
  return context;
}
