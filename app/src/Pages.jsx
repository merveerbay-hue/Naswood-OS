import { useState } from "react";
import { ROLES } from "./labels.js";
import {
  addInspection,
  addInvoice,
  addMaintenance,
  addParty,
  addProduction,
  addPurchase,
  addSale,
  addUser,
  answerQuestion,
  decideInspection,
  orderPurchase,
  payInvoice,
  receivePurchase,
  stockByCategory,
  toggleUser,
  transitionMaintenance,
  transitionProduction,
  transitionSale,
  available,
} from "./logic.js";
import { PLANTS } from "./logic.js";
import { useStore } from "./store.jsx";
import { Badge, Field, Modal, Table } from "./ui.jsx";

function materialName(state, id) {
  return state.materials.find((item) => item.id === id)?.code || id;
}

function usePlant(session, rows) {
  return rows.filter((row) => session.allPlants || row.plant === session.plant);
}

export function Purchasing({ session }) {
  const { state, run } = useStore();
  const rows = usePlant(session, state.purchases);
  const materials = usePlant(session, state.materials);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ supplier: "", materialId: materials[0]?.id || "", qty: "" });

  return (
    <section>
      <div className="title-row">
        <div>
          <h1>Satınalma</h1>
          <p className="lead">Taslak onaylanır, sipariş teslim alınca stok girişi olur.</p>
        </div>
        <button onClick={() => setOpen(true)} disabled={!materials.length}>Yeni sipariş</button>
      </div>
      <Table
        rows={rows}
        columns={[
          { key: "id", label: "No" },
          { key: "supplier", label: "Tedarikçi" },
          { key: "materialId", label: "Malzeme", render: (row) => materialName(state, row.materialId) },
          { key: "qty", label: "Miktar" },
          { key: "status", label: "Durum", render: (row) => <Badge value={row.status} /> },
          {
            key: "act",
            label: "",
            render: (row) =>
              row.status === "Taslak" ? (
                <button className="ghost" onClick={() => run((current) => orderPurchase(current, row.id, session.name), "Sipariş onaylandı.")}>Onayla</button>
              ) : row.status === "Sipariş" ? (
                <button onClick={() => run((current) => receivePurchase(current, row.id, session.name), "Mal kabul stoka işlendi.")}>Teslim al</button>
              ) : null,
          },
        ]}
      />
      {open && (
        <Modal title="Satınalma siparişi" onClose={() => setOpen(false)}>
          <div className="grid">
            <Field label="Tedarikçi"><input value={form.supplier} onChange={(event) => setForm({ ...form, supplier: event.target.value })} /></Field>
            <Field label="Malzeme">
              <select value={form.materialId} onChange={(event) => setForm({ ...form, materialId: event.target.value })}>
                {materials.map((item) => <option key={item.id} value={item.id}>{item.code} {item.name}</option>)}
              </select>
            </Field>
            <Field label="Miktar"><input value={form.qty} onChange={(event) => setForm({ ...form, qty: event.target.value })} /></Field>
          </div>
          <footer>
            <button
              data-submit
              type="button"
              onClick={() => {
                const ok = run((current) => addPurchase(current, { ...form, plant: session.plant, actor: session.name }), "Taslak sipariş açıldı.");
                if (ok) setOpen(false);
              }}
            >
              Kaydet
            </button>
          </footer>
        </Modal>
      )}
    </section>
  );
}

