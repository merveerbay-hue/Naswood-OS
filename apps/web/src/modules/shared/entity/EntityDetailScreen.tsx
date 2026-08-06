import { useQuery } from '@tanstack/react-query';
import { Link } from '@tanstack/react-router';
import { Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { getResource } from '@/api/business';
import { useI18n } from '@/i18n';
import { StatusBadge } from '@/modules/shared/entity/StatusBadge';

interface EntityDetailScreenProps {
  screenId: string;
  title: string;
  route: string;
  id: string;
  listPath: string;
  fields: { key: string; label: string; status?: boolean }[];
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

export function EntityDetailScreen({ screenId, title, route, id, listPath, fields }: EntityDetailScreenProps) {
  const { t } = useI18n();
  const detailQuery = useQuery({
    queryKey: ['business', route, id],
    queryFn: () => getResource<Record<string, unknown>>(route, id),
  });

  const row = detailQuery.data;
  const status = row ? String(readField(row, 'Status') ?? readField(row, 'status') ?? '') : '';

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">{screenId}</p>
          <h2 className="text-xl font-semibold tracking-tight">{title}</h2>
          <p className="mt-1 text-sm text-[var(--text-secondary)]">
            {t('entity.masterDetail')} {id}
          </p>
        </div>
        <Link to={listPath} className="text-sm text-[var(--color-primary)] hover:underline">
          {t('backToList')}
        </Link>
      </div>

      <Card>
        <CardHeader className="flex flex-row items-center justify-between gap-3">
          <CardTitle>{t('entity.header')}</CardTitle>
          <StatusBadge status={status || null} />
        </CardHeader>
        <CardContent>
          {detailQuery.isLoading ? (
            <p className="text-sm text-[var(--text-secondary)]">{t('loading')}</p>
          ) : detailQuery.isError ? (
            <p className="text-sm text-[var(--color-danger)]">{(detailQuery.error as Error).message}</p>
          ) : (
            <dl className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
              {fields.map((field) => (
                <div key={field.key}>
                  <dt className="text-xs text-[var(--text-muted)]">{field.label}</dt>
                  <dd className="mt-0.5 text-sm font-medium">
                    {field.status ? (
                      <StatusBadge status={String(readField(row ?? {}, field.key) ?? '')} />
                    ) : (
                      String(readField(row ?? {}, field.key) ?? '—')
                    )}
                  </dd>
                </div>
              ))}
            </dl>
          )}
        </CardContent>
      </Card>

      <div className="grid gap-4 lg:grid-cols-3">
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle>{t('entity.linesRelated')}</CardTitle>
          </CardHeader>
          <CardContent className="text-sm text-[var(--text-secondary)]">{t('entity.linesHint')}</CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>{t('entity.auditTimeline')}</CardTitle>
          </CardHeader>
          <CardContent className="text-sm text-[var(--text-secondary)]">{t('entity.auditHint')}</CardContent>
        </Card>
      </div>
    </div>
  );
}
