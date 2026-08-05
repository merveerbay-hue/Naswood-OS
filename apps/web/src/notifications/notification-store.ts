import type { AppNotification } from './types';

const STORAGE_KEY = 'naswood.notifications';

const SEED: AppNotification[] = [
  {
    id: 'n-1',
    title: 'Welcome to Naswood OS',
    message: 'Platform shell is ready. Notification delivery will connect to the event bus later.',
    priority: 'info',
    read: false,
    createdAt: new Date(Date.now() - 5 * 60_000).toISOString(),
    module: 'Platform',
  },
  {
    id: 'n-2',
    title: 'Inventory sync completed',
    message: 'Nightly stock reconciliation finished with no exceptions.',
    priority: 'success',
    read: false,
    createdAt: new Date(Date.now() - 45 * 60_000).toISOString(),
    module: 'Inventory',
  },
  {
    id: 'n-3',
    title: 'Purchase order awaiting approval',
    message: 'PO-2026-001254 is waiting for plant manager approval.',
    priority: 'warning',
    read: true,
    createdAt: new Date(Date.now() - 3 * 60 * 60_000).toISOString(),
    module: 'Purchasing',
  },
];

export function loadNotifications(): AppNotification[] {
  if (typeof window === 'undefined') {
    return SEED;
  }
  const raw = localStorage.getItem(STORAGE_KEY);
  if (!raw) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(SEED));
    return SEED;
  }
  try {
    return JSON.parse(raw) as AppNotification[];
  } catch {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(SEED));
    return SEED;
  }
}

export function saveNotifications(items: AppNotification[]) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
}
