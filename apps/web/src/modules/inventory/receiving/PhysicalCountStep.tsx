import { Button, Input } from '@naswood/ui';
import { useI18n } from '@/i18n';

export type CountMethod = 'handwriting' | 'excel' | 'manual';

export type CountLine = { id: string; product: string; qty: string };

export type PhysicalCountState = {
  method: CountMethod;
  /** Qty from supplier document / incoming list (reference only). */
  documentQty: string;
  /** AI suggested physical qty — not authoritative. */
  aiQty: string;
  aiNote: string;
  handwritingPhoto: boolean;
  handwritingParsed: boolean;
  excelUploaded: boolean;
  excelParsed: boolean;
  packages: string;
  perPackage: string;
  /** Operator final physical count — stock uses this when verified. */
  operatorQty: string;
  lines: CountLine[];
  verified: boolean;
};

export function createDefaultPhysicalCount(): PhysicalCountState {
  return {
    method: 'manual',
    documentQty: '48',
    aiQty: '46',
    aiNote: '',
    handwritingPhoto: false,
    handwritingParsed: false,
    excelUploaded: false,
    excelParsed: false,
    packages: '4',
    perPackage: '12',
    operatorQty: '48',
    lines: [],
    verified: false,
  };
}

/** Final physical qty: operator wins over AI. */
export function finalPhysicalQty(state: PhysicalCountState): number {
  const op = Number(String(state.operatorQty).replace(',', '.'));
  if (Number.isFinite(op) && op > 0) return op;
  const ai = Number(String(state.aiQty).replace(',', '.'));
  if (Number.isFinite(ai) && ai > 0) return ai;
  return 0;
}

function packageProduct(packages: string, perPackage: string): number | null {
  const p = Number(packages);
  const n = Number(perPackage);
  if (!Number.isFinite(p) || !Number.isFinite(n) || p <= 0 || n <= 0) return null;
  return p * n;
}

type Props = {
  value: PhysicalCountState;
  onChange: (next: PhysicalCountState) => void;
  disabled?: boolean;
};

const DEMO_HANDWRITING_LINES: CountLine[] = [
  { id: 'h1', product: 'Çam 26x140x3000', qty: '48' },
  { id: 'h2', product: 'Çam 26x92x3000', qty: '32' },
  { id: 'h3', product: 'Çam 20x140x3000', qty: '24' },
];

const DEMO_EXCEL_LINES: CountLine[] = [
  { id: 'e1', product: 'Thermowood Deck 26×140×3000', qty: '48' },
  { id: 'e2', product: 'Thermowood Deck 26×115×3000', qty: '24' },
];

