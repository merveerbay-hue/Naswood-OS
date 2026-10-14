import { ResourcePage } from './ResourcePage';

export function PurchasingReportPage() {
  return (
    <ResourcePage
      title="Purchasing Reports"
      description="TASK-035 · Purchasing MVP"
      route="purchasing/reports"
      kind="report"
      fields={[
    { key: 'ReportCode', label: 'ReportCode', type: 'string' as const },
    { key: 'Name', label: 'Name', type: 'string' as const },
    { key: 'Category', label: 'Category', type: 'string' as const },
    { key: 'Description', label: 'Description', type: 'string' as const }
      ]}
    />
  );
}
