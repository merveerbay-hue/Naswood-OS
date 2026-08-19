import { Link } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Button, Input, buttonVariants } from '@naswood/ui';
import { createResource, searchAllResource, searchResource } from '@/api/business';
import { usePlantContext } from '@/auth/usePlantContext';
import { plantDisplayName } from '@/modules/inventory/locations/locationCatalog';
import {
  GRADING_METHODS,
  readComplianceFromDefinition,
  type GradingMethod,
} from '@/modules/inventory/materials/materialCompliance';
import {
  canCreateStructuralProductionLot,
  type StructuralProductionLotCreateInput,
} from './structuralProductionLotRules';

type MaterialOpt = {
  id?: string;
  code?: string;
  name?: string;
  definitionJson?: string;
  status?: string;
};

type BatchOpt = {
  id?: string;
  batchNumber?: string;
  materialCode?: string;
  plantId?: string;
};

type BalanceOpt = {
  id?: string;
  materialCode?: string;
  warehouseCode?: string;
  locationCode?: string;
  batchNumber?: string;
  quantityOnHand?: number;
  plantId?: string;
};

type CreatedLot = {
  id: string;
  productionLotNumber: string;
  status: string;
  actualGradingMethod: string;
  materialCode: string;
};

type SourceKind = 'batch' | 'balance';

/**
 * INV-SPL-001 — EN 14081 Structural Production Lot (foundation only).
 * No stock consumption, strength class, CE/DoP, or FPC in this phase.
 */
