import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { createResource, deleteResource, searchAllResource, searchResource } from '@/api/business';
import { usePlantContext } from '@/auth/usePlantContext';
import { useI18n } from '@/i18n';
import { StatusBadge } from '@/modules/shared/entity/StatusBadge';
import {
  LOCATION_TYPE_OPTIONS,
  locationTypeLabel,
  plantDisplayName,
  type LocationTypeCode,
} from '@/modules/inventory/locations/locationCatalog';

type WarehouseRow = {
  id?: string;
  code?: string;
  name?: string;
  plantId?: string;
  status?: string;
};

type LocationRow = {
  id?: string;
  code?: string;
  name?: string;
  warehouseCode?: string;
  locationType?: string;
  status?: string;
  description?: string;
  plantId?: string;
};

type FormState = {
  warehouseCode: string;
  code: string;
  name: string;
  locationType: LocationTypeCode;
  description: string;
  status: 'Active' | 'Inactive';
};

const DEFAULT_FORM: FormState = {
  warehouseCode: '',
  code: '',
  name: '',
  locationType: 'OPEN_AREA',
  description: '',
  status: 'Active',
};

/**
 * INV-008 — Lokasyonlar
 * Hierarchy: Factory (Ana Üs / Diğer Tesis) → Warehouse → Location → Stock
 * HomeFactory = user.homePlantId; view plant can switch without changing Ana Üs.
 */
