import { canOpen } from "./labels.js";
import { available, isLow } from "./logic.js";

export default function Dashboard({ session, state, open }) {
  const plant = (rows) => rows.filter((row) => session.allPlants || row.plant === session.plant);
  const materials = plant(state.materials);
  const low = materials.filter(isLow);
  const onHand = materials.reduce((sum, row) => sum + row.onHand, 0);
  const cards = [
    { label: "Malzeme", value: materials.length, page: "inventory" },
    { label: "Düşük stok", value: low.length, page: "inventory" },
    { label: "Açık satınalma", value: plant(state.purchases).filter((row) => row.status !== "Teslim alındı").length, page: "purchasing" },
    { label: "Üretimde", value: plant(state.productions).filter((row) => row.status === "Üretimde" || row.status === "Serbest").length, page: "production" },
    { label: "Kalite bekleyen", value: plant(state.inspections).filter((row) => row.result === "Bekliyor").length, page: "quality" },
    { label: "Açık bakım", value: plant(state.maintenance).filter((row) => row.status !== "Bitti").length, page: "maintenance" },
  ];

  return (
    <section>
      <h1>Gösterge</h1>
      <p className="lead">
        {session.allPlants ? "Tüm tesisler" : `${session.plant} tesisi`} · eldeki toplam {Math.round(onHand)}
      </p>
      <div className="cards">
        {cards.filter((card) => canOpen(session.role, card.page)).map((card) => (
          <button key={card.label} className="card" onClick={() => open(card.page)}>
            <strong>{card.value}</strong>
            <span>{card.label}</span>
          </button>
        ))}
      </div>
      <div className="split">
        <article>
          <h2>Düşük stok</h2>
          {low.length === 0 && <p>Eşiğin altında malzeme yok.</p>}
          <ul className="plain">
            {low.map((row) => (
              <li key={row.id}>
                {row.code} {row.name} · kullanılabilir {available(row)} {row.unit} · eşik {row.reorderPoint}
              </li>
            ))}
          </ul>
        </article>
        <article>
          <h2>Son hareketler</h2>
          <ul className="plain">
            {plant(state.movements).slice(0, 6).map((row) => (
              <li key={row.id}>
                {row.materialCode} · {row.type} · {row.qty}
              </li>
            ))}
          </ul>
        </article>
      </div>
    </section>
  );
}
