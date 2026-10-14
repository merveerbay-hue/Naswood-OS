/**
 * Count-list text parser (handwritten / printed lists).
 * There is no OCR engine on the API — images cannot be read server-side.
 * This parser only runs on extracted/pasted text. Do not invent confidence %.
 */

export type ParsedCountSuggestion = {
  rawMaterial: string;
  thicknessMm: number | null;
  widthMm: number | null;
  lengthMm: number | null;
  pieceCount: number | null;
  measuredVolumeM3: number | null;
  suggestionKind: 'row' | 'package-estimate';
  note?: string;
};

function parseDims(token: string): { t: number; w: number; l: number } | null {
  const m = token.replace(/,/g, '.').match(/(\d+(?:\.\d+)?)\s*[x×]\s*(\d+(?:\.\d+)?)\s*[x×]\s*(\d+(?:\.\d+)?)/i);
  if (!m) return null;
  return { t: Number(m[1]), w: Number(m[2]), l: Number(m[3]) };
}

export function parseCountListText(text: string): ParsedCountSuggestion[] {
  const lines = String(text || '')
    .split(/\r?\n/)
    .map((l) => l.trim())
    .filter(Boolean);
  const rows: ParsedCountSuggestion[] = [];
  let currentMaterial = '';
  for (const line of lines) {
    const pkg = line.match(/(\d+)\s*paket/i);
    if (pkg && !/\d+\s*[x×]\s*\d+/i.test(line)) {
      rows.push({
        rawMaterial: currentMaterial || line,
        thicknessMm: null,
        widthMm: null,
        lengthMm: null,
        pieceCount: null,
        measuredVolumeM3: null,
        suggestionKind: 'package-estimate',
        note: `AI önerisi: fotoğrafta/metinde yaklaşık ${pkg[1]} paket — kesin stok değil.`,
      });
      continue;
    }
    const dimLine = line.match(/(\d+(?:[.,]\d+)?\s*[x×]\s*\d+(?:[.,]\d+)?\s*[x×]\s*\d+(?:[.,]\d+)?)\s*[-:–]?\s*(\d+(?:[.,]\d+)?)/i);
    if (dimLine) {
      const dims = parseDims(dimLine[1]!.replace(/,/g, '.'));
      const pcs = Number(String(dimLine[2]).replace(',', '.'));
      if (dims && Number.isFinite(pcs)) {
        rows.push({
          rawMaterial: currentMaterial || '—',
          thicknessMm: dims.t,
          widthMm: dims.w,
          lengthMm: dims.l,
          pieceCount: pcs,
          measuredVolumeM3: null,
          suggestionKind: 'row',
        });
        continue;
      }
    }
    const vol = line.match(/(\d+)\s*adet.*?(\d+(?:[.,]\d+)?)\s*m3/i);
    if (vol) {
      rows.push({
        rawMaterial: currentMaterial || 'Tomruk',
        thicknessMm: null,
        widthMm: null,
        lengthMm: null,
        pieceCount: Number(vol[1]),
        measuredVolumeM3: Number(vol[2]!.replace(',', '.')),
        suggestionKind: 'row',
      });
      continue;
    }
    if (!/\d/.test(line) || /[a-zA-ZçğıöşüÇĞİÖŞÜ]/.test(line)) {
      currentMaterial = line.replace(/[:\-]+$/, '').trim();
    }
  }
  return rows;
}

export function ocrEngineAvailable(): boolean {
  return false;
}
