import { createContext, useContext, useEffect, useMemo, useRef, useState } from "react";
import { seedState } from "./logic.js";

const KEY = "naswood-os-v1";
const StoreContext = createContext(null);

function readState() {
  try {
    const raw = localStorage.getItem(KEY);
    if (raw) return JSON.parse(raw);
  } catch {
    /* boş veya bozuk kayıtta örnek veriye dön */
  }
  return seedState();
}

export function StoreProvider({ children }) {
  const [state, setState] = useState(readState);
  const ref = useRef(state);
  const [toast, setToast] = useState(null);
  ref.current = state;

  useEffect(() => {
    localStorage.setItem(KEY, JSON.stringify(state));
  }, [state]);

  useEffect(() => {
    if (!toast) return undefined;
    const timer = setTimeout(() => setToast(null), 3200);
    return () => clearTimeout(timer);
  }, [toast]);

  const api = useMemo(
    () => ({
      run(change, success) {
        try {
          const next = change(ref.current);
          ref.current = next;
          setState(next);
          if (success) setToast({ kind: "ok", text: success });
          return true;
        } catch (error) {
          setToast({ kind: "err", text: error.message });
          return false;
        }
      },
      reset() {
        const next = seedState();
        ref.current = next;
        setState(next);
        setToast({ kind: "ok", text: "Örnek fabrika verisi geri yüklendi." });
      },
    }),
    [],
  );

  return (
    <StoreContext.Provider value={{ state, toast, ...api }}>
      {children}
    </StoreContext.Provider>
  );
}

export function useStore() {
  const value = useContext(StoreContext);
  if (!value) throw new Error("Store eksik");
  return value;
}
