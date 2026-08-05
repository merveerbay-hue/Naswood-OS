import {
  Outlet,
  createRootRouteWithContext,
  createRoute,
  createRouter,
  redirect,
} from '@tanstack/react-router';
import type { QueryClient } from '@tanstack/react-query';
import { AuthenticatedLayout } from './layouts/AuthenticatedLayout';
import { DashboardPage } from './pages/DashboardPage';
import { FilesPage } from './pages/FilesPage';
import { LoginPage } from './pages/LoginPage';
import { ModulePlaceholderPage } from './pages/ModulePlaceholderPage';
import { isAuthenticated } from './auth/session';
import { collectNavPaths } from './navigation/nav-config';

export interface RouterContext {
  queryClient: QueryClient;
}

const rootRoute = createRootRouteWithContext<RouterContext>()({
  component: () => <Outlet />,
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

const authenticatedRoute = createRoute({
  getParentRoute: () => rootRoute,
  id: 'authenticated',
  beforeLoad: () => {
    if (!isAuthenticated()) {
      throw redirect({ to: '/login' });
    }
  },
  component: AuthenticatedLayout,
});

const dashboardRoute = createRoute({
  getParentRoute: () => authenticatedRoute,
  path: '/',
  component: DashboardPage,
});

const filesRoute = createRoute({
  getParentRoute: () => authenticatedRoute,
  path: '/administration/files',
  component: FilesPage,
});

const modulePaths = collectNavPaths().filter(
  (path) => path !== '/' && path !== '/administration/files',
);

const moduleRoutes = modulePaths.map((path) =>
  createRoute({
    getParentRoute: () => authenticatedRoute,
    path,
    component: ModulePlaceholderPage,
  }),
);

export const routeTree = rootRoute.addChildren([
  loginRoute,
  authenticatedRoute.addChildren([dashboardRoute, filesRoute, ...moduleRoutes]),
]);

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
