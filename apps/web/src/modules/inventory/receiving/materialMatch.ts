/**
 * OCR material label → Naswood material master matching (phase-1).
 * OCR confidence and material match score are separate concerns.
 */

export type MatchStatus = 'STRONG_MATCH' | 'REVIEW_REQUIRED' | 'NO_MATCH';

export type MaterialDims = {
  thickness: number;
  width: number;
  length: number;
  unit: 'mm';
};

export type MaterialCandidate = {
  id: string;
  code: string;
  name: string;
  description?: string | null;
  category?: string | null;
  unitOfMeasure?: string | null;
  status?: string | null;
  definitionJson?: string | null;
};

export type MaterialMatchResult = {
  material: MaterialCandidate;
  score: number;
  status: MatchStatus;
  reasons: string[];
  dimsMatched: boolean | null;
  nameScore: number;
  codeScore: number;
  dimScore: number;
  unitScore: number;
  categoryScore: number;
};

const STRONG = 95;
const REVIEW = 70;

/** Normalize free text for comparison (case, ×/x/-, spacing, mm). */
export function normalizeMaterialText(input: string): string {
  return (input ?? '')
    .normalize('NFKC')
    .toLowerCase()
    .replace(/[×✕✖xX]/g, 'x')
    .replace(/[\u2013\u2014\-_/.,]/g, ' ')
    .replace(/\bmm\b/g, ' ')
    .replace(/\s+/g, ' ')
    .trim();
}

/** Compact form: letters+digits only with x separators for dims — used for code-like compare. */
export function compactMaterialKey(input: string): string {
  return normalizeMaterialText(input).replace(/\s+/g, '');
}

/**
 * Parse thickness × width × length from OCR / name text.
 * Accepts: 26x140x3000, 26 × 140 × 3000, 26-140-3000, 26 X 140 X 3000
 */
export function parseDimensions(input: string): MaterialDims | null {
  const text = (input ?? '')
    .normalize('NFKC')
    .replace(/[Oo](?=\s*[×xX\-])/g, '0') // OCR O→0 near separators
    .replace(/[×✕✖]/g, 'x');

  const m = text.match(/(\d+(?:[.,]\d+)?)\s*[xX\-]\s*(\d+(?:[.,]\d+)?)\s*[xX\-]\s*(\d+(?:[.,]\d+)?)/);
  if (!m) return null;

  const thickness = Number(m[1].replace(',', '.'));
  const width = Number(m[2].replace(',', '.'));
  const length = Number(m[3].replace(',', '.'));
  if (![thickness, width, length].every((n) => Number.isFinite(n) && n > 0)) return null;

  return { thickness, width, length, unit: 'mm' };
}

export function dimsEqual(a: MaterialDims | null, b: MaterialDims | null, epsilon = 0.01): boolean {
  if (!a || !b) return false;
  return (
    Math.abs(a.thickness - b.thickness) <= epsilon &&
    Math.abs(a.width - b.width) <= epsilon &&
    Math.abs(a.length - b.length) <= epsilon
  );
}

export function parseDefinitionDims(definitionJson?: string | null): MaterialDims | null {
  if (!definitionJson?.trim()) return null;
  try {
    const def = JSON.parse(definitionJson) as Record<string, unknown>;
    const thickness = Number(def.NominalThicknessMm ?? def.nominalThicknessMm ?? def.thicknessMm);
    const width = Number(def.NominalWidthMm ?? def.nominalWidthMm ?? def.widthMm);
    const length = Number(def.NominalLengthMm ?? def.nominalLengthMm ?? def.lengthMm);
    if (![thickness, width, length].every((n) => Number.isFinite(n) && n > 0)) return null;
    return { thickness, width, length, unit: 'mm' };
  } catch {
    return null;
  }
}

function materialDims(m: MaterialCandidate): MaterialDims | null {
  return parseDefinitionDims(m.definitionJson) ?? parseDimensions(m.name) ?? parseDimensions(m.description ?? '');
}

function tokenSet(text: string): Set<string> {
  return new Set(
    normalizeMaterialText(text)
      .split(' ')
      .map((t) => t.trim())
      .filter((t) => t.length > 1 && !/^\d+$/.test(t)),
  );
}

/** Jaccard similarity on non-dimension tokens (0–100). */
export function nameSimilarity(a: string, b: string): number {
  const na = normalizeMaterialText(a);
  const nb = normalizeMaterialText(b);
  if (!na || !nb) return 0;
  if (na === nb) return 100;
  if (compactMaterialKey(a) === compactMaterialKey(b)) return 100;

  const sa = tokenSet(a);
  const sb = tokenSet(b);
  if (sa.size === 0 || sb.size === 0) return 0;
  let inter = 0;
  for (const t of sa) if (sb.has(t)) inter++;
  const union = sa.size + sb.size - inter;
  return Math.round((inter / union) * 100);
}

function normalizeUnit(u?: string | null): string {
  const v = (u ?? '').trim().toLowerCase();
  if (!v) return '';
  if (['adet', 'pcs', 'pc', 'piece', 'pieces', 'ea', 'stk'].includes(v)) return 'piece';
  return v;
}

export function matchStatusFromScore(score: number): MatchStatus {
  if (score >= STRONG) return 'STRONG_MATCH';
  if (score >= REVIEW) return 'REVIEW_REQUIRED';
  return 'NO_MATCH';
}

