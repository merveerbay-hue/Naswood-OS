import { Button, Input } from '@naswood/ui';
import { useQuery } from '@tanstack/react-query';
import { useMemo } from 'react';
import { searchAllResource } from '@/api/business';
import { useAuth } from '@/auth/useAuth';
import { useI18n } from '@/i18n';
import { plantDisplayName } from '@/modules/inventory/locations/locationCatalog';
import {
  finalLineQty,
  formatDocumentDims,
  lineMaterialMatched,
  stockBasisQty,
  stockBasisVolumeM3,
  type IncomingLine,
} from './incomingLines';
import {
  acceptedStockLines,
  addSplitRow,
  distributionDimsLabel,
  distributionSumForLine,
  removeDistributionRow,
  sliceVolumeM3,
  validateDistributions,
  type StockBucket,
  type StockDistributionRow,
} from './stockDistribution';
import { WAREHOUSE_CATALOG } from '@/modules/inventory/warehouses/warehouseCatalog';

type WarehouseOpt = { code?: string; name?: string; status?: string; plantId?: string };
type LocationOpt = { code?: string; name?: string; warehouseCode?: string; status?: string };

type Props = {
  lines: IncomingLine[];
  distributions: StockDistributionRow[];
  onDistributionsChange: (next: StockDistributionRow[]) => void;
  posted: boolean;
  postedDocumentNumber?: string;
  postBlockedReason: string | null;
  disabled?: boolean;
  lotNumber: string;
  lotTracking: boolean;
  onLotTrackingChange: (v: boolean) => void;
  lotNote: string;
  onLotNoteChange: (v: string) => void;
};