export function Sales({ session }) {
  const { state, run } = useStore();
  const rows = usePlant(session, state.sales);
  const materials = usePlant(session, state.materials);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ customer: "", materialId: materials[0]?.id || "", qty: "" });
  return (
    <section>
      <div className="title-row">
        <div>
          <h1>Satış</h1>
          <p className="lead">Onaylı sipariş sevk edilince kullanılabilir stoktan çıkar.</p>
        </div>
        <button onClick={() => setOpen(true)} disabled={!materials.length}>Yeni sipariş</button>
      </div>
      <Table
        rows={rows}
        columns={[
          { key: "id", label: "No" },
          { key: "customer", label: "Müşteri" },
          { key: "materialId", label: "Malzeme", render: (row) => materialName(state, row.materialId) },
          { key: "qty", label: "Miktar" },
          { key: "status", label: "Durum", render: (row) => <Badge value={row.status} /> },
          {
            key: "act",
            label: "",
            render: (row) =>
              row.status === "Sevk edildi" ? null : (
                <button
                  onClick={() =>
                    run(
                      (current) => transitionSale(current, row.id, session.name),
                      row.status === "Taslak" ? "Sipariş onaylandı." : "Sevkiyat stoktan düştü.",
                    )
                  }
                >
                  {row.status === "Taslak" ? "Onayla" : "Sevk et"}
                </button>
              ),
          },
        ]}
      />
      {open && (
        <Modal title="Satış siparişi" onClose={() => setOpen(false)}>
          <div className="grid">
            <Field label="Müşteri"><input value={form.customer} onChange={(event) => setForm({ ...form, customer: event.target.value })} /></Field>
            <Field label="Malzeme">
              <select value={form.materialId} onChange={(event) => setForm({ ...form, materialId: event.target.value })}>
                {materials.map((item) => <option key={item.id} value={item.id}>{item.code} · kullanılabilir {available(item)}</option>)}
              </select>
            </Field>
            <Field label="Miktar"><input value={form.qty} onChange={(event) => setForm({ ...form, qty: event.target.value })} /></Field>
          </div>
          <footer>
            <button data-submit type="button" onClick={() => {
              const ok = run((current) => addSale(current, { ...form, plant: session.plant, actor: session.name }), "Satış taslağı açıldı.");
              if (ok) setOpen(false);
            }}>Kaydet</button>
          </footer>
        </Modal>
      )}
    </section>
  );
}

export function Production({ session }) {
  const { state, run } = useStore();
  const rows = usePlant(session, state.productions);
  const materials = usePlant(session, state.materials);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ product: "", materialId: materials[0]?.id || "", qty: "" });
  const nextLabel = { Planlandı: "Serbest bırak", Serbest: "Üretime al", Üretimde: "Tamamla" };
  return (
    <section>
      <div className="title-row">
        <div>
          <h1>Üretim</h1>
          <p className="lead">Serbest bırakınca malzeme rezerve edilir. Tamamlanınca rezerve miktar tüketilir.</p>
        </div>
        <button onClick={() => setOpen(true)} disabled={!materials.length}>Yeni emir</button>
      </div>
      <Table
        rows={rows}
        columns={[
          { key: "id", label: "No" },
          { key: "product", label: "Ürün" },
          { key: "materialId", label: "Malzeme", render: (row) => materialName(state, row.materialId) },
          { key: "qty", label: "Miktar" },
          { key: "status", label: "Durum", render: (row) => <Badge value={row.status} /> },
          {
            key: "act",
            label: "",
            render: (row) => (
              <span className="actions">
                {nextLabel[row.status] && (
                  <button onClick={() => run((current) => transitionProduction(current, row.id, row.status === "Planlandı" ? "Serbest" : row.status === "Serbest" ? "Üretimde" : "Tamamlandı", session.name), "Üretim durumu güncellendi.")}>
                    {nextLabel[row.status]}
                  </button>
                )}
                {(row.status === "Planlandı" || row.status === "Serbest") && (
                  <button className="ghost" onClick={() => run((current) => transitionProduction(current, row.id, "İptal", session.name), "Emir iptal edildi.")}>İptal</button>
                )}
              </span>
            ),
          },
        ]}
      />
      {open && (
        <Modal title="Üretim emri" onClose={() => setOpen(false)}>
          <div className="grid">
            <Field label="Ürün"><input value={form.product} onChange={(event) => setForm({ ...form, product: event.target.value })} /></Field>
            <Field label="Tüketilecek malzeme">
              <select value={form.materialId} onChange={(event) => setForm({ ...form, materialId: event.target.value })}>
                {materials.map((item) => <option key={item.id} value={item.id}>{item.code} · kullanılabilir {available(item)}</option>)}
              </select>
            </Field>
            <Field label="Miktar"><input value={form.qty} onChange={(event) => setForm({ ...form, qty: event.target.value })} /></Field>
          </div>
          <footer>
            <button data-submit type="button" onClick={() => {
              const ok = run((current) => addProduction(current, { ...form, plant: session.plant, actor: session.name }), "Üretim emri planlandı.");
              if (ok) setOpen(false);
            }}>Kaydet</button>
          </footer>
        </Modal>
      )}
    </section>
  );
}

