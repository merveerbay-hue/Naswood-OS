export const ROLES = [
  "Fabrika müdürü",
  "Depo sorumlusu",
  "Üretim",
  "Kalite",
  "Bakım",
  "Satınalma",
  "Satış",
];

export const NAV = [
  { id: "dashboard", label: "Gösterge" },
  { id: "inventory", label: "Stok" },
  { id: "purchasing", label: "Satınalma" },
  { id: "sales", label: "Satış" },
  { id: "production", label: "Üretim" },
  { id: "quality", label: "Kalite" },
  { id: "maintenance", label: "Bakım" },
  { id: "finance", label: "Finans" },
  { id: "analytics", label: "Analitik" },
  { id: "ai", label: "Copilot" },
  { id: "twin", label: "Dijital ikiz" },
  { id: "master", label: "Ana veri" },
  { id: "admin", label: "Yönetim" },
];

export const ROLE_ACCESS = {
  "Fabrika müdürü": "all",
  "Depo sorumlusu": ["dashboard", "inventory", "purchasing", "quality"],
  Üretim: ["dashboard", "production", "inventory", "quality"],
  Kalite: ["dashboard", "quality", "inventory"],
  Bakım: ["dashboard", "maintenance"],
  Satınalma: ["dashboard", "purchasing", "inventory", "master"],
  Satış: ["dashboard", "sales", "inventory"],
};

export const CATEGORIES = [
  "Ham tomruk",
  "Kereste",
  "Thermowood",
  "Lamel",
  "Mamul",
  "Sarf",
  "Yedek parça",
];

export const UNITS = ["m³", "m²", "m", "adet", "kg", "ton"];

export function canOpen(role, page) {
  const access = ROLE_ACCESS[role];
  if (!access || access === "all") return true;
  return access.includes(page);
}
