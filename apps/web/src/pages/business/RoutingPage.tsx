import { ResourcePage } from './ResourcePage';

export function RoutingPage() {
  return (
    <ResourcePage
      title="Routing"
      description="TASK-047 · Production MVP"
      route="routings"
      kind="document"
      fields={[
    { key: 'Number', label: 'Number', type: 'string' as const },
    { key: 'MaterialCode', label: 'MaterialCode', type: 'string' as const },
    { key: 'Version', label: 'Version', type: 'number' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'Notes', label: 'Notes', type: 'string' as const }
      ]}
    />
  );
}
