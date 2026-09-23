import { useMemo, useState } from "react";
import { CATEGORIES, UNITS } from "./labels.js";
import { addMaterial, available, isLow, moveStock, updateMaterial } from "./logic.js";
import { useStore } from "./store.jsx";
import { Badge, Field, Modal, Table } from "./ui.jsx";

const emptyForm = {
  code: "",
  name: "",
  category: CATEGORIES[0],
  unit: UNITS[0],
  warehouse: "",
  location: "",
  batch: "",
  onHand: "",
  reorderPoint: "",
};

export default function Inventory({ session }) {
  const { state, run } = useStore();
  const [tab, setTab] = useState("balance");
  const [q, setQ] = useState("");
  const [status, setStatus] = useState("Hepsi");
  const [creating, setCreating] = useState(false);
  const [form, setForm] = useState(emptyForm);
  const [edit, setEdit] = useState(null);
  const [move, setMove] = useState(null);

  const rows = useMemo(() => {
    return state.materials.filter((row) => {
      if (!session.allPlants && row.plant !== session.plant) return false;
      if (status !== "Hepsi" && row.status !== status) return false;
      const text = `${row.code} ${row.name} ${row.warehouse} ${row.batch}`.toLocaleLowerCase("tr");
      return text.includes(q.trim().toLocaleLowerCase("tr"));
    });
  }, [state.materials, session, q, status]);

  const movements = state.movements.filter((row) => session.allPlants || row.plant === session.plant);

  function set(key, value) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  return (
    <section>
      <div className="title-row">
        <div>
          <h1>Stok</h1>
          <p className="lead">Eldeki, rezerve ve kullanılabilir miktar aynı kayıttan gelir.</p>
        </div>
        <button onClick={() => { setForm(emptyForm); setCreating(true); }}>Yeni malzeme</button>
      </div>
      <div className="toolbar">
        <button className={tab === "balance" ? "on" : "ghost"} onClick={() => setTab("balance")}>Bakiyeler</button>
        <button className={tab === "moves" ? "on" : "ghost"} onClick={() => setTab("moves")}>Hareketler</button>
        <input value={q} placeholder="Kod, ad, depo, parti" onChange={(event) => setQ(event.target.value)} />
        <select value={status} onChange={(event) => setStatus(event.target.value)}>
          {["Hepsi", "Uygun", "Karantina", "Bloke"].map((item) => (
            <option key={item}>{item}</option>
          ))}
        </select>
      </div>
      {tab === "balance" ? (
        <Table
          rows={rows}
          empty="Bu tesiste malzeme yok. Yeni malzeme ile ekleyin."
          columns={[
            { key: "code", label: "Kod" },
            { key: "name", label: "Ad" },
            { key: "category", label: "Kategori" },
            { key: "warehouse", label: "Depo" },
            { key: "onHand", label: "Eldeki", render: (row) => `${row.onHand} ${row.unit}` },
            { key: "reserved", label: "Rezerve" },
            {
              key: "available",
              label: "Kullanılabilir",
              render: (row) => (
                <span className={isLow(row) ? "low" : ""}>
                  {available(row)} {row.unit}
                </span>
              ),
            },
            { key: "status", label: "Durum", render: (row) => <Badge value={row.status} /> },
            {
              key: "act",
              label: "",
              render: (row) => (
                <span className="actions">
                  <button className="ghost" onClick={() => setEdit(row)}>Düzenle</button>
                  <button className="ghost" onClick={() => setMove({ id: row.id, type: "Giriş", qty: "", note: "" })}>Hareket</button>
                </span>
              ),
            },
          ]}
        />
      ) : (
        <Table
          rows={movements}
          columns={[
            { key: "at", label: "Zaman", render: (row) => new Date(row.at).toLocaleString("tr-TR") },
            { key: "materialCode", label: "Malzeme" },
            { key: "type", label: "Tip" },
            { key: "qty", label: "Miktar" },
            { key: "note", label: "Not" },
            { key: "actor", label: "Kim" },
          ]}
        />
      )}

      {creating && (
        <Modal title="Yeni malzeme" onClose={() => setCreating(false)}>
          <div className="grid">
            <Field label="Kod"><input value={form.code} onChange={(event) => set("code", event.target.value)} /></Field>
            <Field label="Ad"><input value={form.name} onChange={(event) => set("name", event.target.value)} /></Field>
            <Field label="Kategori">
              <select value={form.category} onChange={(event) => set("category", event.target.value)}>
                {CATEGORIES.map((item) => <option key={item}>{item}</option>)}
              </select>
            </Field>
            <Field label="Birim">
              <select value={form.unit} onChange={(event) => set("unit", event.target.value)}>
                {UNITS.map((item) => <option key={item}>{item}</option>)}
              </select>
            </Field>
            <Field label="Depo"><input value={form.warehouse} onChange={(event) => set("warehouse", event.target.value)} /></Field>
            <Field label="Lokasyon"><input value={form.location} onChange={(event) => set("location", event.target.value)} /></Field>
            <Field label="Parti"><input value={form.batch} onChange={(event) => set("batch", event.target.value)} /></Field>
            <Field label="Açılış miktarı"><input value={form.onHand} onChange={(event) => set("onHand", event.target.value)} /></Field>
            <Field label="Yeniden sipariş"><input value={form.reorderPoint} onChange={(event) => set("reorderPoint", event.target.value)} /></Field>
          </div>
          <footer>
            <button
              data-submit
              type="button"
              onClick={() => {
                const ok = run(
                  (current) => addMaterial(current, { ...form, plant: session.plant, actor: session.name }),
                  "Malzeme eklendi.",
                );
                if (ok) setCreating(false);
              }}
            >
              Kaydet
            </button>
          </footer>
        </Modal>
      )}

      {edit && (
        <Modal title={edit.code} onClose={() => setEdit(null)}>
          <div className="grid">
            <Field label="Ad"><input value={edit.name} onChange={(event) => setEdit({ ...edit, name: event.target.value })} /></Field>
            <Field label="Depo"><input value={edit.warehouse} onChange={(event) => setEdit({ ...edit, warehouse: event.target.value })} /></Field>
            <Field label="Lokasyon"><input value={edit.location} onChange={(event) => setEdit({ ...edit, location: event.target.value })} /></Field>
            <Field label="Parti"><input value={edit.batch} onChange={(event) => setEdit({ ...edit, batch: event.target.value })} /></Field>
            <Field label="Yeniden sipariş"><input value={edit.reorderPoint} onChange={(event) => setEdit({ ...edit, reorderPoint: event.target.value })} /></Field>
          </div>
          <p className="lead">Miktar yalnızca stok hareketi ile değişir. Eldeki {edit.onHand}, rezerve {edit.reserved}.</p>
          <footer>
            <button
              data-submit
              type="button"
              onClick={() => {
                const ok = run(
                  (current) => updateMaterial(current, edit.id, edit, session.name),
                  "Malzeme güncellendi.",
                );
                if (ok) setEdit(null);
              }}
            >
              Kaydet
            </button>
          </footer>
        </Modal>
      )}

      {move && (
        <Modal title="Stok hareketi" onClose={() => setMove(null)}>
          <div className="grid">
            <Field label="Tip">
              <select value={move.type} onChange={(event) => setMove({ ...move, type: event.target.value })}>
                <option>Giriş</option>
                <option>Çıkış</option>
              </select>
            </Field>
            <Field label="Miktar"><input value={move.qty} onChange={(event) => setMove({ ...move, qty: event.target.value })} /></Field>
            <Field label="Not"><input value={move.note} onChange={(event) => setMove({ ...move, note: event.target.value })} /></Field>
          </div>
          <footer>
            <button
              data-submit
              type="button"
              onClick={() => {
                const ok = run(
                  (current) => moveStock(current, { ...move, materialId: move.id, actor: session.name }),
                  "Hareket kaydedildi.",
                );
                if (ok) setMove(null);
              }}
            >
              Uygula
            </button>
          </footer>
        </Modal>
      )}
    </section>
  );
}