export function Quality({ session }) {
  const { state, run } = useStore();
  const rows = usePlant(session, state.inspections);
  const materials = usePlant(session, state.materials);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ materialId: materials[0]?.id || "", kind: "Giriş kalite", note: "" });
  return (
    <section>
      <div className="title-row">
        <div>
          <h1>Kalite</h1>
          <p className="lead">Kalır kararı malzemeyi bloke eder. Bloke stok çıkamaz ve sevk edilemez.</p>
        </div>
        <button onClick={() => setOpen(true)} disabled={!materials.length}>Yeni kontrol</button>
      </div>
      <Table
        rows={rows}
        columns={[
          { key: "id", label: "No" },
          { key: "materialId", label: "Malzeme", render: (row) => materialName(state, row.materialId) },
          { key: "kind", label: "Tür" },
          { key: "result", label: "Sonuç", render: (row) => <Badge value={row.result} /> },
          { key: "note", label: "Not" },
          {
            key: "act",
            label: "",
            render: (row) =>
              row.result === "Bekliyor" ? (
                <span className="actions">
                  <button onClick={() => run((current) => decideInspection(current, row.id, "Geçti", session.name), "Kontrol geçti.")}>Geçti</button>
                  <button className="danger" onClick={() => run((current) => decideInspection(current, row.id, "Kaldı", session.name), "Malzeme bloke edildi.")}>Kaldı</button>
                </span>
              ) : null,
          },
        ]}
      />
      {open && (
        <Modal title="Kalite kontrol" onClose={() => setOpen(false)}>
          <div className="grid">
            <Field label="Malzeme">
              <select value={form.materialId} onChange={(event) => setForm({ ...form, materialId: event.target.value })}>
                {materials.map((item) => <option key={item.id} value={item.id}>{item.code}</option>)}
              </select>
            </Field>
            <Field label="Tür">
              <select value={form.kind} onChange={(event) => setForm({ ...form, kind: event.target.value })}>
                {["Giriş kalite", "Proses", "Final"].map((item) => <option key={item}>{item}</option>)}
              </select>
            </Field>
            <Field label="Not"><input value={form.note} onChange={(event) => setForm({ ...form, note: event.target.value })} /></Field>
          </div>
          <footer>
            <button data-submit type="button" onClick={() => {
              const ok = run((current) => addInspection(current, { ...form, plant: session.plant, actor: session.name }), "Kontrol açıldı.");
              if (ok) setOpen(false);
            }}>Kaydet</button>
          </footer>
        </Modal>
      )}
    </section>
  );
}

export function Maintenance({ session }) {
  const { state, run } = useStore();
  const rows = usePlant(session, state.maintenance);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ asset: "", kind: "Arıza", priority: "Orta" });
  return (
    <section>
      <div className="title-row">
        <div>
          <h1>Bakım</h1>
          <p className="lead">İş emri açık, işlemde ve bitti adımlarıyla ilerler.</p>
        </div>
        <button onClick={() => setOpen(true)}>Yeni iş emri</button>
      </div>
      <Table
        rows={rows}
        columns={[
          { key: "id", label: "No" },
          { key: "asset", label: "Varlık" },
          { key: "kind", label: "Tür" },
          { key: "priority", label: "Öncelik", render: (row) => <Badge value={row.priority} /> },
          { key: "status", label: "Durum", render: (row) => <Badge value={row.status} /> },
          {
            key: "act",
            label: "",
            render: (row) =>
              row.status === "Bitti" ? null : (
                <button onClick={() => run((current) => transitionMaintenance(current, row.id, session.name), "Bakım durumu güncellendi.")}>
                  {row.status === "Açık" ? "İşleme al" : "Bitir"}
                </button>
              ),
          },
        ]}
      />
      {open && (
        <Modal title="Bakım iş emri" onClose={() => setOpen(false)}>
          <div className="grid">
            <Field label="Varlık"><input value={form.asset} onChange={(event) => setForm({ ...form, asset: event.target.value })} /></Field>
            <Field label="Tür">
              <select value={form.kind} onChange={(event) => setForm({ ...form, kind: event.target.value })}>
                <option>Arıza</option>
                <option>Periyodik</option>
              </select>
            </Field>
            <Field label="Öncelik">
              <select value={form.priority} onChange={(event) => setForm({ ...form, priority: event.target.value })}>
                <option>Düşük</option>
                <option>Orta</option>
                <option>Yüksek</option>
              </select>
            </Field>
          </div>
          <footer>
            <button data-submit type="button" onClick={() => {
              const ok = run((current) => addMaintenance(current, { ...form, plant: session.plant, actor: session.name }), "İş emri açıldı.");
              if (ok) setOpen(false);
            }}>Kaydet</button>
          </footer>
        </Modal>
      )}
    </section>
  );
}

