import { apiRequest } from '@/api/client';

export type ProductionOutputLine = {
  physicalGroupLabel: string;
  thicknessMm?: number | null;
  widthMm?: number | null;
  lengthMm?: number | null;
  pieceCount?: number | null;
  measuredVolumeM3?: number | null;
};

export type ProductionOutputSource = {
  sourceLotId: string;
  sourceWarehouseCode: string;
  sourceLocationCode: string;
  consumedQuantity: number;
  unit: string;
  sourcePackageId?: string;
};

export type ProductionOutputBody = {
  number?: string;
  productionOrderId: string;
  outputMaterialId: string;
  warehouseCode: string;
  locationCode: string;
  workCenterCode?: string;
  stockStatus?: string;
  plantId: string;
  lines: ProductionOutputLine[];
  sources: ProductionOutputSource[];
};

export type ProductionOutputPreview = {
  productionOrderNumber: string;
  outputMaterialCode: string;
  outputMaterialName: string;
  destinationWarehouse: string;
  destinationLocation: string;
  workCenterCode: string;
  lotHint: string;
  packageCount: number;
  outputQuantity: number;
  inputQuantity: number;
  unit: string;
  sourceLotCount: number;
  packages: {
    physicalGroupLabel: string;
    measurementCount: number;
    quantity: number;
    unit: string;
    pieceCount?: number | null;
    measurements: string[];
  }[];
};

export type ProductionOutputResult = {
  outputId: string;
  number: string;
  status: string;
  productionLotId: string;
  productionLotNumber: string;
  sourceType: string;
  outputMaterialCode: string;
  outputQuantity: number;
  inputQuantity: number;
  unit: string;
  packageCount: number;
  sourceLotCount: number;
  idempotentReplay: boolean;
  packages: {
    packageId: string;
    packageNo: string;
    barcode: string;
    publicId: string;
    physicalGroupLabel: string;
    quantity: number;
    unit: string;
  }[];
  sourceLotNumbers: string[];
};

export function previewProductionOutput(body: ProductionOutputBody) {
  return apiRequest<ProductionOutputPreview>('/api/v1/production-outputs/preview', {
    method: 'POST',
    auth: true,
    body,
  });
}

export function postProductionOutput(body: ProductionOutputBody) {
  return apiRequest<ProductionOutputResult>('/api/v1/production-outputs', {
    method: 'POST',
    auth: true,
    body,
  });
}

export function getProductionLotPassport(id: string) {
  return apiRequest(`/api/v1/production-lots/${id}/passport`, { auth: true });
}
