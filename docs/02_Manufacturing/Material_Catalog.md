# Material Catalog

**Project:** Naswood OS  
**Version:** 1.0

---

# 1. Amaç

Bu doküman Naswood OS içerisinde kullanılan tüm malzemeleri tanımlar.

Buradaki "Malzeme" kavramı;

- Hammadde
- Yarı Mamul
- Nihai Ürün
- Yan Ürün
- Fire

gruplarının tamamını kapsar.

Her fiziksel malzeme yalnızca bir kez tanımlanır.

---

# 2. Malzeme Sınıfları

## 2.1 Hammadde

- Tomruk
- Yaş Kereste
- Fırınlı Kereste
- Yaş Lata
- Kuru Lata

---

## 2.2 Yarı Mamul

- Prizma
- Çıta
- Lamel
- Thermowood Kereste
- Thermowood Profil

---

## 2.3 Nihai Ürün

- Thermowood Profil
- Solid Panel
- Finger Joint Panel
- Pelet

---

## 2.4 Yan Ürün

- Talaş
- Kabuk
- Yonga
- Kısa Parça
- Budak
- Kusurlu Lamel

---

# 3. Ortak Malzeme Özellikleri

Her malzeme aşağıdaki ortak alanlara sahiptir.

- Material Code
- Material Name
- Material Type
- Tree Species
- Moisture
- Thickness
- Width
- Length
- Volume
- Unit
- Quality Grade
- Current Status

---

# 4. Malzeme Yaşam Durumu

Bir malzeme aşağıdaki durumlardan birinde olabilir.

- Satın Alındı
- Stokta
- Üretimde
- Kalite Kontrolde
- Sevke Hazır
- Sevk Edildi
- Hurda
- Yan Ürün

---

# 5. Malzeme Kartı

Her malzeme aşağıdaki bilgileri içerir.

## Malzeme Kodu

## Malzeme Adı

## Açıklama

## Ağaç Türü

## Nem Oranı

## Kalite

## Boyutlar

## Birim

## Satılabilir mi?

## Yarı Mamul olabilir mi?

## Üretimde kullanılabilir mi?

## Hangi proseslerden geçebilir?

## Oluşturabileceği yan ürünler

## Oluşturabileceği fireler

## Alternatif üretim rotaları

## İzlenebilirlik seviyesi

---

# 6. Temel Malzemeler
# Tomruk

Malzeme Tipi

Hammadde

Açıklama

Fabrikaya giren ilk doğal hammaddedir.

Girdi Kaynağı

- Orman İşletmesi
- İhale
- Özel Tedarikçi

Kullanılabilir Hatlar

- Canter
- Satış
- Pelet

Oluşturabileceği Ürünler

- Prizma
- Kereste

Yan Ürünler

- Kabuk
- Talaş

Satılabilir

Evet

# Fırınlı kereste
Malzeme Tipi

Yarı Mamul

Oluşabilir

- Kurutma Fırını

veya

- Satın Alma

Kullanılabilir

- Satış

- Thermowood

- Solid Panel

- Finger Joint

Satılabilir

Evet

Yarı Mamul

Evet
Yarı Mamul

Hayır

İzlenebilirlik

LOT Bazlı

| Tip                   | Açıklama                                              |
| --------------------- | ----------------------------------------------------- |
| Raw Material          | Satın alınan veya ormandan gelen hammadde             |
| Intermediate Material | Üretimde kullanılan yarı mamul                        |
| Finished Product      | Satılabilen nihai ürün                                |
| By-product            | Üretim sonucu oluşan ve ekonomik değeri olan yan ürün |
| Waste                 | Ekonomik değeri olmayan veya bertaraf edilen fire     |

Ama bir istisna eklemeliyiz.

Örneğin kısa parça, bazı durumlarda Waste değildir.

Çünkü Finger Joint hattında tekrar kullanılır.

Bu yüzden her malzeme kartında ayrıca şu alan olsun:

Recoverable

Evet / Hayır

ve

Recovery Process

Finger Joint

Pelet

Yakıt

Yok

---

# 6. Malzeme Kartları

## 6.1 Tomruk

### Genel Bilgiler

| Alan | Değer |
|------|--------|
| Malzeme Tipi | Hammadde |
| Satın Alma | Evet |
| Satış | Evet |
| Yarı Mamul | Hayır |
| İzlenebilirlik | Lot Bazlı |

### Açıklama

Fabrikaya giren ilk hammaddedir.

Tomruk sisteme tır bazında kabul edilir.

Her tomruk lotu;

- Tedarikçi
- İhale
- Orman İşletmesi
- Ağaç Türü
- Çap
- Boy
- Hacim
- Kalite

bilgileriyle kayıt altına alınır.

### Gidebileceği Operasyonlar

- Satış
- Soyma
- Canter
- Pelet Hammaddesi

### Oluşturabileceği Çıktılar

