import { useMemo } from 'react';
import { useAuth } from '@/auth/useAuth';
import { canSwitchPlant, visiblePlantIds } from '@/auth/plantVisibility.selftest';

/** Session working plant + visibility helpers for inventory screens. */
export function usePlantContext() {
  const { user, switchPlant } = useAuth();

  return useMemo(() => {
    const homePlantId = user?.homePlantId || user?.plantId || '';
    const workingPlantId = user?.plantId || homePlantId;
    const roles = user?.roles ?? [];
    const switchable =
      user?.canSwitchPlant === true || canSwitchPlant(roles);
    const plants = visiblePlantIds(roles, homePlantId, user?.plantIds ?? []);

    return {
      homePlantId,
      workingPlantId,
      /** Prefer working plant for data queries; falls back to home. */
      plantId: workingPlantId || homePlantId || 'PLANT-001',
      visiblePlantIds: plants,
      canSwitchPlant: switchable && plants.length > 1,
      isHomeContext:
        !!homePlantId &&
        !!workingPlantId &&
        homePlantId.toUpperCase() === workingPlantId.toUpperCase(),
      switchPlant,
    };
  }, [user, switchPlant]);
}