/**
 * Score one material against OCR label (+ optional OCR dimensions field).
 * Different dimensions ⇒ cannot be STRONG / auto-accept (capped below REVIEW).
 */
export function scoreMaterialMatch(
  ocrMaterialText: string,
  material: MaterialCandidate,
  ocrDimensionsText?: string,
): MaterialMatchResult {
  const reasons: string[] = [];
  const ocrNorm = normalizeMaterialText(ocrMaterialText);
  const ocrCompact = compactMaterialKey(ocrMaterialText);
  const codeNorm = normalizeMaterialText(material.code);
  const codeCompact = compactMaterialKey(material.code);
  const nameNorm = normalizeMaterialText(material.name);

  let codeScore = 0;
  if (ocrNorm === codeNorm || ocrCompact === codeCompact) {
    codeScore = 100;
    reasons.push('exact_code');
  } else if (ocrCompact.includes(codeCompact) || codeCompact.includes(ocrCompact)) {
    codeScore = 80;
    reasons.push('partial_code');
  }

  let nameScore = nameSimilarity(ocrMaterialText, material.name);
  if (ocrNorm === nameNorm) {
    nameScore = 100;
    reasons.push('exact_name');
  } else if (nameScore >= 70) {
    reasons.push('similar_name');
  }

  const ocrDims = parseDimensions(ocrMaterialText) ?? parseDimensions(ocrDimensionsText ?? '');
  const matDims = materialDims(material);
  let dimScore = 50; // unknown dims — neutral
  let dimsMatched: boolean | null = null;

  if (ocrDims && matDims) {
    if (dimsEqual(ocrDims, matDims)) {
      dimScore = 100;
      dimsMatched = true;
      reasons.push('dims_equal');
    } else {
      dimScore = 0;
      dimsMatched = false;
      reasons.push('dims_mismatch');
    }
  } else if (ocrDims && !matDims) {
    dimScore = 40;
    dimsMatched = null;
    reasons.push('dims_ocr_only');
  } else if (!ocrDims && matDims) {
    dimScore = 45;
    dimsMatched = null;
    reasons.push('dims_master_only');
  }

  const ocrUnitHint = normalizeUnit(
    (ocrMaterialText.match(/\b(adet|pcs|piece|pieces|m2|m³|m3|mm)\b/i)?.[1] as string) ?? '',
  );
  const matUnit = normalizeUnit(material.unitOfMeasure);
  let unitScore = 50;
  if (ocrUnitHint && matUnit) {
    unitScore = ocrUnitHint === matUnit ? 100 : ocrUnitHint === 'mm' ? 60 : 30;
    if (unitScore === 100) reasons.push('unit_match');
  } else if (matUnit) {
    unitScore = 60;
  }

  // Category: soft signal from tokens
  let categoryScore = 50;
  const cat = normalizeMaterialText(material.category ?? '');
  if (cat) {
    const tokens = tokenSet(ocrMaterialText);
    const catTokens = tokenSet(material.category ?? '');
    let hit = 0;
    for (const t of catTokens) if (tokens.has(t)) hit++;
    categoryScore = catTokens.size ? Math.round((hit / catTokens.size) * 100) : 50;
    if (categoryScore >= 50) reasons.push('category_signal');
  }

  // Weighted total — dimensions are decisive when both sides present.
  let score: number;
  if (codeScore === 100) {
    score = 100;
  } else if (dimsMatched === false) {
    // Different size = different product. Name similarity must not auto-accept.
    score = Math.min(REVIEW - 1, Math.round(nameScore * 0.35 + dimScore * 0.55 + unitScore * 0.05 + categoryScore * 0.05));
    reasons.push('capped_dim_mismatch');
  } else if (nameScore === 100 && dimsMatched === true) {
    score = 98;
  } else if (nameScore === 100 && dimsMatched === null) {
    score = 92;
  } else {
    score = Math.round(nameScore * 0.4 + dimScore * 0.4 + codeScore * 0.1 + unitScore * 0.05 + categoryScore * 0.05);
  }

  score = Math.max(0, Math.min(100, score));
  const status = matchStatusFromScore(score);

  return {
    material,
    score,
    status,
    reasons,
    dimsMatched,
    nameScore,
    codeScore,
    dimScore,
    unitScore,
    categoryScore,
  };
}

export function rankMaterialMatches(
  ocrMaterialText: string,
  materials: MaterialCandidate[],
  ocrDimensionsText?: string,
  limit = 8,
): MaterialMatchResult[] {
  const active = materials.filter((m) => !m.status || m.status.toLowerCase() === 'active');
  return active
    .map((m) => scoreMaterialMatch(ocrMaterialText, m, ocrDimensionsText))
    .sort((a, b) => b.score - a.score || a.material.code.localeCompare(b.material.code))
    .slice(0, limit);
}

export function bestMaterialMatch(
  ocrMaterialText: string,
  materials: MaterialCandidate[],
  ocrDimensionsText?: string,
): MaterialMatchResult | null {
  const ranked = rankMaterialMatches(ocrMaterialText, materials, ocrDimensionsText, 1);
  return ranked[0] ?? null;
}

export function formatDims(d: MaterialDims | null): string {
  if (!d) return '—';
  return `${d.thickness} × ${d.width} × ${d.length} ${d.unit}`;
}
