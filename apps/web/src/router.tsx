import {
  Outlet,
  createRootRouteWithContext,
  createRoute,
  createRouter,
} from '@tanstack/react-router';
import type { QueryClient } from '@tanstack/react-query';
import { FoundationHomePage } from './pages/FoundationHomePage';

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
  component: FoundationHomePage,
});

export const routeTree = rootRoute.addChildren([indexRoute]);

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
