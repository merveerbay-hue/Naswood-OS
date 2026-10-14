import { ResourcePage } from './ResourcePage';

export function SupplierPage() {
  return (
    <ResourcePage
      title="Supplier"
      description="TASK-026 · Purchasing MVP"
      route="suppliers"
      kind="master"
      fields={[
    { key: 'Code', label: 'Code', type: 'string' as const },
    { key: 'Name', label: 'Name', type: 'string' as const },
    { key: 'TaxNumber', label: 'TaxNumber', type: 'string' as const },
    { key: 'Email', label: 'Email', type: 'string' as const },
    { key: 'Phone', label: 'Phone', type: 'string' as const },
    { key: 'Status', label: 'Status', type: 'string' as const }
      ]}
    />
  );
}