export function Finance({ session }) {
  const { state, run } = useStore();
  const rows = usePlant(session, state.invoices);
  const openAmount = rows.filter((row) => row.status === "Açık").reduce((sum, row) => sum + row.amount, 0);
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ party: "", amount: "" });
  return (
    <section>
      <div className="title-row">
        <div>
          <h1>Finans</h1>
          <p className="lead">Açık fatura tutarı {openAmount.toLocaleString("tr-TR")}.</p>
        </div>
        <button onClick={() => setOpen(true)}>Yeni fatura</button>
      </div>
      <Table
        rows={rows}
        columns={[
          { key: "id", label: "No" },
          { key: "party", label: "Cari" },
          { key: "amount", label: "Tutar", render: (row) => row.amount.toLocaleString("tr-TR") },
          { key: "status", label: "Durum", render: (row) => <Badge value={row.status} /> },
          {
            key: "act",
            label: "",
            render: (row) => row.status === "Açık" ? <button onClick={() => run((current) => payInvoice(current, row.id, session.name), "Fatura ödendi.")}>Öde</button> : null,
          },
        ]}
      />
      {open && (
        <Modal title="Fatura" onClose={() => setOpen(false)}>
          <div className="grid">
            <Field label="Cari"><input value={form.party} onChange={(event) => setForm({ ...form, party: event.target.value })} /></Field>
            <Field label="Tutar"><input value={form.amount} onChange={(event) => setForm({ ...form, amount: event.target.value })} /></Field>
          </div>
          <footer>
            <button data-submit type="button" onClick={() => {
              const ok = run((current) => addInvoice(current, { ...form, plant: session.plant, actor: session.name }), "Fatura açıldı.");
              if (ok) setOpen(false);
            }}>Kaydet</button>
          </footer>
        </Modal>
      )}
    </section>
  );
}

export function Master({ session }) {
  const { state, run } = useStore();
  const [kind, setKind] = useState("customers");
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ name: "", city: "" });
  const rows = usePlant(session, state[kind]);
  return (
    <section>
      <div className="title-row">
        <div>
          <h1>Ana veri</h1>
          <p className="lead">Müşteri ve tedarikçi kartları.</p>
        </div>
        <button onClick={() => setOpen(true)}>Yeni kart</button>
      </div>
      <div className="toolbar">
        <button className={kind === "customers" ? "on" : "ghost"} onClick={() => setKind("customers")}>Müşteriler</button>
        <button className={kind === "suppliers" ? "on" : "ghost"} onClick={() => setKind("suppliers")}>Tedarikçiler</button>
      </div>
      <Table rows={rows} columns={[{ key: "id", label: "No" }, { key: "name", label: "Ad" }, { key: "city", label: "Şehir" }, { key: "plant", label: "Tesis" }]} />
      {open && (
        <Modal title={kind === "customers" ? "Müşteri" : "Tedarikçi"} onClose={() => setOpen(false)}>
          <div className="grid">
            <Field label="Ad"><input value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} /></Field>
            <Field label="Şehir"><input value={form.city} onChange={(event) => setForm({ ...form, city: event.target.value })} /></Field>
          </div>
          <footer>
            <button data-submit type="button" onClick={() => {
              const ok = run(
                (current) => addParty(current, kind, kind === "customers" ? "CUS" : "SUP", { ...form, plant: session.plant, actor: session.name }),
                "Kart eklendi.",
              );
              if (ok) setOpen(false);
            }}>Kaydet</button>
          </footer>
        </Modal>
      )}
    </section>
  );
}

export function Analytics({ session }) {
  const { state } = useStore();
  const materials = usePlant(session, state.materials);
  const bars = stockByCategory(materials);
  const max = Math.max(1, ...bars.map((item) => item.qty));
  return (
    <section>
      <h1>Analitik</h1>
      <p className="lead">Kategori bazında eldeki miktar. Yeni stok hareketi bu çubukları değiştirir.</p>
      <div className="bars">
        {bars.map((item) => (
          <div key={item.category}>
            <span>{item.category}</span>
            <div className="bar"><i style={{ width: `${(item.qty / max) * 100}%` }} /></div>
            <strong>{item.qty}</strong>
          </div>
        ))}
        {bars.length === 0 && <p>Bu tesiste stok yok.</p>}
      </div>
    </section>
  );
}

