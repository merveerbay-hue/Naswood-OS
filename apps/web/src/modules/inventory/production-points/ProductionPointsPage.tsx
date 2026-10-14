import { Link } from '@tanstack/react-router';
import { EntityListScreen } from '@/modules/shared/entity/EntityListScreen';
import { useI18n } from '@/i18n';

/**
 * INV-PP — Üretim noktaları (Work Center).
 * Spatial master after stock locations. Not a stock location. Not a parent of lots.
 */
export function ProductionPointsPage() {
  const { t } = useI18n();
  return (
    <div className="space-y-3">
      <EntityListScreen
        screenId="INV-PP"
        title={t('inventory.productionPointsTitle')}
        description={t('inventory.productionPointsDesc')}
        route="work-centers"
        fields={[
          { key: 'Code', label: t('inventory.fields.code') },
          { key: 'Name', label: t('inventory.fields.name') },
          { key: 'CapacityPerHour', label: t('inventory.fields.capacity'), type: 'number' },
          { key: 'Status', label: t('inventory.fields.status'), status: true },
        ]}
        createLabel={t('inventory.newProductionPoint')}
        jobPath="/production/engineering/work-center-designer"
      />
      <p className="text-sm text-[var(--text-secondary)]">
        {t('inventory.productionPointsHint')}{' '}
        <Link to="/production/master-data/work-centers" className="font-medium text-[var(--color-primary)] hover:underline">
          {t('inventory.productionPointsProdLink')}
        </Link>
      </p>
    </div>
  );
}
