import { useRef, useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Button } from '@naswood/ui';
import { createResource } from '@/api/business';
import { useI18n } from '@/i18n';
import {
  buildCsvTemplate,
  buildSpreadsheetMlTemplate,
  downloadTextFile,
  parseMaterialCardWorkbook,
  tableToDrafts,
  type BulkRowResult,
} from './materialCardBulk';
import { buildMaterialCreateBody, type MaterialLookupRow } from './materialCardDraft';

export function MaterialCardBulkPanel({
  materials,
  existingCodes,
}: {
  materials: MaterialLookupRow[];
  existingCodes: string[];
}) {
  const { t } = useI18n();
  const queryClient = useQueryClient();
  const fileRef = useRef<HTMLInputElement>(null);
  const [open, setOpen] = useState(false);
  const [parseError, setParseError] = useState<string | null>(null);
  const [preview, setPreview] = useState<BulkRowResult[] | null>(null);
  const [fileName, setFileName] = useState<string | null>(null);
  const [created, setCreated] = useState<{ line: number; code: string; name: string }[]>([]);

  function downloadExcel() {
    downloadTextFile(
      'Naswood-Malzeme-Kart-Sablonu.xls',
      buildSpreadsheetMlTemplate(),
      'application/vnd.ms-excel;charset=utf-8',
    );
  }

  function downloadCsv() {
    downloadTextFile(
      'Naswood-Malzeme-Kart-Sablonu.csv',
      buildCsvTemplate(),
      'text/csv;charset=utf-8',
    );
  }

  async function onFile(file: File | undefined) {
    if (!file) return;
    setParseError(null);
    setCreated([]);
    setFileName(file.name);
    try {
      const table = await parseMaterialCardWorkbook(file);
      setPreview(tableToDrafts(table, existingCodes, materials));
    } catch (e) {
      setPreview(null);
      setParseError(e instanceof Error ? e.message : String(e));
    }
  }

  const importMutation = useMutation({
    mutationFn: async () => {
      const okRows = (preview ?? []).filter((r): r is Extract<BulkRowResult, { ok: true }> => r.ok);
      if (okRows.length === 0) throw new Error(t('md.bulk.noneValid'));
      const liveCodes = [...existingCodes];
      const out: { line: number; code: string; name: string }[] = [];
      for (const row of okRows) {
        const body = buildMaterialCreateBody(row.draft, liveCodes);
        const createdRow = await createResource<Record<string, unknown>>('materials', body);
        liveCodes.push(body.code);
        const code =
          (typeof createdRow.code === 'string' && createdRow.code) ||
          (typeof createdRow.Code === 'string' && createdRow.Code) ||
          body.code;
        out.push({ line: row.line, code, name: body.name });
      }
      return out;
    },
    onSuccess: async (rows) => {
      setCreated(rows);
      await queryClient.invalidateQueries({ queryKey: ['business', 'materials'] });
    },
  });

  const validCount = preview?.filter((r) => r.ok).length ?? 0;
  const failCount = preview?.filter((r) => !r.ok).length ?? 0;

  return (
    <div className="rounded-lg border border-[var(--border-default)] bg-[var(--color-surface)] px-4 py-3">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <div>
          <p className="text-sm font-medium">{t('md.bulk.title')}</p>
          <p className="text-xs text-[var(--text-muted)]">{t('md.bulk.hint')}</p>
        </div>
        <div className="flex flex-wrap gap-2">
          <Button type="button" size="sm" variant="secondary" onClick={downloadExcel}>
            {t('md.bulk.downloadExcel')}
          </Button>
          <Button type="button" size="sm" variant="secondary" onClick={downloadCsv}>
            {t('md.bulk.downloadCsv')}
          </Button>
          <Button type="button" size="sm" onClick={() => setOpen((v) => !v)}>
            {open ? t('md.bulk.hideUpload') : t('md.bulk.showUpload')}
          </Button>
        </div>
      </div>

      {open ? (
        <div className="mt-3 space-y-3 border-t border-[var(--border-default)] pt-3">
          <p className="text-xs text-[var(--text-secondary)]">{t('md.bulk.uploadHint')}</p>
          <input
            ref={fileRef}
            type="file"
            accept=".xls,.xlsx,.csv,.xml,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,text/csv"
            className="block text-sm"
            onChange={(e) => void onFile(e.target.files?.[0])}
          />
          {fileName ? (
            <p className="text-xs text-[var(--text-muted)]">
              {t('md.bulk.file')}: {fileName}
            </p>
          ) : null}
          {parseError ? <p className="text-sm text-red-600">{parseError}</p> : null}

          {preview ? (
            <div className="space-y-2">
              <p className="text-xs text-[var(--text-secondary)]">
                {t('md.bulk.summary').replace('{ok}', String(validCount)).replace('{fail}', String(failCount))}
              </p>
              <div className="max-h-56 overflow-auto rounded-md border border-[var(--border-default)]">
                <table className="w-full text-left text-xs">
                  <thead className="bg-[var(--color-surface-hover)]">
                    <tr>
                      <th className="px-2 py-1">{t('md.bulk.line')}</th>
                      <th className="px-2 py-1">{t('md.bulk.result')}</th>
                      <th className="px-2 py-1">{t('md.fields.systemCode')}</th>
                      <th className="px-2 py-1">{t('md.fields.name')}</th>
                    </tr>
                  </thead>
                  <tbody>
                    {preview.map((r) => (
                      <tr key={r.line} className="border-t border-[var(--border-default)]">
                        <td className="px-2 py-1 font-mono">{r.line}</td>
                        <td className="px-2 py-1">
                          {r.ok ? (
                            <span className="text-emerald-700">{t('md.bulk.ready')}</span>
                          ) : (
                            <span className="text-red-600">{r.message}</span>
                          )}
                        </td>
                        <td className="px-2 py-1 font-mono">{r.ok ? r.previewCode : '—'}</td>
                        <td className="px-2 py-1">{r.draft?.name ?? '—'}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              <Button
                type="button"
                size="sm"
                disabled={validCount === 0 || importMutation.isPending}
                onClick={() => importMutation.mutate()}
              >
                {importMutation.isPending
                  ? t('md.bulk.creating')
                  : t('md.bulk.createN').replace('{n}', String(validCount))}
              </Button>
              {importMutation.isError ? (
                <p className="text-sm text-red-600">
                  {importMutation.error instanceof Error
                    ? importMutation.error.message
                    : t('md.bulk.createFailed')}
                </p>
              ) : null}
            </div>
          ) : null}

          {created.length > 0 ? (
            <div className="rounded-md border border-emerald-500/30 bg-emerald-500/10 px-3 py-2 text-sm">
              <p className="font-medium">{t('md.bulk.createdBanner').replace('{n}', String(created.length))}</p>
              <ul className="mt-1 font-mono text-xs">
                {created.map((c) => (
                  <li key={`${c.line}-${c.code}`}>
                    {t('md.bulk.line')} {c.line}: {c.code} · {c.name}
                  </li>
                ))}
              </ul>
            </div>
          ) : null}
        </div>
      ) : null}
    </div>
  );
}
