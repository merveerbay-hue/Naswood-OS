# Manufacturing Process

**Project:** Naswood OS  
**Document:** Manufacturing Process  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Amaç

Bu doküman Naswood üretim süreçlerini tanımlar.

Bu belge;

- üretim akışlarını,
- operasyonları,
- alternatif üretim rotalarını,
- malzeme dönüşümlerini,
- geri kazanım süreçlerini

tanımlar.

Bu belge makineyi değil, operasyonları tanımlar.

Makine bilgileri Machine_Catalog.md içerisinde yer alacaktır.

---

# 2. Üretim Felsefesi

Naswood üretimi doğrusal (linear) değildir.

Bir malzeme;

- satış ürünü olabilir,
- yarı mamul olabilir,
- farklı üretim rotalarına yönlendirilebilir,
- tekrar üretime alınabilir,
- farklı nihai ürünlere dönüşebilir.

Üretim rotası sabit değildir.

Rota üretim planlama ve üretim sorumluları tarafından belirlenir.

Naswood OS yalnızca izin verilen üretim rotalarını yönetir ve kayıt altına alır.

---

# 3. Operasyon Tipleri

Üretim operasyonları dört ana gruba ayrılır.

## 3.1 Transformation

Malzemenin fiziksel yapısı değişir.

Örnek:

- Canter
- Kurutma
- Thermowood
- Finger Joint
- Pres

---

## 3.2 Preparation

Malzeme sonraki operasyona hazırlanır.

Örnek:

- Boylama
- Ön Silim
- 4 Taraf Planya
- Profil

---

## 3.3 Inspection

Malzemenin kalite kontrolü yapılır.

Örnek:

- Kusur Ayıklama
- Nem Ölçümü
- Kalite Sınıflandırma

---

## 3.4 Logistics

Malzemenin fiziksel hareketleri yönetilir.

Örnek:

- Mal Kabul
- Depolama
- Paketleme
- Sevkiyat

---

# 4. Üretim Operasyonları

## OP-001 Mal Kabul

### Amaç

Hammaddelerin sisteme kabul edilmesi.

### Girdiler

- Tomruk
- Yaş Kereste
- Fırınlı Kereste
- Kuru Lata
- Yaş Lata

### Çıktılar

Material Lot

### Sonraki Operasyonlar

- Depolama
- Kalite Kontrol

---

## OP-002 Tomruk Depolama

### Amaç

Tomrukların üretim planına kadar stoklanması.

### Sonraki Operasyonlar

- Tomruk Satışı
- Canter
- Pelet Hammaddesi

---

## OP-003 Canter

### Amaç

Tomruğun prizma ve keresteye dönüştürülmesi.

### Girdiler

- Tomruk

### Çıktılar

- Prizma
- Ham Kereste

### Yan Ürünler

- Kabuk
- Talaş
- Yonga

### Sonraki Operasyonlar

- Boylama
- Satış
- Kurutma

---

## OP-004 Boylama

### Amaç

Malzemenin hedef boylarda hazırlanması.

### Not

Boylama operasyonu üretim planına göre;

- kesimden hemen sonra,
- kurutmadan sonra

uygulanabilir.

Bu karar üretim planlama tarafından verilir.

---

## OP-005 Kurutma

### Amaç

Malzemenin hedef nem seviyesine ulaştırılması.

### Girdiler

- Yaş Kereste
- Yaş Lata

### Çıktılar

- Fırınlı Kereste

### Fire

- Çatlak
- Eğilme
- Çarpılma

### Sonraki Operasyonlar

- Satış
- Thermowood
- Panel

---

## OP-006 Thermowood

### Amaç

Ahşabın ısıl işlemden geçirilmesi.

### Girdiler

- Satın Alınan Fırınlı Kereste
- Kendi Kurutma Fırınından Gelen Kereste
- Kuru Lata

### Çıktılar

- Thermowood Kereste

### Sonraki Operasyonlar

- Ön Silim
- Profil

---

## OP-007 Ön Silim

### Amaç

Dönüklük, eğrilik ve yüzey bozukluklarının giderilmesi.

### Girdiler

