import { useMemo, useState } from 'react';
import { Button, Input } from '@naswood/ui';
import { useI18n } from '@/i18n';
import {
  DOC_CONTROL_LINES,
  DOC_CONTROL_META,
  formatExpected,
  type DocControlFilter,
  type DocControlLine,
  type DocLineDisposition,
  type DocLineStatus,
} from './documentControlDemo';

type Props = {
  disabled?: boolean;
  onResolvedChange: (resolved: boolean) => void;
  onRequestMaterialMatch?: (line: DocControlLine) => void;
};

const STATUS_DOT: Record<DocLineStatus, string> = {
  ok: '🟢',
  diff: '🟡',
  unmatched: '🔴',
};

export function DocumentControlPanel({ disabled, onResolvedChange, onRequestMaterialMatch }: Props) {
  const { t } = useI18n();
  const [filter, setFilter] = useState<DocControlFilter>('all');
  const [autoAcceptOk, setAutoAcceptOk] = useState(true);
  const [selected, setSelected] = useState<Record<string, boolean>>({});
  const [disposition, setDisposition] = useState<Record<string, DocLineDisposition>>({});
  const [partialQty, setPartialQty] = useState<Record<string, string>>({});
  const [actionMsg, setActionMsg] = useState<string | null>(null);

  const exceptionLines = useMemo(
    () => DOC_CONTROL_LINES.filter((l) => l.status !== 'ok'),
    [],
  );

  const visibleLines = useMemo(() => {
    let rows = DOC_CONTROL_LINES;
    if (filter === 'diff') rows = rows.filter((l) => l.status === 'diff');
    else if (filter === 'unmatched') rows = rows.filter((l) => l.status === 'unmatched');
    else if (autoAcceptOk) {
      // All: show ok samples collapsed — only list exceptions + a few ok examples
      const okSamples = rows.filter((l) => l.status === 'ok').slice(0, 1);
      rows = [...okSamples, ...exceptionLines];
    }
    return rows;
  }, [filter, autoAcceptOk, exceptionLines]);

  const selectedIds = useMemo(() => Object.keys(selected).filter((id) => selected[id]), [selected]);

  function recomputeResolved(nextDisp: Record<string, DocLineDisposition>, nextAuto: boolean) {
    const exceptionsOk = exceptionLines.every((l) => {
      const d = nextDisp[l.id] ?? 'none';
      return d === 'accepted' || d === 'partial' || d === 'rejected' || d === 'match';
    });
    onResolvedChange(nextAuto && exceptionsOk);
  }

  function toggleSelect(id: string) {
    setSelected((s) => ({ ...s, [id]: !s[id] }));
  }

  function selectVisible() {
    setSelected((s) => {
      const next = { ...s };
      for (const l of visibleLines) next[l.id] = true;
      return next;
    });
  }

  function applyDisposition(ids: string[], value: DocLineDisposition) {
    if (ids.length === 0) {
      setActionMsg(t('wb.rcv.docCtrl.needSelection'));
      return;
    }
    setDisposition((prev) => {
      const next = { ...prev };
      for (const id of ids) next[id] = value;
      recomputeResolved(next, autoAcceptOk);
      return next;
    });
    setActionMsg(t(`wb.rcv.docCtrl.applied.${value}`).replace('{n}', String(ids.length)));
  }

  function statusLabel(status: DocLineStatus) {
    return t(`wb.rcv.docCtrl.status.${status}`);
  }

  function dispLabel(d: DocLineDisposition) {
    if (d === 'none') return '—';
    return t(`wb.rcv.docCtrl.disp.${d}`);
  }

  return (
    <div className="space-y-4">
      <div className="space-y-1 border-b border-[var(--border-default)] pb-3">
        <h3 className="text-base font-semibold tracking-tight">{t('wb.rcv.docCtrl.title')}</h3>
        <p className="text-sm text-[var(--text-secondary)]">
          {t('wb.rcv.docCtrl.deliveryNote')}:{' '}
          <span className="font-mono font-medium text-[var(--text-primary)]">{DOC_CONTROL_META.documentNumber}</span>
          {' · '}
          {t('wb.rcv.supplier')}: <span className="font-medium text-[var(--text-primary)]">{DOC_CONTROL_META.supplier}</span>
          {' · '}
          {t('wb.rcv.docCtrl.totalLines')}:{' '}
          <span className="font-medium text-[var(--text-primary)]">{DOC_CONTROL_META.totalLines}</span>
        </p>
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.docCtrl.demoNote')}</p>
      </div>

      <div className="flex flex-wrap gap-2 text-sm">
        <span className="rounded-md border border-[var(--border-default)] px-3 py-1.5">
          🟢 {DOC_CONTROL_META.okCount} {t('wb.rcv.docCtrl.summary.ok')}
        </span>
        <span className="rounded-md border border-[var(--border-default)] px-3 py-1.5">
          🟡 {DOC_CONTROL_META.diffCount} {t('wb.rcv.docCtrl.summary.diff')}
        </span>
        <span className="rounded-md border border-[var(--border-default)] px-3 py-1.5">
          🔴 {DOC_CONTROL_META.unmatchedCount} {t('wb.rcv.docCtrl.summary.unmatched')}
        </span>
      </div>

      <div className="flex flex-wrap gap-2">
        {(
          [
            ['all', t('wb.rcv.docCtrl.filter.all')],
            ['diff', t('wb.rcv.docCtrl.filter.diff')],
            ['unmatched', t('wb.rcv.docCtrl.filter.unmatched')],
          ] as const
        ).map(([id, label]) => (
          <Button
            key={id}
            type="button"
            size="sm"
            variant={filter === id ? 'default' : 'secondary'}
            disabled={disabled}
            onClick={() => setFilter(id)}
          >
            {label}
          </Button>
        ))}
      </div>

      <label className="flex items-start gap-2 rounded-md border border-[var(--border-default)] px-3 py-2 text-sm">
        <input
          type="checkbox"
          className="mt-0.5"
          checked={autoAcceptOk}
          disabled={disabled}
          onChange={(e) => {
            const v = e.target.checked;
            setAutoAcceptOk(v);
            recomputeResolved(disposition, v);
          }}
        />
        <span>
          <span className="font-medium">
            {t('wb.rcv.docCtrl.autoAcceptOk').replace('{n}', String(DOC_CONTROL_META.okCount))}
          </span>
          <span className="mt-0.5 block text-xs text-[var(--text-muted)]">{t('wb.rcv.docCtrl.autoAcceptHint')}</span>
        </span>
      </label>

      <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
        <table className="w-full min-w-[640px] text-left text-sm">
          <thead className="bg-[var(--color-surface-hover)] text-[10px] uppercase tracking-wide text-[var(--text-muted)]">
            <tr>
              <th className="px-3 py-2 w-10">
                <button type="button" className="underline-offset-2 hover:underline" disabled={disabled} onClick={selectVisible}>
                  {t('wb.rcv.docCtrl.select')}
                </button>
              </th>
              <th className="px-3 py-2">{t('wb.rcv.docCtrl.col.product')}</th>
              <th className="px-3 py-2">{t('wb.rcv.docCtrl.col.expected')}</th>
              <th className="px-3 py-2">{t('wb.rcv.docCtrl.col.incoming')}</th>
              <th className="px-3 py-2">{t('wb.rcv.docCtrl.col.status')}</th>
              <th className="px-3 py-2">{t('wb.rcv.docCtrl.col.decision')}</th>
            </tr>
          </thead>
          <tbody>
            {filter === 'all' && autoAcceptOk ? (
              <tr className="border-t border-[var(--border-default)] bg-[var(--color-surface-hover)]/40 text-xs text-[var(--text-muted)]">
                <td colSpan={6} className="px-3 py-2">
                  {t('wb.rcv.docCtrl.okCollapsed').replace('{n}', String(DOC_CONTROL_META.okCount - 1))}
                </td>
              </tr>
            ) : null}
            {visibleLines.map((line) => {
              const d = disposition[line.id] ?? 'none';
              return (
                <tr key={line.id} className="border-t border-[var(--border-default)]">
                  <td className="px-3 py-2">
                    <input
                      type="checkbox"
                      checked={!!selected[line.id]}
                      disabled={disabled || (line.status === 'ok' && autoAcceptOk)}
                      onChange={() => toggleSelect(line.id)}
                    />
                  </td>
                  <td className="px-3 py-2 font-medium">{line.product}</td>
                  <td className="px-3 py-2 tabular-nums">{formatExpected(line.expected)}</td>
                  <td className="px-3 py-2 tabular-nums">{line.incoming}</td>
                  <td className="px-3 py-2">
                    <span className="inline-flex items-center gap-1.5">
                      <span aria-hidden>{STATUS_DOT[line.status]}</span>
                      {statusLabel(line.status)}
                    </span>
                  </td>
                  <td className="px-3 py-2 text-xs">
                    {d === 'partial' && partialQty[line.id] ? (
                      <span>
                        {dispLabel(d)} ({partialQty[line.id]})
                      </span>
                    ) : (
                      dispLabel(d)
                    )}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>

      {selectedIds.some((id) => DOC_CONTROL_LINES.find((l) => l.id === id)?.status === 'diff') ? (
        <div className="flex flex-wrap items-end gap-2">
          <label className="block text-xs text-[var(--text-muted)]">
            {t('wb.rcv.docCtrl.partialQty')}
            <Input
              className="mt-1 w-32"
              type="number"
              disabled={disabled}
              value={partialQty[selectedIds[0]] ?? ''}
              onChange={(e) => setPartialQty((p) => ({ ...p, [selectedIds[0]]: e.target.value }))}
            />
          </label>
        </div>
      ) : null}

      <div className="space-y-2">
        <p className="text-xs font-semibold uppercase tracking-wide text-[var(--text-muted)]">{t('wb.rcv.docCtrl.bulk')}</p>
        <div className="flex flex-wrap gap-2">
          <Button type="button" disabled={disabled} onClick={() => applyDisposition(selectedIds, 'accepted')}>
            {t('wb.rcv.docCtrl.actions.accept')}
          </Button>
          <Button
            type="button"
            variant="secondary"
            disabled={disabled}
            onClick={() => {
              if (selectedIds.length === 0) {
                setActionMsg(t('wb.rcv.docCtrl.needSelection'));
                return;
              }
              applyDisposition(selectedIds, 'partial');
            }}
          >
            {t('wb.rcv.docCtrl.actions.partial')}
          </Button>
          <Button
            type="button"
            variant="secondary"
            disabled={disabled}
            onClick={() => {
              if (selectedIds.length === 0) {
                setActionMsg(t('wb.rcv.docCtrl.needSelection'));
                return;
              }
              applyDisposition(selectedIds, 'match');
              const first = DOC_CONTROL_LINES.find((l) => l.id === selectedIds[0]);
              if (first) onRequestMaterialMatch?.(first);
            }}
          >
            {t('wb.rcv.docCtrl.actions.match')}
          </Button>
          <Button
            type="button"
            variant="secondary"
            disabled={disabled}
            onClick={() => applyDisposition(selectedIds, 'rejected')}
          >
            {t('wb.rcv.docCtrl.actions.reject')}
          </Button>
        </div>
      </div>

      {actionMsg ? <p className="text-xs text-[var(--text-secondary)]">{actionMsg}</p> : null}

      <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.docCtrl.resolveHint')}</p>
    </div>
  );
}
