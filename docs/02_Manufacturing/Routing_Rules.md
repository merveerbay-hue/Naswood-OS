
# Routing Rules

**Project:** Naswood OS  
**Document:** Routing Rules  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Amaç

Bu doküman Naswood üretim sistemindeki malzeme yönlendirme (Routing) kurallarını tanımlar.

Routing;

Bir malzemenin bulunduğu mevcut durumdan hangi operasyonlara geçebileceğini belirleyen iş kurallarıdır.

Bu belge üretim planlamasının temel referansıdır.

---

# 2. Routing Felsefesi

Naswood üretim sistemi doğrusal değildir.

Her malzemenin;

- birden fazla üretim rotası olabilir.
- aynı malzeme hem satış ürünü hem yarı mamul olabilir.
- üretim sırasında rota değiştirilebilir.
- rota üretim planlama tarafından belirlenir.

Yazılım rota önermez.

Yazılım yalnızca izin verilen rotaları kontrol eder ve kayıt altına alır.

---

# 3. Routing Kuralları

Her malzeme aşağıdaki bilgilere sahiptir.

- Bulunduğu Durum
- Gidebileceği Operasyonlar
- Oluşabilecek Yeni Malzemeler
- Alternatif Rotalar

---

# 4. Hammadde Routing

---

## Tomruk

### Giriş

Mal Kabul

### Gidebilir

- Satış
- Tomruk Deposu
- Canter
- Pelet Hammaddesi

### Oluşabilir

- Prizma
- Ham Kereste
- Kabuk
- Talaş
- Yonga

---

## Yaş Kereste

### Gidebilir

- Kurutma
- Satış

### Oluşturur

- Fırınlı Kereste

---

## Fırınlı Kereste

### Kaynak

- Satın Alma
- Kurutma Fırını

### Gidebilir

- Satış
- Ön Silim
- Thermowood
- Solid Panel
- Finger Joint

---

## Kuru Lata

### Gidebilir

- Boylama
- Ön Silim
- Thermowood
- Solid Panel
- Finger Joint

---

# 5. Thermowood Routing

Thermowood hattına aşağıdaki malzemeler girebilir.

- Satın Alınan Fırınlı Kereste
- Kendi Kurutma Fırınından Gelen Kereste
- Kuru Lata

Çıkış

Thermowood Kereste

---

Thermowood Kereste

↓

Ön Silim

↓

Profil

↓

Thermowood Profil

↓

Paketleme

↓

Sevkiyat

---

Alternatif

Thermowood Kereste

↓

Panel Üretimi

---

# 6. Panel Routing

Panel üretimi iki farklı hatta ayrılır.

- Solid Panel
- Finger Joint Panel

---

## Solid Panel

Girdi olabilir;

- Fırınlı Kereste
- Thermowood Kereste
- Kuru Lata

Operasyon

Ön Silim

↓

Kusur Ayıklama

↓

4 Taraf Planya

↓

Tutkal

↓

Pres

↓

Kalibrasyon

↓

Ebatlama

↓

Paketleme

---

Karar

Kalite uygun ise

↓

Solid Panel

Kalite uygun değilse

↓

Finger Joint

---

## Finger Joint

Girdi olabilir;

- Kusurlu Lameller
- Kısa Boy Parçalar
- Recovery Parçaları

Operasyon

Opticut

↓

Temiz Parçalar

↓

Finger Joint

↓

Lamel

↓

4 Taraf Planya

↓

Tutkal

↓

Pres

↓

Kalibrasyon

↓

Paketleme

---

# 7. Recovery Routing

Üretim sırasında oluşan kısa parçalar kaybolmaz.

Solid Panel

↓

Kısa Parçalar

↓

Finger Joint

↓

Yeni Lamel

↓

Panel

---

# 8. Waste Routing

Normal Talaş

↓

Pelet

---

Odun Parçaları

↓

Kırıcı

↓

Öğütücü

↓

Pelet

---

Yaş Talaş

↓

Kurutucu

↓

Pelet

---

Thermowood Talaşı

↓

Yakıt

↓

Thermowood Fırını

---

Kabuk

↓

Yakıt / Pelet (işletme kararına göre)

---

# 9. Satış Routing

Aşağıdaki malzemeler doğrudan satılabilir.

- Tomruk
- Ham Kereste
- Fırınlı Kereste
- Thermowood Profil
- Solid Panel
- Finger Joint Panel
- Pelet

---

# 10. Karar Noktaları

Üretim sırasında aşağıdaki kararlar kullanıcı tarafından verilir.

## Boylama

Kesimden sonra mı?

Kurutmadan sonra mı?

---

## Kurutma

Satış mı?

Kurutma mı?

---

## Thermowood

Profil mi?

Panel mi?

---

## Panel

Solid mi?

Finger Joint mı?

---

## Satış

Stokta beklesin mi?

Sevk edilsin mi?

Üretime girsin mi?

---

# 11. Routing Kuralları

- Her malzemenin en az bir çıkış rotası bulunmalıdır.
- Rota değişiklikleri kayıt altına alınmalıdır.
- Aynı malzeme farklı üretim rotalarında kullanılabilir.
- Aynı lot farklı operasyonlara bölünebilir.
- Farklı lotlar aynı operasyonda birleşebilir.
- Her dönüşüm Parent Lot → Child Lot ilişkisi oluşturur.
- Üretim planı rota değiştirebilir.
- Yazılım yalnızca izin verilen rotaları uygular.

---

# 12. Traceability

Her routing işlemi aşağıdaki bilgileri oluşturur.

- Parent Material Lot
- Child Material Lot
- Operation
- Tarih
- Operatör
- Makine
- Kalite Sonucu
- Fire
- Yan Ürün

Hiçbir routing işlemi izlenebilirlik zincirini kıramaz.

---

# 13. Gelecek Genişletmeler

İlerleyen sürümlerde routing kurallarına aşağıdaki üretim hatları eklenecektir.

- CLT
- Glulam
- Lamine
- CNC
- Boyama
- Montaj
- Prefabrik Yapılar

Bu yeni üretim hatları mevcut routing yapısını değiştirmeden sisteme eklenebilecektir.
