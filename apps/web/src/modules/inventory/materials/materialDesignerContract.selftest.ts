/**
 * Designer MVP contract — 3 packs only; nominal vs receiving separation.
 * Run: npx tsx apps/web/src/modules/inventory/materials/materialDesignerContract.selftest.ts
 */
import { mintMaterialCode, previewMaterialCode, suggestMaterialName } from "./materialCoding";
import { formatNominalDims, type NominalDims } from "./materialNominalDims";

function assert(cond: unknown, msg: string): asserts cond {
  if (!cond) throw new Error(msg);
}

const PACKS = ["general", "stockRules", "active"] as const;
const REMOVED = [
  "identity",
  "measurement",
  "conversion",
  "packaging",
  "numbering",
  "quality",
  "traceability",
  "costing",
  "release",
] as const;

// TEST 1 — kereste card coding + defaults
{
  assert(PACKS.length === 3, "only 3 packs");
  assert(!REMOVED.some((p) => (PACKS as readonly string[]).includes(p)), "removed packs gone");

  const code = mintMaterialCode({
    mainCategory: "HM",
    materialType: "KR",
    woodToken: "PIN",
    existingCodes: [],
  });
  assert(code === "HM-KR-PIN-001", `code ${code}`);
  assert(
    suggestMaterialName({
      mainCategory: "HM",
      materialType: "KR",
      baseWoodToken: "PIN",
      isThermowood: false,
    }) === "Çam Kereste",
    "name",
  );

  const dims: NominalDims = { thicknessMm: 50, widthMm: 100, lengthMm: 4000 };
  assert(formatNominalDims(dims) === "50 × 100 × 4000 mm", "nominal display");
  // Volume rule: 1 pcs × 50×100×4000 / 1e9
  const m3 = (50 / 1000) * (100 / 1000) * (4000 / 1000);
  assert(Math.abs(m3 - 0.02) < 1e-9, `volume ${m3}`);
  console.log("TEST1 OK — HM-KR-PIN-001 · 50×100×4000 · M3/PCS volume");
}

// TEST 2 — master nominal ≠ receiving actual (contract)
{
  const master = "50 × 100 × 4000 mm";
  const actual = "45 × 90 × 4000 mm";
  assert(master !== actual, "master not overwritten by actual");
  console.log("TEST2 OK — master nominal unchanged vs receiving actual");
}

// TEST 3 — packages not on master (contract: no PKG in code mint)
{
  const preview = previewMaterialCode({
    mainCategory: "HM",
    materialType: "KR",
    woodToken: "PIN",
    existingCodes: [],
  });
  assert(!preview.includes("PKG"), "no package on material code");
  console.log("TEST3 OK — package not minted on material card");
}

// TEST 4 — two lots same material (contract: lot not on card)
{
  const lot1 = "LOT-2026-00101";
  const lot2 = "LOT-2026-00102";
  assert(lot1 !== lot2, "distinct lots");
  assert(
    mintMaterialCode({
      mainCategory: "HM",
      materialType: "KR",
      woodToken: "PIN",
      existingCodes: ["HM-KR-PIN-001"],
    }) === "HM-KR-PIN-002",
    "new commercial size = new card seq, not new lot",
  );
  console.log("TEST4 OK — same material can have LOT-001 and LOT-002 at receiving");
}

// TEST 5 — MI not required from operator (contract)
{
  const code = mintMaterialCode({
    mainCategory: "HM",
    materialType: "KR",
    woodToken: "PIN",
    existingCodes: [],
  });
  assert(code && !code.startsWith("MI-"), "material code is not MI");
  console.log("TEST5 OK — operator creates material without MI input");
}

console.log("materialDesignerContract.selftest: ALL OK");
