import { ResourcePage } from './ResourcePage';

export function ProductionConfirmationPage() {
  return (
    <ResourcePage
      title="ProductionConfirmation"
      description="TASK-059 · Production MVP"
      route="production-confirmations"
      kind="master"
      fields={[
        { key: 'Code', label: 'Code', type: 'string' as const },
        { key: 'Name', label: 'Name', type: 'string' as const },
        { key: 'Status', label: 'Status', type: 'string' as const },
        { key: 'Notes', label: 'Notes', type: 'string' as const },
      ]}
    />
  );
}
