import { z } from 'zod';
import { tr } from '@/i18n/tr';

export const loginFormSchema = z.object({
  username: z.string().trim().min(1, tr.login.usernameRequired),
  password: z.string().min(1, tr.login.passwordRequired),
  companyId: z.string().trim().optional(),
  plantId: z.string().trim().optional(),
  rememberMe: z.boolean(),
});

export type LoginFormValues = z.infer<typeof loginFormSchema>;

export function mapAuthErrorMessage(code: string | null, fallback: string): string {
  if (code && tr.authErrors[code]) return tr.authErrors[code];
  return fallback;
}
