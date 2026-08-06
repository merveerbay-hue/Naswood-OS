export interface NavItem {
  id: string;
  label: string;
  path?: string;
  icon?: string;
  children?: NavItem[];
  roles?: string[];
}

/** Menü etiketleri Türkçe (varsayılan arayüz dili). */
export const navigationTree: NavItem[] = [
  { id: 'dashboard', label: 'Gösterge Paneli', path: '/', icon: 'LayoutDashboard' },
  {
    id: 'inventory',
    label: 'Envanter',
    path: '/inventory/dashboard',
    icon: 'Package',
    children: [
      { id: 'inventory-dashboard', label: 'Komuta Merkezi', path: '/inventory/dashboard' },
      {
        id: 'inventory-operations',
        label: 'Operasyonlar',
        children: [
          { id: 'inventory-receive', label: 'Mal kabul başlat', path: '/inventory/operations/receive' },
          { id: 'inventory-goods-receipts', label: 'Kabul kayıtları', path: '/inventory/operations/goods-receipts' },
          { id: 'inventory-issue', label: 'Mal çıkışı başlat', path: '/inventory/operations/issue' },
          { id: 'inventory-goods-issues', label: 'Çıkış kayıtları', path: '/inventory/operations/goods-issues' },
          { id: 'inventory-transfers', label: 'Transfer', path: '/inventory/operations/transfers' },
        ],
      },
      {
        id: 'inventory-stock',
        label: 'Stok',
        children: [
          { id: 'inventory-balances', label: 'Stok Bakiyesi', path: '/inventory/stock/balances' },
          { id: 'inventory-lots', label: 'Lotlar', path: '/inventory/stock/lots' },
        ],
      },
      {
        id: 'inventory-counts-ws',
        label: 'Sayım & Düzeltme',
        children: [
          { id: 'inventory-counts', label: 'Çevrim Sayımı', path: '/inventory/counts/cycle-counts' },
          { id: 'inventory-adjustments', label: 'Düzeltme', path: '/inventory/counts/adjustments' },
        ],
      },
      {
        id: 'inventory-master-data',
        label: 'Ana Veri',
        children: [
          { id: 'inventory-materials', label: 'Malzemeler', path: '/inventory/master-data/materials' },
          { id: 'inventory-warehouses', label: 'Depolar', path: '/inventory/master-data/warehouses' },
          { id: 'inventory-locations', label: 'Lokasyonlar', path: '/inventory/master-data/locations' },
        ],
      },
      { id: 'inventory-reports', label: 'Raporlar', path: '/inventory/reports' },
    ],
  },
  {
    id: 'purchasing',
    label: 'Satınalma',
    path: '/purchasing',
    icon: 'ShoppingCart',
    children: [
      { id: 'purchasing-suppliers', label: 'Tedarikçi', path: '/purchasing/suppliers' },
      { id: 'purchasing-purchase-requests', label: 'Satınalma Talepleri', path: '/purchasing/purchase-requests' },
      { id: 'purchasing-rfqs', label: 'Teklif Talebi', path: '/purchasing/rfqs' },
      { id: 'purchasing-supplier-quotations', label: 'Tedarikçi Teklifleri', path: '/purchasing/supplier-quotations' },
      { id: 'purchasing-purchase-orders', label: 'Satınalma Siparişleri', path: '/purchasing/purchase-orders' },
      { id: 'purchasing-purchase-goods-receipts', label: 'Satınalma Mal Kabul', path: '/purchasing/purchase-goods-receipts' },
      { id: 'purchasing-purchase-returns', label: 'Satınalma İadeleri', path: '/purchasing/purchase-returns' },
      { id: 'purchasing-supplier-invoices', label: 'Tedarikçi Faturaları', path: '/purchasing/supplier-invoices' },
      { id: 'purchasing-dashboard', label: 'Gösterge Paneli', path: '/purchasing/dashboard' },
      { id: 'purchasing-reports', label: 'Raporlar', path: '/purchasing/reports' },
    ],
  },
  {
    id: 'sales',
    label: 'Satış',
    path: '/sales',
    icon: 'TrendingUp',
  },
  {
    id: 'production',
    label: 'Üretim',
    path: '/production/dashboard',
    icon: 'Factory',
    children: [
      { id: 'production-dashboard', label: 'Gösterge Paneli', path: '/production/dashboard' },
      {
        id: 'production-planning',
        label: 'Planlama',
        children: [
          { id: 'production-orders', label: 'Üretim Emirleri', path: '/production/planning/orders' },
          { id: 'production-work-orders', label: 'İş Emirleri', path: '/production/planning/work-orders' },
          { id: 'production-scheduling', label: 'Çizelgeleme', path: '/production/planning/scheduling' },
          { id: 'production-capacity', label: 'Kapasite', path: '/production/planning/capacity' },
          { id: 'production-dispatch', label: 'Sevk Listesi', path: '/production/planning/dispatch' },
        ],
      },
      {
        id: 'production-execution',
        label: 'İcra',
        children: [
          { id: 'production-operator', label: 'Operatör Terminali', path: '/production/execution/operator-terminal' },
          { id: 'production-machine-panel', label: 'Makine Paneli', path: '/production/execution/machine-panel' },
          { id: 'production-material-consumptions', label: 'Sarfiyat', path: '/production/execution/consumption' },
          { id: 'production-confirmations', label: 'Teyit', path: '/production/execution/confirmation' },
          { id: 'production-wips', label: 'Yarı Mamul (WIP)', path: '/production/execution/wip' },
          { id: 'production-packagings', label: 'Paketleme', path: '/production/execution/packaging' },
          { id: 'production-finished-goods', label: 'Mamul', path: '/production/execution/finished-goods' },
          { id: 'production-scraps', label: 'Hurda', path: '/production/execution/scrap' },
          { id: 'production-reworks', label: 'Yeniden İşlem', path: '/production/execution/rework' },
        ],
      },
      {
        id: 'production-master-data',
        label: 'Ana Veri',
        children: [
          { id: 'production-boms', label: 'Ürün Ağacı (BOM)', path: '/production/master-data/boms' },
          { id: 'production-routings', label: 'Rota', path: '/production/master-data/routings' },
          { id: 'production-operations', label: 'Operasyonlar', path: '/production/master-data/operations' },
          { id: 'production-machines', label: 'Makineler', path: '/production/master-data/machines' },
          { id: 'production-work-centers', label: 'İş Merkezleri', path: '/production/master-data/work-centers' },
          { id: 'production-production-lines', label: 'Hatlar', path: '/production/master-data/lines' },
          { id: 'production-shifts', label: 'Vardiyalar', path: '/production/master-data/shifts' },
          { id: 'production-calendars', label: 'Takvimler', path: '/production/master-data/calendars' },
          { id: 'production-toolings', label: 'Takım', path: '/production/master-data/toolings' },
        ],
      },
      {
        id: 'production-insights',
        label: 'Rapor & Analitik',
        children: [
          { id: 'production-reports', label: 'Raporlar', path: '/production/reports' },
          { id: 'production-analytics', label: 'Analitik', path: '/production/analytics' },
          { id: 'production-settings', label: 'Ayarlar', path: '/production/settings' },
        ],
      },
    ],
  },
  { id: 'quality', label: 'Kalite', path: '/quality', icon: 'BadgeCheck' },
  { id: 'maintenance', label: 'Bakım', path: '/maintenance', icon: 'Wrench' },
  { id: 'finance', label: 'Finans', path: '/finance', icon: 'Wallet' },
  { id: 'analytics', label: 'Analitik', path: '/analytics', icon: 'BarChart3' },
  { id: 'ai', label: 'Yapay Zeka', path: '/ai', icon: 'Sparkles' },
  {
    id: 'administration',
    label: 'Yönetim',
    path: '/administration',
    icon: 'Settings',
    roles: ['Administrator'],
    children: [
      { id: 'admin-users', label: 'Kullanıcılar', path: '/administration/users', roles: ['Administrator'] },
      { id: 'admin-roles', label: 'Roller', path: '/administration/roles', roles: ['Administrator'] },
      { id: 'admin-permissions', label: 'İzinler', path: '/administration/permissions', roles: ['Administrator'] },
      { id: 'admin-files', label: 'Dosyalar', path: '/administration/files', roles: ['Administrator'] },
      { id: 'admin-settings', label: 'Ayarlar', path: '/administration/settings', roles: ['Administrator'] },
      { id: 'admin-audit', label: 'Denetim Kayıtları', path: '/administration/audit', roles: ['Administrator'] },
      { id: 'admin-health', label: 'Sistem Sağlığı', path: '/administration/health', roles: ['Administrator'] },
    ],
  },
];

