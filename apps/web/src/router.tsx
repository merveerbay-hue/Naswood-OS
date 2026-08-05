import {
  Outlet,
  createRootRouteWithContext,
  createRoute,
  createRouter,
  redirect,
} from '@tanstack/react-router';
import type { QueryClient } from '@tanstack/react-query';
import { FoundationHomePage } from './pages/FoundationHomePage';
import { LoginPage } from './pages/LoginPage';
import { isAuthenticated } from './auth/session';

export interface RouterContext {
  queryClient: QueryClient;
}

const rootRoute = createRootRouteWithContext<RouterContext>()({
  component: () => (
    <div className="min-h-screen bg-[var(--color-background)] text-[var(--text-primary)]">
      <Outlet />
    </div>
  ),
});

const indexRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/',
  beforeLoad: () => {
    if (!isAuthenticated()) {
      throw redirect({ to: '/login' });
    }
  },
  component: FoundationHomePage,
});

const loginRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/login',
  beforeLoad: () => {
    if (isAuthenticated()) {
      throw redirect({ to: '/' });
    }
  },
  component: LoginPage,
});

export const routeTree = rootRoute.addChildren([indexRoute, loginRoute]);

export function createAppRouter(queryClient: QueryClient) {
  return createRouter({
    routeTree,
    context: { queryClient },
    defaultPreload: 'intent',
  });
}

declare module '@tanstack/react-router' {
  interface Register {
    router: ReturnType<typeof createAppRouter>;
  }
}
