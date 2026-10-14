export type NotificationPriority = 'info' | 'success' | 'warning' | 'danger';

export interface AppNotification {
  id: string;
  title: string;
  message: string;
  priority: NotificationPriority;
  read: boolean;
  createdAt: string;
  module?: string;
}

export type NotificationFilter = 'all' | 'unread' | 'read';