navigationTree.find((i) => i.id === 'sales')!.children = [
  { id: 'sales-customers', label: 'Müşteri', path: '/sales/customers' },
  { id: 'sales-leads', label: 'Aday', path: '/sales/leads' },
  { id: 'sales-opportunities', label: 'Fırsat', path: '/sales/opportunities' },
  { id: 'sales-quotations', label: 'Teklifler', path: '/sales/quotations' },
  { id: 'sales-sales-orders', label: 'Satış Siparişleri', path: '/sales/sales-orders' },
  { id: 'sales-shipments', label: 'Sevkiyat', path: '/sales/shipments' },
  { id: 'sales-deliveries', label: 'Teslimat', path: '/sales/deliveries' },
  { id: 'sales-customer-invoices', label: 'Müşteri Faturaları', path: '/sales/customer-invoices' },
  { id: 'sales-dashboard', label: 'Gösterge Paneli', path: '/sales/dashboard' },
  { id: 'sales-reports', label: 'Raporlar', path: '/sales/reports' },
];

export function filterNavigationByRoles(items: NavItem[], roles: string[]): NavItem[] {
  return items
    .filter((item) => !item.roles || item.roles.some((role) => roles.includes(role)))
    .map((item) => ({
      ...item,
      children: item.children ? filterNavigationByRoles(item.children, roles) : undefined,
    }));
}

export function findNavTrail(pathname: string, items: NavItem[] = navigationTree): NavItem[] {
  const normalized = pathname === '' ? '/' : pathname;
  for (const item of items) {
    if (item.path === normalized) return [item];
    if (item.children?.length) {
      const childTrail = findNavTrail(normalized, item.children);
      if (childTrail.length > 0) return [item, ...childTrail];
    }
  }
  return [];
}

export function collectNavPaths(items: NavItem[] = navigationTree): string[] {
  const paths: string[] = [];
  for (const item of items) {
    if (item.path) paths.push(item.path);
    if (item.children) paths.push(...collectNavPaths(item.children));
  }
  return paths;
}

export function isPathActive(pathname: string, itemPath?: string): boolean {
  if (!itemPath) return false;
  if (itemPath === '/') return pathname === '/';
  return pathname === itemPath || pathname.startsWith(`${itemPath}/`);
}
