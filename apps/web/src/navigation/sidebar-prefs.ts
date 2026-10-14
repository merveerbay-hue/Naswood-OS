const COLLAPSED_KEY = 'naswood.sidebar.collapsed';
const MOBILE_OPEN_KEY = 'naswood.sidebar.mobileOpen';

export function readSidebarCollapsed(): boolean {
  if (typeof window === 'undefined') {
    return false;
  }
  return localStorage.getItem(COLLAPSED_KEY) === '1';
}

export function writeSidebarCollapsed(collapsed: boolean) {
  localStorage.setItem(COLLAPSED_KEY, collapsed ? '1' : '0');
}

export function readMobileSidebarOpen(): boolean {
  return sessionStorage.getItem(MOBILE_OPEN_KEY) === '1';
}

export function writeMobileSidebarOpen(open: boolean) {
  sessionStorage.setItem(MOBILE_OPEN_KEY, open ? '1' : '0');
}
