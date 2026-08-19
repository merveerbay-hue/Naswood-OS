/**
 * EN 14081 material compliance scope + supported grading methods (DefinitionJson).
 * Real VISUAL/MACHINE choice belongs to future Production Lot / quality record — not here.
 */

export type ComplianceScope = 'NORMAL_STOCK' | 'STRUCTURAL_TIMBER';
export type GradingMethod = 'VISUAL' | 'MACHINE';

export const COMPLIANCE_SCOPES: { token: ComplianceScope; labelTr: string }[] = [
  { token: 'NORMAL_STOCK', labelTr: 'Normal Stok' },
  { token: 'STRUCTURAL_TIMBER', labelTr: 'Yapısal Kereste' },
];

export const GRADING_METHODS: { token: GradingMethod; labelTr: string }[] = [
  { token: 'VISUAL', labelTr: 'Görsel Sınıflandırma' },
  { token: 'MACHINE', labelTr: 'Makine Sınıflandırması' },
];

export type ComplianceInput = {
  complianceScope: ComplianceScope;
  supportedGradingMethods: GradingMethod[];
  /** Existing designer signals for excluded product families */
  mainCategory?: string;
  isThermowood?: boolean;
  productTypeToken?: string;
  isChemicallyModified?: boolean;
  isFireRetardantTreated?: boolean;
};

export function isExcludedFromEn14081(input: {
  mainCategory?: string;
  isThermowood?: boolean;
  productTypeToken?: string;
  isChemicallyModified?: boolean;
  isFireRetardantTreated?: boolean;
}): boolean {
  const cat = String(input.mainCategory || '').toUpperCase();
  if (cat === 'TW') return true;
  if (input.isThermowood) return true;
  if (String(input.productTypeToken || '').toUpperCase() === 'FJ') return true;
  if (input.isChemicallyModified || input.isFireRetardantTreated) return true;
  return false;
}

export type ComplianceValidation =
  | { ok: true }
  | { ok: false; code: string; message: string };

export function validateMaterialCompliance(input: ComplianceInput): ComplianceValidation {
  const scope = input.complianceScope || 'NORMAL_STOCK';
  const methods = [...(input.supportedGradingMethods || [])];

  if (isExcludedFromEn14081(input) && scope === 'STRUCTURAL_TIMBER') {
    return {
      ok: false,
      code: 'BUS-MAT-EN14081-EXCL',
      message:
        'Thermowood, finger-jointed veya kapsam dışı ürünler STRUCTURAL_TIMBER olamaz.',
    };
  }

  if (scope === 'NORMAL_STOCK') {
    if (methods.length > 0) {
      return {
        ok: false,
        code: 'BUS-MAT-EN14081-SCOPE',
        message: 'Normal stokta VISUAL/MACHINE desteklenen yöntem tanımlanamaz.',
      };
    }
    return { ok: true };
  }

  if (scope !== 'STRUCTURAL_TIMBER') {
    return {
      ok: false,
      code: 'BUS-MAT-EN14081-SCOPE',
      message: 'ComplianceScope NORMAL_STOCK veya STRUCTURAL_TIMBER olmalıdır.',
    };
  }

  if (methods.length === 0) {
    return {
      ok: false,
      code: 'BUS-MAT-EN14081-METHOD',
      message: 'Yapısal kereste için en az bir yöntem (VISUAL ve/veya MACHINE) seçilmelidir.',
    };
  }

  for (const m of methods) {
    if (m !== 'VISUAL' && m !== 'MACHINE') {
      return {
        ok: false,
        code: 'BUS-MAT-EN14081-METHOD',
        message: `Desteklenmeyen yöntem: ${m}`,
      };
    }
  }

  return { ok: true };
}

/** Payload fragment for DefinitionJson merge. */
export function complianceDefinitionFragment(input: ComplianceInput): {
  complianceScope: ComplianceScope;
  supportedGradingMethods: GradingMethod[];
} {
  const excluded = isExcludedFromEn14081(input);
  const scope: ComplianceScope =
    excluded || input.complianceScope !== 'STRUCTURAL_TIMBER'
      ? 'NORMAL_STOCK'
      : 'STRUCTURAL_TIMBER';
  const methods =
    scope === 'STRUCTURAL_TIMBER'
      ? ([...new Set(input.supportedGradingMethods)].filter(
          (m): m is GradingMethod => m === 'VISUAL' || m === 'MACHINE',
        ) as GradingMethod[])
      : [];
  return { complianceScope: scope, supportedGradingMethods: methods };
}

/** Legacy / missing → NORMAL_STOCK. */
export function readComplianceFromDefinition(definitionJson: string | null | undefined): {
  complianceScope: ComplianceScope;
  supportedGradingMethods: GradingMethod[];
} {
  if (!definitionJson || !String(definitionJson).trim()) {
    return { complianceScope: 'NORMAL_STOCK', supportedGradingMethods: [] };
  }
  try {
    const o = JSON.parse(definitionJson) as Record<string, unknown>;
    const scopeRaw = String(o.complianceScope ?? o.ComplianceScope ?? 'NORMAL_STOCK')
      .trim()
      .toUpperCase();
    const scope: ComplianceScope =
      scopeRaw === 'STRUCTURAL_TIMBER' ? 'STRUCTURAL_TIMBER' : 'NORMAL_STOCK';
    const rawMethods = o.supportedGradingMethods ?? o.SupportedGradingMethods;
    const methods: GradingMethod[] = [];
    if (Array.isArray(rawMethods)) {
      for (const m of rawMethods) {
        const u = String(m || '')
          .trim()
          .toUpperCase();
        if (u === 'VISUAL' || u === 'MACHINE') methods.push(u);
      }
    }
    if (scope === 'NORMAL_STOCK') return { complianceScope: scope, supportedGradingMethods: [] };
    return { complianceScope: scope, supportedGradingMethods: [...new Set(methods)] };
  } catch {
    return { complianceScope: 'NORMAL_STOCK', supportedGradingMethods: [] };
  }
}
