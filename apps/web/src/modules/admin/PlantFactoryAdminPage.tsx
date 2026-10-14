import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { apiRequest } from '@/api/client';
import { ProcessWorkspace } from '@/modules/shared/ProcessWorkspace';

type OrgPlant = {
  code: string;
  name: string;
  companyCode: string;
  isActive: boolean;
};

type UserRow = {
  id: string;
  username: string;
  displayName: string;
  plantIds: string[];
  homePlantId?: string | null;
  roles?: string[];
};

/**
 * Sistem Yöneticisi — tesis tanımı + kullanıcı-fabrika / Ana Üs ataması.
 */
export function PlantFactoryAdminPage() {
  const queryClient = useQueryClient();
  const [code, setCode] = useState('');
  const [name, setName] = useState('');
  const [companyCode, setCompanyCode] = useState('COMP-001');
  const [userId, setUserId] = useState('');
  const [assignPlants, setAssignPlants] = useState('');
  const [homePlantId, setHomePlantId] = useState('');
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const plantsQuery = useQuery({
    queryKey: ['organization', 'plants'],
    queryFn: () => apiRequest<OrgPlant[]>('/api/v1/organization/plants', { method: 'GET', auth: true }),
  });

  const usersQuery = useQuery({
    queryKey: ['users', 'admin-plant'],
    queryFn: async () => {
      const page = await apiRequest<{ items: UserRow[]; Items?: UserRow[] }>('/api/v1/users?page=1&pageSize=50', {
        method: 'GET',
        auth: true,
      });
      return page.items ?? page.Items ?? [];
    },
  });

  const createPlant = useMutation({
    mutationFn: () =>
      apiRequest<OrgPlant>('/api/v1/organization/plants', {
        method: 'POST',
        auth: true,
        body: { code, name, companyCode },
      }),
    onSuccess: async () => {
      setMessage(`Tesis oluşturuldu: ${code}`);
      setError(null);
      setCode('');
      setName('');
      await queryClient.invalidateQueries({ queryKey: ['organization', 'plants'] });
    },
    onError: (e: Error) => {
      setError(e.message);
      setMessage(null);
    },
  });

  const assign = useMutation({
    mutationFn: () => {
      const plantIds = assignPlants
        .split(/[,;\s]+/)
        .map((p) => p.trim())
        .filter(Boolean);
      return apiRequest(`/api/v1/users/${userId}/assign-plant`, {
        method: 'POST',
        auth: true,
        body: { plantIds, homePlantId: homePlantId || plantIds[0] },
      });
    },
    onSuccess: async () => {
      setMessage('Kullanıcı tesis yetkisi güncellendi.');
      setError(null);
      await queryClient.invalidateQueries({ queryKey: ['users'] });
    },
    onError: (e: Error) => {
      setError(e.message);
      setMessage(null);
    },
  });

  return (
    <ProcessWorkspace
      screenId="ADM-PLANT"
      title="Tesis / Fabrika Yetkisi"
      description="Sistem Yöneticisi: tesis tanımı, kullanıcı-fabrika yetkisi ve Ana Üs ataması."
    >
      <div className="grid gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle className="text-base">Tesis tanımı</CardTitle>
            <CardDescription>Yeni fabrika / plant referansı ekleyin.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            <Input placeholder="Kod (ör. F03)" value={code} onChange={(e) => setCode(e.target.value)} />
            <Input placeholder="Ad" value={name} onChange={(e) => setName(e.target.value)} />
            <Input
              placeholder="Şirket kodu"
              value={companyCode}
              onChange={(e) => setCompanyCode(e.target.value)}
            />
            <Button
              type="button"
              disabled={!code.trim() || !name.trim() || createPlant.isPending}
              onClick={() => createPlant.mutate()}
            >
              Tesis oluştur
            </Button>
            <ul className="mt-4 space-y-1 text-sm text-[var(--text-secondary)]">
              {(plantsQuery.data ?? []).map((p) => (
                <li key={p.code}>
                  <span className="font-medium text-[var(--text-primary)]">{p.code}</span> — {p.name} (
                  {p.companyCode})
                </li>
              ))}
            </ul>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="text-base">Kullanıcı → fabrika ataması</CardTitle>
            <CardDescription>
              Operatör / Mühendis / Depo: yalnızca Ana Üs. Üst Yönetici: birden fazla tesis + Ana Üs.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            <select
              className="h-10 w-full rounded-md border border-[var(--border-default)] bg-transparent px-3 text-sm"
              value={userId}
              onChange={(e) => {
                setUserId(e.target.value);
                const u = (usersQuery.data ?? []).find((x) => x.id === e.target.value);
                if (u) {
                  setAssignPlants((u.plantIds ?? []).join(', '));
                  setHomePlantId(u.homePlantId || u.plantIds?.[0] || '');
                }
              }}
            >
              <option value="">Kullanıcı seçin</option>
              {(usersQuery.data ?? []).map((u) => (
                <option key={u.id} value={u.id}>
                  {u.displayName || u.username} ({u.roles?.[0] ?? '—'})
                </option>
              ))}
            </select>
            <Input
              placeholder="PlantIds (virgülle: F01, F02)"
              value={assignPlants}
              onChange={(e) => setAssignPlants(e.target.value)}
            />
            <Input
              placeholder="Ana Üs (HomePlantId)"
              value={homePlantId}
              onChange={(e) => setHomePlantId(e.target.value)}
            />
            <Button
              type="button"
              disabled={!userId || !assignPlants.trim() || assign.isPending}
              onClick={() => assign.mutate()}
            >
              Yetkiyi kaydet
            </Button>
          </CardContent>
        </Card>
      </div>

      {message ? <p className="mt-4 text-sm text-[var(--color-success)]">{message}</p> : null}
      {error ? <p className="mt-4 text-sm text-[var(--color-danger)]">{error}</p> : null}
    </ProcessWorkspace>
  );
}
