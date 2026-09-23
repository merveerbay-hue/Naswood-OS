export const PLANTS = ["Gebze", "Düzce"];

export function round(n) {
  return Math.round(Number(n) * 1000) / 1000;
}

export function available(row) {
  return round((row.onHand || 0) - (row.reserved || 0));
}

export function isLow(row) {
  return available(row) < Number(row.reorderPoint || 0);
}

function nextId(list, prefix) {
  const max = (list || []).reduce((acc, row) => {
    const match = String(row.id || "").match(/(\d+)$/);
    return Math.max(acc, match ? Number(match[1]) : 0);
  }, 0);
  return `${prefix}-${String(max + 1).padStart(4, "0")}`;
}

function now() {
  return new Date().toISOString();
}

function audit(state, action, detail, actor) {
  const row = {
    id: nextId(state.audit, "AUD"),
    action,
    detail,
    actor: actor || "Sistem",
    at: now(),
  };
  return [row, ...state.audit].slice(0, 200);
}

function requireText(input, keys) {
  for (const key of keys) {
    if (!String(input[key] ?? "").trim()) {
      throw new Error("Zorunlu alan eksik.");
    }
  }
}

function findMaterial(state, id) {
  const row = state.materials.find((item) => item.id === id);
  if (!row) throw new Error("Malzeme bulunamadı.");
  return row;
}

function replaceMaterial(state, next) {
  return state.materials.map((item) => (item.id === next.id ? next : item));
}

function applyStock(material, deltaOnHand, deltaReserved) {
  const onHand = round(material.onHand + deltaOnHand);
  const reserved = round(material.reserved + deltaReserved);
  if (onHand < 0 || reserved < 0 || reserved > onHand + 0.0001) {
    throw new Error("Stok yetersiz.");
  }
  return { ...material, onHand, reserved };
}

function addMovement(state, material, type, qty, note, actor) {
  const row = {
    id: nextId(state.movements, "HRE"),
    materialId: material.id,
    materialCode: material.code,
    type,
    qty: round(qty),
    note: note || "",
    plant: material.plant,
    actor: actor || "Sistem",
    at: now(),
  };
  return [row, ...state.movements];
}

