import { Link } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { createResource, getResource, searchAllResource } from '@/api/business';
import { useAuth } from '@/auth/useAuth';
import { usePlantContext } from '@/auth/usePlantContext';
import { plantDisplayName } from '@/modules/inventory/locations/locationCatalog';
import { rankMaterialMatches, type MaterialCandidate } from '@/modules/inventory/receiving/materialMatch';
import { cancelCount, completeCount, isCountId, postCount, putCountLines, sessionId } from './countApi';
import { ocrEngineAvailable, parseCountListText, type ParsedCountSuggestion } from './cycleCountAi';
import {
  applyLabelMapping,
  downloadFieldCountTemplate,
  parseCountWorkbook,
  skipUniqueLabel,
  type CountExcelMaterial,
  type CountExcelPreview,
} from './cycleCountExcel';
import {
  buildCountMaterialCreateBody,
  canCreateCountMaterial,
  emptyCountMaterialDraft,
  findCountMaterialDuplicate,
  previewCountMaterialCode,
  HM_TYPE_OPTIONS,
  WOOD_OPTIONS_HM,
  type CountMaterialCreateDraft,
} from './countMaterialCreate';
import {
  calculateStockQty,
  formatMm,
  lineStatus,
  physicalKey,
  resolvePolicy,
} from './inventoryCountCalc';
import {
  canOpenCountDocument,
  canOpenCountSession,
  canSaveCountLines,
  showSystemQuantity,
  type CountLineDto,
  type CycleCountOpenDraft,
  type InventoryCountSession,
} from './cycleCountSession';

type WarehouseOpt = { code?: string; name?: string; status?: string };
type LocationOpt = { code?: string; name?: string; warehouseCode?: string; status?: string; locationType?: string };
type MaterialOpt = MaterialCandidate & { unitOfMeasure?: string | null; category?: string | null; definitionJson?: string | null };

type DraftLine = {
  key: string;
  materialId?: string;
  materialCode: string;
  materialName: string;
  locationCode: string;
  thicknessMm: string;
  widthMm: string;
  lengthMm: string;
  pieceCount: string;
  measuredVolumeM3: string;
  source: 'MANUAL' | 'EXCEL' | 'AI';
  keepSeparate: boolean;
  note: string;
  physicalGroupLabel?: string;
};

function n(v: string): number | null {
  const x = Number(String(v).replace(',', '.').trim());
  return Number.isFinite(x) && String(v).trim() !== '' ? x : null;
}

function statusLabel(st: string): string {
  switch (st) {
    case 'MATCHED':
      return 'UYUMLU';
    case 'VARIANCE':
      return 'FARK VAR';
    case 'UNEXPECTED':
      return 'BEKLENMEYEN STOK';
    case 'MISSING':
      return 'EKSİK STOK';
    case 'DUPLICATE':
      return 'ÇİFT SATIR';
    default:
      return st;
  }
}