export function StructuralProductionLotCreatePage() {
  const { homePlantId, plantId: sessionPlantId, visiblePlantIds, canSwitchPlant } = usePlantContext();
  const queryClient = useQueryClient();
  const plantIds = visiblePlantIds.length ? visiblePlantIds : [homePlantId || 'F01'];
  const [plantId, setPlantId] = useState(sessionPlantId || homePlantId);

  useEffect(() => {
    setPlantId(sessionPlantId || homePlantId);
  }, [sessionPlantId, homePlantId]);

  const [materialId, setMaterialId] = useState('');
  const [method, setMethod] = useState<GradingMethod | ''>('');
  const [workCenterCode, setWorkCenterCode] = useState('');
  const [productionDate, setProductionDate] = useState(() => new Date().toISOString().slice(0, 10));
  const [shiftCode, setShiftCode] = useState('');
  const [notes, setNotes] = useState('');
  const [sourceKind, setSourceKind] = useState<SourceKind>('batch');
  const [sourceId, setSourceId] = useState('');
  const [sourceQty, setSourceQty] = useState('1');
  const [error, setError] = useState<string | null>(null);
  const [created, setCreated] = useState<CreatedLot | null>(null);

  const materialsQuery = useQuery({
    queryKey: ['business', 'materials', 'spl'],
    queryFn: () => searchAllResource<MaterialOpt>('materials'),
  });

  const workCentersQuery = useQuery({
    queryKey: ['business', 'work-centers', plantId],
    queryFn: () => searchAllResource<{ code?: string; name?: string }>('work-centers', undefined, { plantId }),
    enabled: Boolean(plantId),
  });

  const selectedMaterial = useMemo(
    () => (materialsQuery.data ?? []).find((m) => String(m.id) === materialId),
    [materialsQuery.data, materialId],
  );

  const compliance = useMemo(
    () => readComplianceFromDefinition(selectedMaterial?.definitionJson),
    [selectedMaterial?.definitionJson],
  );

  const structuralMaterials = useMemo(
    () =>
      (materialsQuery.data ?? []).filter((m) => {
        const c = readComplianceFromDefinition(m.definitionJson);
        return c.complianceScope === 'STRUCTURAL_TIMBER' && String(m.status ?? 'Active').toLowerCase() === 'active';
      }),
    [materialsQuery.data],
  );

  const materialCode = String(selectedMaterial?.code ?? '').trim();

  const batchesQuery = useQuery({
    queryKey: ['business', 'batches', 'spl', plantId, materialCode],
    queryFn: () =>
      searchResource<BatchOpt>('batches', materialCode || undefined, { page: 1, pageSize: 50, plantId }),
    enabled: Boolean(plantId) && Boolean(materialCode),
  });

  const balancesQuery = useQuery({
    queryKey: ['business', 'inventory', 'spl', plantId, materialCode],
    queryFn: () =>
      searchResource<BalanceOpt>('inventory', materialCode || undefined, { page: 1, pageSize: 50, plantId }),
    enabled: Boolean(plantId) && Boolean(materialCode),
  });

  useEffect(() => {
    setMethod('');
    setSourceId('');
  }, [materialId]);

  const createMutation = useMutation({
    mutationFn: async () => {
      const input: StructuralProductionLotCreateInput = {
        plantId: plantId || '',
        materialId,
        complianceScope: compliance.complianceScope,
        supportedGradingMethods: compliance.supportedGradingMethods,
        actualGradingMethod: method || undefined,
        productionDate,
        workCenterCode: workCenterCode || undefined,
        shiftCode: shiftCode || undefined,
        notes,
        inputs: sourceId
          ? [
              {
                sourceBatchId: sourceKind === 'batch' ? sourceId : undefined,
                sourceInventoryBalanceId: sourceKind === 'balance' ? sourceId : undefined,
                quantity: Number(sourceQty) || 0,
                unit: 'PCS',
              },
            ]
          : [],
      };
      const gate = canCreateStructuralProductionLot(input);
      if (!gate.ok) throw new Error(gate.message);

      return createResource<CreatedLot>(
        'structural-production-lots',
        {
          materialId,
          actualGradingMethod: method,
          workCenterCode: workCenterCode.trim() || null,
          productionDate,
          shiftCode: shiftCode.trim() || null,
          notes: notes.trim(),
          inputs: input.inputs.map((i) => ({
            sourceBatchId: i.sourceBatchId || null,
            sourceInventoryBalanceId: i.sourceInventoryBalanceId || null,
            sourcePackageId: i.sourcePackageId || null,
            quantity: i.quantity,
            unit: i.unit || 'PCS',
          })),
        },
        { plantId },
      );
    },
    onSuccess: (lot) => {
      setError(null);
      setCreated(lot);
      void queryClient.invalidateQueries({ queryKey: ['business', 'structural-production-lots'] });
    },
    onError: (err: Error) => {
      setCreated(null);
      setError(err.message || 'Kayıt oluşturulamadı.');
    },
  });

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">INV-SPL-001</p>
        <h2 className="text-xl font-semibold tracking-tight">Yapısal Üretim Lotu</h2>
        <p className="mt-1 text-sm text-[var(--text-secondary)]">
          EN 14081 üretim/sınıflandırma çalışması kaydı. Stok düşümü veya dayanım sonucu bu aşamada yok.
        </p>
      </div>

      <div className="space-y-4 border-t border-[var(--border)] pt-4">
        <label className="block space-y-1.5">
          <span className="text-sm font-medium">Çalışma Tesisi</span>
          {canSwitchPlant ? (
            <select
              className="w-full rounded-md border border-[var(--border)] bg-[var(--surface)] px-3 py-2 text-sm"
              value={plantId}
              onChange={(e) => setPlantId(e.target.value)}
            >
              {plantIds.map((p) => (
                <option key={p} value={p}>
                  {plantDisplayName(p)}
                </option>
              ))}
            </select>
          ) : (
            <Input value={plantDisplayName(plantId || homePlantId)} readOnly />
          )}
        </label>

        <label className="block space-y-1.5">
          <span className="text-sm font-medium">Malzeme</span>
          <select
            className="w-full rounded-md border border-[var(--border)] bg-[var(--surface)] px-3 py-2 text-sm"
            value={materialId}
            onChange={(e) => setMaterialId(e.target.value)}
          >
            <option value="">Yapısal kereste seçin…</option>
            {structuralMaterials.map((m) => (
              <option key={String(m.id)} value={String(m.id)}>
                {m.code} — {m.name}
              </option>
            ))}
          </select>
          {selectedMaterial && compliance.complianceScope !== 'STRUCTURAL_TIMBER' ? (
            <p className="text-sm text-[var(--danger)]">
              Bu malzeme NORMAL_STOCK kapsamındadır; Production Lot oluşturulamaz.
            </p>
          ) : null}
        </label>

        <fieldset className="space-y-2">
          <legend className="text-sm font-medium">Üretim / Sınıflandırma Yöntemi</legend>
          <div className="flex flex-col gap-2 sm:flex-row">
            {GRADING_METHODS.map((g) => {
              const supported = compliance.supportedGradingMethods.includes(g.token);
              return (
                <label
                  key={g.token}
                  className={`flex flex-1 items-center gap-2 rounded-md border px-3 py-2 text-sm ${
                    method === g.token ? 'border-[var(--accent)]' : 'border-[var(--border)]'
                  } ${supported ? '' : 'opacity-40'}`}
                >
                  <input
                    type="radio"
                    name="grading"
                    disabled={!supported || !materialId}
                    checked={method === g.token}
                    onChange={() => setMethod(g.token)}
                  />
                  {g.labelTr}
                </label>
              );
            })}
          </div>
        </fieldset>

        <label className="block space-y-1.5">
          <span className="text-sm font-medium">Üretim Noktası (Work Center, opsiyonel)</span>
          <select
            className="w-full rounded-md border border-[var(--border)] bg-[var(--surface)] px-3 py-2 text-sm"
            value={workCenterCode}
            onChange={(e) => setWorkCenterCode(e.target.value)}
          >
            <option value="">—</option>
            {(workCentersQuery.data ?? []).map((wc) => (
              <option key={String(wc.code)} value={String(wc.code ?? '')}>
                {wc.code} — {wc.name}
              </option>
            ))}
          </select>
        </label>

        <div className="grid gap-4 sm:grid-cols-2">
          <label className="block space-y-1.5">
            <span className="text-sm font-medium">Tarih</span>
            <Input type="date" value={productionDate} onChange={(e) => setProductionDate(e.target.value)} />
          </label>
          <label className="block space-y-1.5">
            <span className="text-sm font-medium">Vardiya (opsiyonel)</span>
            <Input value={shiftCode} onChange={(e) => setShiftCode(e.target.value)} placeholder="A / B / C" />
          </label>
        </div>

        <div className="space-y-2">
          <span className="text-sm font-medium">Kaynak Lot / Stok (opsiyonel izlenebilirlik)</span>
          <div className="flex gap-2">
            <select
              className="rounded-md border border-[var(--border)] bg-[var(--surface)] px-3 py-2 text-sm"
              value={sourceKind}
              onChange={(e) => {
                setSourceKind(e.target.value as SourceKind);
                setSourceId('');
              }}
            >
              <option value="batch">Batch / Lot</option>
              <option value="balance">Stok bakiyesi</option>
            </select>
            <select
              className="min-w-0 flex-1 rounded-md border border-[var(--border)] bg-[var(--surface)] px-3 py-2 text-sm"
              value={sourceId}
              onChange={(e) => setSourceId(e.target.value)}
              disabled={!materialCode}
            >
              <option value="">Seçilmedi</option>
              {sourceKind === 'batch'
                ? (batchesQuery.data?.items ?? []).map((b) => (
                    <option key={String(b.id)} value={String(b.id)}>
                      {b.batchNumber} · {b.materialCode}
                    </option>
                  ))
                : (balancesQuery.data?.items ?? []).map((b) => (
                    <option key={String(b.id)} value={String(b.id)}>
                      {b.warehouseCode}/{b.locationCode} · {b.batchNumber} · {b.quantityOnHand}
                    </option>
                  ))}
            </select>
            <Input
              className="w-24"
              type="number"
              min={0.001}
              step="any"
              value={sourceQty}
              onChange={(e) => setSourceQty(e.target.value)}
              disabled={!sourceId}
            />
          </div>
        </div>

        <label className="block space-y-1.5">
          <span className="text-sm font-medium">Not</span>
          <textarea
            className="min-h-[80px] w-full rounded-md border border-[var(--border)] bg-[var(--surface)] px-3 py-2 text-sm"
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
          />
        </label>

        {error ? <p className="text-sm text-[var(--danger)]">{error}</p> : null}
        {created ? (
          <p className="text-sm text-[var(--success,theme(colors.emerald.600))]">
            Oluşturuldu: <strong>{created.productionLotNumber}</strong> · {created.status} ·{' '}
            {created.actualGradingMethod}
          </p>
        ) : null}

        <div className="flex flex-wrap gap-3 pt-2">
          <Button
            type="button"
            disabled={createMutation.isPending}
            onClick={() => createMutation.mutate()}
          >
            Üretim Lotu Oluştur
          </Button>
          <Link to="/inventory/operations/structural-production-lots" className={buttonVariants({ variant: 'outline' })}>
            Listeye dön
          </Link>
        </div>
      </div>
    </div>
  );
}