export function seedState() {
  const materials = [
    { id: "INV-1001", code: "TOM-MES-001", name: "Meşe tomruk", category: "Ham tomruk", unit: "m³", warehouse: "Tomruk sahası", location: "A-01", batch: "B-2401", onHand: 128, reserved: 20, reorderPoint: 40, status: "Uygun", plant: "Gebze" },
    { id: "INV-1002", code: "KER-KAY-014", name: "Kayın kereste", category: "Kereste", unit: "m³", warehouse: "Kereste deposu", location: "B-12", batch: "B-2408", onHand: 18, reserved: 6, reorderPoint: 25, status: "Uygun", plant: "Gebze" },
    { id: "INV-1003", code: "THW-CAM-007", name: "Thermowood çam", category: "Thermowood", unit: "m³", warehouse: "Mamul depo", location: "C-03", batch: "B-2412", onHand: 42, reserved: 0, reorderPoint: 15, status: "Uygun", plant: "Gebze" },
    { id: "INV-1004", code: "LAM-MES-003", name: "Meşe lamel", category: "Lamel", unit: "m²", warehouse: "Mamul depo", location: "C-11", batch: "B-2415", onHand: 860, reserved: 120, reorderPoint: 200, status: "Uygun", plant: "Gebze" },
    { id: "INV-1005", code: "TUT-D4-001", name: "D4 tutkal", category: "Sarf", unit: "kg", warehouse: "Kimyasal depo", location: "K-02", batch: "B-2390", onHand: 64, reserved: 0, reorderPoint: 50, status: "Karantina", plant: "Gebze" },
  ];
  return {
    materials,
    movements: [
      { id: "HRE-1001", materialId: "INV-1001", materialCode: "TOM-MES-001", type: "Giriş", qty: 128, note: "Açılış", plant: "Gebze", actor: "Sistem", at: "2026-09-01T08:00:00.000Z" },
      { id: "HRE-1002", materialId: "INV-1001", materialCode: "TOM-MES-001", type: "Rezerv", qty: 20, note: "Üretim rezervi", plant: "Gebze", actor: "Sistem", at: "2026-09-12T08:00:00.000Z" },
    ],
    purchases: [
      { id: "PO-3001", supplier: "Karadeniz Orman", materialId: "INV-1001", qty: 40, status: "Sipariş", plant: "Gebze", actor: "Sistem", at: "2026-09-18T09:00:00.000Z" },
      { id: "PO-3002", supplier: "Baltık Timber", materialId: "INV-1002", qty: 30, status: "Taslak", plant: "Gebze", actor: "Sistem", at: "2026-09-20T09:00:00.000Z" },
    ],
    productions: [
      { id: "UO-5001", product: "Meşe panel", materialId: "INV-1004", qty: 120, status: "Üretimde", plant: "Gebze", actor: "Sistem", at: "2026-09-19T07:30:00.000Z" },
      { id: "UO-5002", product: "Thermowood deck", materialId: "INV-1003", qty: 8, status: "Planlandı", plant: "Gebze", actor: "Sistem", at: "2026-09-21T07:30:00.000Z" },
    ],
    inspections: [
      { id: "KK-7001", materialId: "INV-1005", kind: "Giriş kalite", result: "Bekliyor", note: "Tedarik partisi", plant: "Gebze", actor: "Sistem", at: "2026-09-22T10:00:00.000Z" },
      { id: "KK-7002", materialId: "INV-1002", kind: "Proses", result: "Geçti", note: "Nem uygun", plant: "Gebze", actor: "Sistem", at: "2026-09-15T10:00:00.000Z" },
    ],
    maintenance: [
      { id: "BK-8001", asset: "Kurutma fırını 2", kind: "Arıza", priority: "Yüksek", status: "Açık", plant: "Gebze", actor: "Sistem", at: "2026-09-22T06:00:00.000Z" },
      { id: "BK-8002", asset: "Ebatlama hattı", kind: "Periyodik", priority: "Orta", status: "İşlemde", plant: "Gebze", actor: "Sistem", at: "2026-09-20T06:00:00.000Z" },
    ],
    sales: [
      { id: "SO-4001", customer: "Nordic Floor", materialId: "INV-1004", qty: 300, status: "Onaylı", plant: "Gebze", actor: "Sistem", at: "2026-09-17T11:00:00.000Z" },
    ],
    customers: [
      { id: "CUS-2001", name: "Nordic Floor", city: "Helsinki", plant: "Gebze" },
      { id: "CUS-2002", name: "Ege Parke", city: "İzmir", plant: "Gebze" },
    ],
    suppliers: [
      { id: "SUP-2001", name: "Karadeniz Orman", city: "Trabzon", plant: "Gebze" },
      { id: "SUP-2002", name: "Baltık Timber", city: "Riga", plant: "Gebze" },
    ],
    invoices: [
      { id: "FAT-6001", party: "Nordic Floor", amount: 186000, status: "Açık", plant: "Gebze" },
      { id: "FAT-6002", party: "Karadeniz Orman", amount: 42000, status: "Ödendi", plant: "Gebze" },
    ],
    users: [
      { id: "USR-0001", name: "Merve", role: "Fabrika müdürü", plant: "Gebze", active: true },
      { id: "USR-0002", name: "Kerem", role: "Depo sorumlusu", plant: "Gebze", active: true },
      { id: "USR-0003", name: "Selin", role: "Kalite", plant: "Düzce", active: true },
    ],
    audit: [
      { id: "AUD-0001", action: "Açılış", detail: "Örnek fabrika verisi yüklendi", actor: "Sistem", at: "2026-09-01T08:00:00.000Z" },
    ],
  };
}

