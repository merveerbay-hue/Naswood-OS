import { Moon, Sun, Monitor } from 'lucide-react';
import { Button } from '@naswood/ui';
import { useI18n } from '@/i18n';
import { useTheme } from '@/theme/useTheme';

export function ThemeToggle() {
  const { t } = useI18n();
  const { preference, resolved, cyclePreference } = useTheme();

  const Icon = preference === 'system' ? Monitor : resolved === 'dark' ? Moon : Sun;
  const label =
    preference === 'system'
      ? t('themeLabels.system')
      : preference === 'dark'
        ? t('themeLabels.dark')
        : t('themeLabels.light');

  return (
    <Button
      type="button"
      variant="ghost"
      size="sm"
      onClick={cyclePreference}
      aria-label={label}
      title={t('themeCycle')}
    >
      <Icon className="size-4" />
    </Button>
  );
}
