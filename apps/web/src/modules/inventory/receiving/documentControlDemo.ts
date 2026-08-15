/** Phase-1 document control demo — not wired to real PO/ASN APIs yet. */

export type DocLineStatus = 'ok' | 'diff' | 'unmatched';
export type DocLineDisposition = 'none' | 'accepted' | 'partial' | 'rejected' | 'match';
export type DocControlFilter = 'all' | 'diff' | 'unmatched';

export type DocControlLine = {
  id: string;
  product: string;
  expected: number | null;
  incoming: number;
  status: DocLineStatus;
};

export const DOC_CONTROL_META = {
  documentNumber: 'INV-2026-00852',
  supplier: 'Nordic Timber',
  totalLines: 326,
  okCount: 294,
  diffCount: 24,
  unmatchedCount: 8,
} as const;

function buildDemoLines(): DocControlLine[] {
  const lines: DocControlLine[] = [
    { id: 'ok-a', product: 'Thermowood A', expected: 500, incoming: 500, status: 'ok' },
    { id: 'ok-e', product: 'Thermowood E 26×140×3000', expected: 120, incoming: 120, status: 'ok' },
    { id: 'ok-f', product: 'Thermowood F 26×115×3000', expected: 80, incoming: 80, status: 'ok' },
    { id: 'diff-b', product: 'Thermowood B', expected: 200, incoming: 198, status: 'diff' },
    { id: 'diff-c', product: 'Thermowood C', expected: 100, incoming: 105, status: 'diff' },
  ];

  for (let i = 1; i <= 22; i++) {
    const expected = 40 + (i % 7) * 10;
    const delta = i % 2 === 0 ? -2 : 3;
    lines.push({
      id: `diff-${i}`,
      product: `Thermowood Diff ${String(i).padStart(2, '0')}`,
      expected,
      incoming: expected + delta,
      status: 'diff',
    });
  }

  lines.push({ id: 'un-d', product: 'Thermowood D', expected: null, incoming: 50, status: 'unmatched' });
  for (let i = 1; i <= 7; i++) {
    lines.push({
      id: `un-${i}`,
      product: `Bilinmeyen Kalem ${i}`,
      expected: null,
      incoming: 10 + i * 5,
      status: 'unmatched',
    });
  }

  return lines;
}

export const DOC_CONTROL_LINES: DocControlLine[] = buildDemoLines();

export function formatExpected(value: number | null): string {
  return value == null ? '—' : String(value);
}
