import {
  findCatalogItem,
  isKnownWarehouseType,
  stockBalanceKey,
  WAREHOUSE_CATALOG,
  WAREHOUSE_TYPE_OPTIONS,
} from "./warehouseCatalog";

function assert(cond: unknown, msg: string): asserts cond {
  if (!cond) throw new Error(msg);
}

export function runWarehouseCatalogSelftest(): void {
  assert(WAREHOUSE_TYPE_OPTIONS.length === 14, "14 types");
  assert(WAREHOUSE_CATALOG.length === 14, "14 catalog WHs");
  assert(isKnownWarehouseType("RAW_MATERIAL"), "rm type");
  assert(isKnownWarehouseType("HARDWARE"), "hdw type");
  assert(isKnownWarehouseType("GENERAL_CONSUMABLE"), "off type");
  assert(!isKnownWarehouseType("FOO"), "unknown");

  const rm = findCatalogItem("WH-RM");
  assert(rm?.warehouseType === "RAW_MATERIAL", "WH-RM");
  assert(findCatalogItem("WH-HDW")?.warehouseType === "HARDWARE", "WH-HDW");
  assert(findCatalogItem("WH-QA")?.warehouseType === "QUARANTINE", "WH-QA");
  assert(findCatalogItem("WH-MRO")?.warehouseType === "MAINTENANCE", "WH-MRO");

  // Same material can sit in different warehouses — keys differ by WH only
  const a = stockBalanceKey({
    materialCode: "HDW-BOLT-001",
    lotNumber: "LOT-1",
    warehouseCode: "WH-HDW",
    locationCode: "A-01",
  });
  const b = stockBalanceKey({
    materialCode: "HDW-BOLT-001",
    lotNumber: "LOT-1",
    warehouseCode: "WH-MRO",
    locationCode: "A-01",
  });
  assert(a !== b, "same material different WH = different stock key");
  assert(a.startsWith("HDW-BOLT-001|LOT-1|WH-HDW") || a.includes("|HDW-BOLT-001|LOT-1|WH-HDW"), "key shape");
}

if (import.meta.url === `file://${process.argv[1]}`) {
  runWarehouseCatalogSelftest();
  console.log("warehouseCatalog.selftest: OK");
}
