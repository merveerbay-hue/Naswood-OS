# FJ Pilot — FJ-PILOT-1

Dondurulmuş sürüm. Pilot bitene kadar Phase 2 / routing / BOM / OEE / APS / dashboard / IoT yok.

| Alan | Değer |
|---|---|
| Release | FJ-PILOT-1 |
| Dondurma | 2026-09-11 |
| Branch | `cursor/pilot-fj-v1-ce37` |
| Tag | `fj-pilot-1` |
| Kaynak uç | `cursor/production-execution-shop-floor-ce37` (stok + shop floor + geri bildirim) |

Sürüm kontrolü: `GET /api/v1/production-execution/release` → `name`, `appVersion`, `gitSha`.

Yol: Deploy → Barkod testi → 2 güvenlik testi → 1 gerçek FJ emri → WIP → fiziksel etiket → sonraki operasyon → vardiya kontrolü → feedback → 3–5 tekrar → karar.

## Aşama 1 — Kodu dondur
Tek pilot versiyonu belli. Yeni geliştirme açma.

## Aşama 2 — Canlı ortam
Yayın, PostgreSQL, Bucak/FJ tesis, kullanıcı/yetki, WC-FJ, WH-SFG, WIP-FJ-OUT, quarantine, Material Master, gerçek paket stoku.
Geçiş: operatör giriş → kendi tesisi → FJ görünür → başka tesise 403.

## Aşama 3 — Barkod ve etiket
Gerçek yazıcı + okuyucu. Passport, 100×70, Code128, QR, aynı paket, PackageNo/Material/Lot/Qty/WH-Loc, yeniden basımda barkod değişmez.

## Aşama 4 — Güvenlik 1 (pilot öncesi, bir kez)
İki terminal, aynı paket 2.00, 1.20+1.20 aynı anda. Biri başarılı, diğeri conflict. Kalan 0.80. Toplam tüketim 1.20. Double-spend yok.

## Aşama 5 — Güvenlik 2 (pilot öncesi, bir kez)
Kısmi consume → cancel. Qty, balance, content restore. Aynı PackageNo/Barcode. Yeni paket yok. Reversal hareketi var.

**Bu iki test geçmeden gerçek FJ üretimine başlanmaz.**

## Aşama 6 — 1 gerçek emir
PO → FJ → WC-FJ → 2 paket / mümkünse 2 lot → FJ Lamel → WH-SFG / WIP-FJ-OUT. Kâğıt/Excel yok. Emir kuyrukta.

## Aşama 7 — START
Doğru emir/op/WC/kullanıcı, timer, RUNNING. Yenilemede kaybolmaz.

## Aşama 8 — Paket A
Barkod → miktar → Onayla. Sistem material/lot/package/wh/loc/available getirir.

## Aşama 9 — Paket B / ikinci lot
Execution A+B paket ve lot bilir.

## Aşama 10 — Pause / downtime / fire
Birer kez. History yenilemede durur.

## Aşama 11 — Complete
Fiziksel istif sayısı = paket sayısı.

## Aşama 12 — Output
PEX → POUT → LOT-PR → paketler → movement → balance. Genealogy: Lot A+B → FJ → LOT-PR.

## Aşama 13 — WIP etiket (atlanmaz)
Passport/label ile aynı PackageNo/Barcode. Fiziksel yapıştır, tekrar okut.

## Aşama 14 — Sonraki operasyon
Planer etiketi okur; elle yazmaz.

## Aşama 15 — Genealogy
Planer ← WIP paket ← LOT-PR ← FJ ← giriş paket/lot.

## Aşama 16 — Vardiya sonu (yalnız 4)
1. Fiziksel stok ↔ InventoryBalance
2. Fiziksel paket ↔ barcode
3. Input lot ↔ WIP genealogy
4. Kâğıt/Excel’siz kullanım

## Aşama 17 — Geri bildirim
İşi durduruyor / Devam edebiliyorum. Sınıf: P0 durdur · P1 aynı gün · P2 biriktir · P3 kod yok · Phase 2 backlog.

## Aşama 18 — 3–5 gerçek emir
START → SCAN → CONSUME → COMPLETE → WIP → LABEL → NEXT. Stok ve genealogy bozulmuyorsa pilot başarılı.