export function addMaterial(state, input) {
  requireText(input, ["code", "name", "category", "unit", "warehouse", "plant"]);
  const code = input.code.trim();
  if (state.materials.some((item) => item.code === code && item.plant === input.plant)) {
    throw new Error("Bu malzeme kodu bu tesiste var.");
  }
  const onHand = round(input.onHand || 0);
  if (onHand < 0) throw new Error("Miktar negatif olamaz.");
  const row = {
    id: nextId(state.materials, "INV"),
    code,
    name: input.name.trim(),
    category: input.category,
    unit: input.unit,
    warehouse: input.warehouse.trim(),
    location: String(input.location || "").trim(),
    batch: String(input.batch || "").trim(),
    onHand,
    reserved: 0,
    reorderPoint: round(input.reorderPoint || 0),
    status: "Uygun",
    plant: input.plant,
  };
  let movements = state.movements;
  if (onHand > 0) movements = addMovement({ ...state, movements }, row, "Giriş", onHand, "Açılış bakiyesi", input.actor);
  return {
    ...state,
    materials: [row, ...state.materials],
    movements,
    audit: audit(state, "Malzeme", `${row.code} eklendi`, input.actor),
  };
}

export function updateMaterial(state, id, patch, actor) {
  const current = findMaterial(state, id);
  const next = {
    ...current,
    name: String(patch.name ?? current.name).trim(),
    category: patch.category ?? current.category,
    unit: patch.unit ?? current.unit,
    warehouse: String(patch.warehouse ?? current.warehouse).trim(),
    location: String(patch.location ?? current.location).trim(),
    batch: String(patch.batch ?? current.batch).trim(),
    reorderPoint: round(patch.reorderPoint ?? current.reorderPoint),
  };
  if (!next.name || !next.warehouse) throw new Error("Zorunlu alan eksik.");
  return {
    ...state,
    materials: replaceMaterial(state, next),
    audit: audit(state, "Malzeme", `${current.code} güncellendi`, actor),
  };
}

export function moveStock(state, input) {
  requireText(input, ["materialId", "type"]);
  const qty = round(input.qty);
  if (!qty || qty <= 0) throw new Error("Miktar sıfırdan büyük olmalı.");
  const material = findMaterial(state, input.materialId);
  if (material.status === "Bloke" && input.type === "Çıkış") {
    throw new Error("Bloke stok çıkamaz.");
  }
  let next = material;
  if (input.type === "Giriş") next = applyStock(material, qty, 0);
  else if (input.type === "Çıkış") {
    if (qty > available(material)) throw new Error("Kullanılabilir stok yetersiz.");
    next = applyStock(material, -qty, 0);
  } else {
    throw new Error("Hareket tipi geçersiz.");
  }
  return {
    ...state,
    materials: replaceMaterial(state, next),
    movements: addMovement(state, next, input.type, qty, input.note, input.actor),
    audit: audit(state, "Stok", `${material.code} ${input.type} ${qty}`, input.actor),
  };
}

export function addPurchase(state, input) {
  requireText(input, ["supplier", "materialId", "plant"]);
  const qty = round(input.qty);
  if (!qty || qty <= 0) throw new Error("Miktar sıfırdan büyük olmalı.");
  findMaterial(state, input.materialId);
  const row = {
    id: nextId(state.purchases, "PO"),
    supplier: input.supplier.trim(),
    materialId: input.materialId,
    qty,
    status: "Taslak",
    plant: input.plant,
    actor: input.actor || "Sistem",
    at: now(),
  };
  return {
    ...state,
    purchases: [row, ...state.purchases],
    audit: audit(state, "Satınalma", `${row.id} taslak`, input.actor),
  };
}

export function orderPurchase(state, id, actor) {
  const row = state.purchases.find((item) => item.id === id);
  if (!row) throw new Error("Sipariş bulunamadı.");
  if (row.status !== "Taslak") throw new Error("Yalnızca taslak sipariş onaylanır.");
  return {
    ...state,
    purchases: state.purchases.map((item) => (item.id === id ? { ...item, status: "Sipariş" } : item)),
    audit: audit(state, "Satınalma", `${id} sipariş edildi`, actor),
  };
}

