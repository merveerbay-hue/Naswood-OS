import type { ReactNode } from 'react';

interface ProcessWorkspaceProps {
  screenId: string;
  title: string;
  description?: string;
  children: ReactNode;
}

/**
 * Lightweight page chrome for admin / process screens (screen id + title + body).
 * Not a multi-step wizard — see ProcessWizard for that.
 */
export function ProcessWorkspace({ screenId, title, description, children }: ProcessWorkspaceProps) {
  return (
    <div className="space-y-4">
      <div>
        <p className="text-xs font-medium text-[var(--text-muted)]">{screenId}</p>
        <h2 className="text-xl font-semibold tracking-tight">{title}</h2>
        {description ? (
          <p className="mt-1 text-sm text-[var(--text-secondary)]">{description}</p>
        ) : null}
      </div>
      {children}
    </div>
  );
}
