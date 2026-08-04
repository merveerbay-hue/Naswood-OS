# Traceability Model

**Project:** Naswood OS  
**Document:** Traceability Model  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Amaç

Bu doküman Naswood OS içerisinde uygulanacak tam izlenebilirlik (Full Traceability) modelini tanımlar.

Naswood OS'un temel hedeflerinden biri;

> Fabrikaya giren her hammaddenin, üretim boyunca geçirdiği tüm operasyonların ve nihai ürünün müşteriye teslimine kadar olan sürecin eksiksiz olarak takip edilebilmesidir.

İzlenebilirlik yalnızca ürün bazında değil;

- malzeme,
- operasyon,
- makine,
- operatör,
- kalite,
- fire,
- geri kazanım

seviyelerinde de sağlanacaktır.

---

# 2. Temel Felsefe

Naswood OS ürünleri değil, malzemeleri takip eder.

Bir malzeme;

- bölünebilir,
- birleşebilir,
- dönüşebilir,
- tekrar üretime girebilir,
- satış ürünü olabilir.

Fiziksel olarak oluşan her yeni malzeme sistemde yeni bir kimlik kazanır.

Hiçbir fiziksel hareket kayıt dışı değildir.

---

# 3. İzlenebilirlik Seviyeleri

Naswood OS dört seviyede izlenebilirlik sağlar.

## Level 1

Receiving Lot

↓

Fabrikaya giriş

---

## Level 2

Material Instance

↓

Üretimde oluşan fiziksel malzemeler

---

## Level 3

Package

↓

Paket

---

## Level 4

Shipment

↓

Müşteri Sevki

---

# 4. Receiving Lot

Fabrikaya gelen her hammadde önce bir Receiving Lot altında sisteme kabul edilir.

Receiving Lot;

- Kamyon
- Konteyner
- Paketli Kereste
- İthal Lata
- Diğer Toplu Girişler

için ortak yapıdır.

Kod Formatı

RLOT-YYYYMMDD-XXXXX

Örnek

RLOT-20260804-00001

---

Receiving Lot içerisinde;

- Tedarikçi
- İrsaliye
- Araç Plakası
- Şoför
- Giriş Tarihi
- Ağaç Türü
- Toplam Adet
- Toplam Hacim
- Nem
- Açıklamalar

saklanır.

Bu aşamada tek tek tomruk oluşturulmaz.

---

# 5. Material Instance

İlk fiziksel dönüşüm gerçekleştiğinde sistem yeni Material Instance kayıtları oluşturur.

Örneğin

Receiving Lot

↓

Canter

↓

Prizma

↓

Kereste

Bu aşamadan sonra her fiziksel parça benzersiz bir Material ID alır.

---

Material ID örneği

PRM-PN-000001

KDR-PN-000001

THM-PN-000001

SLD-AS-000041

---

# 6. Parent → Child İlişkisi

Her üretim operasyonu Parent → Child ilişkisi oluşturur.

Örnek

RLOT

↓

Prizma

↓

Kereste

↓

Fırınlı Kereste

↓

Thermowood

↓

Profil

↓

Paket

Her yeni oluşan malzeme;

- Parent Material
- Child Material

ilişkisi ile kayıt altına alınır.

Bu yapı sayesinde malzemenin tüm geçmişi korunur.

---

# 7. Split İşlemleri

Bir malzeme birden fazla yeni malzemeye dönüşebilir.

Örnek

Bir prizma;

↓

5 farklı keresteye dönüşebilir.

Her yeni kereste yeni Material ID alır.

Parent ilişki korunur.

---

# 8. Merge İşlemleri

Birden fazla malzeme birleşerek yeni bir ürün oluşturabilir.

Örnek

12 Lamel

↓

Tutkal

↓

Pres

↓

1 Panel

Panel kaydı;

12 farklı Parent Material ID içerir.

---

# 9. Transformation