export function receivePurchase(state, id, actor) {
  const row = state.purchases.find((item) => item.id === id);
  if (!row) throw new Error("Sipariş bulunamadı.");
  if (row.status !== "Sipariş") throw new Error("Yalnızca sipariş durumundaki kayıt teslim alınır.");
  const material = findMaterial(state, row.materialId);
  const next = applyStock(material, row.qty, 0);
  return {
    ...state,
    materials: replaceMaterial(state, next),
    purchases: state.purchases.map((item) => (item.id === id ? { ...item, status: "Teslim alındı" } : item)),
    movements: addMovement(state, next, "Giriş", row.qty, `${id} mal kabul`, actor),
    audit: audit(state, "Mal kabul", `${id} ${material.code} +${row.qty}`, actor),
  };
}

export function addProduction(state, input) {
  requireText(input, ["product", "materialId", "plant"]);
  const qty = round(input.qty);
  if (!qty || qty <= 0) throw new Error("Miktar sıfırdan büyük olmalı.");
  findMaterial(state, input.materialId);
  const row = {
    id: nextId(state.productions, "UO"),
    product: input.product.trim(),
    materialId: input.materialId,
    qty,
    status: "Planlandı",
    plant: input.plant,
    actor: input.actor || "Sistem",
    at: now(),
  };
  return {
    ...state,
    productions: [row, ...state.productions],
    audit: audit(state, "Üretim", `${row.id} planlandı`, input.actor),
  };
}

export function transitionProduction(state, id, nextStatus, actor) {
  const order = state.productions.find((item) => item.id === id);
  if (!order) throw new Error("Üretim emri bulunamadı.");
  const material = findMaterial(state, order.materialId);
  let nextMaterial = material;
  let movements = state.movements;
  const allowed = {
    Planlandı: ["Serbest", "İptal"],
    Serbest: ["Üretimde", "İptal"],
    Üretimde: ["Tamamlandı"],
  };
  if (!(allowed[order.status] || []).includes(nextStatus)) {
    throw new Error("Bu durum geçişi yapılamaz.");
  }
  if (nextStatus === "Serbest") {
    if (material.status === "Bloke") throw new Error("Bloke malzeme rezerve edilemez.");
    if (order.qty > available(material)) throw new Error("Rezerv için kullanılabilir stok yetersiz.");
    nextMaterial = applyStock(material, 0, order.qty);
    movements = addMovement(state, nextMaterial, "Rezerv", order.qty, id, actor);
  }
  if (nextStatus === "İptal" && order.status === "Serbest") {
    nextMaterial = applyStock(material, 0, -order.qty);
    movements = addMovement(state, nextMaterial, "Rezerv çöz", order.qty, id, actor);
  }
  if (nextStatus === "Tamamlandı") {
    nextMaterial = applyStock(material, -order.qty, -order.qty);
    movements = addMovement(state, nextMaterial, "Tüketim", order.qty, id, actor);
  }
  return {
    ...state,
    materials: replaceMaterial(state, nextMaterial),
    movements,
    productions: state.productions.map((item) => (item.id === id ? { ...item, status: nextStatus } : item)),
    audit: audit(state, "Üretim", `${id} ${nextStatus}`, actor),
  };
}

export function addInspection(state, input) {
  requireText(input, ["materialId", "kind", "plant"]);
  findMaterial(state, input.materialId);
  const row = {
    id: nextId(state.inspections, "KK"),
    materialId: input.materialId,
    kind: input.kind,
    result: "Bekliyor",
    note: String(input.note || "").trim(),
    plant: input.plant,
    actor: input.actor || "Sistem",
    at: now(),
  };
  return {
    ...state,
    inspections: [row, ...state.inspections],
    audit: audit(state, "Kalite", `${row.id} açıldı`, input.actor),
  };
}

