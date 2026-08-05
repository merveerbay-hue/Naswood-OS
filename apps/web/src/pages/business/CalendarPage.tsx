import { ResourcePage } from './ResourcePage';

export function CalendarPage() {
  return (
    <ResourcePage
      title="Calendar"
      description="TASK-052 · Production MVP"
      route="calendars"
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
