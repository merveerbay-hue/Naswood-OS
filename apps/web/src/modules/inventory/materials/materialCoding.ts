/**
 * Master MaterialCode mint — user never types the code.
 * Format examples:
 *   HM-KR-PIN-001 · YM-LM-PIN-S-001 · MP-LP-AA-18-S · TW-CP-001
 * No automatic SKU/variant generation.
 */

export type MainCategory = 'HM' | 'YM' | 'MP' | 'TW';

export type MaterialCodingInput = {
  mainCategory: MainCategory;
  /** HM: TR|KR|LT|LAMT · YM: LM|LAM|PR · MP ignored (from wood+quality) · TW: C|Z|S|K family */
  materialType: string;
  /** Wood token: PIN|SP|… or MP LP|CP|… or TW species letter */
  woodToken: string;
  /** YM lamel: S|FJ · MP: S|FJ · else empty */
  productTypeToken?: string;
  /** MP quality AA|AB|… */
  quality?: string;
  /** MP thickness in code (18) */
  thicknessForCode?: number | null;
  /** Existing material codes to compute next sequence */
  existingCodes: string[];
};

const HM_TYPES = new Set(['TR', 'KR', 'LT', 'LAMT']);
const YM_TYPES = new Set(['LM', 'LAM', 'PR']);
const MP_WOOD = new Set(['LP', 'CP', 'KP', 'OP', 'CV', 'SP', 'IR', 'DB', 'KS', 'TK', 'LM', 'SD', 'GK', 'HS']);
const TW_FAMILY = new Set(['CP', 'CA', 'CI', 'CY', 'CAP', 'ZP', 'ZA', 'ZI', 'ZY', 'ZAP', 'SP', 'SA', 'SI', 'SY', 'SAP', 'KP', 'KA', 'KI']);

export const WOOD_OPTIONS_HM: { token: string; label: string }[] = [
  { token: 'PIN', label: 'Pine / Çam' },
  { token: 'SP', label: 'Spruce / Ladin' },
  { token: 'FIR', label: 'Fir / Göknar' },
  { token: 'BEE', label: 'Beech / Kayın' },
  { token: 'OAK', label: 'Oak / Meşe' },
  { token: 'ASH', label: 'Ash / Dişbudak' },
  { token: 'WAL', label: 'Walnut / Ceviz' },
  { token: 'CHE', label: 'Chestnut / Kestane' },
  { token: 'IRO', label: 'Iroko' },
  { token: 'SAP', label: 'Sapelli' },
  { token: 'AYO', label: 'Ayous' },
  { token: 'CAP', label: 'American Pine' },
  { token: 'TEK', label: 'Teak' },
];

export const HM_TYPE_OPTIONS = [
  { token: 'TR', label: 'Tomruk' },
  { token: 'KR', label: 'Kereste' },
  { token: 'LT', label: 'Lata' },
  { token: 'LAMT', label: 'Lamel Taslak' },
];

export const YM_TYPE_OPTIONS = [
  { token: 'LM', label: 'Lamel' },
  { token: 'LAM', label: 'Lamine' },
  { token: 'PR', label: 'Profil Taslağı' },
];

export const MP_WOOD_OPTIONS = [
  { token: 'LP', label: 'Ladin' },
  { token: 'CP', label: 'Çam' },
  { token: 'KP', label: 'Kayın' },
  { token: 'OP', label: 'Meşe' },
  { token: 'CV', label: 'Ceviz' },
  { token: 'SP', label: 'Sapelli' },
  { token: 'IR', label: 'İroko' },
  { token: 'DB', label: 'Dişbudak' },
  { token: 'KS', label: 'Kestane' },
  { token: 'TK', label: 'Teak' },
  { token: 'LM', label: 'Limba' },
  { token: 'SD', label: 'Sedir' },
  { token: 'GK', label: 'Göknar' },
  { token: 'HS', label: 'Huş' },
];

