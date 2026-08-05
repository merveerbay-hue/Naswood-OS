import { Moon, Sun, Monitor } from 'lucide-react';
import { Button } from '@naswood/ui';
import { useTheme } from '@/theme/useTheme';

export function ThemeToggle() {
  const { preference, resolved, cyclePreference } = useTheme();

  const Icon = preference === 'system' ? Monitor : resolved === 'dark' ? Moon : Sun;
  const label =
    preference === 'system'
      ? 'Theme: System'
      : preference === 'dark'
        ? 'Theme: Dark'
        : 'Theme: Light';

  return (
    <Button
      type="button"
      variant="ghost"
      size="sm"
      onClick={cyclePreference}
      aria-label={label}
      title={`${label} (click to cycle light → dark → system)`}
    >
      <Icon className="size-4" />
    </Button>
  );
}
