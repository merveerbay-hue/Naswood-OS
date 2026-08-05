import { ResourcePage } from './ResourcePage';

export function ProductionLinePage() {
  return (
    <ResourcePage
      title="ProductionLine"
      description="TASK-050 · Production MVP"
      route="production-lines"
      kind="master"
      fields={[
    { key: 'Code', label: 'Code', type: 'string' as const },
    { key: 'Name', label: 'Name', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const },
    { key: 'Notes', label: 'Notes', type: 'string' as const }
      ]}
    />
  );
}
