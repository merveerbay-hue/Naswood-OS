import { Link } from '@tanstack/react-router';
import { useState } from 'react';
import { Button, Card, CardContent, CardDescription, CardHeader, CardTitle } from '@naswood/ui';
import { useI18n } from '@/i18n';

export interface ProcessWizardStep {
  title: string;
  hint?: string;
}

interface ProcessWizardProps {
  screenId: string;
  title: string;
  description: string;
  steps: ProcessWizardStep[];
  finishLabel: string;
  libraryPath: string;
  libraryLabel: string;
}

/** Job wizard shell — replaces shared Create / Yeni entity forms. */
export function ProcessWizard({
  screenId,
  title,
  description,
  steps,
  finishLabel,
  libraryPath,
  libraryLabel,
}: ProcessWizardProps) {
  const { t } = useI18n();
  const [step, setStep] = useState(0);
  const current = steps[step];
  const isLast = step >= steps.length - 1;

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <p className="text-xs font-medium text-[var(--text-muted)]">{screenId}</p>
          <h2 className="text-xl font-semibold tracking-tight">{title}</h2>
          <p className="mt-1 text-sm text-[var(--text-secondary)]">{description}</p>
          <p className="mt-1 text-xs text-[var(--text-muted)]">{t('wizard.screenType')}</p>
        </div>
        <Link
          to={libraryPath}
          className="text-sm font-medium text-[var(--color-primary)] hover:underline"
        >
          {libraryLabel}
        </Link>
      </div>

      <ol className="flex flex-wrap gap-2">
        {steps.map((s, i) => (
          <li key={s.title}>
            <button
              type="button"
              onClick={() => setStep(i)}
              className={`rounded-md px-3 py-1.5 text-xs font-medium ${
                i === step
                  ? 'bg-[var(--color-primary)] text-white'
                  : i < step
                    ? 'bg-[var(--color-surface-hover)] text-[var(--text-primary)]'
                    : 'bg-[var(--color-surface)] text-[var(--text-muted)]'
              }`}
            >
              {i + 1}. {s.title}
            </button>
          </li>
        ))}
      </ol>

      <Card>
        <CardHeader>
          <CardTitle>
            {step + 1}. {current?.title}
          </CardTitle>
          <CardDescription>{current?.hint ?? t('wizard.stepHint')}</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <p className="text-sm text-[var(--text-secondary)]">{t('wizard.bodyHint')}</p>
          <div className="flex flex-wrap gap-2">
            <Button type="button" variant="secondary" disabled={step === 0} onClick={() => setStep((s) => s - 1)}>
              {t('wizard.back')}
            </Button>
            {!isLast ? (
              <Button type="button" onClick={() => setStep((s) => s + 1)}>
                {t('wizard.next')}
              </Button>
            ) : (
              <Button type="button" onClick={() => setStep(0)}>
                {finishLabel}
              </Button>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
