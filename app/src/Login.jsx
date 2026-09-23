import { useState } from "react";
import { PLANTS } from "./logic.js";
import { ROLES } from "./labels.js";

export default function Login({ onEnter }) {
  const [name, setName] = useState("Merve");
  const [role, setRole] = useState(ROLES[0]);
  const [plant, setPlant] = useState(PLANTS[0]);

  return (
    <main className="login">
      <section>
        <div className="brand">
          <span className="mark">N</span>
          <div>
            <strong>Naswood OS</strong>
            <p>Ahşap üretim işletim sistemi</p>
          </div>
        </div>
        <form
          onSubmit={(event) => {
            event.preventDefault();
            if (!name.trim()) return;
            onEnter({ name: name.trim(), role, plant, allPlants: false });
          }}
        >
          <label>
            Kullanıcı
            <input value={name} onChange={(event) => setName(event.target.value)} />
          </label>
          <label>
            Rol
            <select value={role} onChange={(event) => setRole(event.target.value)}>
              {ROLES.map((item) => (
                <option key={item}>{item}</option>
              ))}
            </select>
          </label>
          <label>
            Tesis
            <select value={plant} onChange={(event) => setPlant(event.target.value)}>
              {PLANTS.map((item) => (
                <option key={item}>{item}</option>
              ))}
            </select>
          </label>
          <button type="submit">Giriş</button>
        </form>
      </section>
    </main>
  );
}
