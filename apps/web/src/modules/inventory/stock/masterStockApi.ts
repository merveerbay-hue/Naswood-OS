import { apiRequest } from '@/api/client';
import { ApiClientError } from '@/api/types';

export type MasterStockFilters = {
  plantId: string;
  warehouseCode?: string;
  locationCode?: string;
  materialCode?: string;
  lotNumber?: string;
  stockStatus?: string;
  q?: string;
  sortBy?: string;
  sortDir?: string;
  page?: number;
  pageSize?: number;
};

export type MasterStockRow = {
  balanceId: string;
  plantId: string;
  materialCode: string;
  materialName: string;
  woodSpecies: string;
  actualMeasurement: string;
  actualThicknessMm?: number | null;
  actualWidthMm?: number | null;
  actualLengthMm?: number | null;
  lot: string;
  factory: string;
  warehouseCode: string;
  warehouse: string;
  locationCode: string;
  location: string;
  pieceCount?: number | null;
  packageCount: number;
  stockUnit: string;
  stockQuantity: number;
  quantityReserved: number;
  quantityAvailable: number;
  stockStatus: string;
  packageBalanceMismatch: boolean;
  packageQuantitySum?: number | null;
};

export type MasterStockPackageRow = {
  id: string;
  balanceId?: string | null;
  packageNo: string;
  physicalGroupLabel: string;
  barcode?: string;
  materialCode: string;
  materialName: string;
  actualMeasurement: string;
  actualThicknessMm?: number | null;
  actualWidthMm?: number | null;
  actualLengthMm?: number | null;
  lot: string;
  factory: string;
  warehouseCode: string;
  warehouse: string;
  locationCode: string;
  location: string;
  pieceCount?: number | null;
  stockUnit: string;
  stockQuantity: number;
  status: string;
  contents?: {
    lineNo: number;
    thicknessMm?: number | null;
    widthMm?: number | null;
    lengthMm?: number | null;
    pieceCount?: number | null;
    quantity: number;
    unitOfMeasure: string;
    measurement: string;
  }[];
};

export type MasterStockTotals = {
  materialRowCount: number;
  packageCount: number;
  byUnit: { unit: string; quantity: number }[];
};

export type PagedMasterStock = {
  items: MasterStockRow[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  totals: MasterStockTotals;
  exportGeneratedAt: string;
  asOfDate?: string | null;
};

export type PagedMasterStockPackages = {
  items: MasterStockPackageRow[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export type MasterStockExport = {
  fileName: string;
  generatedAt: string;
  stockRows: MasterStockRow[];
  packageRows: MasterStockPackageRow[];
  report: {
    generatedAt: string;
    reportDate: string;
    reportTime: string;
    timeZone: string;
    factory: string;
    warehouseFilter: string;
    locationFilter: string;
    stockStatusFilter: string;
    materialFilter: string;
    lotFilter: string;
    preparedBy: string;
    totalStockRows: number;
    totalPackages: number;
    scopeNote: string;
  };
  totals: MasterStockTotals;
};

function qs(filters: MasterStockFilters): string {
  const p = new URLSearchParams();
  p.set('plantId', filters.plantId);
  p.set('page', String(filters.page ?? 1));
  p.set('pageSize', String(filters.pageSize ?? 20));
  if (filters.warehouseCode) p.set('warehouseCode', filters.warehouseCode);
  if (filters.locationCode) p.set('locationCode', filters.locationCode);
  if (filters.materialCode) p.set('materialCode', filters.materialCode);
  if (filters.lotNumber) p.set('lotNumber', filters.lotNumber);
  if (filters.stockStatus) p.set('stockStatus', filters.stockStatus);
  if (filters.q) p.set('q', filters.q);
  if (filters.sortBy) p.set('sortBy', filters.sortBy);
  if (filters.sortDir) p.set('sortDir', filters.sortDir);
  return p.toString();
}

function staleApi(path: string, err: unknown): Error {
  if (err instanceof ApiClientError && err.status === 404) {
    return new Error(
      `Master Stok API 404: ${path}. API süreci eski — Naswood.Api’yi yeniden başlatın (GET /api/v1/inventory-master-stock).`,
    );
  }
  return err instanceof Error ? err : new Error(String(err));
}

export async function searchMasterStock(filters: MasterStockFilters) {
  const path = `/api/v1/inventory-master-stock?${qs(filters)}`;
  try {
    return await apiRequest<PagedMasterStock>(path, { method: 'GET', auth: true });
  } catch (err) {
    throw staleApi(path, err);
  }
}

export async function searchMasterStockPackages(filters: MasterStockFilters) {
  const path = `/api/v1/inventory-master-stock/packages?${qs(filters)}`;
  try {
    return await apiRequest<PagedMasterStockPackages>(path, { method: 'GET', auth: true });
  } catch (err) {
    throw staleApi(path, err);
  }
}

export async function getMasterStockRowPackages(balanceId: string) {
  const path = `/api/v1/inventory-master-stock/${balanceId}/packages`;
  try {
    return await apiRequest<MasterStockPackageRow[]>(path, { method: 'GET', auth: true });
  } catch (err) {
    throw staleApi(path, err);
  }
}

export async function exportMasterStock(filters: Omit<MasterStockFilters, 'page' | 'pageSize' | 'sortBy' | 'sortDir'>) {
  const p = new URLSearchParams();
  p.set('plantId', filters.plantId);
  if (filters.warehouseCode) p.set('warehouseCode', filters.warehouseCode);
  if (filters.locationCode) p.set('locationCode', filters.locationCode);
  if (filters.materialCode) p.set('materialCode', filters.materialCode);
  if (filters.lotNumber) p.set('lotNumber', filters.lotNumber);
  if (filters.stockStatus) p.set('stockStatus', filters.stockStatus);
  if (filters.q) p.set('q', filters.q);
  const path = `/api/v1/inventory-master-stock/export?${p}`;
  try {
    return await apiRequest<MasterStockExport>(path, { method: 'GET', auth: true });
  } catch (err) {
    throw staleApi(path, err);
  }
}
