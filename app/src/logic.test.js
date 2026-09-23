import test from "node:test";
import assert from "node:assert/strict";
import {
  seedState,
  addMaterial,
  moveStock,
  orderPurchase,
  receivePurchase,
  addProduction,
  transitionProduction,
  decideInspection,
  transitionSale,
  available,
} from "./logic.js";

test("mal kabul stoku artırır", () => {
  const start = seedState();
  const before = start.materials.find((item) => item.id === "INV-1001").onHand;
  const next = receivePurchase(start, "PO-3001", "Merve");
  const after = next.materials.find((item) => item.id === "INV-1001");
  assert.equal(after.onHand, before + 40);
  assert.equal(next.purchases.find((item) => item.id === "PO-3001").status, "Teslim alındı");
});

test("taslak sipariş doğrudan teslim alınmaz", () => {
  assert.throws(() => receivePurchase(seedState(), "PO-3002", "Merve"), /sipariş durum/);
});

test("taslak onaylanıp teslim alınır", () => {
  let state = orderPurchase(seedState(), "PO-3002", "Merve");
  const before = state.materials.find((item) => item.id === "INV-1002").onHand;
  state = receivePurchase(state, "PO-3002", "Merve");
  assert.equal(state.materials.find((item) => item.id === "INV-1002").onHand, before + 30);
});

test("çıkış kullanılabilir stoğu aşamaz ve rezervi bozmaz", () => {
  const state = seedState();
  const row = state.materials.find((item) => item.id === "INV-1004");
  assert.equal(available(row), 740);
  assert.throws(() => moveStock(state, { materialId: "INV-1004", type: "Çıkış", qty: 741, actor: "Merve" }), /yetersiz/);
  const next = moveStock(state, { materialId: "INV-1004", type: "Çıkış", qty: 40, actor: "Merve" });
  const updated = next.materials.find((item) => item.id === "INV-1004");
  assert.equal(updated.onHand, 820);
  assert.equal(updated.reserved, 120);
});

test("üretim emri rezerve eder ve tamamlayınca tüketir", () => {
  let state = addProduction(seedState(), {
    product: "Çam deck",
    materialId: "INV-1003",
    qty: 10,
    plant: "Gebze",
    actor: "Merve",
  });
  const id = state.productions[0].id;
  state = transitionProduction(state, id, "Serbest", "Merve");
  assert.equal(state.materials.find((item) => item.id === "INV-1003").reserved, 10);
  state = transitionProduction(state, id, "Üretimde", "Merve");
  state = transitionProduction(state, id, "Tamamlandı", "Merve");
  const material = state.materials.find((item) => item.id === "INV-1003");
  assert.equal(material.onHand, 32);
  assert.equal(material.reserved, 0);
});

test("serbest emir iptal edilince rezerv çözülür", () => {
  let state = transitionProduction(seedState(), "UO-5002", "Serbest", "Merve");
  assert.equal(state.materials.find((item) => item.id === "INV-1003").reserved, 8);
  state = transitionProduction(state, "UO-5002", "İptal", "Merve");
  assert.equal(state.materials.find((item) => item.id === "INV-1003").reserved, 0);
  assert.equal(state.productions.find((item) => item.id === "UO-5002").status, "İptal");
});

test("kalite kalırsa stok bloke olur ve çıkış durur", () => {
  let state = decideInspection(seedState(), "KK-7001", "Kaldı", "Merve");
  assert.equal(state.materials.find((item) => item.id === "INV-1005").status, "Bloke");
  assert.throws(() => moveStock(state, { materialId: "INV-1005", type: "Çıkış", qty: 1, actor: "Merve" }), /Bloke/);
});

test("sevkiyat stoğu düşürür", () => {
  const state = transitionSale(seedState(), "SO-4001", "Merve");
  const material = state.materials.find((item) => item.id === "INV-1004");
  assert.equal(material.onHand, 560);
  assert.equal(material.reserved, 120);
  assert.equal(state.sales[0].status, "Sevk edildi");
});

test("aynı kod aynı tesiste tekrar açılamaz", () => {
  assert.throws(
    () =>
      addMaterial(seedState(), {
        code: "TOM-MES-001",
        name: "Kopya",
        category: "Ham tomruk",
        unit: "m³",
        warehouse: "Tomruk sahası",
        plant: "Gebze",
        onHand: 1,
      }),
    /malzeme kodu/,
  );
});