export function Copilot({ session }) {
  const { state } = useStore();
  const [text, setText] = useState("düşük stok");
  const [answer, setAnswer] = useState("");
  return (
    <section>
      <h1>Copilot</h1>
      <p className="lead">Cevaplar bu oturumdaki stok, üretim, kalite ve bakım kayıtlarından gelir.</p>
      <form
        className="ask"
        onSubmit={(event) => {
          event.preventDefault();
          setAnswer(answerQuestion(state, session.allPlants ? null : session.plant, text));
        }}
      >
        <input value={text} onChange={(event) => setText(event.target.value)} />
        <button type="submit">Sor</button>
      </form>
      {answer && <pre className="answer">{answer}</pre>}
    </section>
  );
}

export function Twin({ session }) {
  const { state } = useStore();
  const materials = usePlant(session, state.materials);
  const zones = [
    { name: "Tomruk sahası", match: "Tomruk" },
    { name: "Kereste deposu", match: "Kereste" },
    { name: "Mamul depo", match: "Mamul" },
    { name: "Kimyasal depo", match: "Kimyasal" },
  ];
  return (
    <section>
      <h1>Dijital ikiz</h1>
      <p className="lead">{session.plant} yerleşimi, depo adlarındaki stoğa bağlı.</p>
      <div className="zones">
        {zones.map((zone) => {
          const rows = materials.filter((item) => item.warehouse.includes(zone.match));
          const qty = rows.reduce((sum, item) => sum + item.onHand, 0);
          return (
            <article key={zone.name}>
              <h2>{zone.name}</h2>
              <strong>{Math.round(qty)}</strong>
              <ul className="plain">
                {rows.map((row) => (
                  <li key={row.id}>{row.code} · {row.onHand} {row.unit}</li>
                ))}
                {rows.length === 0 && <li>Bu bölgede stok yok.</li>}
              </ul>
            </article>
          );
        })}
      </div>
    </section>
  );
}

export function Admin({ session }) {
  const { state, run } = useStore();
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState({ name: "", role: ROLES[0], plant: session.plant });
  return (
    <section>
      <div className="title-row">
        <div>
          <h1>Yönetim</h1>
          <p className="lead">Kullanıcı dizini ve son işlem kaydı.</p>
        </div>
        <button onClick={() => setOpen(true)}>Yeni kullanıcı</button>
      </div>
      <Table
        rows={state.users}
        columns={[
          { key: "name", label: "Ad" },
          { key: "role", label: "Rol" },
          { key: "plant", label: "Tesis" },
          { key: "active", label: "Durum", render: (row) => (row.active ? "Aktif" : "Pasif") },
          {
            key: "act",
            label: "",
            render: (row) => (
              <button className="ghost" onClick={() => run((current) => toggleUser(current, row.id, session.name), "Kullanıcı güncellendi.")}>
                {row.active ? "Pasifleştir" : "Aktifleştir"}
              </button>
            ),
          },
        ]}
      />
      <h2>İşlem kaydı</h2>
      <Table
        rows={state.audit.slice(0, 12)}
        columns={[
          { key: "at", label: "Zaman", render: (row) => new Date(row.at).toLocaleString("tr-TR") },
          { key: "actor", label: "Kim" },
          { key: "action", label: "İşlem" },
          { key: "detail", label: "Ayrıntı" },
        ]}
      />
      {open && (
        <Modal title="Kullanıcı" onClose={() => setOpen(false)}>
          <div className="grid">
            <Field label="Ad"><input value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} /></Field>
            <Field label="Rol">
              <select value={form.role} onChange={(event) => setForm({ ...form, role: event.target.value })}>
                {ROLES.map((item) => <option key={item}>{item}</option>)}
              </select>
            </Field>
            <Field label="Tesis">
              <select value={form.plant} onChange={(event) => setForm({ ...form, plant: event.target.value })}>
                {PLANTS.map((item) => <option key={item}>{item}</option>)}
              </select>
            </Field>
          </div>
          <footer>
            <button data-submit type="button" onClick={() => {
              const ok = run((current) => addUser(current, { ...form, actor: session.name }), "Kullanıcı eklendi.");
              if (ok) setOpen(false);
            }}>Kaydet</button>
          </footer>
        </Modal>
      )}
    </section>
  );
}