- Fırınlı Kereste
- Thermowood Kereste
- Lata

### Çıktılar

Düzgün yüzeyli malzeme

### Sonraki Operasyonlar

- Profil
- Kusur Ayıklama

---

## OP-008 Profil

### Amaç

Nihai profil geometrisinin oluşturulması.

### Girdiler

- Thermowood Kereste
- Sert Ağaç Kereste

### Çıktılar

- Thermowood Profil
- Profil Ürünleri

### Sonraki Operasyonlar

- Paketleme
- Sevkiyat
- Panel (gerektiğinde)

---

## OP-009 Kusur Ayıklama

### Amaç

Budak, çatlak ve kusurlu bölgelerin belirlenmesi.

### Sonuç

Malzeme iki gruba ayrılır.

- Solid Panel
- Finger Joint

---

## OP-010 Opticut

### Amaç

Kusurlu bölgelerin kesilerek temiz parçaların elde edilmesi.

### Çıktılar

- Temiz Kısa Parçalar
- Fire

### Sonraki Operasyon

Finger Joint

---

## OP-011 Finger Joint

### Amaç

Kısa temiz parçaların eklenerek panel boyunda lamel oluşturulması.

### Girdiler

- Opticut çıkışı
- Kısa boy geri kazanım parçaları

### Çıktılar

Finger Joint Lamel

### Sonraki Operasyon

4 Taraf Planya

---

## OP-012 4 Taraf Planya

### Amaç

Lamellerin pres öncesi ölçü ve yüzey hazırlığının yapılması.

### Girdiler

- Solid Lamel
- Finger Joint Lamel

### Çıktılar

Pres Hazır Lamel

---

## OP-013 Tutkal

### Amaç

Lamellerin pres öncesi tutkallanması.

### Sonraki Operasyon

Pres

---

## OP-014 Pres

### Amaç

Lamellerin panel haline getirilmesi.

### Çıktılar

Masif Panel

---

## OP-015 Kalibrasyon

### Amaç

Panel kalınlığının hassas ölçüye getirilmesi.

---

## OP-016 Ebatlama

### Amaç

Panelin sipariş ölçülerine getirilmesi.

---

## OP-017 Paketleme

### Amaç

Ürünün sevkiyata hazırlanması.

---

## OP-018 Sevkiyat

### Amaç

Nihai ürünün müşteriye gönderilmesi.

---

# 5. Geri Kazanım Süreçleri

Naswood üretiminde oluşan yan ürünler mümkün olduğunca tekrar değerlendirilir.

## Finger Joint Geri Kazanımı

Solid panel üretiminde oluşan;

- kısa parçalar
- kusursuz kesilmiş parçalar

Finger Joint hattına gönderilir.

Bu parçalar tekrar birleştirilerek panel boyunda lamel üretilir.

---

## Pelet Üretimi

Pelet üretiminde;

- normal talaş
- uygun ahşap atıkları
- kırıcıdan gelen odun parçaları

kullanılır.

Yaş talaş önce kurutma sistemine alınır.

---

## Thermowood Talaşı

Thermowood talaşı pelet üretiminde kullanılmaz.

Thermowood fırınlarında yakıt olarak değerlendirilir.

---

# 6. Temel Üretim Kuralları

- Aynı malzeme birden fazla üretim rotasında kullanılabilir.
- Aynı malzeme hem satış ürünü hem yarı mamul olabilir.
- Üretim rotaları dinamik olarak belirlenir.
- Üretim kararları sistem tarafından değil, yetkili kullanıcı tarafından verilir.
- Her operasyon giriş ve çıkış lotlarını kayıt altına alır.
- Her operasyon fire ve yan ürün miktarlarını kaydeder.
- Her operasyon kalite sonuçlarını saklar.
- Hiçbir malzeme izlenebilirlik zincirinin dışına çıkamaz.

---

# 7. Sonraki Dokümanlar

Bu doküman aşağıdaki belgeler için referans niteliğindedir.

- Machine_Catalog.md
- Routing_Rules.md
- Quality_Standards.md
- Waste_Management.md
- Traceability_Model.md
- Database.md
- Event_Catalog.md
