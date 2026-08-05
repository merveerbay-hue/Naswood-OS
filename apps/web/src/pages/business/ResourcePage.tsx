import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle, Input } from '@naswood/ui';
import { createResource, deleteResource, getDashboard, searchResource } from '@/api/business';

export interface ResourceField {
  key: string;
  label: string;
  type?: 'string' | 'number' | 'date';
}

interface ResourcePageProps {
  title: string;
  description: string;
  route: string;
  fields: ResourceField[];
  kind?: 'master' | 'document' | 'dashboard' | 'report';
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

export function ResourcePage({ title, description, route, fields, kind = 'master' }: ResourcePageProps) {
  const queryClient = useQueryClient();
  const [q, setQ] = useState('');
  const [form, setForm] = useState<Record<string, string>>(() =>
    Object.fromEntries(fields.map((f) => [f.key, f.key === 'Status' ? 'Active' : ''])),
  );
  const [error, setError] = useState<string | null>(null);

  const listQuery = useQuery({
    queryKey: ['business', route, q],
    queryFn: () =>
      kind === 'dashboard'
        ? getDashboard<Record<string, number>>(route).then((data) => ({
            items: [data],
            page: 1,
            pageSize: 1,
            totalCount: 1,
            totalPages: 1,
          }))
        : searchResource<Record<string, unknown>>(route, q || undefined),
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

  const columns = useMemo(() => fields.slice(0, 5), [fields]);

  if (kind === 'dashboard') {
    const data = listQuery.data?.items?.[0] as Record<string, number> | undefined;
    return (
      <section className="space-y-6">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">{title}</h1>
          <p className="mt-1 text-[var(--text-secondary)]">{description}</p>
        </div>
        <div className="grid gap-4 md:grid-cols-4">
          {fields.map((field) => (
            <Card key={field.key}>
              <CardHeader>
                <CardTitle className="text-base">{field.label}</CardTitle>
              </CardHeader>
              <CardContent className="text-2xl font-semibold">{data?.[field.key] ?? 0}</CardContent>
            </Card>
          ))}
        </div>
      </section>
    );
  }

  return (
    <section className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">{title}</h1>
        <p className="mt-1 text-[var(--text-secondary)]">{description}</p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Create</CardTitle>
          <CardDescription>Quick create for Sprint MVP.</CardDescription>
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
            {createMutation.isPending ? 'Saving…' : 'Create'}
          </Button>
          {error ? <p className="text-sm text-[var(--color-danger)]">{error}</p> : null}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Library</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <form
            className="flex gap-2"
            onSubmit={(e) => {
              e.preventDefault();
              void listQuery.refetch();
            }}
          >
            <Input value={q} onChange={(e) => setQ(e.target.value)} placeholder="Search" className="max-w-sm" />
            <Button type="submit" variant="secondary">Search</Button>
          </form>
          {listQuery.isLoading ? (
            <p className="text-sm text-[var(--text-secondary)]">Loading…</p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full min-w-[640px] text-left text-sm">
                <thead>
                  <tr className="border-b border-[var(--border-default)] text-[var(--text-muted)]">
                    {columns.map((c) => (
                      <th key={c.key} className="px-2 py-2 font-medium">{c.label}</th>
                    ))}
                    <th className="px-2 py-2 font-medium">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {(listQuery.data?.items ?? []).map((row) => (
                    <tr key={String(readField(row, 'id'))} className="border-b border-[var(--border-default)]">
                      {columns.map((c) => (
                        <td key={c.key} className="px-2 py-2">{String(readField(row, c.key) ?? '—')}</td>
                      ))}
                      <td className="px-2 py-2">
                        <Button
                          type="button"
                          size="sm"
                          variant="danger"
                          onClick={() => deleteMutation.mutate(String(readField(row, 'id') ?? ''))}
                        >
                          Delete
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </CardContent>
      </Card>
    </section>
  );
}
