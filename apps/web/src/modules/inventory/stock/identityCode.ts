/**
 * ADR-002 §6 identity contract — same scanner rule as the backend (IdentityCodes.cs) and Operations.
 * A code without site prefix belongs to Antalya.
 */
export const IDENTITY_SCANNER_RULE = /^(PKG|LOT|LAM|AUTO|TRF|REST)-(?:(ANT|BUC)-)?(\d{6,})$/;

export const LOCAL_SITE_CODE = 'BUC';
export const FOREIGN_SITE_MESSAGE = 'Antalya kodu — salt okunur görünüm henüz bağlı değil';

export type IdentityType = 'PKG' | 'LOT' | 'LAM' | 'AUTO' | 'TRF' | 'REST';

export type IdentityCode = {
  code: string;
  type: IdentityType;
  siteCode: 'ANT' | 'BUC';
  sequence: number;
  hasSitePrefix: boolean;
  isLocal: boolean;
};

export function parseIdentityCode(raw: string): IdentityCode | null {
  const code = raw.trim().toUpperCase();
  const m = IDENTITY_SCANNER_RULE.exec(code);
  if (!m) return null;
  const sequence = Number(m[3]);
  if (!Number.isSafeInteger(sequence) || sequence <= 0) return null;
  const siteCode = (m[2] ?? 'ANT') as 'ANT' | 'BUC';
  return {
    code,
    type: m[1] as IdentityType,
    siteCode,
    sequence,
    hasSitePrefix: m[2] !== undefined,
    isLocal: siteCode === LOCAL_SITE_CODE,
  };
}

export type ScanResolution =
  | { kind: 'package'; code: string }
  | { kind: 'lot'; code: string }
  | { kind: 'transfer'; code: string }
  | { kind: 'unsupported'; code: string; message: string }
  | { kind: 'foreign'; code: string; message: string }
  | { kind: 'invalid'; message: string };

/** Decides what a scanned code opens. Foreign (Antalya) codes never reach this plant's API. */
export function resolveScan(raw: string): ScanResolution {
  const parsed = parseIdentityCode(raw);
  if (!parsed) return { kind: 'invalid', message: 'Geçersiz kod. Beklenen biçim: PKG-BUC-000001.' };
  if (!parsed.isLocal) return { kind: 'foreign', code: parsed.code, message: FOREIGN_SITE_MESSAGE };
  switch (parsed.type) {
    case 'PKG':
      return { kind: 'package', code: parsed.code };
    case 'LOT':
      return { kind: 'lot', code: parsed.code };
    case 'TRF':
      return { kind: 'transfer', code: parsed.code };
    default:
      return { kind: 'unsupported', code: parsed.code, message: `${parsed.type} kimlik tipi NOS'ta henüz yok.` };
  }
}