export const TW_FAMILY_OPTIONS = [
  { token: 'CP', label: 'Cephe / Pine' },
  { token: 'CA', label: 'Cephe / Ash' },
  { token: 'CI', label: 'Cephe / Iroko' },
  { token: 'CY', label: 'Cephe / Ayous' },
  { token: 'CAP', label: 'Cephe / American Pine' },
  { token: 'ZP', label: 'Zemin / Pine' },
  { token: 'ZA', label: 'Zemin / Ash' },
  { token: 'ZI', label: 'Zemin / Iroko' },
  { token: 'ZY', label: 'Zemin / Ayous' },
  { token: 'ZAP', label: 'Zemin / American Pine' },
  { token: 'SP', label: 'Sunshade / Pine' },
  { token: 'SA', label: 'Sunshade / Ash' },
  { token: 'SI', label: 'Sunshade / Iroko' },
  { token: 'SY', label: 'Sunshade / Ayous' },
  { token: 'SAP', label: 'Sunshade / American Pine' },
  { token: 'KP', label: 'Karkas / Pine' },
  { token: 'KA', label: 'Karkas / Ash' },
  { token: 'KI', label: 'Karkas / Iroko' },
];

function normToken(v: string): string {
  return String(v || '')
    .trim()
    .toUpperCase()
    .replace(/[^A-Z0-9]/g, '');
}

/** Prefix without sequence, e.g. HM-KR-PIN or MP-LP-AA-18-S or YM-LM-PIN-S */
export function buildCodePrefix(input: MaterialCodingInput): string | null {
  const cat = input.mainCategory;
  const type = normToken(input.materialType);
  const wood = normToken(input.woodToken);
  const pt = normToken(input.productTypeToken ?? '');
  const q = normToken(input.quality ?? '');

  if (cat === 'HM') {
    if (!HM_TYPES.has(type) || !wood) return null;
    return `HM-${type}-${wood}`;
  }
  if (cat === 'YM') {
    if (!YM_TYPES.has(type) || !wood) return null;
    if (type === 'LM') {
      if (pt !== 'S' && pt !== 'FJ') return null;
      return `YM-LM-${wood}-${pt}`;
    }
    return `YM-${type}-${wood}`;
  }
  if (cat === 'MP') {
    if (!MP_WOOD.has(wood) || !q) return null;
    if (pt !== 'S' && pt !== 'FJ') return null;
    const th = input.thicknessForCode;
    if (th == null || !(th > 0)) return null;
    return `MP-${wood}-${q}-${Math.round(th)}-${pt}`;
  }
  if (cat === 'TW') {
    if (!TW_FAMILY.has(wood)) return null;
    return `TW-${wood}`;
  }
  return null;
}

function nextSeq(prefix: string, existingCodes: string[]): number {
  const re = new RegExp(`^${prefix.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}-(\\d{3,})$`, 'i');
  let max = 0;
  for (const raw of existingCodes) {
    const c = String(raw || '').trim().toUpperCase();
    const m = c.match(re);
    if (m) max = Math.max(max, Number(m[1]));
  }
  return max + 1;
}

/** Mint next MaterialCode for prefix. */
export function mintMaterialCode(input: MaterialCodingInput): string | null {
  const prefix = buildCodePrefix(input);
  if (!prefix) return null;
  // MP codes already include thickness/type — sequence not used; uniqueness = full code
  if (input.mainCategory === 'MP') {
    const code = prefix;
    const exists = input.existingCodes.some((c) => c.trim().toUpperCase() === code.toUpperCase());
    return exists ? null : code; // null = duplicate exact MP code
  }
  const seq = nextSeq(prefix, input.existingCodes);
  return `${prefix}-${String(seq).padStart(3, '0')}`;
}

/** Preview without allocating (shows ### or next). */
export function previewMaterialCode(input: MaterialCodingInput): string {
  const prefix = buildCodePrefix(input);
  if (!prefix) return '—';
  if (input.mainCategory === 'MP') return prefix;
  const seq = nextSeq(prefix, input.existingCodes);
  return `${prefix}-${String(seq).padStart(3, '0')}`;
}
