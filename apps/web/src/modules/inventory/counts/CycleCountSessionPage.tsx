import { Link } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { createResource, searchAllResource, searchResource, updateResource } from '@/api/business';
import { useAuth } from '@/auth/useAuth';
import { usePlantContext } from '@/auth/usePlantContext';
import { useI18n } from '@/i18n';
import { plantDisplayName } from '@/modules/inventory/locations/locationCatalog';
import {
  COUNT_TYPES,
  FREEZE_MODES,
  buildCountSessionCreateBody,
  canOpenCountDocument,
  canOpenCountSession,
  canSaveCountLines,
  lineVariance,
  showSystemQuantity,
  summarizeVariances,
  type CountLine,
  type CountType,
  type FreezeMode,
} from './cycleCountSession';

type WarehouseOpt = { code?: string; name?: string; status?: string };
type LocationOpt = { code?: string; name?: string; warehouseCode?: string; status?: string };
type BalanceRow = {
  materialCode?: string;
  warehouseCode?: string;
  locationCode?: string;
  batchNumber?: string;
  quantityOnHand?: number;
};
type CountDoc = { id: string; number: string; warehouseCode: string; status: string; plantId?: string; notes?: string };

const STEPS = ['scope', 'open', 'count', 'variance', 'close'] as const;
type StepId = (typeof STEPS)[number];

function lineKey(material: string, loc: string, lot: string): string {
  return `${material}|${loc}|${lot}`.toUpperCase();
}

/**
 * INV-CNT-001 — Cycle count wizard.
 * Login already authorized the user. Step 2 opens a count *document*, not a second login.
 * Administrator always sees every step page; save is gated separately.
 */