export function decideInspection(state, id, result, actor) {
  if (result !== "Geçti" && result !== "Kaldı") throw new Error("Karar geçersiz.");
  const row = state.inspections.find((item) => item.id === id);
  if (!row) throw new Error("Kontrol bulunamadı.");
  if (row.result !== "Bekliyor") throw new Error("Bu kontrol sonuçlanmış.");
  const material = findMaterial(state, row.materialId);
  const status = result === "Kaldı" ? "Bloke" : "Uygun";
  return {
    ...state,
    inspections: state.inspections.map((item) => (item.id === id ? { ...item, result } : item)),
    materials: replaceMaterial(state, { ...material, status }),
    audit: audit(state, "Kalite", `${id} ${result}`, actor),
  };
}

export function addMaintenance(state, input) {
  requireText(input, ["asset", "kind", "priority", "plant"]);
  const row = {
    id: nextId(state.maintenance, "BK"),
    asset: input.asset.trim(),
    kind: input.kind,
    priority: input.priority,
    status: "Açık",
    plant: input.plant,
    actor: input.actor || "Sistem",
    at: now(),
  };
  return {
    ...state,
    maintenance: [row, ...state.maintenance],
    audit: audit(state, "Bakım", `${row.id} açıldı`, input.actor),
  };
}

export function transitionMaintenance(state, id, actor) {
  const row = state.maintenance.find((item) => item.id === id);
  if (!row) throw new Error("Bakım kaydı bulunamadı.");
  const next = row.status === "Açık" ? "İşlemde" : row.status === "İşlemde" ? "Bitti" : null;
  if (!next) throw new Error("Bu iş emri kapanmış.");
  return {
    ...state,
    maintenance: state.maintenance.map((item) => (item.id === id ? { ...item, status: next } : item)),
    audit: audit(state, "Bakım", `${id} ${next}`, actor),
  };
}

export function addSale(state, input) {
  requireText(input, ["customer", "materialId", "plant"]);
  const qty = round(input.qty);
  if (!qty || qty <= 0) throw new Error("Miktar sıfırdan büyük olmalı.");
  findMaterial(state, input.materialId);
  const row = {
    id: nextId(state.sales, "SO"),
    customer: input.customer.trim(),
    materialId: input.materialId,
    qty,
    status: "Taslak",
    plant: input.plant,
    actor: input.actor || "Sistem",
    at: now(),
  };
  return {
    ...state,
    sales: [row, ...state.sales],
    audit: audit(state, "Satış", `${row.id} taslak`, input.actor),
  };
}

export function transitionSale(state, id, actor) {
  const row = state.sales.find((item) => item.id === id);
  if (!row) throw new Error("Satış siparişi bulunamadı.");
  if (row.status === "Taslak") {
    return {
      ...state,
      sales: state.sales.map((item) => (item.id === id ? { ...item, status: "Onaylı" } : item)),
      audit: audit(state, "Satış", `${id} onaylandı`, actor),
    };
  }
  if (row.status !== "Onaylı") throw new Error("Bu sipariş sevk edilemez.");
  const material = findMaterial(state, row.materialId);
  if (material.status === "Bloke") throw new Error("Bloke stok sevk edilemez.");
  if (row.qty > available(material)) throw new Error("Sevk için kullanılabilir stok yetersiz.");
  const next = applyStock(material, -row.qty, 0);
  return {
    ...state,
    materials: replaceMaterial(state, next),
    movements: addMovement(state, next, "Çıkış", row.qty, `${id} sevkiyat`, actor),
    sales: state.sales.map((item) => (item.id === id ? { ...item, status: "Sevk edildi" } : item)),
    audit: audit(state, "Sevkiyat", `${id} ${material.code} -${row.qty}`, actor),
  };
}

export function addParty(state, key, prefix, input) {
  requireText(input, ["name", "city", "plant"]);
  const row = {
    id: nextId(state[key], prefix),
    name: input.name.trim(),
    city: input.city.trim(),
    plant: input.plant,
  };
  return {
    ...state,
    [key]: [row, ...state[key]],
    audit: audit(state, "Ana veri", `${row.name} eklendi`, input.actor),
  };
}