export function LocationListPage() {
  const { t } = useI18n();
  const { homePlantId, plantId: workingPlantId, visiblePlantIds, canSwitchPlant } = usePlantContext();
  const queryClient = useQueryClient();

  const plantIds = visiblePlantIds.length ? visiblePlantIds : [homePlantId || 'PLANT-001'];
  const otherPlants = canSwitchPlant
    ? plantIds.filter((p) => p.toUpperCase() !== (homePlantId || '').toUpperCase())
    : [];

  const [viewPlantId, setViewPlantId] = useState(workingPlantId || homePlantId);
  const [showCreate, setShowCreate] = useState(false);
  const [form, setForm] = useState<FormState>(DEFAULT_FORM);
  const [q, setQ] = useState('');
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setViewPlantId(workingPlantId || homePlantId);
  }, [workingPlantId, homePlantId]);

  useEffect(() => {
    setViewPlantId((prev) => {
      if (plantIds.some((p) => p.toUpperCase() === prev.toUpperCase())) return prev;
      return homePlantId;
    });
  }, [homePlantId, plantIds]);

  // Keep view in sync when auth loads
  const effectiveView = useMemo(() => {
    if (plantIds.some((p) => p.toUpperCase() === viewPlantId.toUpperCase())) return viewPlantId;
    return homePlantId;
  }, [viewPlantId, homePlantId, plantIds]);

  const isHomeView = effectiveView.toUpperCase() === homePlantId.toUpperCase();
  const plantColumnLabel = isHomeView ? 'Ana Üs' : 'Tesis';

  const warehousesQuery = useQuery({
    queryKey: ['business', 'warehouses', 'by-plant', effectiveView],
    queryFn: () => searchAllResource<WarehouseRow>('warehouses', undefined, { plantId: effectiveView }),
  });

  const activeWarehouses = useMemo(
    () =>
      (warehousesQuery.data ?? []).filter(
        (w) => String(w.status ?? 'Active').toLowerCase() === 'active',
      ),
    [warehousesQuery.data],
  );

  const listQuery = useQuery({
    queryKey: ['business', 'locations', effectiveView, q],
    queryFn: () =>
      searchResource<LocationRow>('locations', q || undefined, {
        pageSize: 100,
        plantId: effectiveView,
      }),
  });

  const createMutation = useMutation({
    mutationFn: async () => {
      if (!form.warehouseCode.trim()) throw new Error('Depo seçimi zorunludur.');
      if (!form.code.trim()) throw new Error('Lokasyon kodu zorunludur.');
      const code = form.code.trim().toUpperCase();
      const name = form.name.trim() || code;
      return createResource<LocationRow>('locations', {
        code,
        name,
        warehouseCode: form.warehouseCode.trim().toUpperCase(),
        locationType: form.locationType,
        description: form.description.trim(),
        status: form.status,
        plantId: effectiveView,
      });
    },
    onSuccess: async () => {
      setError(null);
      setShowCreate(false);
      setForm(DEFAULT_FORM);
      await queryClient.invalidateQueries({ queryKey: ['business', 'locations'] });
    },
    onError: (e: Error) => setError(e.message),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteResource('locations', id),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['business', 'locations'] });
    },
  });

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-008</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('inventory.locationsTitle')}</h2>
          <p className="mt-1 text-sm text-[var(--text-secondary)]">
            Fabrika → Depo → Lokasyon. Stoklar Ana Üs kapsamında tutulur. Staging/WIP stok
            lokasyonudur; İş Merkezi (Work Center) değildir.
          </p>
        </div>
        <Button type="button" onClick={() => setShowCreate((v) => !v)}>
          {showCreate ? t('close') : t('inventory.newLocation')}
        </Button>
      </div>

      <div className="grid gap-3 md:grid-cols-2">
        <Card>
          <CardHeader className="pb-2">
            <CardDescription>Ana Fabrika</CardDescription>
            <CardTitle className="text-base">
              {plantDisplayName(homePlantId)}{' '}
              <span className="text-sm font-normal text-[var(--text-muted)]">({homePlantId})</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            {canSwitchPlant ? (
              <Button
                type="button"
                variant={isHomeView ? 'default' : 'secondary'}
                size="sm"
                onClick={() => setViewPlantId(homePlantId)}
              >
                Ana Üs görünümü
              </Button>
            ) : (
              <p className="text-sm text-[var(--text-muted)]">Yalnızca kendi Ana Fabrikanız.</p>
            )}
          </CardContent>
        </Card>

        {canSwitchPlant ? (
          <Card>
            <CardHeader className="pb-2">
              <CardDescription>Çalışma Tesisi</CardDescription>
              <CardTitle className="text-base">
                {otherPlants.length === 0
                  ? 'Yetkili başka tesis yok'
                  : 'Yetkili aktif tesisler (Ana Üs değişmez)'}
              </CardTitle>
            </CardHeader>
            <CardContent className="flex flex-wrap gap-2">
              {otherPlants.map((pid) => (
                <Button
                  key={pid}
                  type="button"
                  variant={effectiveView.toUpperCase() === pid.toUpperCase() ? 'default' : 'secondary'}
                  size="sm"
                  onClick={() => setViewPlantId(pid)}
                >
                  {plantDisplayName(pid)} ({pid})
                </Button>
              ))}
            </CardContent>
          </Card>
        ) : null}
      </div>

      {canSwitchPlant && !isHomeView ? (
        <p className="rounded-md border border-[var(--border)] bg-[var(--surface-muted)] px-3 py-2 text-sm text-[var(--text-secondary)]">
          Çalışma tesisi: <strong>{plantDisplayName(effectiveView)}</strong>. Ana Fabrika (
          {plantDisplayName(homePlantId)}) değişmez.
        </p>
      ) : null}

      {showCreate ? (
        <Card>
          <CardHeader>
            <CardTitle>Lokasyon Ekle</CardTitle>
            <CardDescription>
              Ana Üs / aktif tesis otomatik; depo yalnızca bu tesise aittir.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            <label className="block space-y-1 text-sm">
              <span className="text-[var(--text-secondary)]">{isHomeView ? 'Ana Üs' : 'Tesis'}</span>
              <Input value={`${plantDisplayName(effectiveView)} (${effectiveView})`} readOnly disabled />
            </label>

            <div className="grid gap-3 md:grid-cols-2">
              <label className="space-y-1 text-sm">
                <span className="text-[var(--text-secondary)]">Depo</span>
                <select
                  className="flex h-10 w-full rounded-md border border-[var(--border)] bg-transparent px-3 text-sm"
                  value={form.warehouseCode}
                  onChange={(e) => setForm((f) => ({ ...f, warehouseCode: e.target.value }))}
                >
                  <option value="">Seçin…</option>
                  {activeWarehouses.map((w) => (
                    <option key={String(w.code)} value={String(w.code)}>
                      {String(w.code)} — {String(w.name ?? w.code)}
                    </option>
                  ))}
                </select>
              </label>

              <label className="space-y-1 text-sm">
                <span className="text-[var(--text-secondary)]">Lokasyon Tipi</span>
                <select
                  className="flex h-10 w-full rounded-md border border-[var(--border)] bg-transparent px-3 text-sm"
                  value={form.locationType}
                  onChange={(e) =>
                    setForm((f) => ({ ...f, locationType: e.target.value as LocationTypeCode }))
                  }
                >
                  {LOCATION_TYPE_OPTIONS.map((o) => (
                    <option key={o.token} value={o.token}>
                      {o.label}
                    </option>
                  ))}
                </select>
              </label>

              <label className="space-y-1 text-sm">
                <span className="text-[var(--text-secondary)]">Lokasyon Kodu</span>
                <Input
                  placeholder="A-03"
                  value={form.code}
                  onChange={(e) => {
                    const code = e.target.value;
                    setForm((f) => ({
                      ...f,
                      code,
                      name: f.name === f.code || !f.name ? code : f.name,
                    }));
                  }}
                />
              </label>

              <label className="space-y-1 text-sm">
                <span className="text-[var(--text-secondary)]">Lokasyon Adı</span>
                <Input
                  placeholder="Kurutma Fırını Önü"
                  value={form.name}
                  onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
                />
              </label>

              <label className="space-y-1 text-sm md:col-span-2">
                <span className="text-[var(--text-secondary)]">Açıklama</span>
                <Input
                  value={form.description}
                  onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
                />
              </label>

              <label className="space-y-1 text-sm">
                <span className="text-[var(--text-secondary)]">Durum</span>
                <select
                  className="flex h-10 w-full rounded-md border border-[var(--border)] bg-transparent px-3 text-sm"
                  value={form.status}
                  onChange={(e) =>
                    setForm((f) => ({ ...f, status: e.target.value as 'Active' | 'Inactive' }))
                  }
                >
                  <option value="Active">Aktif</option>
                  <option value="Inactive">Pasif</option>
                </select>
              </label>
            </div>

            <Button type="button" onClick={() => createMutation.mutate()} disabled={createMutation.isPending}>
              {createMutation.isPending ? t('saving') : t('save')}
            </Button>
            {error ? <p className="text-sm text-[var(--color-danger)]">{error}</p> : null}
          </CardContent>
        </Card>
      ) : null}

      <Card>
        <CardHeader className="gap-3 sm:flex-row sm:items-center sm:justify-between">
          <CardTitle>
            {t('entity.library')} — {plantDisplayName(effectiveView)}
          </CardTitle>
          <form
            className="flex gap-2"
            onSubmit={(e) => {
              e.preventDefault();
              void listQuery.refetch();
            }}
          >
            <Input
              value={q}
              onChange={(e) => setQ(e.target.value)}
              placeholder="Kod, ad, depo, tip…"
              className="max-w-sm"
            />
            <Button type="submit" variant="secondary">
              {t('search')}
            </Button>
          </form>
        </CardHeader>
        <CardContent>
          {listQuery.isLoading ? (
            <p className="text-sm text-[var(--text-secondary)]">{t('loading')}</p>
          ) : (listQuery.data?.items?.length ?? 0) === 0 ? (
            <p className="text-sm text-[var(--text-secondary)]">Bu tesiste lokasyon yok.</p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full min-w-[640px] text-left text-sm">
                <thead className="border-b border-[var(--border)] text-[var(--text-secondary)]">
                  <tr>
                    <th className="py-2 pr-3 font-medium">{plantColumnLabel}</th>
                    <th className="py-2 pr-3 font-medium">Depo</th>
                    <th className="py-2 pr-3 font-medium">Kod</th>
                    <th className="py-2 pr-3 font-medium">Ad</th>
                    <th className="py-2 pr-3 font-medium">Tip</th>
                    <th className="py-2 pr-3 font-medium">Durum</th>
                    <th className="py-2 font-medium" />
                  </tr>
                </thead>
                <tbody>
                  {(listQuery.data?.items ?? []).map((row) => {
                    const id = String(row.id ?? '');
                    return (
                      <tr key={id} className="border-b border-[var(--border)]">
                        <td className="py-2 pr-3">{plantDisplayName(row.plantId ?? effectiveView)}</td>
                        <td className="py-2 pr-3">{String(row.warehouseCode ?? '—')}</td>
                        <td className="py-2 pr-3 font-medium">{String(row.code ?? '—')}</td>
                        <td className="py-2 pr-3">{String(row.name ?? '—')}</td>
                        <td className="py-2 pr-3">{locationTypeLabel(String(row.locationType ?? ''))}</td>
                        <td className="py-2 pr-3">
                          <StatusBadge status={String(row.status ?? 'Active')} />
                        </td>
                        <td className="py-2 text-right">
                          {id ? (
                            <Button
                              type="button"
                              variant="ghost"
                              size="sm"
                              onClick={() => deleteMutation.mutate(id)}
                              disabled={deleteMutation.isPending}
                            >
                              Pasifleştir
                            </Button>
                          ) : null}
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