export function CycleCountSessionPage() {
  const { t } = useI18n();
  const { user } = useAuth();
  const roles = user?.roles ?? [];
  const canSave = canSaveCountLines(roles);
  const canOpenDoc = canOpenCountDocument(roles);
  const queryClient = useQueryClient();
  const { homePlantId, plantId: sessionPlantId, visiblePlantIds, canSwitchPlant } = usePlantContext();
  const plantIds = visiblePlantIds.length ? visiblePlantIds : [homePlantId || 'PLANT-001'];
  const [plantId, setPlantId] = useState(sessionPlantId || homePlantId);
  const [step, setStep] = useState<StepId>('scope');

  const [warehouseCode, setWarehouseCode] = useState('');
  const [zone, setZone] = useState('');
  const [abcClass, setAbcClass] = useState('');
  const [countType, setCountType] = useState<CountType>('Cycle');
  const [countDate, setCountDate] = useState(() => new Date().toISOString().slice(0, 10));
  const [assignedTo, setAssignedTo] = useState('');
  const [blindCount, setBlindCount] = useState(false);
  const [freezeMode, setFreezeMode] = useState<FreezeMode>('None');
  const [opened, setOpened] = useState<CountDoc | null>(null);
  const [lines, setLines] = useState<CountLine[]>([]);
  const [scan, setScan] = useState('');
  const [savedNote, setSavedNote] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setPlantId(sessionPlantId || homePlantId);
  }, [sessionPlantId, homePlantId]);

  useEffect(() => {
    if (countType === 'Blind') setBlindCount(true);
  }, [countType]);

  const warehousesQuery = useQuery({
    queryKey: ['business', 'warehouses', 'cnt', plantId],
    queryFn: () => searchAllResource<WarehouseOpt>('warehouses', undefined, { plantId }),
  });
  const locationsQuery = useQuery({
    queryKey: ['business', 'locations', 'cnt', plantId],
    queryFn: () => searchAllResource<LocationOpt>('locations', undefined, { plantId }),
  });
  const balancesQuery = useQuery({
    queryKey: ['business', 'inventory', 'cnt', plantId, warehouseCode],
    enabled: Boolean(warehouseCode),
    queryFn: () =>
      searchResource<BalanceRow>('inventory', undefined, {
        page: 1,
        pageSize: 100,
        plantId,
        warehouseCode,
      }),
  });
  const openDocsQuery = useQuery({
    queryKey: ['business', 'inventory-counts', 'open', plantId],
    queryFn: () => searchResource<CountDoc>('inventory-counts', undefined, { page: 1, pageSize: 50, plantId }),
  });

  useEffect(() => {
    if (opened) return;
    const items = openDocsQuery.data?.items ?? [];
    const existing = items.find((d) => {
      const st = String(d.status ?? '').toLowerCase();
      return st === 'in progress' || st === 'inprogress' || st === 'released' || st === 'draft';
    });
    if (existing) {
      setOpened(existing);
      if (existing.warehouseCode) setWarehouseCode(existing.warehouseCode);
    }
  }, [openDocsQuery.data, opened]);

  const warehouses = useMemo(
    () =>
      (warehousesQuery.data ?? []).filter(
        (w) => String(w.status ?? 'Active').toLowerCase() === 'active' && String(w.code ?? '').trim(),
      ),
    [warehousesQuery.data],
  );
  const zones = useMemo(
    () =>
      (locationsQuery.data ?? []).filter(
        (l) =>
          String(l.status ?? 'Active').toLowerCase() === 'active' &&
          String(l.warehouseCode ?? '').toUpperCase() === warehouseCode.toUpperCase(),
      ),
    [locationsQuery.data, warehouseCode],
  );

  useEffect(() => {
    setLines((prev) => {
      if (prev.length > 0) return prev;
      return [
        { key: 'DEMO-1', materialCode: '', locationCode: '', lotNumber: '', systemQty: 0, countedQty: '' },
        { key: 'DEMO-2', materialCode: '', locationCode: '', lotNumber: '', systemQty: 0, countedQty: '' },
        { key: 'DEMO-3', materialCode: '', locationCode: '', lotNumber: '', systemQty: 0, countedQty: '' },
      ];
    });
  }, []);

  useEffect(() => {
    const items = balancesQuery.data?.items ?? [];
    if (!items.length) return;
    setLines((prev) => {
      const next = [...prev];
      const have = new Set(next.map((l) => l.key));
      for (const row of items) {
        if (zone && String(row.locationCode ?? '').toUpperCase() !== zone.toUpperCase()) continue;
        const materialCode = String(row.materialCode ?? '').trim();
        if (!materialCode) continue;
        const locationCode = String(row.locationCode ?? '').trim();
        const lotNumber = String(row.batchNumber ?? '').trim();
        const key = lineKey(materialCode, locationCode, lotNumber);
        if (have.has(key)) continue;
        have.add(key);
        next.push({
          key,
          materialCode,
          locationCode,
          lotNumber,
          systemQty: Number(row.quantityOnHand ?? 0),
          countedQty: '',
        });
      }
      return next;
    });
  }, [balancesQuery.data, zone]);

  const draft = {
    plantId,
    warehouseCode,
    zone,
    abcClass,
    countType,
    countDate,
    assignedTo,
    blindCount,
    freezeMode,
  };
  const gate = canOpenCountSession(draft);
  const showSys = showSystemQuantity(roles, blindCount);
  const totals = summarizeVariances(lines);

  const openMutation = useMutation({
    mutationFn: async () => {
      if (!canOpenDoc) throw new Error(t('wizard.cnt.countSaveDenied'));
      if (!gate.ok) throw new Error(gate.reason);
      return createResource<CountDoc>('inventory-counts', buildCountSessionCreateBody(draft), { plantId });
    },
    onSuccess: async (row) => {
      setError(null);
      setOpened(row);
      await queryClient.invalidateQueries({ queryKey: ['business', 'inventory-counts'] });
    },
    onError: (e: Error) => setError(e.message),
  });

  const saveLinesMutation = useMutation({
    mutationFn: async () => {
      if (!canSave) throw new Error(t('wizard.cnt.countSaveDenied'));
      if (!opened) throw new Error(t('wizard.cnt.needOpen'));
      const notes = `${opened.notes ?? ''}\nlines=${JSON.stringify(
        lines.map((l) => ({ m: l.materialCode, loc: l.locationCode, lot: l.lotNumber, qty: l.countedQty })),
      )}`;
      return updateResource<CountDoc>('inventory-counts', opened.id, {
        number: opened.number,
        warehouseCode: opened.warehouseCode || warehouseCode,
        status: opened.status || 'In Progress',
        notes,
      });
    },
    onSuccess: () => {
      setError(null);
      setSavedNote(t('wizard.cnt.countSaved'));
    },
    onError: (e: Error) => setError(e.message),
  });

  const closeMutation = useMutation({
    mutationFn: async () => {
      if (!canSave) throw new Error(t('wizard.cnt.countSaveDenied'));
      if (!opened) throw new Error(t('wizard.cnt.needOpen'));
      return updateResource<CountDoc>('inventory-counts', opened.id, {
        number: opened.number,
        warehouseCode: opened.warehouseCode || warehouseCode,
        status: 'Closed',
        notes: opened.notes ?? '',
      });
    },
    onSuccess: (row) => {
      setOpened(row);
      setError(null);
    },
    onError: (e: Error) => setError(e.message),
  });

  const stepIndex = STEPS.indexOf(step);
  const stepTitle: Record<StepId, string> = {
    scope: t('wizard.cnt.scope'),
    open: t('wizard.cnt.open'),
    count: t('wizard.cnt.count'),
    variance: t('wizard.cnt.variance'),
    close: t('wizard.cnt.close'),
  };
  const stepHint: Record<StepId, string> = {
    scope: t('wizard.cnt.stepHint'),
    open: t('wizard.cnt.openHint'),
    count: t('wizard.cnt.countHint'),
    variance: t('wizard.cnt.varianceHint'),
    close: t('wizard.cnt.closeHint'),
  };

  function applyScan() {
    const q = scan.trim().toUpperCase();
    if (!q) return;
    const hit = lines.find(
      (l) =>
        l.materialCode.toUpperCase().includes(q) ||
        l.lotNumber.toUpperCase().includes(q) ||
        l.locationCode.toUpperCase().includes(q),
    );
    if (hit) {
      const el = document.getElementById(`cnt-qty-${hit.key}`);
      el?.focus();
      return;
    }
    setLines((prev) => [
      ...prev,
      {
        key: lineKey(q, zone || warehouseCode || '—', ''),
        materialCode: scan.trim(),
        locationCode: zone || '',
        lotNumber: '',
        systemQty: 0,
        countedQty: '',
      },
    ]);
  }

  function addEmptyLine() {
    const n = lines.length + 1;
    setLines((prev) => [
      ...prev,
      {
        key: `NEW-${n}-${Date.now()}`,
        materialCode: '',
        locationCode: zone || '',
        lotNumber: '',
        systemQty: 0,
        countedQty: '',
      },
    ]);
  }

  function patchLine(key: string, patch: Partial<CountLine>) {
    setLines((prev) => prev.map((l) => (l.key === key ? { ...l, ...patch } : l)));
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-CNT-001</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('wizard.countTitle')}</h2>
          <p className="mt-1 text-sm text-[var(--text-secondary)]">{t('wizard.countDesc')}</p>
          <p className="mt-1 text-xs text-[var(--text-muted)]">{t('wizard.screenType')}</p>
        </div>
        <div className="flex flex-col items-end gap-2">
          <div className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 py-2 text-right">
            <p className="text-[10px] uppercase tracking-wide text-[var(--text-muted)]">{t('wizard.systemCode')}</p>
            <p className="font-mono text-sm font-medium text-[var(--text-primary)]">
              {opened?.number || t('wizard.cnt.numberPending')}
            </p>
            <p className="text-[10px] text-[var(--text-muted)]">{t('wizard.cnt.numberHint')}</p>
          </div>
          <Link to="/inventory/counts/cycle-counts" className="text-sm font-medium text-[var(--color-primary)] hover:underline">
            {t('wizard.backToLibrary')}
          </Link>
        </div>
      </div>

      <ol className="flex flex-wrap gap-2">
        {STEPS.map((id, i) => (
          <li key={id}>
            <button
              type="button"
              onClick={() => setStep(id)}
              className={`rounded-md px-3 py-1.5 text-xs font-medium ${
                id === step
                  ? 'bg-[var(--color-primary)] text-white'
                  : 'bg-[var(--color-surface-hover)] text-[var(--text-primary)]'
              }`}
            >
              {i + 1}. {stepTitle[id]}
            </button>
          </li>
        ))}
      </ol>

      <Card>
        <CardHeader>
          <CardTitle>
            {stepIndex + 1}. {stepTitle[step]}
          </CardTitle>
          <CardDescription>{stepHint[step]}</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {step === 'scope' ? (
            <div className="grid gap-3 md:grid-cols-2">
              <label className="space-y-1 text-sm">
                <span className="text-[var(--text-secondary)]">{t('wizard.cnt.plant')}</span>
                {canSwitchPlant && plantIds.length > 1 ? (
                  <select
                    className="h-10 w-full rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 text-sm"
                    value={plantId}
                    onChange={(e) => {
                      setPlantId(e.target.value);
                      setWarehouseCode('');
                      setZone('');
                    }}
                  >
                    {plantIds.map((p) => (
                      <option key={p} value={p}>
                        {plantDisplayName(p)} · {p}
                      </option>
                    ))}
                  </select>
                ) : (
                  <Input value={`${plantDisplayName(plantId)} · ${plantId}`} readOnly />
                )}
              </label>
              <label className="space-y-1 text-sm">
                <span className="text-[var(--text-secondary)]">{t('wizard.cnt.warehouse')}</span>
                <select
                  className="h-10 w-full rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 text-sm"
                  value={warehouseCode}
                  onChange={(e) => {
                    setWarehouseCode(e.target.value);
                    setZone('');
                  }}
                >
                  <option value="">{t('wizard.cnt.warehousePlaceholder')}</option>
                  {warehouses.map((w) => (
                    <option key={w.code} value={w.code}>
                      {w.name || w.code} · {w.code}
                    </option>
                  ))}
                </select>
              </label>
              <label className="space-y-1 text-sm">
                <span className="text-[var(--text-secondary)]">{t('wizard.cnt.zone')}</span>
                <select
                  className="h-10 w-full rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 text-sm"
                  value={zone}
                  onChange={(e) => setZone(e.target.value)}
                  disabled={!warehouseCode}
                >
                  <option value="">{t('wizard.cnt.zoneAll')}</option>
                  {zones.map((z) => (
                    <option key={z.code} value={z.code}>
                      {z.name || z.code} · {z.code}
                    </option>
                  ))}
                </select>
              </label>
              <label className="space-y-1 text-sm">
                <span className="text-[var(--text-secondary)]">{t('wizard.cnt.abc')}</span>
                <select
                  className="h-10 w-full rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 text-sm"
                  value={abcClass}
                  onChange={(e) => setAbcClass(e.target.value)}
                >
                  <option value="">{t('wizard.cnt.abcAll')}</option>
                  <option value="A">A</option>
                  <option value="B">B</option>
                  <option value="C">C</option>
                </select>
              </label>
            </div>
          ) : null}

          {step === 'open' ? (
            <div className="space-y-4">
              <p className="text-sm text-[var(--text-secondary)]">{t('wizard.cnt.openBody')}</p>
              <div className="grid gap-3 md:grid-cols-2">
                <label className="space-y-1 text-sm">
                  <span className="text-[var(--text-secondary)]">{t('wizard.cnt.countType')}</span>
                  <select
                    className="h-10 w-full rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 text-sm"
                    value={countType}
                    disabled={Boolean(opened)}
                    onChange={(e) => setCountType(e.target.value as CountType)}
                  >
                    {COUNT_TYPES.map((c) => (
                      <option key={c} value={c}>
                        {t(`wizard.cnt.type${c}`)}
                      </option>
                    ))}
                  </select>
                </label>
                <label className="space-y-1 text-sm">
                  <span className="text-[var(--text-secondary)]">{t('wizard.cnt.countDate')}</span>
                  <Input type="date" value={countDate} disabled={Boolean(opened)} onChange={(e) => setCountDate(e.target.value)} />
                </label>
                <label className="space-y-1 text-sm">
                  <span className="text-[var(--text-secondary)]">{t('wizard.cnt.assignedTo')}</span>
                  <Input
                    value={assignedTo}
                    disabled={Boolean(opened)}
                    placeholder={t('wizard.cnt.assignedPlaceholder')}
                    onChange={(e) => setAssignedTo(e.target.value)}
                  />
                </label>
                <label className="space-y-1 text-sm">
                  <span className="text-[var(--text-secondary)]">{t('wizard.cnt.freeze')}</span>
                  <select
                    className="h-10 w-full rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 text-sm"
                    value={freezeMode}
                    disabled={Boolean(opened)}
                    onChange={(e) => setFreezeMode(e.target.value as FreezeMode)}
                  >
                    {FREEZE_MODES.map((m) => (
                      <option key={m} value={m}>
                        {t(`wizard.cnt.freeze${m}`)}
                      </option>
                    ))}
                  </select>
                </label>
                <label className="flex items-center gap-2 text-sm md:col-span-2">
                  <input
                    type="checkbox"
                    checked={blindCount}
                    disabled={Boolean(opened) || countType === 'Blind'}
                    onChange={(e) => setBlindCount(e.target.checked)}
                  />
                  <span>{t('wizard.cnt.blind')}</span>
                </label>
              </div>
              {opened ? (
                <p className="text-sm font-medium text-[var(--color-primary)]">
                  {t('wizard.cnt.alreadyOpen')} {opened.number} · {opened.status}
                </p>
              ) : (
                <Button type="button" disabled={!gate.ok || !canOpenDoc || openMutation.isPending} onClick={() => openMutation.mutate()}>
                  {openMutation.isPending ? t('saving') : t('wizard.cnt.openAction')}
                </Button>
              )}
              {!opened && !gate.ok ? <p className="text-sm text-[var(--text-muted)]">{gate.reason}</p> : null}
            </div>
          ) : null}

          {step === 'count' ? (
            <div className="space-y-3">
              <p className="text-sm text-[var(--text-secondary)]">{t('wizard.cnt.countBody')}</p>
              <p className="font-mono text-sm text-[var(--text-primary)]">
                {opened?.number ?? t('wizard.cnt.countViewWithoutDoc')}
              </p>
              <div className="flex flex-wrap gap-2">
                <Input
                  className="max-w-xs"
                  value={scan}
                  placeholder={t('wizard.cnt.countScanPh')}
                  onChange={(e) => setScan(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === 'Enter') applyScan();
                  }}
                />
                <Button type="button" variant="secondary" onClick={applyScan}>
                  {t('wizard.cnt.countScan')}
                </Button>
                <Button type="button" variant="secondary" onClick={addEmptyLine}>
                  {t('wizard.cnt.countAdd')}
                </Button>
              </div>
              {lines.length === 0 ? (
                <p className="text-sm text-[var(--text-muted)]">{t('wizard.cnt.countEmpty')}</p>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full min-w-[640px] text-left text-sm">
                    <thead>
                      <tr className="border-b border-[var(--border-default)] text-[var(--text-muted)]">
                        <th className="py-2 pr-3 font-medium">{t('wizard.cnt.countColMaterial')}</th>
                        <th className="py-2 pr-3 font-medium">{t('wizard.cnt.countColLoc')}</th>
                        <th className="py-2 pr-3 font-medium">{t('wizard.cnt.countColLot')}</th>
                        {showSys ? <th className="py-2 pr-3 font-medium">{t('wizard.cnt.countColSystem')}</th> : null}
                        <th className="py-2 pr-3 font-medium">{t('wizard.cnt.countColCounted')}</th>
                        {showSys ? <th className="py-2 font-medium">{t('wizard.cnt.countColVar')}</th> : null}
                      </tr>
                    </thead>
                    <tbody>
                      {lines.map((line) => {
                        const v = lineVariance(line);
                        return (
                          <tr key={line.key} className="border-b border-[var(--border-default)]">
                            <td className="py-1.5 pr-3">
                              <Input value={line.materialCode} onChange={(e) => patchLine(line.key, { materialCode: e.target.value })} />
                            </td>
                            <td className="py-1.5 pr-3">
                              <Input value={line.locationCode} onChange={(e) => patchLine(line.key, { locationCode: e.target.value })} />
                            </td>
                            <td className="py-1.5 pr-3">
                              <Input value={line.lotNumber} onChange={(e) => patchLine(line.key, { lotNumber: e.target.value })} />
                            </td>
                            {showSys ? <td className="py-1.5 pr-3 font-mono">{line.systemQty}</td> : null}
                            <td className="py-1.5 pr-3">
                              <Input
                                id={`cnt-qty-${line.key}`}
                                type="number"
                                value={line.countedQty}
                                onChange={(e) => patchLine(line.key, { countedQty: e.target.value })}
                              />
                            </td>
                            {showSys ? (
                              <td className={`py-1.5 font-mono ${v && v !== 0 ? 'text-[var(--color-danger)]' : ''}`}>
                                {v === null ? '—' : v}
                              </td>
                            ) : null}
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>
              )}
              <Button type="button" disabled={!canSave || !opened || saveLinesMutation.isPending} onClick={() => saveLinesMutation.mutate()}>
                {saveLinesMutation.isPending ? t('saving') : t('wizard.cnt.countSave')}
              </Button>
              {!canSave ? <p className="text-sm text-[var(--text-muted)]">{t('wizard.cnt.countSaveDenied')}</p> : null}
              {savedNote ? <p className="text-sm text-[var(--color-primary)]">{savedNote}</p> : null}
            </div>
          ) : null}

          {step === 'variance' ? (
            <div className="space-y-3 text-sm">
              <p className="text-[var(--text-secondary)]">{t('wizard.cnt.varianceBody')}</p>
              <p className="text-[var(--text-primary)]">
                {totals.counted} sayılan · {totals.differed} fark
              </p>
              {totals.counted === 0 ? (
                <p className="text-[var(--text-muted)]">{t('wizard.cnt.varianceNone')}</p>
              ) : (
                <ul className="space-y-1">
                  {lines
                    .filter((l) => {
                      const v = lineVariance(l);
                      return v !== null && v !== 0;
                    })
                    .map((l) => (
                      <li key={l.key} className="font-mono">
                        {l.materialCode} · {l.locationCode} · {showSys ? lineVariance(l) : t('wizard.cnt.countColCounted')}
                      </li>
                    ))}
                </ul>
              )}
            </div>
          ) : null}

          {step === 'close' ? (
            <div className="space-y-3 text-sm">
              <p className="text-[var(--text-secondary)]">{t('wizard.cnt.closeBody')}</p>
              <p className="font-medium text-[var(--text-primary)]">
                {opened ? `${opened.number} · ${opened.status}` : t('wizard.cnt.countViewWithoutDoc')}
              </p>
              <Button type="button" disabled={!canSave || !opened || closeMutation.isPending} onClick={() => closeMutation.mutate()}>
                {closeMutation.isPending ? t('saving') : t('wizard.cnt.closeAction')}
              </Button>
            </div>
          ) : null}

          {error ? <p className="text-sm text-[var(--color-danger)]">{error}</p> : null}

          <div className="flex flex-wrap gap-2">
            <Button type="button" variant="secondary" disabled={stepIndex === 0} onClick={() => setStep(STEPS[Math.max(0, stepIndex - 1)])}>
              {t('wizard.back')}
            </Button>
            {stepIndex < STEPS.length - 1 ? (
              <Button type="button" onClick={() => setStep(STEPS[stepIndex + 1])}>
                {t('wizard.next')}
              </Button>
            ) : (
              <Link to="/inventory/counts/cycle-counts">
                <Button type="button" variant="secondary">
                  {t('wizard.backToLibrary')}
                </Button>
              </Link>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