- Prizma
- Ham Kereste
- Kabuk
- Talaş
- Yonga

---

## 6.2 Prizma

### Genel Bilgiler

| Alan | Değer |
|------|--------|
| Malzeme Tipi | Yarı Mamul |
| Satış | Gerektiğinde |
| Üretimde Kullanılır | Evet |

### Açıklama

Canter hattından elde edilen ara üründür.

Üretim planına göre;

- satışa ayrılabilir,
- tekrar işlenebilir,
- keresteye dönüştürülebilir.

### Gidebileceği Operasyonlar

- Çoklu Dilimleme
- Boylama
- Satış

---

## 6.3 Ham Kereste

### Genel Bilgiler

| Alan | Değer |
|------|--------|
| Malzeme Tipi | Yarı Mamul |
| Satış | Evet |
| Kurutma | Evet |

### Açıklama

Tomruktan kesildikten sonra henüz kurutulmamış kerestedir.

Üretim planına göre;

- yaş olarak satılabilir,
- kurutma fırınına gönderilebilir.

### Gidebileceği Operasyonlar

- Boylama
- Kurutma Fırını
- Satış

---

## 6.4 Fırınlı Kereste

### Genel Bilgiler

| Alan | Değer |
|------|--------|
| Malzeme Tipi | Yarı Mamul / Satış Ürünü |
| Satın Alma | Evet |
| Satış | Evet |

### Açıklama

İki farklı şekilde oluşabilir.

1. Kendi kurutma fırınından çıkar.

2. Hazır olarak satın alınır.

### Gidebileceği Operasyonlar

- Satış
- Ön Silim
- Thermowood
- Solid Panel
- Finger Joint

---

## 6.5 Yaş Lata

### Açıklama

Satın alınabilir.

Kesim sonrası kurutma fırınına gönderilir.

### Gidebileceği Operasyonlar

- Boylama
- Kurutma
- Panel

---

## 6.6 Kuru Lata

### Açıklama

Genellikle ithal ürünlerde kullanılır.

Örneğin;

- Ayous
- İroko

Kuru geldiği için doğrudan üretime alınabilir.

### Gidebileceği Operasyonlar

- Boylama
- Ön Silim
- Thermowood
- Solid Panel
- Finger Joint

---

## 6.7 Thermowood Kereste

### Genel Bilgiler

| Alan | Değer |
|------|--------|
| Malzeme Tipi | Yarı Mamul |
| Satış | Gerektiğinde |

### Açıklama

Thermowood prosesi tamamlanmış kerestedir.

Ancak çoğu zaman nihai ürün değildir.

Profil hattına gönderilir.

### Gidebileceği Operasyonlar

- Ön Silim
- Profil
- Panel

---

## 6.8 Thermowood Profil

### Genel Bilgiler

| Alan | Değer |
|------|--------|
| Malzeme Tipi | Nihai Ürün |
| Satış | Evet |

### Açıklama

Thermowood kerestenin;

- ön silim,
- profil

işlemlerinden geçmesiyle oluşur.

Naswood'un en önemli satış ürünlerinden biridir.

### Gidebileceği Operasyonlar

- Paketleme
- Sevkiyat

---

## 6.9 Lamel

### Açıklama

Panel üretimi için hazırlanmış malzemedir.

Solid veya Finger Joint hattında kullanılabilir.

### Gidebileceği Operasyonlar

- Ön Silim
- Kusur Ayıklama
- Finger Joint
- 4 Taraf Planya
- Pres

---

## 6.10 Solid Panel

### Malzeme Tipi

Nihai Ürün

### Oluşur

- Pres
- Kalibrasyon
- Ebatlama

operasyonlarından sonra.

### Satış

Evet

---

## 6.11 Finger Joint Panel

### Malzeme Tipi

Nihai Ürün

### Açıklama

Kusurlu veya kısa parçaların değerlendirilmesiyle üretilen lamellerden oluşur.

Üretim sırasında;

- kusur tarama,
- opticut,
- finger joint

operasyonları uygulanır.

### Satış

Evet

---

## 6.12 Pelet

### Malzeme Tipi

Nihai Ürün

### Açıklama

Normal üretim talaşı ve uygun ahşap atıklarından üretilir.

Thermowood talaşı pelet üretiminde kullanılmaz.

Thermowood talaşı yakıt olarak değerlendirilir.

---

# 7. Malzeme Dönüşüm Kuralları

Bir malzeme;

- satış ürünü olabilir,
- yarı mamul olabilir,
- başka bir malzemeye dönüşebilir,
- geri kazanılabilir.

Naswood OS bu dönüşümlerin tamamını kayıt altına alacaktır.

Her malzeme için;

- nereden geldiği,
- hangi operasyonlardan geçtiği,
- hangi ürüne dönüştüğü,
- hangi yan ürünleri oluşturduğu

izlenebilir olacaktır.
