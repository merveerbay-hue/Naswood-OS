import { Link } from '@tanstack/react-router';
import { useQuery } from '@tanstack/react-query';
import { Input, buttonVariants } from '@naswood/ui';
import { searchResource } from '@/api/business';
import { usePlantContext } from '@/auth/usePlantContext';
import { plantDisplayName } from '@/modules/inventory/locations/locationCatalog';
import { useState } from 'react';

type LotRow = {
  id?: string;
  productionLotNumber?: string;
  materialCode?: string;
  actualGradingMethod?: string;
  productionDate?: string;
  status?: string;
  workCenterCode?: string;
  plantId?: string;
};

export function StructuralProductionLotListPage() {
  const { plantId } = usePlantContext();
  const [q, setQ] = useState('');
  const list = useQuery({
    queryKey: ['business', 'structural-production-lots', plantId, q],
    queryFn: () =>
      searchResource<LotRow>('structural-production-lots', q.trim() || undefined, {
        page: 1,
        pageSize: 50,
        plantId,
      }),
    enabled: Boolean(plantId),
  });

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-end justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">INV-SPL-001</p>
          <h2 className="text-xl font-semibold tracking-tight">Yapısal Üretim Lotları</h2>
          <p className="mt-1 text-sm text-[var(--text-secondary)]">
            Tesis: {plantDisplayName(plantId)} — yalnızca bu fabrikanın kayıtları
          </p>
        </div>
        <Link
          to="/inventory/operations/structural-production-lots/new"
          className={buttonVariants({ variant: 'default' })}
        >
          Yeni üretim lotu
        </Link>
      </div>

      <Input
        placeholder="Numara / malzeme / durum ara…"
        value={q}
        onChange={(e) => setQ(e.target.value)}
      />

      <div className="overflow-x-auto">
        <table className="w-full min-w-[640px] border-collapse text-left text-sm">
          <thead>
            <tr className="border-b border-[var(--border)] text-[var(--text-muted)]">
              <th className="px-2 py-2 font-medium">Numara</th>
              <th className="px-2 py-2 font-medium">Malzeme</th>
              <th className="px-2 py-2 font-medium">Yöntem</th>
              <th className="px-2 py-2 font-medium">Tarih</th>
              <th className="px-2 py-2 font-medium">Durum</th>
              <th className="px-2 py-2 font-medium">WC</th>
            </tr>
          </thead>
          <tbody>
            {(list.data?.items ?? []).map((row) => (
              <tr key={String(row.id)} className="border-b border-[var(--border)]">
                <td className="px-2 py-2 font-medium">{row.productionLotNumber}</td>
                <td className="px-2 py-2">{row.materialCode}</td>
                <td className="px-2 py-2">{row.actualGradingMethod}</td>
                <td className="px-2 py-2">{row.productionDate}</td>
                <td className="px-2 py-2">{row.status}</td>
                <td className="px-2 py-2">{row.workCenterCode || '—'}</td>
              </tr>
            ))}
            {!list.isLoading && (list.data?.items?.length ?? 0) === 0 ? (
              <tr>
                <td colSpan={6} className="px-2 py-6 text-[var(--text-secondary)]">
                  Bu tesiste yapısal üretim lotu yok.
                </td>
              </tr>
            ) : null}
          </tbody>
        </table>
      </div>
    </div>
  );
}
