import {
  applyThermowoodToken,
  buildCodePrefix,
  mintMaterialCode,
  previewMaterialCode,
  suggestMaterialName,
  type MaterialCodingInput,
} from "./materialCoding";

function assert(cond: unknown, msg: string): asserts cond {
  if (!cond) throw new Error(msg);
}

export function runMaterialCodingSelftest(): void {
  assert(applyThermowoodToken("PIN", false) === "PIN", "pin base");
  assert(applyThermowoodToken("PIN", true) === "TPIN", "tpin");
  assert(applyThermowoodToken("CP", true) === "TCP", "tcp");
  assert(applyThermowoodToken("TPIN", true) === "TPIN", "already t");

  const hm: MaterialCodingInput = {
    mainCategory: "HM",
    materialType: "KR",
    woodToken: "PIN",
    existingCodes: [],
  };
  assert(buildCodePrefix(hm) === "HM-KR-PIN", "hm prefix");
  assert(mintMaterialCode(hm) === "HM-KR-PIN-001", "hm mint");

  const hmTw: MaterialCodingInput = {
    ...hm,
    woodToken: applyThermowoodToken("PIN", true),
  };
  assert(buildCodePrefix(hmTw) === "HM-KR-TPIN", "hm tpin prefix");
  assert(mintMaterialCode(hmTw) === "HM-KR-TPIN-001", "hm tpin mint");
  assert(
    suggestMaterialName({
      mainCategory: "HM",
      materialType: "KR",
      baseWoodToken: "PIN",
      isThermowood: false,
    }) === "Çam Kereste",
    "name plain",
  );
  assert(
    suggestMaterialName({
      mainCategory: "HM",
      materialType: "KR",
      baseWoodToken: "PIN",
      isThermowood: true,
    }) === "Thermowood Çam Kereste",
    "name thermo",
  );

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
  assert(mintMaterialCode(ym) === "YM-LM-PIN-S-001", "ym mint");
  assert(
    mintMaterialCode({ ...ym, woodToken: "TPIN" }) === "YM-LM-TPIN-S-001",
    "ym tpin",
  );
  assert(
    suggestMaterialName({
      mainCategory: "YM",
      materialType: "LM",
      baseWoodToken: "PIN",
      isThermowood: true,
      productTypeToken: "S",
    }) === "Thermowood Çam Solid Lamel",
    "ym name",
  );

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
  assert(
    buildCodePrefix({ ...mp, woodToken: "TCP" }) === "MP-TCP-AA-18-S",
    "mp tcp",
  );
  assert(mintMaterialCode({ ...mp, woodToken: "TCP" }) === "MP-TCP-AA-18-S", "mp tcp mint");

  const tw: MaterialCodingInput = {
    mainCategory: "TW",
    materialType: "",
    woodToken: "CP",
    existingCodes: ["TW-CP-001"],
  };
  assert(mintMaterialCode(tw) === "TW-CP-002", "tw seq");
  assert(previewMaterialCode(hmTw) === "HM-KR-TPIN-001", "preview");
}

if (import.meta.url === `file://${process.argv[1]}`) {
  runMaterialCodingSelftest();
  console.log("materialCoding.selftest: OK");
}
