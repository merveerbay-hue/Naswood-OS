import { Link, useNavigate } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { createResource, searchAllResource } from '@/api/business';
import { useI18n } from '@/i18n';
import {
  WAREHOUSE_CATALOG,
  WAREHOUSE_TYPE_OPTIONS,
  findCatalogItem,
  type WarehouseCatalogItem,
  type WarehouseTypeCode,
} from '@/modules/inventory/warehouses/warehouseCatalog';

/**
 * INV-WH-001 — Warehouse define
 * Open warehouses on demand from catalog types. Material ≠ Warehouse binding.
 */

type FormState = {
  code: string;
  name: string;
  warehouseType: WarehouseTypeCode;
  description: string;
  status: 'Active' | 'Passive';
  plantId: string;
};

const DEFAULT: FormState = {
  code: 'WH-RM',
  name: 'Hammadde Deposu',
  warehouseType: 'RAW_MATERIAL',
  description: 'Kereste, panel, thermowood hammaddeleri',
  status: 'Active',
  plantId: 'PLANT-001',
};

export function WarehouseDefinePage() {
  const { t } = useI18n();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [form, setForm] = useState<FormState>(DEFAULT);
  const [error, setError] = useState<string | null>(null);
  const [createdCode, setCreatedCode] = useState<string | null>(null);

  const existingQuery = useQuery({
    queryKey: ['business', 'warehouses', 'all-for-define'],
    queryFn: () => searchAllResource<{ code?: string }>('warehouses'),
  });
  const existingCodes = useMemo(
    () => new Set((existingQuery.data ?? []).map((w) => String(w.code ?? '').toUpperCase())),
    [existingQuery.data],
  );

  const catalogOpen = useMemo(
    () => WAREHOUSE_CATALOG.map((c) => ({ ...c, alreadyOpen: existingCodes.has(c.warehouseCode.toUpperCase()) })),
    [existingCodes],
  );

  function applyCatalog(item: WarehouseCatalogItem) {
    setForm({
      code: item.warehouseCode,
      name: item.warehouseName,
      warehouseType: item.warehouseType,
      description: item.description,
      status: 'Active',
      plantId: form.plantId,
    });
    setError(null);
  }

  const saveMutation = useMutation({
    mutationFn: async () => {
      if (!form.name.trim()) throw new Error(t('wizard.wh.needName'));
      if (!form.code.trim()) throw new Error(t('wizard.wh.needCode'));
      if (existingCodes.has(form.code.trim().toUpperCase())) {
        throw new Error(t('wizard.wh.codeExists'));
      }
      return createResource<{ code?: string }>('warehouses', {
        code: form.code.trim().toUpperCase(),
        name: form.name.trim(),
        warehouseType: form.warehouseType,
        description: form.description.trim(),
        status: form.status,
        plantId: form.plantId || 'PLANT-001',
      });
    },
    onSuccess: async (created) => {
      setError(null);
      setCreatedCode(String(created.code ?? form.code));
      await queryClient.invalidateQueries({ queryKey: ['business', 'warehouses'] });
    },
    onError: (e: Error) => setError(e.message),
  });

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-WH-001</p>
          <h2 className="text-xl font-semibold tracking-tight">{t('wizard.warehouseTitle')}</h2>
          <p className="mt-1 max-w-2xl text-sm text-[var(--text-secondary)]">{t('wizard.warehouseDesc')}</p>
          <p className="mt-1 text-xs text-[var(--text-muted)]">{t('wizard.wh.noMaterialBind')}</p>
        </div>
        <Link
          to="/inventory/master-data/warehouses"
          className="text-sm font-medium text-[var(--color-primary)] hover:underline"
        >
          {t('wizard.backToLibrary')}
        </Link>
      </div>

      <div className="grid gap-4 lg:grid-cols-[1fr_320px]">
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-base">{t('wizard.wh.general')}</CardTitle>
            <CardDescription>{t('wizard.wh.formHint')}</CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            <label className="block space-y-1">
              <span className="text-xs font-medium text-[var(--text-muted)]">{t('wizard.wh.code')}</span>
              <Input
                value={form.code}
                onChange={(e) => setForm((f) => ({ ...f, code: e.target.value.toUpperCase() }))}
                className="font-mono"
                list="wh-catalog-codes"
              />
              <datalist id="wh-catalog-codes">
                {WAREHOUSE_CATALOG.map((c) => (
                  <option key={c.warehouseCode} value={c.warehouseCode} />
                ))}
              </datalist>
            </label>
            <label className="block space-y-1">
              <span className="text-xs font-medium text-[var(--text-muted)]">{t('wizard.wh.name')}</span>
              <Input
                value={form.name}
                onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
              />
            </label>
            <label className="block space-y-1">
              <span className="text-xs font-medium text-[var(--text-muted)]">{t('wizard.wh.type')}</span>
              <select
                value={form.warehouseType}
                onChange={(e) =>
                  setForm((f) => ({ ...f, warehouseType: e.target.value as WarehouseTypeCode }))
                }
                className="flex h-10 w-full rounded-md border border-[var(--border-default)] bg-[var(--color-surface)] px-3 text-sm"
              >
                <optgroup label={t('wizard.wh.groupProduction')}>
                  {WAREHOUSE_TYPE_OPTIONS.filter((o) => o.group === 'production').map((o) => (
                    <option key={o.token} value={o.token}>
                      {o.token} — {o.label}
                    </option>
                  ))}
                </optgroup>
                <optgroup label={t('wizard.wh.groupTechnical')}>
                  {WAREHOUSE_TYPE_OPTIONS.filter((o) => o.group === 'technical').map((o) => (
                    <option key={o.token} value={o.token}>
                      {o.token} — {o.label}
                    </option>
                  ))}
                </optgroup>
              </select>
            </label>
            <label className="block space-y-1">
              <span className="text-xs font-medium text-[var(--text-muted)]">{t('wizard.wh.description')}</span>
              <Input
                value={form.description}
                onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
              />
            </label>
            <label className="block space-y-1">
              <span className="text-xs font-medium text-[var(--text-muted)]">{t('wizard.wh.plant')}</span>
              <Input
                value={form.plantId}
                onChange={(e) => setForm((f) => ({ ...f, plantId: e.target.value }))}
              />
            </label>
            <label className="flex items-center gap-2 text-sm font-medium">
              <input
                type="checkbox"
                checked={form.status === 'Active'}
                onChange={(e) =>
                  setForm((f) => ({ ...f, status: e.target.checked ? 'Active' : 'Passive' }))
                }
              />
              {t('wizard.wh.active')}
            </label>

            {error ? <p className="text-sm text-[var(--color-danger)]">{error}</p> : null}
            {createdCode ? (
              <p className="text-sm font-medium text-[var(--color-primary)]">
                {t('wizard.wh.created')} · {createdCode}
              </p>
            ) : null}

            <div className="flex flex-wrap gap-2 border-t border-[var(--border-default)] pt-3">
              {!createdCode ? (
                <Button
                  type="button"
                  disabled={saveMutation.isPending}
                  onClick={() => {
                    const cat = findCatalogItem(form.code);
                    if (cat && form.warehouseType !== cat.warehouseType) {
                      setForm((f) => ({ ...f, warehouseType: cat.warehouseType }));
                    }
                    saveMutation.mutate();
                  }}
                >
                  {t('wizard.saveRelease')}
                </Button>
              ) : (
                <Button
                  type="button"
                  onClick={() => navigate({ to: '/inventory/master-data/warehouses' })}
                >
                  {t('wizard.backToLibrary')}
                </Button>
              )}
            </div>
          </CardContent>
        </Card>

        <aside className="space-y-3">
          <Card>
            <CardHeader className="pb-2">
              <CardTitle className="text-base">{t('wizard.wh.catalogTitle')}</CardTitle>
              <CardDescription>{t('wizard.wh.catalogHint')}</CardDescription>
            </CardHeader>
            <CardContent className="max-h-[28rem] space-y-2 overflow-y-auto">
              {catalogOpen.map((c) => (
                <button
                  key={c.warehouseCode}
                  type="button"
                  disabled={c.alreadyOpen || Boolean(createdCode)}
                  onClick={() => applyCatalog(c)}
                  className={`w-full rounded-md border px-3 py-2 text-left text-sm ${
                    c.alreadyOpen
                      ? 'opacity-50'
                      : form.code === c.warehouseCode
                        ? 'border-[var(--color-primary)] bg-[var(--color-primary)]/5'
                        : 'border-[var(--border-default)] hover:bg-[var(--color-surface-hover)]'
                  }`}
                >
                  <p className="font-mono text-xs font-medium">{c.warehouseCode}</p>
                  <p className="font-medium">{c.warehouseName}</p>
                  <p className="text-[10px] text-[var(--text-muted)]">
                    {c.warehouseType}
                    {c.alreadyOpen ? ` · ${t('wizard.wh.alreadyOpen')}` : ''}
                  </p>
                </button>
              ))}
            </CardContent>
          </Card>
        </aside>
      </div>
    </div>
  );
}
