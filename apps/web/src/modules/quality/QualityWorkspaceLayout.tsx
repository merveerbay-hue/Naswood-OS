import { Outlet } from '@tanstack/react-router';
import { WorkspaceShell } from '@/components/workspace/WorkspaceShell';
import { qualityWorkspaces } from './quality-workspaces';

export function QualityWorkspaceLayout() {
  return (
    <WorkspaceShell moduleLabel="Kalite" moduleHomePath="/quality/dashboard" workspaces={qualityWorkspaces}>
      <Outlet />
    </WorkspaceShell>
  );
}
