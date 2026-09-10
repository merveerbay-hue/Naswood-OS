export function previewOpeningGroups(
  lines: Array<{
    materialCode: string;
    materialName?: string;
    locationCode: string;
    physicalGroupLabel?: string;
    thicknessMm?: number | null;
    widthMm?: number | null;
    lengthMm?: number | null;
    pieceCount?: number | null;
    qty: number;
    unit: string;
    key?: string;
  }>,
) {
  const lots = new Map<string, { materialCode: string; materialName: string; stacks: Map<string, typeof lines> }>();
  for (const line of lines) {
    const mat = line.materialCode.trim().toUpperCase();
    if (!lots.has(mat)) lots.set(mat, { materialCode: line.materialCode, materialName: line.materialName ?? '', stacks: new Map() });
    const lot = lots.get(mat)!;
    const label = (line.physicalGroupLabel ?? '').trim();
    const stackKey = label ? `${line.locationCode}|${label}` : `LINE-${line.key ?? Math.random()}`;
    if (!lot.stacks.has(stackKey)) lot.stacks.set(stackKey, []);
    lot.stacks.get(stackKey)!.push(line);
  }
  return [...lots.values()].map((lot) => ({
    materialCode: lot.materialCode,
    materialName: lot.materialName,
    lotHint: 'Otomatik oluşturulacak',
    packages: [...lot.stacks.values()].map((rows) => ({
      label: rows[0]?.physicalGroupLabel || 'satır paketi',
      locationCode: rows[0]?.locationCode ?? '',
      qty: rows.reduce((s, r) => s + r.qty, 0),
      unit: rows[0]?.unit ?? '',
      rows,
    })),
  }));
}