/** Stage 6 — Stoklaştırma: satır/grup bazlı depo·lokasyon dağıtımı + çift stok engeli. */
export function StockStep({
  lines,
  distributions,
  onDistributionsChange,
  posted,
  postedDocumentNumber,
  postBlockedReason,
  disabled,
  lotNumber,
  lotTracking,
  onLotTrackingChange,
  lotNote,
  onLotNoteChange,
}: Props) {
  const { t } = useI18n();
  const { user } = useAuth();
  const workingPlantId = user?.plantId || user?.homePlantId || 'PLANT-001';
  const homePlantId = user?.homePlantId || workingPlantId;
  const accepted = acceptedStockLines(lines);
  const validation = validateDistributions(lines, distributions);

  const warehousesQuery = useQuery({
    queryKey: ['business', 'warehouses', 'rcv', workingPlantId],
    queryFn: () => searchAllResource<WarehouseOpt>('warehouses', undefined, { plantId: workingPlantId }),
  });
  const locationsQuery = useQuery({
    queryKey: ['business', 'locations', 'rcv', workingPlantId],
    queryFn: () => searchAllResource<LocationOpt>('locations', undefined, { plantId: workingPlantId }),
  });

  const activeWarehouses = useMemo(() => {
    const fromApi = (warehousesQuery.data ?? []).filter(
      (w) => String(w.status ?? 'Active').toLowerCase() === 'active',
    );
    if (fromApi.length > 0) return fromApi;
    // Fallback catalog codes until plant warehouses are opened.
    return WAREHOUSE_CATALOG.map((w) => ({ code: w.warehouseCode, name: w.warehouseName, status: 'Active' }));
  }, [warehousesQuery.data]);

  const locationsByWh = useMemo(() => {
    const map = new Map<string, LocationOpt[]>();
    for (const loc of locationsQuery.data ?? []) {
      if (String(loc.status ?? 'Active').toLowerCase() !== 'active') continue;
      const wh = String(loc.warehouseCode ?? '').toUpperCase();
      if (!map.has(wh)) map.set(wh, []);
      map.get(wh)!.push(loc);
    }
    return map;
  }, [locationsQuery.data]);

  function updateRow(id: string, patch: Partial<StockDistributionRow>) {
    onDistributionsChange(distributions.map((d) => (d.id === id ? { ...d, ...patch } : d)));
  }

  function setBucket(id: string, bucket: StockBucket) {
    updateRow(id, {
      bucket,
      locationCode: bucket === 'quarantine' ? 'K-01' : 'A-03',
      warehouseCode: bucket === 'quarantine' ? 'WH-QA' : 'WH-RM',
    });
  }

  return (
    <div className="space-y-5">
      <div className="space-y-1">
        <h3 className="text-base font-semibold tracking-tight">{t('wb.rcv.stockStep.title')}</h3>
        <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.stockStep.intro')}</p>
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.stockStep.rules')}</p>
        <p className="text-xs text-[var(--text-muted)]">
          Ana Üs: {plantDisplayName(homePlantId)} ({homePlantId})
          {workingPlantId.toUpperCase() !== homePlantId.toUpperCase()
            ? ` · Çalışma tesisi: ${plantDisplayName(workingPlantId)} (${workingPlantId})`
            : ''}
          {' · '}Depo/lokasyon yalnızca bu tesisin aktif kayıtlarından.
        </p>
      </div>


      <div className="rounded-lg border border-[var(--border-default)] bg-[var(--color-surface)] p-3 space-y-3">
        <p className="text-[10px] font-semibold uppercase tracking-wide text-[var(--text-muted)]">
          {t('wb.rcv.lot.title')}
        </p>
        <label className="flex items-center gap-2 text-sm font-medium">
          <input
            type="checkbox"
            checked={lotTracking}
            disabled={disabled || posted}
            onChange={(e) => onLotTrackingChange(e.target.checked)}
          />
          {t('wb.rcv.lot.tracking')}
        </label>
        <div>
          <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('wb.rcv.lot.number')}</p>
          <p className="font-mono text-sm font-medium">{lotTracking ? lotNumber : '—'}</p>
          <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.lot.autoHint')}</p>
        </div>
        <label className="block space-y-1">
          <span className="text-xs font-medium text-[var(--text-muted)]">{t('wb.rcv.lot.note')}</span>
          <Input
            value={lotNote}
            disabled={disabled || posted}
            onChange={(e) => onLotNoteChange(e.target.value)}
            placeholder={t('wb.rcv.lot.notePh')}
          />
        </label>
      </div>

      {posted ? (
        <p className="rounded-md border border-[var(--color-primary)]/40 bg-[var(--color-primary)]/5 px-3 py-2 text-sm font-medium text-[var(--color-primary)]">
          {t('wb.rcv.postedBanner')}
          {postedDocumentNumber ? ` · ${postedDocumentNumber}` : ''}
          <span className="mt-1 block text-xs font-normal text-[var(--text-muted)]">
            {t('wb.rcv.stockStep.idempotentNote')}
          </span>
        </p>
      ) : null}

      {accepted.length === 0 ? (
        <p className="text-sm text-[var(--color-danger)]">{t('wb.rcv.stockStep.noAccepted')}</p>
      ) : (
        <div className="space-y-4">
          <datalist id="rcv-wh-catalog">
            {activeWarehouses.map((w) => (
              <option key={String(w.code)} value={String(w.code)}>
                {String(w.name ?? w.code)}
              </option>
            ))}
          </datalist>
          <datalist id="rcv-loc-catalog">
            {(locationsQuery.data ?? [])
              .filter((l) => String(l.status ?? 'Active').toLowerCase() === 'active')
              .map((l) => (
                <option
                  key={`${l.warehouseCode}-${l.code}`}
                  value={String(l.code)}
                >
                  {String(l.warehouseCode)} · {String(l.name ?? l.code)}
                </option>
              ))}
          </datalist>
          {accepted.map((line) => {
            const lineDists = distributions.filter((d) => d.lineId === line.id);
            const acceptedQty = stockBasisQty(line);
            const sum = distributionSumForLine(distributions, line.id);
            const over = sum > acceptedQty + 1e-9;
            const under = sum < acceptedQty - 1e-9;
            const vol = stockBasisVolumeM3(line);

            return (
              <section
                key={line.id}
                className="space-y-3 rounded-md border border-[var(--border-default)] px-3 py-3"
              >
                <div className="flex flex-wrap items-start justify-between gap-2">
                  <div>
                    <p className="text-sm font-semibold">{line.name}</p>
                    <p className="font-mono text-xs text-[var(--text-muted)]">
                      {lineMaterialMatched(line) ? line.matchedMaterialCode : '—'} ·{' '}
                      {t('wb.rcv.stockStep.cardDims')}: {formatDocumentDims(line)}
                    </p>
                    <p className="text-xs text-[var(--text-secondary)]">
                      {t('wb.rcv.stockStep.accepted')}:{' '}
                      <span className="font-medium tabular-nums">
                        {acceptedQty} {line.unit}
                      </span>
                      {' · '}
                      {t('wb.rcv.stockStep.physical')}: {finalLineQty(line)} {line.unit}
                      {vol != null ? ` · ${vol.toFixed(3)} m³` : ''}
                    </p>
                  </div>
                  <p
                    className={`text-xs font-medium tabular-nums ${
                      over
                        ? 'text-[var(--color-danger)]'
                        : under
                          ? 'text-[var(--color-warning,var(--color-danger))]'
                          : 'text-[var(--color-primary)]'
                    }`}
                  >
                    {t('wb.rcv.stockStep.distSum')
                      .replace('{sum}', String(sum))
                      .replace('{accepted}', String(acceptedQty))}
                    {over ? ` · ${t('wb.rcv.stockStep.over')}` : null}
                    {under ? ` · ${t('wb.rcv.stockStep.under')}` : null}
                    {!over && !under ? ` · ✓` : null}
                  </p>
                </div>

                <div className="overflow-x-auto">
                  <table className="w-full min-w-[720px] text-left text-sm">
                    <thead className="text-[10px] uppercase text-[var(--text-muted)]">
                      <tr>
                        <th className="px-2 py-1">{t('wb.rcv.stockStep.col.dims')}</th>
                        <th className="px-2 py-1">{t('wb.rcv.stockStep.col.qty')}</th>
                        <th className="px-2 py-1">m³</th>
                        <th className="px-2 py-1">{t('wb.rcv.whPick')}</th>
                        <th className="px-2 py-1">{t('wb.rcv.locPick')}</th>
                        <th className="px-2 py-1">{t('wb.rcv.stockStep.col.bucket')}</th>
                        <th className="px-2 py-1" />
                      </tr>
                    </thead>
                    <tbody>
                      {lineDists.map((d) => {
                        const group = d.groupId
                          ? (line.physicalGroups.find((g) => g.id === d.groupId) ?? null)
                          : null;
                        const sliceVol = sliceVolumeM3(line, group, parseQtySafe(d.qty));
                        return (
                          <tr key={d.id} className="border-t border-[var(--border-default)] align-top">
                            <td className="px-2 py-2 text-xs">{distributionDimsLabel(line, d.groupId)}</td>
                            <td className="px-2 py-2">
                              <Input
                                className="w-20"
                                value={d.qty}
                                disabled={disabled || posted}
                                onChange={(e) => updateRow(d.id, { qty: e.target.value })}
                              />
                            </td>
                            <td className="px-2 py-2 text-xs tabular-nums text-[var(--text-muted)]">
                              {sliceVol != null ? sliceVol.toFixed(3) : '—'}
                            </td>
                            <td className="px-2 py-2">
                              <Input
                                className="w-28"
                                value={d.warehouseCode}
                                disabled={disabled || posted}
                                list="rcv-wh-catalog"
                                onChange={(e) =>
                                  updateRow(d.id, {
                                    warehouseCode: e.target.value,
                                    // Clear location when WH changes if it no longer belongs.
                                    locationCode: locationsByWh
                                      .get(e.target.value.trim().toUpperCase())
                                      ?.some(
                                        (l) =>
                                          String(l.code).toUpperCase() ===
                                          d.locationCode.trim().toUpperCase(),
                                      )
                                      ? d.locationCode
                                      : d.locationCode,
                                  })
                                }
                              />
                            </td>
                            <td className="px-2 py-2">
                              <Input
                                className="w-24"
                                value={d.locationCode}
                                disabled={disabled || posted}
                                list="rcv-loc-catalog"
                                onChange={(e) => updateRow(d.id, { locationCode: e.target.value })}
                              />
                            </td>
                            <td className="px-2 py-2">
                              <select
                                className="h-9 rounded-md border border-[var(--border-default)] bg-transparent px-2 text-xs"
                                value={d.bucket}
                                disabled={disabled || posted}
                                onChange={(e) => setBucket(d.id, e.target.value as StockBucket)}
                              >
                                <option value="available">{t('wb.rcv.stockStep.bucket.available')}</option>
                                <option value="quarantine">{t('wb.rcv.stockStep.bucket.quarantine')}</option>
                              </select>
                            </td>
                            <td className="px-2 py-2">
                              <div className="flex flex-col gap-1">
                                <Button
                                  type="button"
                                  variant="secondary"
                                  className="h-7 px-2 text-[10px]"
                                  disabled={disabled || posted || parseQtySafe(d.qty) < 2}
                                  onClick={() => onDistributionsChange(addSplitRow(distributions, d))}
                                >
                                  {t('wb.rcv.stockStep.split')}
                                </Button>
                                {lineDists.length > 1 ? (
                                  <Button
                                    type="button"
                                    variant="secondary"
                                    className="h-7 px-2 text-[10px]"
                                    disabled={disabled || posted}
                                    onClick={() =>
                                      onDistributionsChange(removeDistributionRow(distributions, d.id))
                                    }
                                  >
                                    {t('wb.rcv.stockStep.remove')}
                                  </Button>
                                ) : null}
                              </div>
                            </td>
                          </tr>
                        );
                      })}
                    </tbody>
                  </table>
                </div>
              </section>
            );
          })}
        </div>
      )}

      {!posted && !validation.ok && validation.code === 'over' ? (
        <p className="text-sm text-[var(--color-danger)]">{t('wb.rcv.stockStep.errOver')}</p>
      ) : null}
      {!posted && !validation.ok && validation.code === 'under' ? (
        <p className="text-sm text-[var(--color-danger)]">{t('wb.rcv.stockStep.errUnder')}</p>
      ) : null}
      {!posted && !validation.ok && validation.code === 'missingWh' ? (
        <p className="text-sm text-[var(--color-danger)]">{t('wb.rcv.stockStep.errWh')}</p>
      ) : null}

      {postBlockedReason && !posted ? (
        <p className="rounded-md border border-[var(--color-danger)]/40 bg-[var(--color-danger)]/5 px-3 py-2 text-sm text-[var(--color-danger)]">
          {postBlockedReason}
        </p>
      ) : null}

      {!posted && validation.ok && !postBlockedReason ? (
        <p className="rounded-md border border-[var(--border-default)] bg-[var(--color-surface-hover)] px-3 py-2 text-sm">
          {t('wb.rcv.stockStep.ready')
            .replace('{n}', String(distributions.length))
            .replace('{lines}', String(accepted.length))}
        </p>
      ) : null}
    </div>
  );
}

function parseQtySafe(v: string): number {
  const n = Number(String(v).replace(',', '.'));
  return Number.isFinite(n) && n > 0 ? n : 0;
}
