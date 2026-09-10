import { Link, useNavigate, useParams } from '@tanstack/react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Button, Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { getShopFloorQueue, startOperation } from './shopFloorApi';

export function ShopFloorWorkCenterPage() {
  const { id } = useParams({ strict: false }) as { id: string };
  const nav = useNavigate();
  const qc = useQueryClient();
  const q = useQuery({ queryKey: ['shop-floor-queue', id], queryFn: () => getShopFloorQueue(id), refetchInterval: 8000 });
  const start = useMutation({
    mutationFn: (operationId: string) => startOperation(operationId, id),
    onSuccess: (exec) => {
      qc.invalidateQueries({ queryKey: ['shop-floor-queue', id] });
      void nav({ to: '/production/shop-floor/executions/$id', params: { id: exec.id } });
    },
  });

  const data = q.data;
  return (
    <div className="mx-auto max-w-3xl space-y-4 p-4">
      <Link to="/production/shop-floor" className="text-sm underline">← İş merkezleri</Link>
      <h1 className="text-2xl font-semibold">{data?.workCenterName ?? 'İş merkezi'}</h1>
      <p className="text-muted-foreground">{data?.workCenterCode}</p>
      <Section title="Çalışan" items={data?.running ?? []} running />
      <Section
        title="Bekleyen"
        items={data?.pending ?? []}
        action={(item) => (
          <Button className="h-12 w-full" onClick={() => start.mutate(item.productionOperationId)} disabled={start.isPending}>
            İşi aç
          </Button>
        )}
      />
      <Section title="Bugün tamamlanan" items={data?.completedToday ?? []} />
    </div>
  );
}

function Section({
  title,
  items,
  running,
  action,
}: {
  title: string;
  items: { productionOrderNumber: string; productionOrderName: string; operationName: string; executionId?: string | null; productionOperationId: string; status: string }[];
  running?: boolean;
  action?: (item: { productionOperationId: string; executionId?: string | null }) => React.ReactNode;
}) {
  return (
    <section className="space-y-2">
      <h2 className="text-lg font-medium">{title}</h2>
      {items.length === 0 ? <p className="text-sm text-muted-foreground">Kayıt yok</p> : null}
      {items.map((item) => (
        <Card key={`${item.productionOperationId}-${item.executionId ?? ''}`}>
          <CardHeader className="pb-2">
            <CardTitle className="text-lg">{item.productionOrderNumber}</CardTitle>
            <p>{item.productionOrderName} · {item.operationName}</p>
          </CardHeader>
          <CardContent className="space-y-2">
            {running && item.executionId ? (
              <Link to="/production/shop-floor/executions/$id" params={{ id: item.executionId }}>
                <Button className="h-12 w-full">İşe dön</Button>
              </Link>
            ) : action ? action(item) : null}
          </CardContent>
        </Card>
      ))}
    </section>
  );
}
