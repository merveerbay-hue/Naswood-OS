import { Outlet } from '@tanstack/react-router';
import { WorkspaceShell } from '@/components/workspace/WorkspaceShell';
import { inventoryWorkspaces } from './inventory-workspaces';

export function InventoryWorkspaceLayout() {
  return (
    <WorkspaceShell
      moduleLabel="Envanter"
      moduleHomePath="/inventory/dashboard"
      workspaces={inventoryWorkspaces}
    >
      <Outlet />
    </WorkspaceShell>
  );
}
