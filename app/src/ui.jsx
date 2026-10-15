export function Badge({ value }) {
  const tone =
    value === "Uygun" || value === "Geçti" || value === "Ödendi" || value === "Teslim alındı" || value === "Tamamlandı" || value === "Sevk edildi" || value === "Bitti"
      ? "ok"
      : value === "Bloke" || value === "Kaldı" || value === "Yüksek"
        ? "bad"
        : value === "Karantina" || value === "Taslak" || value === "Bekliyor" || value === "Açık"
          ? "warn"
          : "info";
  return <span className={`badge ${tone}`}>{value}</span>;
}

export function Modal({ title, onClose, children }) {
  return (
    <div className="modal-back" onMouseDown={onClose}>
      <form
        className="modal"
        onMouseDown={(event) => event.stopPropagation()}
        onSubmit={(event) => {
          event.preventDefault();
          event.currentTarget.querySelector("[data-submit]")?.click();
        }}
      >
        <header>
          <h2>{title}</h2>
          <button type="button" className="ghost" onClick={onClose}>
            Kapat
          </button>
        </header>
        {children}
      </form>
    </div>
  );
}

export function Field({ label, children }) {
  return (
    <label className="field">
      <span>{label}</span>
      {children}
    </label>
  );
}

export function Table({ columns, rows, empty }) {
  return (
    <div className="table-wrap">
      <table>
        <thead>
          <tr>
            {columns.map((column) => (
              <th key={column.key}>{column.label}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.length === 0 ? (
            <tr>
              <td colSpan={columns.length} className="empty">
                {empty || "Kayıt yok."}
              </td>
            </tr>
          ) : (
            rows.map((row) => (
              <tr key={row.id}>
                {columns.map((column) => (
                  <td key={column.key}>{column.render ? column.render(row) : row[column.key]}</td>
                ))}
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
