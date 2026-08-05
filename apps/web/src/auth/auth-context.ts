import { createContext } from 'react';
import type { AuthenticatedUser, CurrentUser, LoginRequest } from '@/api/types';

export interface AuthContextValue {
  user: CurrentUser | AuthenticatedUser | null;
  isAuthenticated: boolean;
  isBootstrapping: boolean;
  login: (request: LoginRequest) => Promise<AuthenticatedUser>;
  logout: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextValue | null>(null);
