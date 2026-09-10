import { Link } from '@tanstack/react-router';
import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@naswood/ui';
import { listShopFloorWorkCenters } from './shopFloorApi';

export function ShopFloorHomePage() {
  const q = useQuery({ queryKey: ['shop-floor-wcs'], queryFn: listShopFloorWorkCenters });
  return (
    <div className="mx-auto max-w-3xl space-y-4 p-4">
      <h1 className="text-2xl font-semibold">İş Merkezlerim</h1>
      <div className="grid gap-3">
        {(q.data ?? []).map((wc) => (
          <Link key={wc.id} to="/production/shop-floor/work-centers/$id" params={{ id: wc.id }}>
            <Card className="min-h-24 active:scale-[0.99]">
              <CardHeader className="pb-2">
                <CardTitle className="text-xl">{wc.name}</CardTitle>
                <p className="text-sm text-muted-foreground">{wc.code}</p>
              </CardHeader>
              <CardContent className="flex gap-4 text-sm">
                <span>{wc.pendingCount} Bekleyen</span>
                <span>{wc.runningCount} Çalışıyor</span>
                <span>{wc.completedTodayCount} Bugün</span>
              </CardContent>
            </Card>
          </Link>
        ))}
        {q.data?.length === 0 ? <p>Bu tesiste iş merkezi yok.</p> : null}
      </div>
    </div>
  );
}
