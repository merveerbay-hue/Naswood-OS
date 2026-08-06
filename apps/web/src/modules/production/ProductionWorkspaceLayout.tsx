import { Outlet } from '@tanstack/react-router';
import { WorkspaceShell } from '@/components/workspace/WorkspaceShell';
import { productionWorkspaces } from './production-workspaces';

export function ProductionWorkspaceLayout() {
  return (
    <WorkspaceShell
      moduleLabel="Üretim"
      moduleHomePath="/production/dashboard"
      workspaces={productionWorkspaces}
    >
      <Outlet />
    </WorkspaceShell>
  );
}
