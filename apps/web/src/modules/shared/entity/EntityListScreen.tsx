import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link } from '@tanstack/react-router';
import { useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { createResource, deleteResource, searchResource } from '@/api/business';
import { useI18n } from '@/i18n';
import { StatusBadge } from '@/modules/shared/entity/StatusBadge';

export interface EntityField {
  key: string;
  label: string;
  type?: 'string' | 'number' | 'date';
  status?: boolean;
}

interface EntityListScreenProps {
  screenId: string;
  title: string;
  description: string;
  route: string;
  fields: EntityField[];
  detailPath?: (id: string) => string;
  /** Job CTA label — never bare “Yeni”. */
  createLabel?: string;
  /**
   * When set, CTA navigates to a process Wizard (no shared inline Create form).
   * Authority: docs/13_Design/Common/Screen_Types.md § Create matrix.
   */
  jobPath?: string;
}

function toCamelKey(key: string): string {
  return key.length === 0 ? key : key.charAt(0).toLowerCase() + key.slice(1);
}

function readField(row: Record<string, unknown>, key: string): unknown {
  if (key in row) return row[key];
  const camel = toCamelKey(key);
  if (camel in row) return row[camel];
  return undefined;
}

export function EntityListScreen({
  screenId,
  title,
  description,
  route,
  fields,
  detailPath,
  createLabel,
  jobPath,
}: EntityListScreenProps) {
  const { t } = useI18n();
  const actionLabel = createLabel ?? t('new');
  const isJobCta = Boolean(jobPath);
  const queryClient = useQueryClient();
  const [q, setQ] = useState('');
  const [showCreate, setShowCreate] = useState(false);
  const [form, setForm] = useState<Record<string, string>>(() =>
    Object.fromEntries(
      fields.map((f) => [f.key, f.key === 'Status' ? (route.includes('goods') || route.includes('transfer') || route.includes('count') || route.includes('adjust') ? 'Draft' : 'Active') : '']),
    ),
  );
  const [error, setError] = useState<string | null>(null);

  const listQuery = useQuery({
    queryKey: ['business', route, q],
    queryFn: () => searchResource<Record<string, unknown>>(route, q || undefined),
  });

  const createMutation = useMutation({
    mutationFn: async () => {
      const body: Record<string, unknown> = {};
      for (const field of fields) {
        const raw = form[field.key] ?? '';
        const key = toCamelKey(field.key);
        if (field.type === 'number') body[key] = Number(raw || 0);
        else if (field.type === 'date') body[key] = raw || null;
        else body[key] = raw;
      }
      return createResource(route, body);
    },
    onSuccess: async () => {
      setError(null);
      setShowCreate(false);
      await queryClient.invalidateQueries({ queryKey: ['business', route] });
    },
    onError: (e: Error) => setError(e.message),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteResource(route, id),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['business', route] });
    },
  });

  const columns = useMemo(() => fields.slice(0, 6), [fields]);

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">{screenId}</p>
          <h2 className="text-xl font-semibold tracking-tight">{title}</h2>
          <p className="mt-1 text-sm text-[var(--text-secondary)]">{description}</p>
        </div>
        {isJobCta && jobPath ? (
          <Link to={jobPath}>
            <Button type="button">{actionLabel}</Button>
          </Link>
        ) : (
          <Button type="button" onClick={() => setShowCreate((v) => !v)}>
            {showCreate ? t('close') : actionLabel}
          </Button>
        )}
      </div>

      {!isJobCta && showCreate ? (
        <Card>
          <CardHeader>
            <CardTitle>{actionLabel}</CardTitle>
            <CardDescription>{t('entity.createHint')}</CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            <div className="grid gap-3 md:grid-cols-3">
              {fields.map((field) => (
                <label key={field.key} className="space-y-1 text-sm">
                  <span className="text-[var(--text-secondary)]">{field.label}</span>
                  <Input
                    value={form[field.key] ?? ''}
                    type={field.type === 'number' ? 'number' : field.type === 'date' ? 'date' : 'text'}
                    onChange={(e) => setForm((prev) => ({ ...prev, [field.key]: e.target.value }))}
                  />
                </label>
              ))}
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
          <CardTitle>{t('entity.library')}</CardTitle>
          <form
            className="flex gap-2"
            onSubmit={(e) => {
              e.preventDefault();
              void listQuery.refetch();
            }}
          >
            <Input value={q} onChange={(e) => setQ(e.target.value)} placeholder={t('search')} className="max-w-sm" />
            <Button type="submit" variant="secondary">
              {t('search')}
            </Button>
          </form>
        </CardHeader>
        <CardContent>
          {listQuery.isLoading ? (
            <p className="text-sm text-[var(--text-secondary)]">{t('loading')}</p>
          ) : (listQuery.data?.items?.length ?? 0) === 0 ? (
            <p className="text-sm text-[var(--text-secondary)]">
              {t('entity.noRecords').replace('{action}', actionLabel)}
            </p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full min-w-[720px] text-left text-sm">
                <thead>
                  <tr className="border-b border-[var(--border-default)] text-[var(--text-muted)]">
                    {columns.map((c) => (
                      <th key={c.key} className="px-2 py-2 font-medium">
                        {c.label}
                      </th>
                    ))}
                    <th className="px-2 py-2 font-medium">İşlemler</th>
                  </tr>
                </thead>
                <tbody>
                  {(listQuery.data?.items ?? []).map((row) => {
                    const id = String(readField(row, 'id') ?? '');
                    return (
                      <tr key={id} className="border-b border-[var(--border-default)] hover:bg-[var(--color-surface-hover)]/60">
                        {columns.map((c) => {
                          const value = readField(row, c.key);
                          return (
                            <td key={c.key} className="px-2 py-2">
                              {c.status || c.key.toLowerCase() === 'status' ? (
                                <StatusBadge status={value == null ? null : String(value)} />
                              ) : (
                                String(value ?? '—')
                              )}
                            </td>
                          );
                        })}
                        <td className="px-2 py-2">
                          <div className="flex flex-wrap gap-2">
                            {detailPath && id ? (
                              <Link to={detailPath(id)} className="text-sm font-medium text-[var(--color-primary)] hover:underline">
                                {t('open')}
                              </Link>
                            ) : null}
                            <Button type="button" size="sm" variant="danger" onClick={() => deleteMutation.mutate(id)}>
                              {t('delete')}
                            </Button>
                          </div>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          )}
          <p className="mt-3 text-xs text-[var(--text-muted)]">
            {listQuery.data?.totalCount ?? 0} {t('entity.records')} · {t('entity.entityGridHint')}
          </p>
        </CardContent>
      </Card>
    </div>
  );
}
