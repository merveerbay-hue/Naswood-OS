import { useMemo, useState } from "react";
import { NAV, ROLES, canOpen } from "./labels.js";
import { PLANTS, available, isLow } from "./logic.js";
import { useStore } from "./store.jsx";

export default function Shell({ session, setSession, page, setPage, onLogout, children }) {
  const { state, reset } = useStore();
  const [collapsed, setCollapsed] = useState(false);
  const [query, setQuery] = useState("");
  const [openNotes, setOpenNotes] = useState(false);

  const items = NAV.filter((item) => canOpen(session.role, item.id));
  const plantRows = (rows) => rows.filter((row) => session.allPlants || row.plant === session.plant);

  const notes = useMemo(() => {
    const list = [];
    plantRows(state.materials)
      .filter(isLow)
      .forEach((row) => list.push({ page: "inventory", text: `${row.code} düşük stok` }));
    plantRows(state.inspections)
      .filter((row) => row.result === "Bekliyor")
      .forEach((row) => list.push({ page: "quality", text: `${row.id} kalite bekliyor` }));
    plantRows(state.maintenance)
      .filter((row) => row.status !== "Bitti")
      .forEach((row) => list.push({ page: "maintenance", text: `${row.asset} bakım ${row.status.toLowerCase()}` }));
    return list;
  }, [state, session]);

  const hits = useMemo(() => {
    const q = query.trim().toLocaleLowerCase("tr");
    if (!q) return [];
    return plantRows(state.materials)
      .filter((row) => `${row.code} ${row.name}`.toLocaleLowerCase("tr").includes(q))
      .slice(0, 6)
      .map((row) => ({
        id: row.id,
        label: `${row.code} · ${row.name}`,
        meta: `kullanılabilir ${available(row)} ${row.unit}`,
      }));
  }, [query, state, session]);

  return (
    <div className={`shell ${collapsed ? "collapsed" : ""}`}>
      <aside>
        <button className="logo" onClick={() => setPage("dashboard")}>
          <span className="mark">N</span>
          {!collapsed && (
            <span>
              <strong>Naswood OS</strong>
              <small>{session.plant}</small>
            </span>
          )}
        </button>
        <nav>
          {items.map((item) => (
            <button key={item.id} className={page === item.id ? "active" : ""} onClick={() => setPage(item.id)}>
              {item.label}
            </button>
          ))}
        </nav>
        <button className="collapse" onClick={() => setCollapsed((value) => !value)}>
          {collapsed ? "»" : "Menüyü daralt"}
        </button>
      </aside>
      <div className="workspace">
        <header className="top">
          <div className="search">
            <input
              value={query}
              placeholder="Malzeme kodu veya adı"
              onChange={(event) => setQuery(event.target.value)}
            />
            {hits.length > 0 && (
              <div className="hits">
                {hits.map((hit) => (
                  <button
                    key={hit.id}
                    onClick={() => {
                      setQuery("");
                      setPage("inventory");
                    }}
                  >
                    <strong>{hit.label}</strong>
                    <small>{hit.meta}</small>
                  </button>
                ))}
              </div>
            )}
          </div>
          <label className="plant">
            Tesis
            <select
              value={session.plant}
              onChange={(event) => setSession({ ...session, plant: event.target.value, allPlants: false })}
            >
              {PLANTS.map((plant) => (
                <option key={plant}>{plant}</option>
              ))}
            </select>
          </label>
          <label className="check">
            <input
              type="checkbox"
              checked={session.allPlants}
              onChange={(event) => setSession({ ...session, allPlants: event.target.checked })}
            />
            Tüm tesisler
          </label>
          <button className="icon" onClick={() => setOpenNotes((value) => !value)}>
            Bildirim {notes.length}
          </button>
          {openNotes && (
            <div className="notes">
              {notes.length === 0 && <p>Açık uyarı yok.</p>}
              {notes.map((note) => (
                <button
                  key={note.text}
                  onClick={() => {
                    setPage(note.page);
                    setOpenNotes(false);
                  }}
                >
                  {note.text}
                </button>
              ))}
            </div>
          )}
          <label className="plant">
            Rol
            <select value={session.role} onChange={(event) => setSession({ ...session, role: event.target.value })}>
              {ROLES.map((role) => (
                <option key={role}>{role}</option>
              ))}
            </select>
          </label>
          <div className="who">
            <strong>{session.name}</strong>
          </div>
          <button className="ghost" onClick={reset}>
            Veriyi sıfırla
          </button>
          <button className="ghost" onClick={onLogout}>
            Çıkış
          </button>
        </header>
        <div className="content">{children}</div>
      </div>
    </div>
  );
}
