import { apiRequest } from '@/api/client';

export type PackageContentRow = {
  id?: string;
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
  productionOrderNumber?: string;
  sourceLotCount?: number;
  sourceLotNumbers?: string[];
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
  allowedActions?: string[];
  inactiveReason?: string | null;
  labelHint?: string | null;
  relations?: PackageRelationRow[];
  contents: PackageContentRow[];
  movements: PackageMovementRow[];
};

export type PackageRelationRow = {
  relationType: string;
  sourcePackageId: string;
  targetPackageId: string;
  sourcePackageNo: string;
  targetPackageNo: string;
  quantity: number;
  unit: string;
  direction: string;
};

export type PackageContentSplitLine = {
  sourceContentId?: string;
  thicknessMm?: number | null;
  widthMm?: number | null;
  lengthMm?: number | null;
  quantity: number;
  pieceCount?: number | null;
};

export type PackageOperationResult = {
  operationId: string;
  number: string;
  operationType: string;
  idempotentReplay: boolean;
  source?: PackagePassport | null;
  target?: PackagePassport | null;
  sources?: PackagePassport[];
  message: string;
  reprintOriginal: boolean;
  printTarget: boolean;
};

export async function getPackagePassport(id: string) {
  return apiRequest<PackagePassport>(`/api/v1/packages/${id}/passport`, { auth: true });
}

export async function getPackageByBarcode(barcode: string) {
  return apiRequest<PackagePassport>(`/api/v1/packages/by-barcode/${encodeURIComponent(barcode)}`, { auth: true });
}

export async function getPackageByPublicId(publicId: string) {
  return apiRequest<PackagePassport>(`/api/v1/packages/by-public/${encodeURIComponent(publicId)}`, { auth: true });
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

export async function splitPackage(id: string, lines: PackageContentSplitLine[], number?: string, physicalGroupLabel?: string) {
  return apiRequest<PackageOperationResult>(`/api/v1/packages/${id}/split`, {
    method: 'POST',
    auth: true,
    body: { number: number ?? '', physicalGroupLabel: physicalGroupLabel ?? '', lines },
  });
}

export async function mergePackages(sourcePackageIds: string[], number?: string, physicalGroupLabel?: string) {
  return apiRequest<PackageOperationResult>('/api/v1/packages/merge', {
    method: 'POST',
    auth: true,
    body: { number: number ?? '', sourcePackageIds, physicalGroupLabel: physicalGroupLabel ?? '' },
  });
}

export async function repackPackage(id: string, number?: string, physicalGroupLabel?: string) {
  return apiRequest<PackageOperationResult>(`/api/v1/packages/${id}/repack`, {
    method: 'POST',
    auth: true,
    body: { number: number ?? '', physicalGroupLabel: physicalGroupLabel ?? '' },
  });
}

export async function partialMovePackage(
  id: string,
  warehouseCode: string,
  locationCode: string,
  lines: PackageContentSplitLine[],
  number?: string,
  physicalGroupLabel?: string,
) {
  return apiRequest<PackageOperationResult>(`/api/v1/packages/${id}/partial-move`, {
    method: 'POST',
    auth: true,
    body: { number: number ?? '', warehouseCode, locationCode, physicalGroupLabel: physicalGroupLabel ?? '', lines },
  });
}

export { previewOpeningGroups } from './openingPackagePreview';
