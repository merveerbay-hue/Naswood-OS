import { apiRequest } from '@/api/client';

export type PackageContentRow = {
  lineNo: number;
  thicknessMm?: number | null;
  widthMm?: number | null;
  lengthMm?: number | null;
  pieceCount?: number | null;
  quantity: number;
  unitOfMeasure: string;
  measurement: string;
};

export type PackageMovementRow = {
  at: string;
  action: string;
  fromLocation: string;
  toLocation: string;
  quantity: number;
  reference: string;
  unit: string;
};

export type PackagePassport = {
  id: string;
  packageNo: string;
  barcode: string;
  publicId: string;
  qrPath: string;
  materialCode: string;
  materialName: string;
  materialGroup: string;
  materialType: string;
  woodSpecies: string;
  quality: string;
  stockUnit: string;
  countUnit: string;
  lotNumber: string;
  sourceType: string;
  sourceReferenceNo: string;
  factory: string;
  warehouseCode: string;
  locationCode: string;
  status: string;
  quantity: number;
  unitOfMeasure: string;
  totalPieceCount?: number | null;
  physicalGroupLabel: string;
  createdAt: string;
  lastMovementAt?: string | null;
  labelPrintedAt?: string | null;
  labelPrintCount: number;
  packageBalanceMismatch: boolean;
  contents: PackageContentRow[];
  movements: PackageMovementRow[];
};

export async function getPackagePassport(id: string) {
  return apiRequest<PackagePassport>(`/api/v1/packages/${id}/passport`, { auth: true });
}

export async function getPackageByBarcode(barcode: string) {
  return apiRequest<PackagePassport>(`/api/v1/packages/by-barcode/${encodeURIComponent(barcode)}`, { auth: true });
}

export async function recordLabelPrint(id: string) {
  return apiRequest<PackagePassport>(`/api/v1/packages/${id}/label-print`, { method: 'POST', auth: true, body: {} });
}

export async function relocatePackage(id: string, warehouseCode: string, locationCode: string) {
  return apiRequest<PackagePassport>(`/api/v1/packages/${id}/relocate`, {
    method: 'POST',
    auth: true,
    body: { warehouseCode, locationCode },
  });
}

export { previewOpeningGroups } from './openingPackagePreview';
