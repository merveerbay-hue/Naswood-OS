import { Link } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { createResource, searchAllResource } from '@/api/business';
import { usePlantContext } from '@/auth/usePlantContext';
import { useI18n } from '@/i18n';
import { plantDisplayName } from '@/modules/inventory/locations/locationCatalog';
import {
  COUNT_TYPES,
  FREEZE_MODES,
  buildCountSessionCreateBody,
  canOpenCountSession,
  type CountType,
  type FreezeMode,
} from './cycleCountSession';

type WarehouseOpt = { code?: string; name?: string; status?: string };
type LocationOpt = { code?: string; name?: string; warehouseCode?: string; status?: string };
type OpenedSession = { id: string; number: string; warehouseCode: string; status: string; plantId?: string };

const STEPS = ['scope', 'open', 'count', 'variance', 'close'] as const;
type StepId = (typeof STEPS)[number];

/**
 * INV-CNT-001 — Cycle Count Session wizard.
 * Step 2 opens the session: business fields only; CNT-… is minted by numbering.
 */
export function CycleCountSessionPage() {
  const { t } = useI18n();
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
  const [opened, setOpened] = useState<OpenedSession | null>(null);
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

  const openMutation = useMutation({
    mutationFn: async () => {
      if (!gate.ok) throw new Error(gate.reason);
      return createResource<OpenedSession>('inventory-counts', buildCountSessionCreateBody(draft), { plantId });
    },
    onSuccess: async (row) => {
      setError(null);
      setOpened(row);
      await queryClient.invalidateQueries({ queryKey: ['business', 'inventory-counts'] });
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
                  : i < stepIndex || opened
                    ? 'bg-[var(--color-surface-hover)] text-[var(--text-primary)]'
                    : 'bg-[var(--color-surface)] text-[var(--text-muted)]'
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
          <CardDescription>
            {step === 'open' ? t('wizard.cnt.openHint') : t('wizard.cnt.stepHint')}
          </CardDescription>
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
              {!warehousesQuery.isLoading && warehouses.length === 0 ? (
                <p className="md:col-span-2 text-sm text-[var(--color-danger)]">{t('wizard.cnt.noWarehouse')}</p>
              ) : null}
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
                  <Input
                    type="date"
                    value={countDate}
                    disabled={Boolean(opened)}
                    onChange={(e) => setCountDate(e.target.value)}
                  />
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

              <div className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 py-3 text-sm">
                <p className="font-medium">{t('wizard.cnt.gateTitle')}</p>
                <ul className="mt-2 list-disc space-y-1 pl-5 text-[var(--text-secondary)]">
                  <li>
                    {warehouseCode
                      ? `${t('wizard.cnt.gateScopeOk')} ${warehouseCode}`
                      : t('wizard.cnt.gateScopeMissing')}
                  </li>
                  <li>{t('wizard.cnt.gateNumbering')}</li>
                  <li>{t('wizard.cnt.gateBlind')}</li>
                  <li>{t('wizard.cnt.gateAdjust')}</li>
                  <li>
                    {t('wizard.cnt.gatePlant')} {plantId}
                  </li>
                </ul>
              </div>

              {opened ? (
                <p className="text-sm font-medium text-[var(--color-primary)]">
                  {t('wizard.cnt.openedBanner')} {opened.number} · {opened.status}
                </p>
              ) : (
                <Button
                  type="button"
                  disabled={!gate.ok || openMutation.isPending}
                  onClick={() => openMutation.mutate()}
                >
                  {openMutation.isPending ? t('saving') : t('wizard.cnt.openAction')}
                </Button>
              )}
              {!gate.ok ? <p className="text-sm text-[var(--text-muted)]">{gate.reason}</p> : null}
            </div>
          ) : null}

          {step === 'count' ? (
            <div className="space-y-2 text-sm text-[var(--text-secondary)]">
              <p>{t('wizard.cnt.countBody')}</p>
              <p className="font-mono text-[var(--text-primary)]">
                {opened ? opened.number : t('wizard.cnt.needOpen')}
              </p>
            </div>
          ) : null}

          {step === 'variance' ? (
            <div className="space-y-2 text-sm text-[var(--text-secondary)]">
              <p>{t('wizard.cnt.varianceBody')}</p>
            </div>
          ) : null}

          {step === 'close' ? (
            <div className="space-y-2 text-sm text-[var(--text-secondary)]">
              <p>{t('wizard.cnt.closeBody')}</p>
              {opened ? (
                <p className="font-medium text-[var(--text-primary)]">
                  {opened.number} · {opened.status}
                </p>
              ) : null}
            </div>
          ) : null}

          {error ? <p className="text-sm text-[var(--color-danger)]">{error}</p> : null}

          <div className="flex flex-wrap gap-2">
            <Button
              type="button"
              variant="secondary"
              disabled={stepIndex === 0}
              onClick={() => setStep(STEPS[Math.max(0, stepIndex - 1)])}
            >
              {t('wizard.back')}
            </Button>
            {stepIndex < STEPS.length - 1 ? (
              <Button
                type="button"
                disabled={step === 'scope' && !warehouseCode}
                onClick={() => setStep(STEPS[stepIndex + 1])}
              >
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
