import { ResourcePage } from './ResourcePage';

export function SalesReportPage() {
  return (
    <ResourcePage
      title="Sales Reports"
      description="TASK-045 · Sales MVP"
      route="sales/reports"
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