Malzeme dönüşümü sırasında kimlik korunmaz.

Her dönüşüm yeni Material ID oluşturur.

Örnek

Ham Kereste

↓

Kurutma

↓

Fırınlı Kereste

↓

Thermowood

↓

Profil

Her aşama yeni Material Instance oluşturur.

---

# 10. Recovery

Üretim sırasında oluşan geri kazanılabilir parçalar tekrar sisteme alınır.

Örnek

Solid Panel

↓

Kısa Parça

↓

Finger Joint

↓

Yeni Lamel

Recovery kayıtları da Parent → Child ilişkisini korur.

---

# 11. Waste

Fire oluştuğunda;

- Fire Tipi
- Operasyon
- Sebep
- Miktar
- Operatör
- Makine

kayıt altına alınır.

Fireler;

- Talaş
- Kabuk
- Yonga
- Kusurlu Parça
- Kırık Parça

olarak sınıflandırılır.

---

# 12. Package Traceability

Paket oluşturulduğunda;

- Package ID
- İçerdiği Material ID'ler
- Kalite
- Miktar
- Ölçüler

kayıt edilir.

Örnek

PKG-20260804-00018

↓

24 adet Thermowood Profil

---

# 13. Shipment Traceability

Sevkiyat sırasında;

- Shipment ID
- Package ID
- Müşteri
- Araç
- Sevkiyat Tarihi

ilişkisi kurulur.

---

# 14. QR Kod

Her paket QR kod taşır.

QR kod;

- Material Code
- Material ID
- Receiving Lot
- Package ID
- Ağaç Türü
- Kalite
- Ölçüler
- Üretim Tarihi

bilgilerini içerir.

---

# 15. RFID

İlerleyen sürümlerde;

- Paketler
- Paletler
- Ara stoklar

RFID ile takip edilecektir.

---

# 16. Geriye Dönük Sorgular

Sistem aşağıdaki soruların tamamını cevaplayabilmelidir.

- Bu panel hangi Receiving Lot'tan üretildi?
- Hangi makinede işlendi?
- Hangi operatör çalıştı?
- Hangi kalite kontrolünden geçti?
- Hangi reçete kullanıldı?
- Hangi takım kullanıldı?
- Ne kadar fire oluştu?
- Hangi müşteriye sevk edildi?

---

# 17. Genealogy Tree

Her malzeme soy ağacı şeklinde izlenebilir.

Örnek

Receiving Lot

↓

Prizma

↓

Kereste

↓

Kurutma

↓

Thermowood

↓

Profil

↓

Paket

↓

Sevkiyat

↓

Müşteri

---

# 18. Traceability Kuralları

- Fabrikaya giren her malzeme Receiving Lot altında kabul edilir.
- İlk fiziksel dönüşümde Material Instance oluşturulur.
- Her dönüşüm Parent → Child ilişkisi oluşturur.
- Split işlemleri desteklenir.
- Merge işlemleri desteklenir.
- Recovery işlemleri desteklenir.
- Fire kayıt altına alınır.
- Hiçbir fiziksel hareket kayıt dışı olamaz.
- Hiçbir Material ID tekrar kullanılamaz.
- İzlenebilirlik zinciri hiçbir aşamada koparılamaz.

---

# 19. AI Kullanımı

Yapay zekâ aşağıdaki analizleri yapacaktır.

- Fire Analizi
- Lot Analizi
- Üretim Geçmişi
- Kalite Analizi
- Operatör Analizi
- Makine Performansı
- Traceability Doğrulama
- Anormal Hareket Analizi

---

# 20. Gelecek Genişletmeler

İlerleyen sürümlerde aşağıdaki özellikler sisteme eklenecektir.

- RFID
- IoT Sensörleri
- Digital Product Passport
- Blockchain Destekli İzlenebilirlik
- Avrupa Birliği Dijital Ürün Pasaportu Uyumluluğu
- Müşteri QR Doğrulama Portalı
