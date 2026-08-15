import { Button } from '@naswood/ui';
import { useI18n } from '@/i18n';
import { formatLineDims, type IncomingLine, type PreAcceptDecision } from './incomingLines';

type Props = {
  lines: IncomingLine[];
  onChange: (next: IncomingLine[]) => void;
  disabled?: boolean;
};

const KIND_LABEL: Record<IncomingLine['kind'], string> = {
  lumber: 'Kereste',
  log: 'Tomruk',
  lamella: 'Lamel',
  thermowood: 'Thermowood',
  panel: 'Masif Panel',
  packaged: 'Paketli',
  other: 'Diğer',
};

/** Stage 2 — per-line ön kabul (red stoka girmez). */
export function IncomingPreAcceptPanel({ lines, onChange, disabled }: Props) {
  const { t } = useI18n();

  function setDecision(id: string, preAccept: PreAcceptDecision) {
    onChange(lines.map((l) => (l.id === id ? { ...l, preAccept } : l)));
  }

  if (lines.length === 0) {
    return (
      <p className="text-sm text-[var(--text-muted)]">{t('wb.rcv.ops.check.noIncomingYet')}</p>
    );
  }

  return (
    <section className="space-y-3">
      <div>
        <h3 className="text-sm font-semibold tracking-tight">{t('wb.rcv.ops.check.incomingLinesTitle')}</h3>
        <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.check.incomingLinesHint')}</p>
      </div>
      <ul className="space-y-2">
        {lines.map((line) => (
          <li
            key={line.id}
            className="flex flex-col gap-2 rounded-md border border-[var(--border-default)] px-3 py-2 sm:flex-row sm:items-center sm:justify-between"
          >
            <div className="min-w-0">
              <p className="text-sm font-medium">
                {line.name}{' '}
                <span className="font-normal text-[var(--text-secondary)]">{formatLineDims(line)}</span>
              </p>
              <p className="text-xs text-[var(--text-muted)]">
                {KIND_LABEL[line.kind]} · {t('wb.rcv.ops.count.colDocQty')}: {line.documentQty} {line.unit}
              </p>
            </div>
            <div className="flex flex-wrap gap-1">
              {(
                [
                  ['ok', t('wb.rcv.ops.preAccept.ok')],
                  ['conditional', t('wb.rcv.ops.preAccept.conditional')],
                  ['reject', t('wb.rcv.ops.preAccept.reject')],
                ] as const
              ).map(([id, label]) => (
                <Button
                  key={id}
                  type="button"
                  size="sm"
                  variant={line.preAccept === id ? 'default' : 'secondary'}
                  disabled={disabled}
                  onClick={() => setDecision(line.id, id)}
                >
                  {label}
                </Button>
              ))}
            </div>
          </li>
        ))}
      </ul>
    </section>
  );
}
