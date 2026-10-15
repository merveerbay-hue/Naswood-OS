import { useEffect, useState } from "react";
import { canOpen } from "./labels.js";
import { StoreProvider, useStore } from "./store.jsx";
import Login from "./Login.jsx";
import Shell from "./Shell.jsx";
import Dashboard from "./Dashboard.jsx";
import Inventory from "./Inventory.jsx";
import {
  Admin,
  Analytics,
  Copilot,
  Finance,
  Maintenance,
  Master,
  Production,
  Purchasing,
  Quality,
  Sales,
  Twin,
} from "./Pages.jsx";

const SESSION_KEY = "naswood-os-session";

function readSession() {
  try {
    const raw = sessionStorage.getItem(SESSION_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}

function Toasts() {
  const { toast } = useStore();
  if (!toast) return null;
  return <div className={`toast ${toast.kind}`}>{toast.text}</div>;
}

function Root() {
  const { state } = useStore();
  const [session, setSession] = useState(readSession);
  const [page, setPage] = useState("dashboard");

  useEffect(() => {
    if (session) sessionStorage.setItem(SESSION_KEY, JSON.stringify(session));
    else sessionStorage.removeItem(SESSION_KEY);
  }, [session]);

  useEffect(() => {
    if (session && !canOpen(session.role, page)) setPage("dashboard");
  }, [session, page]);

  if (!session) return <Login onEnter={setSession} />;

  const screens = {
    dashboard: <Dashboard session={session} state={state} open={setPage} />,
    inventory: <Inventory session={session} />,
    purchasing: <Purchasing session={session} />,
    sales: <Sales session={session} />,
    production: <Production session={session} />,
    quality: <Quality session={session} />,
    maintenance: <Maintenance session={session} />,
    finance: <Finance session={session} />,
    analytics: <Analytics session={session} />,
    ai: <Copilot session={session} />,
    twin: <Twin session={session} />,
    master: <Master session={session} />,
    admin: <Admin session={session} />,
  };

  return (
    <Shell
      session={session}
      setSession={setSession}
      page={page}
      setPage={setPage}
      onLogout={() => setSession(null)}
    >
      {screens[page] || screens.dashboard}
      <Toasts />
    </Shell>
  );
}

export default function App() {
  return (
    <StoreProvider>
      <Root />
    </StoreProvider>
  );
}
