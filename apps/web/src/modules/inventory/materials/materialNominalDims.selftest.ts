import {
  buildDuplicateFingerprint,
  buildDefinitionWithNominal,
  findDuplicateMaterial,
  formatNominalDims,
  parseDefinitionNominal,
  parseMasterOlcuToNominal,
} from "./materialNominalDims";

function assert(cond: unknown, msg: string): asserts cond {
  if (!cond) throw new Error(msg);
}

export function runMaterialNominalDimsSelftest(): void {
  const kereste = parseMasterOlcuToNominal("50x100x4000");
  assert(kereste?.thicknessMm === 50, "t");
  assert(kereste?.widthMm === 100, "w");
  assert(kereste?.lengthMm === 4000, "l");
  assert(formatNominalDims(kereste!) === "50 × 100 × 4000 mm", "fmt kereste");

  const panel = parseMasterOlcuToNominal("18 mm / 1200-1220 mm");
  assert(panel?.thicknessMm === 18, "panel t");
  assert(panel?.widthMinMm === 1200 && panel?.widthMaxMm === 1220, "panel range");
  assert(panel?.widthMm == null, "panel no single width");
  assert(!panel?.lengthMm, "panel no forced length");
  assert(formatNominalDims(panel!) === "18 × 1200–1220 mm", "fmt panel");

  const singleW = parseMasterOlcuToNominal("18 mm / 1220 mm");
  assert(singleW?.widthMm === 1220, "single width stays single");
  assert(singleW?.widthMinMm == null, "no forced min");

  const tw = parseMasterOlcuToNominal("16 mm / 92-117-138 mm");
  assert(tw?.widthOptionsMm?.join(",") === "92,117,138", "tw options");
  assert(formatNominalDims(tw!) === "16 × 92/117/138 mm", "fmt tw");

  const slash = parseMasterOlcuToNominal("26 mm / 68/92/118/140/166 mm");
  assert(slash?.widthOptionsMm?.length === 5, "slash options");

  const fp1 = buildDuplicateFingerprint({
    mainCategory: "HM",
    materialType: "KR",
    woodSpecies: "PIN",
    productType: "",
    quality: "A",
    dims: kereste!,
  });
  const fp2 = buildDuplicateFingerprint({
    mainCategory: "HM",
    materialType: "KR",
    woodSpecies: "PIN",
    productType: "",
    quality: "A",
    dims: kereste!,
  });
  assert(fp1.thicknessMm === fp2.thicknessMm && fp1.widthMm === fp2.widthMm, "dup stable");

  const wider = parseMasterOlcuToNominal("50x120x4000")!;
  const fp3 = buildDuplicateFingerprint({
    mainCategory: "HM",
    materialType: "KR",
    woodSpecies: "PIN",
    productType: "",
    quality: "A",
    dims: wider,
  });
  assert(fp1.widthMm !== fp3.widthMm, "different width different card");

  const merged = buildDefinitionWithNominal(
    { mainCategory: "HM", materialType: "KR", species: "PIN", grade: "A" },
    kereste!,
  );
  const read = parseDefinitionNominal(JSON.stringify(merged));
  assert(read?.thicknessMm === 50 && read?.widthMm === 100 && read?.lengthMm === 4000, "roundtrip");

  const dup = findDuplicateMaterial(
    [
      {
        code: "HM-KR-PIN-001",
        category: "HM",
        name: "Çam Kereste",
        definitionJson: JSON.stringify(merged),
      },
    ],
    fp1,
  );
  assert(dup?.code === "HM-KR-PIN-001", "find duplicate");
}

if (import.meta.url === `file://${process.argv[1]}`) {
  runMaterialNominalDimsSelftest();
  console.log("materialNominalDims.selftest: OK");
}