export function CycleCountSessionPage() {
  const { user } = useAuth();
  const roles = user?.roles ?? [];
  const canSave = canSaveCountLines(roles);
  const canOpenDoc = canOpenCountDocument(roles);
  const canMintMaterial = canCreateCountMaterial(roles);
  const queryClient = useQueryClient();
  const { homePlantId, plantId: sessionPlantId, visiblePlantIds, canSwitchPlant, switchPlant, isHomeContext } =
    usePlantContext();

  const [workPlant, setWorkPlant] = useState(sessionPlantId || homePlantId);
  const [warehouseCode, setWarehouseCode] = useState('');
  const [locationCode, setLocationCode] = useState('');
  const [countType, setCountType] = useState<'Normal' | 'Blind'>('Normal');
  const [notes, setNotes] = useState('');
  const [opened, setOpened] = useState<InventoryCountSession | null>(null);
  const [draftLines, setDraftLines] = useState<DraftLine[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<'all' | 'variance' | 'matched' | 'unmatched'>('all');
  const [aiPreview, setAiPreview] = useState<ParsedCountSuggestion[] | null>(null);
  const [aiText, setAiText] = useState('');
  const [addQuery, setAddQuery] = useState('');
  const [addOpen, setAddOpen] = useState(false);
  const [savedNote, setSavedNote] = useState<string | null>(null);
  const [postResult, setPostResult] = useState<string | null>(null);
  const [excelPreview, setExcelPreview] = useState<CountExcelPreview | null>(null);
  const [createFor, setCreateFor] = useState<string | null>(null);
  const [createDraft, setCreateDraft] = useState<CountMaterialCreateDraft>(emptyCountMaterialDraft(''));
  const [createError, setCreateError] = useState<string | null>(null);

  useEffect(() => {
    setWorkPlant(sessionPlantId || homePlantId);
  }, [sessionPlantId, homePlantId]);

  const warehousesQuery = useQuery({
    queryKey: ['business', 'warehouses', 'cnt', workPlant],
    queryFn: () => searchAllResource<WarehouseOpt>('warehouses', undefined, { plantId: workPlant }),
  });
  const locationsQuery = useQuery({
    queryKey: ['business', 'locations', 'cnt', workPlant],
    queryFn: () => searchAllResource<LocationOpt>('locations', undefined, { plantId: workPlant }),
  });
  const materialsQuery = useQuery({
    queryKey: ['business', 'materials', 'cnt'],
    queryFn: () => searchAllResource<MaterialOpt>('materials'),
  });

  const sessionQuery = useQuery({
    queryKey: ['business', 'inventory-counts', opened ? sessionId(opened) : ''],
    enabled: isCountId(opened ? sessionId(opened) : ''),
    queryFn: () => getResource<InventoryCountSession>('inventory-counts', sessionId(opened)),
  });

  useEffect(() => {
    if (sessionQuery.data) setOpened(sessionQuery.data);
  }, [sessionQuery.data]);

  const warehouses = useMemo(
    () =>
      (warehousesQuery.data ?? []).filter(
        (w) => String(w.status ?? 'Active').toLowerCase() === 'active' && String(w.code ?? '').trim(),
      ),
    [warehousesQuery.data],
  );
  const locations = useMemo(
    () =>
      (locationsQuery.data ?? []).filter(
        (l) =>
          String(l.status ?? 'Active').toLowerCase() === 'active' &&
          String(l.warehouseCode ?? '').toUpperCase() === warehouseCode.toUpperCase() &&
          String(l.locationType ?? '').toUpperCase() !== 'WIP',
      ),
    [locationsQuery.data, warehouseCode],
  );
  const materials = materialsQuery.data ?? [];

  const draft: CycleCountOpenDraft = {
    plantId: workPlant,
    warehouseCode,
    locationCode,
    countType,
    notes,
  };
  const gate = canOpenCountSession(draft);
  const blind = (opened?.countType ?? countType) === 'Blind';
  const showSys = showSystemQuantity(roles, blind, opened?.status);

  const startMut = useMutation({
    mutationFn: async () => {
      if (!gate.ok) throw new Error(gate.reason);
      return createResource<InventoryCountSession>(
        'inventory-counts',
        {
          number: '',
          warehouseCode: warehouseCode.trim(),
          locationCode: locationCode.trim(),
          countType,
          status: 'COUNTING',
          notes: notes.trim(),
        },
        { plantId: workPlant },
      );
    },
    onSuccess: (doc) => {
      setOpened(doc);
      setError(null);
      void queryClient.invalidateQueries({ queryKey: ['business', 'inventory-counts'] });
    },
    onError: (e: Error) => setError(e.message),
  });

  const saveMut = useMutation({
    mutationFn: async () => {
      const id = sessionId(opened);
      if (!isCountId(id)) throw new Error('Sayım oturumu yok — önce sayımı başlatın.');
      const lines = draftLines.map((l) => ({
        materialId: l.materialId || undefined,
        materialCode: l.materialCode,
        materialName: l.materialName,
        locationCode: l.locationCode || opened?.locationCode || '',
        lotUnknown: true,
        physicalGroupLabel: l.physicalGroupLabel || '',
        packageNumber: '',
        thicknessMm: n(l.thicknessMm),
        widthMm: n(l.widthMm),
        lengthMm: n(l.lengthMm),
        pieceCount: n(l.pieceCount),
        measuredVolumeM3: n(l.measuredVolumeM3),
        source: l.source,
        keepSeparate: l.keepSeparate,
        notes: l.note,
      }));
      return putCountLines(id, lines);
    },
    onSuccess: (doc) => {
      setOpened(doc);
      setSavedNote('Sayılanlar kaydedildi — stok henüz değişmedi.');
      setError(null);
    },
    onError: (e: Error) => setError(e.message),
  });

  const completeMut = useMutation({
    mutationFn: async () => {
      const id = sessionId(opened);
      if (!isCountId(id)) throw new Error('Sayım yok');
      await saveMut.mutateAsync();
      return completeCount(id);
    },
    onSuccess: (doc) => {
      setOpened(doc);
      setSavedNote('Sayım incelemeye alındı.');
    },
    onError: (e: Error) => setError(e.message),
  });

  const postMut = useMutation({
    mutationFn: async () => {
      const id = sessionId(opened);
      if (!isCountId(id)) throw new Error('Sayım yok');
      return postCount(id);
    },
    onSuccess: (res) => {
      setPostResult(
        `${res.countNumber}: ${res.adjustmentCount} INVENTORY_COUNT_ADJUSTMENT hareketi. Bakiye overwrite edilmedi.`,
      );
      void queryClient.invalidateQueries({ queryKey: ['business', 'inventory-counts', opened?.id] });
      void queryClient.invalidateQueries({ queryKey: ['business', 'inventory'] });
    },
    onError: (e: Error) => setError(e.message),
  });

  const cancelMut = useMutation({
    mutationFn: async () => {
      const id = sessionId(opened);
      if (!isCountId(id)) throw new Error('Sayım yok');
      return cancelCount(id);
    },
    onSuccess: (doc) => setOpened(doc),
    onError: (e: Error) => setError(e.message),
  });

  const snapshotLines = opened?.lines?.filter((l) => l.role === 'SNAPSHOT') ?? [];
  const physicalLines = opened?.lines?.filter((l) => l.role === 'PHYSICAL') ?? [];

  const resultRows = useMemo(() => {
    const groups = new Map<string, { snap?: CountLineDto; phys: CountLineDto[] }>();
    for (const l of opened?.lines ?? []) {
      const k = `${l.materialCode}|${l.locationCode}|${l.batchNumber}`.toUpperCase();
      if (!groups.has(k)) groups.set(k, { phys: [] });
      const g = groups.get(k)!;
      if (l.role === 'SNAPSHOT') g.snap = l;
      else g.phys.push(l);
    }
    const rows: Array<{
      material: string;
      code: string;
      phys: string;
      system: number;
      counted: number;
      diff: number;
      unit: string;
      status: string;
      duplicate: boolean;
    }> = [];
    for (const g of groups.values()) {
      if (g.phys.length === 0) {
        const s = g.snap;
        if (!s) continue;
        rows.push({
          material: s.materialName,
          code: s.materialCode,
          phys: '—',
          system: s.systemQuantityAtStart,
          counted: 0,
          diff: -s.systemQuantityAtStart,
          unit: s.stockUnit,
          status: lineStatus(s.systemQuantityAtStart, 0, false),
          duplicate: false,
        });
        continue;
      }
      for (const p of g.phys) {
        rows.push({
          material: p.materialName,
          code: p.materialCode,
          phys: formatMm(p.thicknessMm, p.widthMm, p.lengthMm),
          system: g.phys.length === 1 ? (g.snap?.systemQuantityAtStart ?? 0) : g.snap?.systemQuantityAtStart ?? 0,
          counted: p.countedQuantity,
          diff: p.difference,
          unit: p.stockUnit,
          status: p.duplicate ? 'DUPLICATE' : p.lineStatus,
          duplicate: p.duplicate,
        });
      }
    }
    return rows;
  }, [opened?.lines]);

  const filteredRows = resultRows.filter((r) => {
    if (filter === 'variance') return r.status === 'VARIANCE' || r.status === 'UNEXPECTED' || r.status === 'MISSING' || r.duplicate;
    if (filter === 'matched') return r.status === 'MATCHED';
    if (filter === 'unmatched') return r.status === 'UNEXPECTED' || !r.code;
    return true;
  });

  const materialHits = useMemo(() => {
    const q = addQuery.trim();
    if (!q) return materials.slice(0, 20);
    const ranked = rankMaterialMatches(q, materials, q, 20);
    return ranked.map((r) => r.material);
  }, [addQuery, materials]);

  function addDraft(mat: MaterialOpt, source: DraftLine['source'] = 'MANUAL', extra?: Partial<DraftLine>) {
    setDraftLines((prev) => [
      ...prev,
      {
        key: `${Date.now()}-${Math.random()}`,
        materialCode: mat.code,
        materialName: mat.name,
        materialId: mat.id,
        locationCode: locationCode || opened?.locationCode || extra?.locationCode || '',
        thicknessMm: extra?.thicknessMm ?? '',
        widthMm: extra?.widthMm ?? '',
        lengthMm: extra?.lengthMm ?? '',
        pieceCount: extra?.pieceCount ?? '',
        measuredVolumeM3: extra?.measuredVolumeM3 ?? '',
        source,
        keepSeparate: false,
        note: extra?.note ?? '',
        physicalGroupLabel: extra?.physicalGroupLabel ?? '',
      },
    ]);
    setAddOpen(false);
    setAddQuery('');
  }

  async function onExcel(file: File) {
    setError(null);
    const preview = await parseCountWorkbook(file, materials);
    setExcelPreview(preview);
    if (preview.total === 0) {
      setError('Excel’de sayım satırı bulunamadı.');
      return;
    }
    setSavedNote(
      `Sayım dosyası kontrolü: ${preview.total} satır · ${preview.uniqueCount} unique tanım · ${preview.matched} eşleşti · ${preview.suggested} kontrol · ${preview.newCandidates} yeni aday · ${preview.invalid} hatalı.`,
    );
  }

  function confirmExcelPreview() {
    if (!excelPreview) return;
    const matched = excelPreview.rows.filter((r) => r.status === 'MATCHED' && r.materialCode && !r.skipped);
    const loc = locationCode || opened?.locationCode || '';
    setDraftLines((prev) => [
      ...prev,
      ...matched.map((r, i) => ({
        key: `xl-${r.excelRow}-${Date.now()}-${i}`,
        materialId: r.materialId,
        materialCode: r.materialCode!,
        materialName: r.materialName || r.materialLabel,
        locationCode: loc,
        thicknessMm: r.thicknessMm != null ? String(r.thicknessMm) : '',
        widthMm: r.widthMm != null ? String(r.widthMm) : '',
        lengthMm: r.lengthMm != null ? String(r.lengthMm) : '',
        pieceCount: r.pieceCount != null ? String(r.pieceCount) : r.quantity != null ? String(r.quantity) : '',
        measuredVolumeM3: r.measuredVolumeM3 != null ? String(r.measuredVolumeM3) : '',
        source: 'EXCEL' as const,
        keepSeparate: false,
        note: r.note,
        physicalGroupLabel: r.physicalGroupLabel,
      })),
    ]);
    setExcelPreview(null);
    setSavedNote(
      `${matched.length} satır sayıma eklendi. ${excelPreview.suggested + excelPreview.newCandidates + excelPreview.invalid} satır bekletildi.`,
    );
  }

  function previewStatusTr(st: string): string {
    if (st === 'MATCHED') return 'EŞLEŞTİ';
    if (st === 'SUGGESTED') return 'KONTROL GEREKİYOR';
    if (st === 'NEW_CANDIDATE') return 'YENİ MATERIAL ADAYI';
    if (st === 'REVIEW_REQUIRED') return 'KONTROL GEREKİYOR';
    return 'HATALI SATIR';
  }

  async function createCandidateMaterial() {
    setCreateError(null);
    const dup = findCountMaterialDuplicate(createDraft, materials);
    if (dup) {
      setCreateError(`Aynı kart zaten var: ${dup.name} (${dup.code}). Yeni kart oluşturulmadı.`);
      if (createFor && excelPreview) {
        setExcelPreview(applyLabelMapping(excelPreview, createFor, dup as CountExcelMaterial));
        setCreateFor(null);
      }
      return;
    }
    try {
      const body = buildCountMaterialCreateBody(createDraft, materials.map((m) => m.code));
      const created = await createResource<CountExcelMaterial>('materials', body);
      void queryClient.invalidateQueries({ queryKey: ['business', 'materials'] });
      if (createFor) {
        setExcelPreview((p) => (p ? applyLabelMapping(p, createFor, { ...created, name: created.name || createDraft.name }) : p));
      }
      setCreateFor(null);
    } catch (e) {
      setCreateError(e instanceof Error ? e.message : String(e));
    }
  }

  function applyAiRow(s: ParsedCountSuggestion, mat: MaterialOpt) {
    addDraft(mat, 'AI', {
      thicknessMm: s.thicknessMm != null ? String(s.thicknessMm) : '',
      widthMm: s.widthMm != null ? String(s.widthMm) : '',
      lengthMm: s.lengthMm != null ? String(s.lengthMm) : '',
      pieceCount: s.pieceCount != null ? String(s.pieceCount) : '',
      measuredVolumeM3: s.measuredVolumeM3 != null ? String(s.measuredVolumeM3) : '',
      note: s.note ?? '',
    });
  }

  const counting = !opened || ['DRAFT', 'COUNTING'].includes(String(opened.status).toUpperCase());
  const review = opened && ['REVIEW', 'APPROVED'].includes(String(opened.status).toUpperCase());
  const posted = opened && String(opened.status).toUpperCase() === 'POSTED';

  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs text-[var(--text-muted)]">INV-CNT · stok ledger</p>
        <h1 className="text-2xl font-semibold">Stok Sayımı</h1>
        <p className="text-sm text-[var(--text-muted)]">
          Mevcut stok listesi gelir. Düzeltme yalnızca INVENTORY_COUNT_ADJUSTMENT hareketidir.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Yeni stok sayımı</CardTitle>
          <CardDescription>Ana Üs otomatik gelir; günlük fabrika seçimi yok.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-3">
          <div className="grid gap-3 md:grid-cols-2">
            <label className="text-sm">
              Ana Üs
              <Input value={plantDisplayName(homePlantId)} disabled readOnly />
            </label>
            {canSwitchPlant ? (
              <label className="text-sm">
                Diğer tesis
                <select
                  className="mt-1 w-full rounded-md border border-[var(--border)] bg-transparent px-3 py-2"
                  value={workPlant}
                  onChange={(e) => {
                    const next = e.target.value;
                    setWorkPlant(next);
                    switchPlant?.(next);
                  }}
                >
                  {visiblePlantIds.map((id) => (
                    <option key={id} value={id}>
                      {plantDisplayName(id)}
                      {id === homePlantId ? ' (Ana Üs)' : ''}
                    </option>
                  ))}
                </select>
              </label>
            ) : (
              <p className="text-sm text-[var(--text-muted)] self-end">
                {isHomeContext ? 'Yalnızca Ana Üs sayımı.' : `Çalışma tesisi: ${plantDisplayName(workPlant)}`}
              </p>
            )}
            <label className="text-sm">
              Depo
              <select
                className="mt-1 w-full rounded-md border border-[var(--border)] bg-transparent px-3 py-2"
                value={warehouseCode}
                onChange={(e) => setWarehouseCode(e.target.value)}
                disabled={Boolean(opened)}
              >
                <option value="">Seçin</option>
                {warehouses.map((w) => (
                  <option key={w.code} value={w.code}>
                    {w.code} · {w.name}
                  </option>
                ))}
              </select>
            </label>
            <label className="text-sm">
              Lokasyon
              <select
                className="mt-1 w-full rounded-md border border-[var(--border)] bg-transparent px-3 py-2"
                value={locationCode}
                onChange={(e) => setLocationCode(e.target.value)}
                disabled={Boolean(opened)}
              >
                <option value="">Tümü</option>
                {locations.map((l) => (
                  <option key={l.code} value={l.code}>
                    {l.code} · {l.name}
                  </option>
                ))}
              </select>
            </label>
            <label className="text-sm">
              Sayım tipi
              <select
                className="mt-1 w-full rounded-md border border-[var(--border)] bg-transparent px-3 py-2"
                value={countType}
                onChange={(e) => setCountType(e.target.value as 'Normal' | 'Blind')}
                disabled={Boolean(opened)}
              >
                <option value="Normal">Normal Sayım</option>
                <option value="Blind">Kör Sayım</option>
              </select>
            </label>
            <label className="text-sm">
              Açıklama
              <Input value={notes} onChange={(e) => setNotes(e.target.value)} disabled={Boolean(opened)} />
            </label>
          </div>
          {!opened ? (
            <div className="flex flex-wrap gap-2">
              <Button disabled={!canOpenDoc || !gate.ok || startMut.isPending} onClick={() => startMut.mutate()}>
                Sayımı başlat
              </Button>
              <Button
                variant="secondary"
                disabled={!warehouseCode}
                onClick={() =>
                  downloadFieldCountTemplate(
                    materials,
                    [],
                  )
                }
              >
                Sayım şablonunu indir
              </Button>
            </div>
          ) : (
            <p className="text-sm">
              {opened.number} · {opened.status}
              {opened.snapshotAt ? ` · snapshot ${new Date(opened.snapshotAt).toLocaleString('tr-TR')}` : ''}
            </p>
          )}
          {!gate.ok && !opened ? <p className="text-sm text-red-600">{gate.reason}</p> : null}
        </CardContent>
      </Card>

      {opened ? (
        <Card>
          <CardHeader>
            <CardTitle>Giriş yöntemleri</CardTitle>
            <CardDescription>Yüzlerce satırı tek tek yazmayın. AI sonucu stoğa yazılmaz.</CardDescription>
          </CardHeader>
          <CardContent className="flex flex-wrap gap-2">
            <Button
              variant="secondary"
              disabled={!counting}
              onClick={() =>
                downloadFieldCountTemplate(
                  materials,
                  snapshotLines.map((l) => ({
                    materialCode: l.materialCode,
                    materialName: l.materialName,
                  })),
                )
              }
            >
              Sayım şablonunu indir
            </Button>
            <label className="inline-flex">
              <input
                type="file"
                accept=".csv,.xls,.xlsx,.xml,text/csv"
                className="hidden"
                disabled={!counting || !canSave}
                onChange={(e) => {
                  const f = e.target.files?.[0];
                  if (f) void onExcel(f);
                  e.target.value = '';
                }}
              />
              <span className="inline-flex h-9 cursor-pointer items-center rounded-md border px-3 text-sm">Excel yükle</span>
            </label>
            <Button variant="secondary" disabled={!counting} onClick={() => setAiPreview([])}>
              Sayım listesi / fotoğraf
            </Button>
            <Button variant="secondary" disabled={!counting} onClick={() => setAddOpen(true)}>
              Hızlı giriş / malzeme ekle
            </Button>
          </CardContent>
        </Card>
      ) : null}

      {excelPreview ? (
        <Card>
          <CardHeader>
            <CardTitle>Sayım dosyası kontrolü</CardTitle>
            <CardDescription>
              Toplam {excelPreview.total} · Unique tanım {excelPreview.uniqueCount} · Eşleşen {excelPreview.matched} ·
              Eşleştirme gereken {excelPreview.suggested} · Yeni aday {excelPreview.newCandidates} · Hatalı{' '}
              {excelPreview.invalid}. Stoğa yazılmaz.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            {excelPreview.uniques
              .filter((u) => u.status === 'SUGGESTED' || u.status === 'NEW_CANDIDATE')
              .map((u) => (
                <div key={u.key} className="rounded-md border border-[var(--border)] p-3 space-y-2">
                  <p className="text-sm font-medium">
                    Excel tanımı: {u.excelLabel}{' '}
                    <span className="text-[var(--text-muted)] font-normal">({u.rowCount} satır)</span>
                  </p>
                  {u.status === 'SUGGESTED' ? (
                    <>
                      <p className="text-sm">Önerilen material: {u.suggestedName || '—'}</p>
                      <div className="flex flex-wrap gap-2">
                        <Button
                          disabled={!u.suggestedId}
                          onClick={() => {
                            const card = materials.find((m) => m.id === u.suggestedId);
                            if (card) setExcelPreview((p) => (p ? applyLabelMapping(p, u.excelLabel, card) : p));
                          }}
                        >
                          Eşleştir
                        </Button>
                        <select
                          className="rounded-md border border-[var(--border)] bg-transparent px-2 py-1 text-sm"
                          defaultValue=""
                          onChange={(e) => {
                            const card = materials.find((m) => m.id === e.target.value);
                            if (!card) return;
                            setExcelPreview((p) => (p ? applyLabelMapping(p, u.excelLabel, card) : p));
                          }}
                        >
                          <option value="">Başka malzeme seç</option>
                          {materials.map((m) => (
                            <option key={m.id || m.code} value={m.id}>
                              {m.name}
                            </option>
                          ))}
                        </select>
                        <Button
                          variant="secondary"
                          onClick={() => {
                            setCreateDraft(emptyCountMaterialDraft(u.excelLabel));
                            setCreateError(null);
                            setCreateFor(u.excelLabel);
                          }}
                        >
                          Yeni malzeme oluştur
                        </Button>
                      </div>
                    </>
                  ) : (
                    <>
                      <p className="text-sm">Bu tanım Material Master’da bulunamadı.</p>
                      <div className="flex flex-wrap gap-2">
                        <select
                          className="rounded-md border border-[var(--border)] bg-transparent px-2 py-1 text-sm"
                          defaultValue=""
                          onChange={(e) => {
                            const card = materials.find((m) => m.id === e.target.value);
                            if (!card) return;
                            setExcelPreview((p) => (p ? applyLabelMapping(p, u.excelLabel, card) : p));
                          }}
                        >
                          <option value="">Mevcut malzemeyle eşleştir</option>
                          {materials.map((m) => (
                            <option key={m.id || m.code} value={m.id}>
                              {m.name}
                            </option>
                          ))}
                        </select>
                        <Button
                          disabled={!canMintMaterial}
                          onClick={() => {
                            setCreateDraft(emptyCountMaterialDraft(u.excelLabel));
                            setCreateError(null);
                            setCreateFor(u.excelLabel);
                          }}
                        >
                          Yeni malzeme kartı oluştur
                        </Button>
                        <Button
                          variant="secondary"
                          onClick={() => setExcelPreview((p) => (p ? skipUniqueLabel(p, u.excelLabel) : p))}
                        >
                          Bu satırları atla
                        </Button>
                      </div>
                      {!canMintMaterial ? (
                        <p className="text-xs text-[var(--text-muted)]">
                          Yeni kart yalnızca Material.Create yetkisi (Administrator) ile oluşturulur. Aday kaydı duruyor.
                        </p>
                      ) : null}
                    </>
                  )}
                </div>
              ))}
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="text-left text-[var(--text-muted)]">
                    <th>Excel Malzemesi</th>
                    <th>NASWOOD Malzemesi</th>
                    <th>Gerçek ölçü</th>
                    <th>Adet</th>
                    <th>Paket/İstif</th>
                    <th>Durum</th>
                  </tr>
                </thead>
                <tbody>
                  {excelPreview.rows.map((r) => (
                    <tr key={r.excelRow} className="border-t border-[var(--border)]">
                      <td>{r.materialLabel}</td>
                      <td>{r.materialName || r.suggestedName || '—'}</td>
                      <td>{[r.thicknessMm, r.widthMm, r.lengthMm].filter((x) => x != null).join('×') || '—'}</td>
                      <td>{r.pieceCount ?? r.quantity ?? '—'}</td>
                      <td>{r.physicalGroupLabel || '—'}</td>
                      <td>
                        {r.skipped ? 'ATLANDI' : previewStatusTr(r.status)}
                        {r.error ? <div className="text-xs text-red-600">{r.error}</div> : null}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <div className="flex flex-wrap gap-2">
              <Button disabled={excelPreview.matched === 0} onClick={confirmExcelPreview}>
                Eşleşenleri sayıma ekle
              </Button>
              <Button variant="secondary" onClick={() => setExcelPreview(null)}>
                Önizlemeyi kapat
              </Button>
            </div>
          </CardContent>
        </Card>
      ) : null}

      {createFor ? (
        <Card>
          <CardHeader>
            <CardTitle>Yeni malzeme kartı</CardTitle>
            <CardDescription>
              Excel tanımı: {createFor}. Kod otomatik. Fiziksel sayım ölçüsü nominal olmaz.
            </CardDescription>
          </CardHeader>
          <CardContent className="grid gap-3 md:grid-cols-2">
            <label className="text-sm md:col-span-2">
              Ad
              <Input value={createDraft.name} onChange={(e) => setCreateDraft((d) => ({ ...d, name: e.target.value }))} />
            </label>
            <label className="text-sm">
              Ana grup
              <select
                className="mt-1 w-full rounded-md border border-[var(--border)] bg-transparent px-3 py-2"
                value={createDraft.mainCategory}
                onChange={(e) =>
                  setCreateDraft((d) => ({ ...d, mainCategory: e.target.value as CountMaterialCreateDraft['mainCategory'] }))
                }
              >
                <option value="HM">Hammadde</option>
                <option value="YM">Yarı Mamul</option>
                <option value="MP">Masif Panel</option>
                <option value="TW">Thermowood</option>
              </select>
            </label>
            <label className="text-sm">
              Malzeme cinsi
              <select
                className="mt-1 w-full rounded-md border border-[var(--border)] bg-transparent px-3 py-2"
                value={createDraft.materialTypeToken}
                onChange={(e) => setCreateDraft((d) => ({ ...d, materialTypeToken: e.target.value }))}
              >
                {HM_TYPE_OPTIONS.map((o) => (
                  <option key={o.token} value={o.token}>
                    {o.label}
                  </option>
                ))}
              </select>
            </label>
            <label className="text-sm">
              Ağaç türü
              <select
                className="mt-1 w-full rounded-md border border-[var(--border)] bg-transparent px-3 py-2"
                value={createDraft.woodToken}
                onChange={(e) => setCreateDraft((d) => ({ ...d, woodToken: e.target.value }))}
              >
                {WOOD_OPTIONS_HM.map((o) => (
                  <option key={o.token} value={o.token}>
                    {o.label}
                  </option>
                ))}
              </select>
            </label>
            <label className="text-sm">
              Nominal kalınlık (opsiyonel)
              <Input value={createDraft.thicknessMm} onChange={(e) => setCreateDraft((d) => ({ ...d, thicknessMm: e.target.value }))} />
            </label>
            <label className="text-sm">
              Nominal genişlik (opsiyonel)
              <Input value={createDraft.widthMm} onChange={(e) => setCreateDraft((d) => ({ ...d, widthMm: e.target.value }))} />
            </label>
            <label className="text-sm">
              Stok birimi
              <Input value={createDraft.stockUom} onChange={(e) => setCreateDraft((d) => ({ ...d, stockUom: e.target.value }))} />
            </label>
            <label className="text-sm">
              Sayım birimi
              <Input value={createDraft.countUom} onChange={(e) => setCreateDraft((d) => ({ ...d, countUom: e.target.value }))} />
            </label>
            <p className="text-sm md:col-span-2">
              Sistem kodu: {previewCountMaterialCode(createDraft, materials.map((m) => m.code)) || '—'}
            </p>
            {createError ? <p className="text-sm text-red-600 md:col-span-2">{createError}</p> : null}
            <div className="flex gap-2 md:col-span-2">
              <Button onClick={() => void createCandidateMaterial()}>Oluştur ve eşleştir</Button>
              <Button variant="secondary" onClick={() => setCreateFor(null)}>
                Vazgeç
              </Button>
            </div>
          </CardContent>
        </Card>
      ) : null}

      {opened?.midCountMovement ? (
        <p className="rounded-md border border-amber-500/40 bg-amber-500/10 px-3 py-2 text-sm">
          SAYIM SIRASINDA STOK HAREKETİ VAR ({opened.midCountMovementCount}). Snapshot korunur; bu otomatik hata değildir.
        </p>
      ) : null}

      {opened && counting ? (
        <Card>
          <CardHeader>
            <CardTitle>Sistem stok listesi</CardTitle>
            <CardDescription>Sayım başındaki snapshot. Kör sayımda miktar gizlidir.</CardDescription>
          </CardHeader>
          <CardContent className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-left text-[var(--text-muted)]">
                  <th className="py-1">Malzeme</th>
                  <th>Lokasyon</th>
                  <th>Lot</th>
                  <th>Sistem</th>
                </tr>
              </thead>
              <tbody>
                {snapshotLines.map((l) => (
                  <tr key={l.id} className="border-t border-[var(--border)]">
                    <td className="py-1">
                      {l.materialCode}
                      <div className="text-xs text-[var(--text-muted)]">{l.materialName}</div>
                    </td>
                    <td>{l.locationCode}</td>
                    <td>{l.lotUnknown ? 'LOT BELİRSİZ' : l.batchNumber || '—'}</td>
                    <td>{showSys ? `${l.systemQuantityAtStart} ${l.stockUnit}` : 'GİZLİ'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            {snapshotLines.length === 0 ? <p className="text-sm text-[var(--text-muted)]">Bu depoda snapshot satırı yok — malzeme ekleyin.</p> : null}
          </CardContent>
        </Card>
      ) : null}

      {opened && counting ? (
        <Card>
          <CardHeader>
            <CardTitle>Sayılanlar</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            {draftLines.map((l, idx) => {
              const mat = materials.find((m) => m.code === l.materialCode);
              const policy = resolvePolicy(mat ?? {});
              const calc = calculateStockQty(policy, {
                thicknessMm: n(l.thicknessMm),
                widthMm: n(l.widthMm),
                lengthMm: n(l.lengthMm),
                pieceCount: n(l.pieceCount),
                measuredVolumeM3: n(l.measuredVolumeM3),
              });
              const dup = draftLines.filter(
                (o) =>
                  physicalKey(o.materialCode, o.locationCode, '', n(o.thicknessMm), n(o.widthMm), n(o.lengthMm)) ===
                  physicalKey(l.materialCode, l.locationCode, '', n(l.thicknessMm), n(l.widthMm), n(l.lengthMm)),
              );
              return (
                <div key={l.key} className="grid gap-2 rounded-md border border-[var(--border)] p-2 md:grid-cols-8">
                  <div className="md:col-span-2 text-sm">
                    {l.materialName}
                    <div className="text-xs text-[var(--text-muted)]">{l.materialCode}</div>
                    {l.physicalGroupLabel ? (
                      <div className="text-xs text-[var(--text-muted)]">İstif: {l.physicalGroupLabel}</div>
                    ) : null}
                    {dup.length > 1 && !l.keepSeparate ? (
                      <div className="text-xs text-amber-600">
                        Çift satır{' '}
                        <button
                          className="underline"
                          type="button"
                          onClick={() => {
                            const keep = dup[0]!;
                            const pcs = dup.reduce((s, x) => s + (n(x.pieceCount) ?? 0), 0);
                            setDraftLines((prev) => [
                              ...prev.filter((x) => !dup.some((d) => d.key === x.key)),
                              { ...keep, pieceCount: String(pcs) },
                            ]);
                          }}
                        >
                          Birleştir
                        </button>{' '}
                        /{' '}
                        <button
                          className="underline"
                          type="button"
                          onClick={() =>
                            setDraftLines((prev) => prev.map((x) => (x.key === l.key ? { ...x, keepSeparate: true } : x)))
                          }
                        >
                          Ayrı tut
                        </button>
                      </div>
                    ) : null}
                  </div>
                  {policy.dimsRequired || policy.mode === 'CubicMeter' ? (
                    <>
                      <Input placeholder="Kalınlık" value={l.thicknessMm} onChange={(e) => setDraftLines((p) => p.map((x, i) => (i === idx ? { ...x, thicknessMm: e.target.value } : x)))} />
                      <Input placeholder="Genişlik" value={l.widthMm} onChange={(e) => setDraftLines((p) => p.map((x, i) => (i === idx ? { ...x, widthMm: e.target.value } : x)))} />
                      <Input placeholder="Boy" value={l.lengthMm} onChange={(e) => setDraftLines((p) => p.map((x, i) => (i === idx ? { ...x, lengthMm: e.target.value } : x)))} />
                    </>
                  ) : policy.mode === 'SquareMeter' ? (
                    <>
                      <Input placeholder="Genişlik" value={l.widthMm} onChange={(e) => setDraftLines((p) => p.map((x, i) => (i === idx ? { ...x, widthMm: e.target.value } : x)))} />
                      <Input placeholder="Boy" value={l.lengthMm} onChange={(e) => setDraftLines((p) => p.map((x, i) => (i === idx ? { ...x, lengthMm: e.target.value } : x)))} />
                      <span />
                    </>
                  ) : policy.mode === 'MeasuredVolume' ? (
                    <>
                      <Input placeholder="Hacim m³" value={l.measuredVolumeM3} onChange={(e) => setDraftLines((p) => p.map((x, i) => (i === idx ? { ...x, measuredVolumeM3: e.target.value } : x)))} />
                      <span />
                      <span />
                    </>
                  ) : (
                    <span className="md:col-span-3 text-xs text-[var(--text-muted)]">Ölçü zorunlu değil</span>
                  )}
                  <Input placeholder="Adet" value={l.pieceCount} onChange={(e) => setDraftLines((p) => p.map((x, i) => (i === idx ? { ...x, pieceCount: e.target.value } : x)))} />
                  <div className="text-sm self-center">{calc.ok ? `${calc.qty.toFixed(4)} ${policy.stockUnit}` : calc.error}</div>
                  <Button variant="secondary" onClick={() => setDraftLines((p) => p.filter((_, i) => i !== idx))}>
                    Sil
                  </Button>
                </div>
              );
            })}
            <div className="flex flex-wrap gap-2">
              <Button disabled={!canSave || saveMut.isPending} onClick={() => saveMut.mutate()}>
                Sayılanları kaydet
              </Button>
              <Button disabled={!canSave || completeMut.isPending} onClick={() => completeMut.mutate()}>
                Sayımı tamamla
              </Button>
              <Button variant="secondary" onClick={() => cancelMut.mutate()}>
                İptal
              </Button>
            </div>
          </CardContent>
        </Card>
      ) : null}

      {opened && (review || posted || physicalLines.length > 0) ? (
        <Card>
          <CardHeader>
            <CardTitle>Stok sayım sonucu</CardTitle>
            <CardDescription>
              Toplam {opened.summary?.totalLines ?? resultRows.length} · Uyumlu {opened.summary?.matched ?? 0} · Farklı{' '}
              {opened.summary?.variance ?? 0} · Eşleşmeyen {(opened.summary?.unexpected ?? 0) + (opened.summary?.missing ?? 0)} ·
              Sayım sırasında hareket {opened.midCountMovement ? 'VAR' : 'yok'}
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            <div className="flex gap-2 text-sm">
              {(['all', 'variance', 'matched', 'unmatched'] as const).map((f) => (
                <button
                  key={f}
                  type="button"
                  className={filter === f ? 'underline' : 'text-[var(--text-muted)]'}
                  onClick={() => setFilter(f)}
                >
                  {f === 'all' ? 'Tümü' : f === 'variance' ? 'Fark olanlar' : f === 'matched' ? 'Uyumlu' : 'Eşleşmeyen'}
                </button>
              ))}
            </div>
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="text-left text-[var(--text-muted)]">
                    <th>Malzeme</th>
                    <th>Fiziksel ölçü</th>
                    <th>Sistem</th>
                    <th>Sayım</th>
                    <th>Fark</th>
                    <th>Birim</th>
                    <th>Durum</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredRows.map((r, i) => (
                    <tr key={`${r.code}-${i}`} className="border-t border-[var(--border)]">
                      <td>
                        {r.code}
                        <div className="text-xs text-[var(--text-muted)]">{r.material}</div>
                      </td>
                      <td>{r.phys}</td>
                      <td>{showSys || review || posted ? r.system : 'GİZLİ'}</td>
                      <td>{r.counted}</td>
                      <td>{showSys || review || posted ? r.diff : '—'}</td>
                      <td>{r.unit}</td>
                      <td>{statusLabel(r.status)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            {review && !posted ? (
              <Button disabled={!canSave || postMut.isPending} onClick={() => postMut.mutate()}>
                Farkları onayla ve stoğa işle
              </Button>
            ) : null}
            {postResult ? <p className="text-sm">{postResult}</p> : null}
          </CardContent>
        </Card>
      ) : null}

      {addOpen ? (
        <Card>
          <CardHeader>
            <CardTitle>Malzeme ekle</CardTitle>
            <CardDescription>
              Material Master araması. Kart yoksa serbest metin stok yok —{' '}
				<Link to="/inventory/master-data/materials" className="underline">
                Material Master
              </Link>
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-2">
            <Input
              placeholder="Kod, ad, ağaç, cins, 50 100…"
              value={addQuery}
              onChange={(e) => setAddQuery(e.target.value)}
            />
            {materialHits.map((m) => (
              <button
                key={m.id || m.code}
                type="button"
                className="block w-full rounded-md border border-[var(--border)] px-3 py-2 text-left text-sm"
                onClick={() => addDraft(m as MaterialOpt)}
              >
                {m.code} · {m.name}
              </button>
            ))}
            {addQuery && materialHits.length === 0 ? (
              <p className="text-sm text-red-600">Malzeme kartı bulunamadı. Yeni kart sayım ekranında oluşturulmaz.</p>
            ) : null}
            <Button variant="secondary" onClick={() => setAddOpen(false)}>
              Kapat
            </Button>
          </CardContent>
        </Card>
      ) : null}

      {aiPreview ? (
        <Card>
          <CardHeader>
            <CardTitle>AI sayım önizlemesi</CardTitle>
            <CardDescription>
              {ocrEngineAvailable()
                ? 'Motor metni okudu. Onayınız olmadan stok yazılmaz.'
                : 'Sunucuda OCR/AI motoru yok. Fotoğraf dosyası otomatik okunmaz. Metni yapıştırın veya yazın; öneriler onayınızdan sonra tabloya eklenir. Sahte güven yüzdesi gösterilmez.'}
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-2">
            <textarea
              className="w-full min-h-28 rounded-md border border-[var(--border)] bg-transparent p-2 text-sm"
              placeholder={'Çam\n45x90x4000 - 120\n45x90x3000 - 80'}
              value={aiText}
              onChange={(e) => setAiText(e.target.value)}
            />
            <div className="flex gap-2">
              <Button
                variant="secondary"
                onClick={() => setAiPreview(parseCountListText(aiText))}
              >
                Metni çözümle
              </Button>
              <label className="inline-flex items-center text-sm">
                <input
                  type="file"
                  accept="image/*,.pdf,.txt"
                  className="hidden"
                  onChange={async (e) => {
                    const f = e.target.files?.[0];
                    if (!f) return;
                    if (f.type.startsWith('text') || f.name.endsWith('.txt')) {
                      setAiText(await f.text());
                    } else {
                      setSavedNote('Görüntü/PDF için OCR motoru yok — listeyi yazın. Dosya yalnızca kanıt olarak yüklenebilir (files API).');
                    }
                    e.target.value = '';
                  }}
                />
                <span className="cursor-pointer underline">Dosya seç</span>
              </label>
            </div>
            {(aiPreview ?? []).map((s, i) => {
              const ranked = rankMaterialMatches(s.rawMaterial, materials, formatMm(s.thicknessMm, s.widthMm, s.lengthMm), 3);
              const top = ranked[0];
              return (
                <div key={i} className="rounded-md border border-[var(--border)] p-2 text-sm space-y-1">
                  <div>
                    Malzeme: {s.rawMaterial}
                    {s.suggestionKind === 'package-estimate' ? ' · AI ÖNERİSİ (paket tahmini, stok değil)' : ''}
                  </div>
                  <div>
                    Önerilen eşleşme:{' '}
                    {top && top.status !== 'NO_MATCH' ? `${top.material.code} (${top.status})` : 'yok — Material Master’dan seçin'}
                  </div>
                  <div>
                    Ölçü: {formatMm(s.thicknessMm, s.widthMm, s.lengthMm)} · Adet: {s.pieceCount ?? '—'}
                  </div>
                  {top && top.status !== 'NO_MATCH' ? (
                    <div className="flex gap-2">
                      <Button
                        onClick={() => {
                          applyAiRow(s, top.material as MaterialOpt);
                          setAiPreview((prev) => (prev ?? []).filter((_, j) => j !== i));
                        }}
                      >
                        Onayla
                      </Button>
                    </div>
                  ) : (
                    <p className="text-xs text-red-600">Malzeme kartı bulunamadı</p>
                  )}
                  {ranked.length > 1 ? (
                    <div className="flex flex-wrap gap-1">
                      {ranked.slice(1).map((r) => (
                        <button
                          key={r.material.code}
                          type="button"
                          className="text-xs underline"
                          onClick={() => {
                            applyAiRow(s, r.material as MaterialOpt);
                            setAiPreview((prev) => (prev ?? []).filter((_, j) => j !== i));
                          }}
                        >
                          Düzelt: {r.material.code}
                        </button>
                      ))}
                    </div>
                  ) : null}
                </div>
              );
            })}
            <Button variant="secondary" onClick={() => setAiPreview(null)}>
              Kapat
            </Button>
          </CardContent>
        </Card>
      ) : null}

      {error ? <p className="text-sm text-red-600">{error}</p> : null}
      {savedNote ? <p className="text-sm text-[var(--text-muted)]">{savedNote}</p> : null}
      {warehousesQuery.isError ? (
        <p className="text-sm text-red-600">{(warehousesQuery.error as Error).message}</p>
      ) : null}
      {sessionQuery.isError ? (
        <p className="text-sm text-red-600">{(sessionQuery.error as Error).message}</p>
      ) : null}
    </div>
  );
}
