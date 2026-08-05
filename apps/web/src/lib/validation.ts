import { z } from 'zod';

export const loginFormSchema = z.object({
  username: z.string().trim().min(1, 'Username is required.'),
  password: z.string().min(1, 'Password is required.'),
  companyId: z.string().trim().optional(),
  plantId: z.string().trim().optional(),
  rememberMe: z.boolean(),
});

export type LoginFormValues = z.infer<typeof loginFormSchema>;

export function mapAuthErrorMessage(code: string | null, fallback: string): string {
  switch (code) {
    case 'AUTH-001':
      return 'Invalid username or password.';
    case 'AUTH-002':
      return 'This account is disabled. Contact your administrator.';
    case 'AUTH-003':
      return 'Account locked after too many failed attempts. Try again later.';
    case 'AUTH-004':
      return 'Your password has expired. Contact your administrator.';
    case 'AUTH-007':
      return 'Your session has expired. Please sign in again.';
    case 'AUTH-008':
      return 'Session could not be refreshed. Please sign in again.';
    case 'AUTH-009':
      return 'Company and plant are required for your account.';
    default:
      return fallback;
  }
}