export function addInvoice(state, input) {
  requireText(input, ["party", "plant"]);
  const amount = round(input.amount);
  if (!amount || amount <= 0) throw new Error("Tutar sıfırdan büyük olmalı.");
  const row = {
    id: nextId(state.invoices, "FAT"),
    party: input.party.trim(),
    amount,
    status: "Açık",
    plant: input.plant,
  };
  return {
    ...state,
    invoices: [row, ...state.invoices],
    audit: audit(state, "Finans", `${row.id} açıldı`, input.actor),
  };
}

export function payInvoice(state, id, actor) {
  const row = state.invoices.find((item) => item.id === id);
  if (!row) throw new Error("Fatura bulunamadı.");
  if (row.status !== "Açık") throw new Error("Fatura zaten kapalı.");
  return {
    ...state,
    invoices: state.invoices.map((item) => (item.id === id ? { ...item, status: "Ödendi" } : item)),
    audit: audit(state, "Finans", `${id} ödendi`, actor),
  };
}

export function addUser(state, input) {
  requireText(input, ["name", "role", "plant"]);
  const row = {
    id: nextId(state.users, "USR"),
    name: input.name.trim(),
    role: input.role,
    plant: input.plant,
    active: true,
  };
  return {
    ...state,
    users: [row, ...state.users],
    audit: audit(state, "Kullanıcı", `${row.name} eklendi`, input.actor),
  };
}

export function toggleUser(state, id, actor) {
  const row = state.users.find((item) => item.id === id);
  if (!row) throw new Error("Kullanıcı bulunamadı.");
  return {
    ...state,
    users: state.users.map((item) => (item.id === id ? { ...item, active: !item.active } : item)),
    audit: audit(state, "Kullanıcı", `${row.name} ${row.active ? "pasif" : "aktif"}`, actor),
  };
}

export function stockByCategory(materials) {
  const map = new Map();
  for (const row of materials) {
    map.set(row.category, round((map.get(row.category) || 0) + row.onHand));
  }
  return [...map.entries()].map(([category, qty]) => ({ category, qty }));
}

export function answerQuestion(state, plant, text) {
  const q = String(text || "").toLocaleLowerCase("tr");
  const inPlant = (item) => !plant || item.plant === plant;
  const rows = state.materials.filter(inPlant);
  if (!q.trim()) return "Stok, üretim, kalite veya bakım diye sorun.";
  if (q.includes("düşük") || q.includes("stok")) {
    const low = rows.filter(isLow);
    const scope = plant || "tüm tesisler";
    if (!low.length) return `${scope} için yeniden sipariş noktasının altında malzeme yok.`;
    return low.map((item) => `${item.code} ${item.name}: kullanılabilir ${available(item)} ${item.unit}, eşik ${item.reorderPoint}`).join("\n");
  }
  if (q.includes("üretim") || q.includes("uretim")) {
    const open = state.productions.filter((item) => inPlant(item) && item.status !== "Tamamlandı" && item.status !== "İptal");
    if (!open.length) return "Açık üretim emri yok.";
    return open.map((item) => `${item.id} ${item.product} · ${item.status} · ${item.qty}`).join("\n");
  }
  if (q.includes("kalite")) {
    const waiting = state.inspections.filter((item) => inPlant(item) && item.result === "Bekliyor");
    if (!waiting.length) return "Bekleyen kalite kontrolü yok.";
    return waiting.map((item) => `${item.id} ${item.kind}`).join("\n");
  }
  if (q.includes("bakım") || q.includes("bakim")) {
    const open = state.maintenance.filter((item) => inPlant(item) && item.status !== "Bitti");
    if (!open.length) return "Açık bakım işi yok.";
    return open.map((item) => `${item.id} ${item.asset} · ${item.status} · ${item.priority}`).join("\n");
  }
  return "Bunu stok, üretim, kalite veya bakım kayıtlarından cevaplayabilirim.";
}