/** Stage 3 — Fiziksel sayım: el yazısı foto / Excel / manuel + AI yardımcı. */
export function PhysicalCountStep({ value, onChange, disabled }: Props) {
  const { t } = useI18n();

  function patch(partial: Partial<PhysicalCountState>) {
    onChange({ ...value, ...partial, verified: partial.verified ?? false });
  }

  function setMethod(method: CountMethod) {
    patch({ method, verified: false });
  }

  const pkgTotal = packageProduct(value.packages, value.perPackage);
  const finalQty = finalPhysicalQty(value);
  const docQty = Number(String(value.documentQty).replace(',', '.'));
  const aiQty = Number(String(value.aiQty).replace(',', '.'));

  function applyPackageMath() {
    if (pkgTotal == null) return;
    patch({ operatorQty: String(pkgTotal), verified: false });
  }

  function runHandwritingAi() {
    patch({
      handwritingParsed: true,
      lines: DEMO_HANDWRITING_LINES,
      aiQty: '46',
      aiNote: t('wb.rcv.ops.count.aiHandwritingNote'),
      operatorQty: value.operatorQty || '48',
      verified: false,
    });
  }

  function runExcelParse() {
    patch({
      excelParsed: true,
      lines: DEMO_EXCEL_LINES,
      aiQty: DEMO_EXCEL_LINES.reduce((s, l) => s + (Number(l.qty) || 0), 0).toString(),
      aiNote: t('wb.rcv.ops.count.aiExcelNote'),
      operatorQty: value.operatorQty || '48',
      verified: false,
    });
  }

  function acceptAiAsOperator() {
    if (!value.aiQty) return;
    patch({ operatorQty: value.aiQty, verified: false });
  }

  return (
    <div className="space-y-5">
      <p className="text-sm text-[var(--text-secondary)]">{t('wb.rcv.ops.count.intro')}</p>

      <div className="flex flex-wrap gap-2">
        {(
          [
            ['handwriting', t('wb.rcv.ops.count.method.handwriting')],
            ['excel', t('wb.rcv.ops.count.method.excel')],
            ['manual', t('wb.rcv.ops.count.method.manual')],
          ] as const
        ).map(([id, label]) => (
          <Button
            key={id}
            type="button"
            size="sm"
            variant={value.method === id ? 'default' : 'secondary'}
            disabled={disabled}
            onClick={() => setMethod(id)}
          >
            {label}
          </Button>
        ))}
      </div>

      {value.method === 'handwriting' ? (
        <section className="space-y-3 rounded-md border border-[var(--border-default)] px-3 py-3">
          <p className="text-sm font-medium">{t('wb.rcv.ops.count.handwritingTitle')}</p>
          <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.count.handwritingHint')}</p>
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={value.handwritingPhoto}
              disabled={disabled}
              onChange={(e) => patch({ handwritingPhoto: e.target.checked, verified: false })}
            />
            {t('wb.rcv.ops.count.handwritingPhoto')}
          </label>
          <Button
            type="button"
            size="sm"
            variant="secondary"
            disabled={disabled || !value.handwritingPhoto}
            onClick={runHandwritingAi}
          >
            {t('wb.rcv.ops.count.runAiRead')}
          </Button>
          <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.count.aiNotFinal')}</p>
        </section>
      ) : null}

      {value.method === 'excel' ? (
        <section className="space-y-3 rounded-md border border-[var(--border-default)] px-3 py-3">
          <p className="text-sm font-medium">{t('wb.rcv.ops.count.excelTitle')}</p>
          <p className="text-xs text-[var(--text-muted)]">{t('wb.rcv.ops.count.excelHint')}</p>
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={value.excelUploaded}
              disabled={disabled}
              onChange={(e) => patch({ excelUploaded: e.target.checked, verified: false })}
            />
            {t('wb.rcv.ops.count.excelUploaded')}
          </label>
          <Button
            type="button"
            size="sm"
            variant="secondary"
            disabled={disabled || !value.excelUploaded}
            onClick={runExcelParse}
          >
            {t('wb.rcv.ops.count.parseExcel')}
          </Button>
        </section>
      ) : null}

      {value.lines.length > 0 ? (
        <div className="overflow-x-auto rounded-md border border-[var(--border-default)]">
          <table className="w-full min-w-[400px] text-left text-sm">
            <thead className="bg-[var(--color-surface-hover)] text-[10px] uppercase text-[var(--text-muted)]">
              <tr>
                <th className="px-3 py-2">{t('wb.rcv.ops.count.colProduct')}</th>
                <th className="px-3 py-2">{t('wb.rcv.ops.count.colQty')}</th>
              </tr>
            </thead>
            <tbody>
              {value.lines.map((line, idx) => (
                <tr key={line.id} className="border-t border-[var(--border-default)]">
                  <td className="px-3 py-2">
                    <Input
                      value={line.product}
                      disabled={disabled}
                      onChange={(e) => {
                        const lines = [...value.lines];
                        lines[idx] = { ...line, product: e.target.value };
                        patch({ lines, verified: false });
                      }}
                    />
                  </td>
                  <td className="px-3 py-2 w-28">
                    <Input
                      value={line.qty}
                      disabled={disabled}
                      onChange={(e) => {
                        const lines = [...value.lines];
                        lines[idx] = { ...line, qty: e.target.value };
                        patch({ lines, verified: false });
                      }}
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : null}

      <section className="space-y-3">
        <h3 className="text-sm font-semibold tracking-tight">{t('wb.rcv.ops.count.packageMath')}</h3>
        <div className="flex flex-wrap items-end gap-2">
          <label className="block space-y-1 text-xs text-[var(--text-muted)]">
            {t('wb.rcv.ops.count.packages')}
            <Input
              className="w-24"
              value={value.packages}
              disabled={disabled}
              onChange={(e) => patch({ packages: e.target.value, verified: false })}
            />
          </label>
          <span className="pb-2 text-sm">×</span>
          <label className="block space-y-1 text-xs text-[var(--text-muted)]">
            {t('wb.rcv.ops.count.perPackage')}
            <Input
              className="w-24"
              value={value.perPackage}
              disabled={disabled}
              onChange={(e) => patch({ perPackage: e.target.value, verified: false })}
            />
          </label>
          <span className="pb-2 text-sm">=</span>
          <p className="pb-2 text-sm font-medium tabular-nums">{pkgTotal ?? '—'}</p>
          <Button type="button" size="sm" variant="secondary" disabled={disabled || pkgTotal == null} onClick={applyPackageMath}>
            {t('wb.rcv.ops.count.applyPackage')}
          </Button>
        </div>
      </section>

      <section className="grid gap-3 sm:grid-cols-3">
        <div className="rounded-md border border-[var(--border-default)] px-3 py-3">
          <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('wb.rcv.ops.count.documentQty')}</p>
          <Input
            className="mt-1"
            value={value.documentQty}
            disabled={disabled}
            onChange={(e) => patch({ documentQty: e.target.value, verified: false })}
          />
        </div>
        <div className="rounded-md border border-[var(--border-default)] px-3 py-3">
          <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('wb.rcv.ops.count.aiQty')}</p>
          <Input
            className="mt-1"
            value={value.aiQty}
            disabled={disabled}
            onChange={(e) => patch({ aiQty: e.target.value, verified: false })}
          />
          <Button type="button" size="sm" variant="secondary" className="mt-2" disabled={disabled || !value.aiQty} onClick={acceptAiAsOperator}>
            {t('wb.rcv.ops.count.acceptAi')}
          </Button>
          {value.aiNote ? <p className="mt-1 text-[10px] text-[var(--text-muted)]">{value.aiNote}</p> : null}
        </div>
        <div className="rounded-md border border-[var(--color-primary)]/30 bg-[var(--color-primary)]/5 px-3 py-3">
          <p className="text-[10px] uppercase text-[var(--text-muted)]">{t('wb.rcv.ops.count.operatorQty')}</p>
          <Input
            className="mt-1"
            value={value.operatorQty}
            disabled={disabled}
            onChange={(e) => patch({ operatorQty: e.target.value, verified: false })}
          />
          <p className="mt-1 text-[10px] text-[var(--text-muted)]">{t('wb.rcv.ops.count.operatorWins')}</p>
        </div>
      </section>

      <div className="rounded-md border border-[var(--border-default)] px-3 py-3 text-sm">
        <p>
          {t('wb.rcv.ops.count.summaryDoc')}: <span className="font-medium tabular-nums">{Number.isFinite(docQty) ? docQty : '—'}</span>
          {' · '}
          {t('wb.rcv.ops.count.summaryAi')}: <span className="font-medium tabular-nums">{Number.isFinite(aiQty) ? aiQty : '—'}</span>
          {' · '}
          {t('wb.rcv.ops.count.summaryFinal')}: <span className="font-semibold tabular-nums">{finalQty || '—'}</span>
        </p>
        {!value.operatorQty && value.aiQty ? (
          <p className="mt-1 text-xs text-[var(--color-danger)]">{t('wb.rcv.ops.count.aiFallbackWarn')}</p>
        ) : null}
      </div>

      <label className="flex items-center gap-2 text-sm font-medium">
        <input
          type="checkbox"
          checked={value.verified}
          disabled={disabled || finalQty <= 0}
          onChange={(e) => onChange({ ...value, verified: e.target.checked })}
        />
        {t('wb.rcv.ops.count.verifyFinal')}
      </label>
    </div>
  );
}
