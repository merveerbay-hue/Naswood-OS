import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { fetchCurrentUser, login as loginRequest, logout as logoutRequest } from '@/api/auth';
import { clearSession, isAuthenticated as hasStoredSession } from '@/auth/session';
import { AuthContext, type AuthContextValue } from './auth-context';

const ME_QUERY_KEY = ['auth', 'me'] as const;

export function AuthProvider({ children }: { children: ReactNode }) {
  const queryClient = useQueryClient();
  const [hasSession, setHasSession] = useState(() => hasStoredSession());

  const meQuery = useQuery({
    queryKey: ME_QUERY_KEY,
    queryFn: fetchCurrentUser,
    enabled: hasSession,
    retry: false,
  });

  useEffect(() => {
    if (meQuery.isError) {
      clearSession();
      setHasSession(false);
      queryClient.removeQueries({ queryKey: ME_QUERY_KEY });
    }
  }, [meQuery.isError, queryClient]);

  const login = useCallback(
    async (request: Parameters<AuthContextValue['login']>[0]) => {
      const result = await loginRequest(request);
      setHasSession(true);
      await queryClient.invalidateQueries({ queryKey: ME_QUERY_KEY });
      return result.user;
    },
    [queryClient],
  );

  const logout = useCallback(async () => {
    await logoutRequest();
    setHasSession(false);
    queryClient.removeQueries({ queryKey: ME_QUERY_KEY });
  }, [queryClient]);

  const value = useMemo<AuthContextValue>(
    () => ({
      user: meQuery.data ?? null,
      isAuthenticated: hasSession && !meQuery.isError,
      isBootstrapping: hasSession && meQuery.isPending,
      login,
      logout,
    }),
    [hasSession, login, logout, meQuery.data, meQuery.isError, meQuery.isPending],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
