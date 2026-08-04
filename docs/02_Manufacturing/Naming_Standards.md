
# Naming Standards

**Project:** Naswood OS  
**Document:** Naming Standards  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Amaç

Bu doküman Naswood OS içerisinde kullanılacak tüm kodlama standartlarını tanımlar.

Amaç;

- kod karmaşasını önlemek,
- tüm sistemlerde aynı dili kullanmak,
- okunabilirliği artırmak,
- izlenebilirliği kolaylaştırmaktır.

Bu standartlar;

- Veritabanı
- API
- Barkod
- QR Kod
- RFID
- Raporlar
- Dashboard
- Mobil Uygulama

tarafından ortak olarak kullanılacaktır.

---

# 2. Genel Kurallar

- Tüm kodlar İngilizce kısaltmalar kullanacaktır.
- Kodlar büyük harf ile yazılır.
- Türkçe karakter kullanılmaz.
- Kodlar değiştirilmez.
- Yeni kodlar eklenebilir ancak mevcut kodlar korunur.
- Kodlar mümkün olduğunca anlam taşımalıdır.

Örnek

THM = Thermowood

PRF = Profile

FJP = Finger Joint Panel

---

# 3. Malzeme Kodları

| Kod | Açıklama |
|------|----------|
| LOG | Log (Tomruk) |
| PRM | Prism |
| GRN | Green Lumber (Yaş Kereste) |
| KDR | Kiln Dried Lumber (Fırınlı Kereste) |
| LAT | Lata |
| THM | Thermowood Lumber |
| LAM | Lamel |
| SLD | Solid Panel |
| FJP | Finger Joint Panel |
| PRF | Profile |
| DCK | Deck |
| CLD | Cladding |
| PEL | Pellet |
| SAW | Sawdust (Talaş) |
| BRK | Bark (Kabuk) |
| CHP | Wood Chips (Yonga) |
| OFF | Offcut (Kısa Parça) |

---

# 4. Ağaç Türü Kodları

| Kod | Ağaç Türü |
|------|-----------|
| PN | Pine (Çam) |
| SP | Spruce |
| FR | Fir (Göknar) |
| AS | Ash (Dişbudak) |
| BK | Beech (Kayın) |
| OK | Oak (Meşe) |
| WR | Walnut (Ceviz) |
| IR | Iroko |
| AY | Ayous |
| TK | Teak |
| AC | Accoya |
| OT | Other |

---

# 5. Kalite Kodları

## Kereste

| Kod | Açıklama |
|------|----------|
| A | A Kalite |
| B | B Kalite |
| C | C Kalite |
| D | D Kalite |

---

## Panel

| Kod | Açıklama |
|------|----------|
| AA | AA |
| AB | AB |
| AC | AC |
| BB | BB |
| BC | BC |
| CC | CC |

---

## Thermowood

| Kod | Açıklama |
|------|----------|
| THM-D | Thermo-D |
| THM-S | Thermo-S |

---

# 6. Operasyon Kodları

| Kod | Operasyon |
|------|-----------|
| RCV | Mal Kabul |
| STO | Depolama |
| PEL | Soyma |
| CAN | Canter |
| RIP | Çoklu Dilimleme |
| CRS | Boylama |
| DRY | Kurutma |
| PLN | Ön Silim |
| INS | Kalite Kontrol |
| OPT | Opticut |
| FJG | Finger Joint |
| MLD | 4 Taraf Planya |
| THM | Thermowood |
| PRF | Profil |
| GLU | Tutkal |
| PRS | Pres |
| CAL | Kalibrasyon |
| CUT | Ebatlama |
| PKG | Paketleme |
| SHP | Sevkiyat |

---

# 7. Makine Kodları

Makine kodu;

[Operasyon]-[Sıra]

formatında oluşturulur.

Örnek

CAN-01

CAN-02

THM-01

PRF-01

PRS-01

PKG-01

---

# 8. Tool Kodları

Format

TOOL-000001

Örnek

TOOL-000145

---

# 9. Cutter Head Kodları

HEAD-001

HEAD-002

HEAD-003

---

# 10. Recipe Kodları

REC-000001

Örnek

REC-PRF-000015

REC-THM-000021

REC-PNL-000004

---

# 11. Lot Kodları

Her üretim partisi benzersizdir.

Format

LOT-YYYYMMDD-XXXXX

Örnek

LOT-20260804-00015

---

# 12. Material Instance (Malzeme Kimliği)

Her fiziksel malzeme aşağıdaki kimlik bilgilerine sahip olacaktır.

Material Type

+

Tree Species

+

Quality

+

Lot

Örnek

KDR-AS-AA

↓

LOT-20260804-00018

---

# 13. Paket Kodları

PKG-YYYYMMDD-XXXXX

Örnek

PKG-20260804-00008

---

# 14. Palet Kodları

PLT-000001

---

# 15. Depo Kodları

| Kod | Depo |
|------|-------|
| RAW | Hammadde |
| LOG | Tomruk |
| DRY | Kurutma Sonrası |
| THM | Thermowood |
| PAN | Panel |
| FIN | Mamul |
| PEL | Pelet |
| WST | Fire |

---

# 16. Lokasyon Kodları

Format

DEPOT-ROW-RACK-LEVEL

Örnek

RAW-A-03-02

THM-C-08-01

PAN-B-05-03

---

# 17. Kullanıcı Kodları

USR-000001

---

# 18. Müşteri Kodları

CUS-000001

---

# 19. Tedarikçi Kodları

SUP-000001

---

# 20. Sipariş Kodları

SO-000001

---

# 21. Üretim Emirleri

WO-000001

---

# 22. Bakım Emirleri

MWO-000001

---

# 23. Barkod ve QR Kod

Her fiziksel malzeme;

- Material Code
- Lot No
- Üretim Tarihi
- Ağaç Türü
- Kalite
- Ölçüler

bilgilerini QR kod içerisinde taşıyacaktır.

---

# 24. Revizyon Kuralları

Kodlar değiştirilmez.

Yeni varyasyonlar oluşturulur.

Örnek

THM → değişmez

THM-S

THM-D

eklenebilir.

---

# 25. Gelecek Genişletmeler

İlerleyen sürümlerde aşağıdaki kod grupları da eklenecektir.

- CLT
- Glulam
- CNC
- Boya
- Montaj
- Prefabrik Yapılar
- Dijital İkiz Varlıkları
