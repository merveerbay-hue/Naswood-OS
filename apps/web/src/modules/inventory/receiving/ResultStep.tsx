import { Input } from '@naswood/ui';
import { useI18n } from '@/i18n';
import {
  finalLineQty,
  formatDocumentDims,
  formatPhysicalDims,
  lineMaterialMatched,
  linePhysicalVolumeM3,
  stockBasisDimsLabel,
  stockBasisQty,
  stockBasisVolumeM3,
  type IncomingLine,
} from './incomingLines';
import type { MaterialCheckState } from './MaterialCheckStep';
import type { TruckInfo } from './TruckEvidenceStep';

type Props = {
  truck: TruckInfo;
  docsCount: number;
  photoCount: number;
  lines: IncomingLine[];
  materialCheck: MaterialCheckState;
  warehouse: string;
  location: string;
  onWarehouseChange: (v: string) => void;
  onLocationChange: (v: string) => void;
  approved: boolean;
  onApprovedChange: (v: boolean) => void;
  canApprove: boolean;
  disabled?: boolean;
};

/** Stage 5 — Sonuç: izlenebilir özet + depo/lokasyon + stok onayı. */
export function ResultStep({
  truck,
  docsCount,
  photoCount,
  lines,
  materialCheck,
  warehouse,
  location,
  onWarehouseChange,
  onLocationChange,
  approved,
  onApprovedChange,
  canApprove,
  disabled,
}: Props) {
  const { t } = useI18n();

  const stockLines = lines.filter(
    (l) =>
      (l.preAccept === 'ok' || l.preAccept === 'conditional') &&
      l.countStatus === 'counted' &&
      l.stockAccept !== 'reject',
  );
  const rejected = lines.filter((l) => l.preAccept === 'reject' || l.stockAccept === 'reject');

  function stockAcceptLabel(line: IncomingLine): string {
    if (line.stockAccept === 'conditional') return t('wb.rcv.compare.disp.conditional');
    if (line.stockAccept === 'ok') return t('wb.rcv.compare.disp.accepted');
    if (line.stockAccept === 'reject') return t('wb.rcv.compare.disp.rejected');
    if (line.preAccept === 'conditional') return t('wb.rcv.ops.preAccept.conditional');
    return t('wb.rcv.compare.disp.accepted');
  }

  return (
    <div className="space-y-5">
      <div className="space-y-1">
        <h3 className="text-base font-semibold tracking-tight">{t('wb.rcv.result.title')}</h3>
        <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.result.intro')}</p>
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.result.traceNote')}</p>
      </div>

      <section className="grid gap-2 sm:grid-cols-2 lg:grid-cols-3">
        {[
          [t('wb.rcv.truck'), `${truck.plate} · ${truck.trailer || '—'} · ${truck.driver || '—'}`],
          [t('wb.rcv.supplier'), truck.supplier || '—'],
          [t('wb.rcv.ops.evidenceSummary'), `${docsCount} belge · ${photoCount} foto`],
          [
            t('wb.rcv.ops.check.quality'),
            materialCheck.qualityVerdict === 'none'
              ? '—'
              : t(`wb.rcv.ops.check.qualityVerdict.${materialCheck.qualityVerdict}`),
          ],
          [t('wb.rcv.result.stockLineCount'), String(stockLines.length)],
          [t('wb.rcv.result.rejectedCount'), String(rejected.length)],
        ].map(([label, value]) => (
          <div key={label} className="rounded-md border border-[var(--border-default)] px-3 py-2">
            <p className="text-[10px] uppercase text-[var(--text-muted)]">{label}</p>
            <p className="text-sm font-medium">{value}</p>
          </div>
        ))}
      </section>

      <section className="space-y-2">
        <h4 className="text-sm font-semibold tracking-tight">{t('wb.rcv.result.stockBasisTitle')}</h4>
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.result.stockBasisHint')}</p>
        {stockLines.length === 0 ? (
          <p className="text-sm text-[var(--color-danger)]">{t('wb.rcv.result.noStockLines')}</p>
        ) : (
          <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
            <table className="w-full min-w-[860px] text-left text-sm">
              <thead className="bg-[var(--color-surface-hover)] text-[10px] uppercase text-[var(--text-muted)]">
                <tr>
                  <th className="px-3 py-2">{t('wb.rcv.result.col.product')}</th>
                  <th className="px-3 py-2">{t('wb.rcv.result.col.card')}</th>
                  <th className="px-3 py-2">{t('wb.rcv.result.col.doc')}</th>
                  <th className="px-3 py-2">{t('wb.rcv.result.col.physical')}</th>
                  <th className="px-3 py-2">{t('wb.rcv.result.col.stock')}</th>
                  <th className="px-3 py-2">{t('wb.rcv.result.col.decision')}</th>
                </tr>
              </thead>
              <tbody>
                {stockLines.map((line) => {
                  const qty = stockBasisQty(line) || finalLineQty(line);
                  const vol = stockBasisVolumeM3(line) ?? linePhysicalVolumeM3(line);
                  return (
                    <tr key={line.id} className="border-t border-[var(--border-default)] align-top">
                      <td className="px-3 py-2 font-medium">{line.name}</td>
                      <td className="px-3 py-2 text-xs">
                        <p className="font-mono">{lineMaterialMatched(line) ? line.matchedMaterialCode : '—'}</p>
                        <p className="text-[var(--text-muted)]">{formatDocumentDims(line)}</p>
                        <p className="text-[10px] text-[var(--text-muted)]">{t('wb.rcv.ops.count.cardDimsUnchanged')}</p>
                      </td>
                      <td className="px-3 py-2 text-xs tabular-nums">
                        <p>
                          {line.documentQty} {line.unit}
                        </p>
                        <p className="text-[var(--text-muted)]">{formatDocumentDims(line)}</p>
                      </td>
                      <td className="px-3 py-2 text-xs tabular-nums">
                        <p className="font-medium">
                          {finalLineQty(line)} {line.unit}
                        </p>
                        <p className="text-[var(--text-muted)]">{formatPhysicalDims(line)}</p>
                        <p className="text-[var(--text-muted)]">
                          {vol != null ? `${vol.toFixed(3)} m³` : '—'}
                        </p>
                      </td>
                      <td className="px-3 py-2 text-xs tabular-nums">
                        <p className="font-semibold">
                          {qty} {line.unit}
                        </p>
                        <p className="font-medium">{stockBasisDimsLabel(line)}</p>
                        <p className="font-medium">
                          {vol != null ? `${vol.toFixed(3)} m³` : '—'}
                        </p>
                      </td>
                      <td className="px-3 py-2 text-xs">
                        <p>{stockAcceptLabel(line)}</p>
                        {line.stockAcceptReason ? (
                          <p className="text-[var(--text-muted)]">{line.stockAcceptReason}</p>
                        ) : null}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </section>

      {rejected.length > 0 ? (
        <section className="space-y-1">
          <h4 className="text-sm font-semibold tracking-tight">{t('wb.rcv.result.rejectedTitle')}</h4>
          <ul className="text-sm text-[var(--text-secondary)]">
            {rejected.map((l) => (
              <li key={l.id}>
                {l.name} · {formatDocumentDims(l)} — {t('wb.rcv.result.excludedFromStock')}
              </li>
            ))}
          </ul>
        </section>
      ) : null}

      <section className="space-y-3 rounded-md border border-[var(--border-default)] px-3 py-3">
        <h4 className="text-sm font-semibold tracking-tight">{t('wb.rcv.result.whTitle')}</h4>
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.result.whHint')}</p>
        <div className="grid gap-2 sm:grid-cols-2">
          <label className="block space-y-1 text-sm">
            <span className="text-[var(--text-secondary)]">{t('wb.rcv.whPick')}</span>
            <Input value={warehouse} disabled={disabled} onChange={(e) => onWarehouseChange(e.target.value)} />
          </label>
          <label className="block space-y-1 text-sm">
            <span className="text-[var(--text-secondary)]">{t('wb.rcv.locPick')}</span>
            <Input value={location} disabled={disabled} onChange={(e) => onLocationChange(e.target.value)} />
          </label>
        </div>
      </section>

      <label className="flex items-start gap-2 rounded-md border border-[var(--border-default)] px-3 py-3 text-sm font-medium">
        <input
          type="checkbox"
          className="mt-0.5"
          checked={approved}
          disabled={disabled || !canApprove}
          onChange={(e) => onApprovedChange(e.target.checked)}
        />
        <span>
          {t('wb.rcv.result.approveLabel')}
          <span className="mt-1 block text-xs font-normal text-[var(--text-muted)]">
            {t('wb.rcv.result.approveHint')}
          </span>
        </span>
      </label>

      {!warehouse.trim() || !location.trim() ? (
        <p className="text-xs text-[var(--color-danger)]">{t('wb.rcv.result.needWh')}</p>
      ) : null}
    </div>
  );
}
