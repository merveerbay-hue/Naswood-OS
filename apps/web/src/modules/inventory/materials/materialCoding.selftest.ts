import {
  buildCodePrefix,
  mintMaterialCode,
  previewMaterialCode,
  type MaterialCodingInput,
} from "./materialCoding";

function assert(cond: unknown, msg: string): asserts cond {
  if (!cond) throw new Error(msg);
}

export function runMaterialCodingSelftest(): void {
  const hm: MaterialCodingInput = {
    mainCategory: "HM",
    materialType: "KR",
    woodToken: "PIN",
    existingCodes: [],
  };
  assert(buildCodePrefix(hm) === "HM-KR-PIN", "hm prefix");
  assert(previewMaterialCode(hm) === "HM-KR-PIN-001", "hm preview empty");
  assert(mintMaterialCode(hm) === "HM-KR-PIN-001", "hm mint");

  const next = mintMaterialCode({
    ...hm,
    existingCodes: ["HM-KR-PIN-001", "HM-KR-PIN-003"],
  });
  assert(next === "HM-KR-PIN-004", `hm next got ${next}`);

  const ym: MaterialCodingInput = {
    mainCategory: "YM",
    materialType: "LM",
    woodToken: "PIN",
    productTypeToken: "S",
    existingCodes: [],
  };
  assert(buildCodePrefix(ym) === "YM-LM-PIN-S", "ym lamel");
  assert(mintMaterialCode(ym) === "YM-LM-PIN-S-001", "ym mint");

  const mp: MaterialCodingInput = {
    mainCategory: "MP",
    materialType: "",
    woodToken: "CP",
    productTypeToken: "S",
    quality: "AA",
    thicknessForCode: 18,
    existingCodes: [],
  };
  assert(buildCodePrefix(mp) === "MP-CP-AA-18-S", "mp prefix");
  assert(mintMaterialCode(mp) === "MP-CP-AA-18-S", "mp mint");
  assert(
    mintMaterialCode({ ...mp, existingCodes: ["MP-CP-AA-18-S"] }) === null,
    "mp duplicate",
  );

  const tw: MaterialCodingInput = {
    mainCategory: "TW",
    materialType: "",
    woodToken: "CP",
    existingCodes: ["TW-CP-001"],
  };
  assert(mintMaterialCode(tw) === "TW-CP-002", "tw seq");
}

if (import.meta.url === `file://${process.argv[1]}`) {
  runMaterialCodingSelftest();
  console.log("materialCoding.selftest: OK");
}
