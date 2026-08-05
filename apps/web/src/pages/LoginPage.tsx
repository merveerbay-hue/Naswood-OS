import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useNavigate } from '@tanstack/react-router';
import { Button, Input, Label } from '@naswood/ui';
import { ApiClientError } from '@/api/types';
import { useAuth } from '@/auth/useAuth';
import { loginFormSchema, mapAuthErrorMessage, type LoginFormValues } from '@/lib/validation';

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [formError, setFormError] = useState<string | null>(null);
  const [requireTenant, setRequireTenant] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginFormSchema),
    defaultValues: {
      username: '',
      password: '',
      companyId: '',
      plantId: '',
      rememberMe: false,
    },
  });

  const onSubmit = handleSubmit(async (values) => {
    setFormError(null);
    try {
      await login({
        username: values.username,
        password: values.password,
        rememberMe: values.rememberMe,
        companyId: values.companyId?.trim() || undefined,
        plantId: values.plantId?.trim() || undefined,
      });
      await navigate({ to: '/' });
    } catch (error) {
      if (error instanceof ApiClientError) {
        if (error.code === 'AUTH-009') {
          setRequireTenant(true);
        }
        setFormError(mapAuthErrorMessage(error.code, error.message));
        return;
      }
      setFormError('Unable to sign in. Check your connection and try again.');
    }
  });

  return (
    <main className="relative flex min-h-screen items-center justify-center overflow-hidden px-4 py-10">
      <div
        aria-hidden
        className="pointer-events-none absolute inset-0 bg-[radial-gradient(ellipse_at_top,_rgba(230,126,34,0.18),_transparent_55%),linear-gradient(160deg,#1a2330_0%,#2f3a45_45%,#1f2937_100%)]"
      />
      <div
        aria-hidden
        className="pointer-events-none absolute inset-0 opacity-[0.08] [background-image:repeating-linear-gradient(90deg,transparent,transparent_24px,rgba(255,255,255,0.35)_25px)]"
      />

      <section className="relative z-10 w-full max-w-md animate-[fade-up_420ms_ease-out]">
        <div className="mb-8 text-center text-white">
          <p className="text-4xl font-semibold tracking-tight sm:text-5xl">Naswood OS</p>
          <p className="mt-3 text-sm text-white/75">Sign in to continue to the platform</p>
        </div>

        <form
          onSubmit={onSubmit}
          className="rounded-[var(--radius-lg)] border border-white/10 bg-white/95 p-6 shadow-xl backdrop-blur-sm sm:p-8"
          noValidate
        >
          <div className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="username">Username</Label>
              <Input
                id="username"
                autoComplete="username"
                aria-invalid={Boolean(errors.username)}
                {...register('username')}
              />
              {errors.username ? (
                <p className="text-sm text-[var(--color-danger)]">{errors.username.message}</p>
              ) : null}
            </div>

            <div className="space-y-2">
              <Label htmlFor="password">Password</Label>
              <Input
                id="password"
                type="password"
                autoComplete="current-password"
                aria-invalid={Boolean(errors.password)}
                {...register('password')}
              />
              {errors.password ? (
                <p className="text-sm text-[var(--color-danger)]">{errors.password.message}</p>
              ) : null}
            </div>

            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <Label htmlFor="companyId">
                  Company{requireTenant ? ' *' : ''}
                </Label>
                <Input
                  id="companyId"
                  placeholder={requireTenant ? 'Required for your account' : 'Optional'}
                  autoComplete="organization"
                  aria-required={requireTenant}
                  {...register('companyId')}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="plantId">
                  Plant{requireTenant ? ' *' : ''}
                </Label>
                <Input
                  id="plantId"
                  placeholder={requireTenant ? 'Required for your account' : 'Optional'}
                  aria-required={requireTenant}
                  {...register('plantId')}
                />
              </div>
            </div>

            <label className="flex items-center gap-2 text-sm text-[var(--text-secondary)]">
              <input
                type="checkbox"
                className="size-4 rounded border-[var(--border-default)] accent-[var(--color-primary)]"
                {...register('rememberMe')}
              />
              Remember me
            </label>

            {formError ? (
              <div
                role="alert"
                className="rounded-[var(--radius-md)] border border-[var(--color-danger)]/30 bg-[var(--color-danger)]/10 px-3 py-2 text-sm text-[var(--color-danger)]"
              >
                {formError}
              </div>
            ) : null}

            <Button type="submit" className="w-full" size="lg" disabled={isSubmitting}>
              {isSubmitting ? 'Signing in…' : 'Sign in'}
            </Button>
          </div>

          <div className="mt-6 flex flex-wrap items-center justify-between gap-3 text-sm">
            <button
              type="button"
              className="text-[var(--text-secondary)] underline-offset-2 hover:underline disabled:opacity-50"
              disabled
              title="Password reset is out of scope for TASK-000"
            >
              Forgot password
            </button>
            <div className="flex gap-3">
              <button
                type="button"
                className="text-[var(--text-secondary)] underline-offset-2 hover:underline disabled:opacity-50"
                disabled
                title="Localization lands in a later task"
              >
                Language
              </button>
              <button
                type="button"
                className="text-[var(--text-secondary)] underline-offset-2 hover:underline disabled:opacity-50"
                disabled
                title="Help center not available yet"
              >
                Help
              </button>
            </div>
          </div>
        </form>
      </section>

      <style>{`
        @keyframes fade-up {
          from { opacity: 0; transform: translateY(12px); }
          to { opacity: 1; transform: translateY(0); }
        }
      `}</style>
    </main>
  );
}
