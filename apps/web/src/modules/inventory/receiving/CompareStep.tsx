import { useEffect, useMemo, useState } from 'react';
import { Button, Input } from '@naswood/ui';
import { useI18n } from '@/i18n';
import type { IncomingLine, StockAcceptDecision } from './incomingLines';
import {
  buildCompareRows,
  compareExceptions,
  compareReadyForResult,
  compareSummary,
  formatQty,
  formatVolume,
  type CompareDisposition,
  type CompareFilter,
  type CompareRow,
} from './compareLines';

type Props = {
  lines: IncomingLine[];
  onChange: (next: IncomingLine[]) => void;
  disabled?: boolean;
  supplier?: string;
  onResolvedChange: (resolved: boolean) => void;
  onRequestMaterialMatch?: (lineId: string) => void;
};

const STATUS_DOT: Record<CompareRow['status'], string> = {
  ok: '🟢',
  diff: '🟡',
  dimDiff: '🟠',
  unmatched: '🔴',
  rejected: '⚫',
};

function updateLine(lines: IncomingLine[], id: string, patch: Partial<IncomingLine>): IncomingLine[] {
  return lines.map((l) => (l.id === id ? { ...l, ...patch } : l));
}

/** Stage 4 — Sipariş ↔ Gelen ↔ Fiziksel (adet + ölçü + hacim) + stoka esas kabul. */
export function CompareStep({
  lines,
  onChange,
  disabled,
  supplier,
  onResolvedChange,
  onRequestMaterialMatch,
}: Props) {
  const { t } = useI18n();
  const [filter, setFilter] = useState<CompareFilter>('all');
  const [autoAcceptOk, setAutoAcceptOk] = useState(true);
  const [selected, setSelected] = useState<Record<string, boolean>>({});
  const [disposition, setDisposition] = useState<Record<string, CompareDisposition>>({});
  const [partialQty, setPartialQty] = useState<Record<string, string>>({});
  const [actionMsg, setActionMsg] = useState<string | null>(null);

  const rows = useMemo(() => buildCompareRows(lines), [lines]);
  const summary = useMemo(() => compareSummary(rows), [rows]);
  const exceptions = useMemo(() => compareExceptions(rows), [rows]);

  useEffect(() => {
    onResolvedChange(compareReadyForResult(lines, disposition, autoAcceptOk));
  }, [lines, disposition, autoAcceptOk, onResolvedChange]);

  const visibleRows = useMemo(() => {
    if (filter === 'diff') return rows.filter((r) => r.status === 'diff');
    if (filter === 'dimDiff') return rows.filter((r) => r.status === 'dimDiff');
    if (filter === 'unmatched') return rows.filter((r) => r.status === 'unmatched');
    if (filter === 'rejected') return rows.filter((r) => r.status === 'rejected');
    if (autoAcceptOk) {
      const okSample = rows.filter((r) => r.status === 'ok').slice(0, 1);
      const rest = rows.filter((r) => r.status !== 'ok');
      return [...okSample, ...rest];
    }
    return rows;
  }, [rows, filter, autoAcceptOk]);

  const selectedIds = useMemo(() => Object.keys(selected).filter((id) => selected[id]), [selected]);

  function setStockAccept(ids: string[], stockAccept: StockAcceptDecision, reason: string, disp: CompareDisposition) {
    let next = lines;
    for (const id of ids) {
      next = updateLine(next, id, { stockAccept, stockAcceptReason: reason });
    }
    onChange(next);
    setDisposition((prev) => {
      const d = { ...prev };
      for (const id of ids) d[id] = disp;
      return d;
    });
  }

  function applyDisposition(ids: string[], value: CompareDisposition) {
    if (ids.length === 0) {
      setActionMsg(t('wb.rcv.compare.needSelection'));
      return;
    }
    if (value === 'accepted') {
      setStockAccept(ids, 'ok', 'Karşılaştırma kabul', 'accepted');
    } else if (value === 'conditional') {
      setStockAccept(ids, 'conditional', 'Ölçü/miktar farkı — şartlı kabul; stoka fiziksel ölçü', 'conditional');
    } else if (value === 'partial') {
      setStockAccept(ids, 'conditional', `Kısmi kabul ${partialQty[ids[0] ?? ''] ?? ''}`.trim(), 'partial');
    } else if (value === 'rejected') {
      setStockAccept(ids, 'reject', 'Karşılaştırma red', 'rejected');
    } else if (value === 'match') {
      setDisposition((prev) => {
        const d = { ...prev };
        for (const id of ids) d[id] = 'match';
        return d;
      });
      if (ids[0]) onRequestMaterialMatch?.(ids[0]);
    }
    setActionMsg(t(`wb.rcv.compare.applied.${value}`).replace('{n}', String(ids.length)));
  }

  function statusLabel(status: CompareRow['status']) {
    return t(`wb.rcv.compare.status.${status}`);
  }

  function dispLabel(row: CompareRow) {
    const line = lines.find((l) => l.id === row.lineId);
    if (row.status === 'rejected') return t('wb.rcv.compare.disp.rejected');
    if (row.status === 'ok' && autoAcceptOk && (line?.stockAccept === 'none' || line?.stockAccept === 'ok')) {
      return t('wb.rcv.compare.disp.accepted');
    }
    if (line?.stockAccept === 'conditional') return t('wb.rcv.compare.disp.conditional');
    if (line?.stockAccept === 'ok') return t('wb.rcv.compare.disp.accepted');
    if (line?.stockAccept === 'reject') return t('wb.rcv.compare.disp.rejected');
    const d = disposition[row.lineId] ?? 'none';
    if (d === 'none') return '—';
    return t(`wb.rcv.compare.disp.${d}`);
  }

  return (
    <div className="space-y-4">
      <div className="space-y-1 border-b border-[var(--border-default)] pb-3">
        <h3 className="text-base font-semibold tracking-tight">{t('wb.rcv.compare.title')}</h3>
        <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.compare.introDims')}</p>
        <p className="text-xs text-[var(--text-muted)]">
          {t('wb.rcv.compare.poRef')}: <span className="font-mono">PO-DEMO-2026-014</span>
          {supplier ? (
            <>
              {' · '}
              {t('wb.rcv.supplier')}: <span className="font-medium">{supplier}</span>
            </>
          ) : null}
        </p>
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.compare.stockBasisNote')}</p>
      </div>

      <div className="flex flex-wrap gap-2 text-sm">
        <span className="rounded-md border border-[var(--border-default)] px-3 py-1.5">
          🟢 {summary.ok} {t('wb.rcv.compare.summary.ok')}
        </span>
        <span className="rounded-md border border-[var(--border-default)] px-3 py-1.5">
          🟡 {summary.diff} {t('wb.rcv.compare.summary.diff')}
        </span>
        <span className="rounded-md border border-[var(--border-default)] px-3 py-1.5">
          🟠 {summary.dimDiff} {t('wb.rcv.compare.summary.dimDiff')}
        </span>
        <span className="rounded-md border border-[var(--border-default)] px-3 py-1.5">
          🔴 {summary.unmatched} {t('wb.rcv.compare.summary.unmatched')}
        </span>
        <span className="rounded-md border border-[var(--border-default)] px-3 py-1.5">
          ⚫ {summary.rejected} {t('wb.rcv.compare.summary.rejected')}
        </span>
      </div>

      <div className="flex flex-wrap gap-2">
        {(
          [
            ['all', t('wb.rcv.compare.filter.all')],
            ['diff', t('wb.rcv.compare.filter.diff')],
            ['dimDiff', t('wb.rcv.compare.filter.dimDiff')],
            ['unmatched', t('wb.rcv.compare.filter.unmatched')],
            ['rejected', t('wb.rcv.compare.filter.rejected')],
          ] as const
        ).map(([id, label]) => (
          <Button key={id} type="button" size="sm" variant={filter === id ? 'default' : 'secondary'} disabled={disabled} onClick={() => setFilter(id)}>
            {label}
          </Button>
        ))}
      </div>

      <label className="flex items-start gap-2 rounded-md border border-[var(--border-default)] px-3 py-2 text-sm">
        <input type="checkbox" className="mt-0.5" checked={autoAcceptOk} disabled={disabled} onChange={(e) => setAutoAcceptOk(e.target.checked)} />
        <span>
          <span className="font-medium">{t('wb.rcv.compare.autoAcceptOk').replace('{n}', String(summary.ok))}</span>
          <span className="mt-0.5 block text-xs text-[var(--text-muted)]">{t('wb.rcv.compare.autoAcceptHintDims')}</span>
        </span>
      </label>

      <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
        <table className="w-full min-w-[900px] text-left text-sm">
          <thead className="bg-[var(--color-surface-hover)] text-[10px] uppercase tracking-wide text-[var(--text-muted)]">
            <tr>
              <th className="w-10 px-3 py-2">{t('wb.rcv.compare.select')}</th>
              <th className="px-3 py-2">{t('wb.rcv.compare.col.product')}</th>
              <th className="px-3 py-2">{t('wb.rcv.compare.col.po')}</th>
              <th className="px-3 py-2">{t('wb.rcv.compare.col.document')}</th>
              <th className="px-3 py-2">{t('wb.rcv.compare.col.physical')}</th>
              <th className="px-3 py-2">{t('wb.rcv.compare.col.status')}</th>
              <th className="px-3 py-2">{t('wb.rcv.compare.col.decision')}</th>
            </tr>
          </thead>
          <tbody>
            {visibleRows.map((row) => (
              <tr key={row.lineId} className="border-t border-[var(--border-default)] align-top">
                <td className="px-3 py-2">
                  <input
                    type="checkbox"
                    checked={!!selected[row.lineId]}
                    disabled={disabled || row.status === 'rejected' || (row.status === 'ok' && autoAcceptOk)}
                    onChange={() => setSelected((s) => ({ ...s, [row.lineId]: !s[row.lineId] }))}
                  />
                </td>
                <td className="px-3 py-2">
                  <p className="font-medium">{row.name}</p>
                  <p className="text-xs text-[var(--text-muted)]">{row.materialCode}</p>
                </td>
                <td className="px-3 py-2 text-xs tabular-nums">
                  <p>{formatQty(row.poQty, row.unit)}</p>
                  <p className="text-[var(--text-muted)]">{row.documentDims}</p>
                  <p className="text-[var(--text-muted)]">{formatVolume(row.poVolumeM3)}</p>
                </td>
                <td className="px-3 py-2 text-xs tabular-nums">
                  <p>{formatQty(row.documentQty, row.unit)}</p>
                  <p className="text-[var(--text-muted)]">{row.documentDims}</p>
                  <p className="text-[var(--text-muted)]">{formatVolume(row.documentVolumeM3)}</p>
                </td>
                <td className="px-3 py-2 text-xs tabular-nums">
                  <p className="font-medium">{formatQty(row.physicalQty > 0 ? row.physicalQty : null, row.unit)}</p>
                  <p className={row.dimsDiffer ? 'font-medium text-[var(--color-danger)]' : 'text-[var(--text-muted)]'}>
                    {row.physicalDims}
                  </p>
                  <p className="font-medium">{formatVolume(row.physicalVolumeM3)}</p>
                </td>
                <td className="px-3 py-2">
                  <span className="inline-flex items-center gap-1.5">
                    <span aria-hidden>{STATUS_DOT[row.status]}</span>
                    {statusLabel(row.status)}
                  </span>
                </td>
                <td className="px-3 py-2 text-xs">{dispLabel(row)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {selectedIds.length > 0 ? (
        <div className="flex flex-wrap items-end gap-2">
          <label className="block text-xs text-[var(--text-muted)]">
            {t('wb.rcv.compare.partialQty')}
            <Input
              className="mt-1 w-32"
              type="number"
              disabled={disabled}
              value={partialQty[selectedIds[0]!] ?? ''}
              onChange={(e) => setPartialQty((p) => ({ ...p, [selectedIds[0]!]: e.target.value }))}
            />
          </label>
        </div>
      ) : null}

      <div className="space-y-2">
        <p className="text-xs font-semibold uppercase tracking-wide text-[var(--text-muted)]">{t('wb.rcv.compare.bulk')}</p>
        <div className="flex flex-wrap gap-2">
          <Button type="button" disabled={disabled} onClick={() => applyDisposition(selectedIds, 'accepted')}>
            {t('wb.rcv.compare.actions.accept')}
          </Button>
          <Button type="button" disabled={disabled} onClick={() => applyDisposition(selectedIds, 'conditional')}>
            {t('wb.rcv.compare.actions.conditional')}
          </Button>
          <Button type="button" variant="secondary" disabled={disabled} onClick={() => applyDisposition(selectedIds, 'partial')}>
            {t('wb.rcv.compare.actions.partial')}
          </Button>
          <Button type="button" variant="secondary" disabled={disabled} onClick={() => applyDisposition(selectedIds, 'match')}>
            {t('wb.rcv.compare.actions.match')}
          </Button>
          <Button type="button" variant="secondary" disabled={disabled} onClick={() => applyDisposition(selectedIds, 'rejected')}>
            {t('wb.rcv.compare.actions.reject')}
          </Button>
        </div>
      </div>

      {actionMsg ? <p className="text-xs text-[var(--text-secondary)]">{actionMsg}</p> : null}
      {exceptions.length > 0 && !compareReadyForResult(lines, disposition, autoAcceptOk) ? (
        <p className="text-xs text-[var(--color-danger)]">{t('wb.rcv.compare.resolveHintDims')}</p>
      ) : (
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.compare.resolvedOk')}</p>
      )}
    </div>
  );
}
