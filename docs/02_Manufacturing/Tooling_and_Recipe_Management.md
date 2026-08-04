# Tooling and Recipe Management

**Project:** Naswood OS  
**Document:** Tooling and Recipe Management  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Amaç

Bu doküman;

- üretim takımlarının,
- profil bıçaklarının,
- testere ve frezelerin,
- takım dizilimlerinin,
- üretim reçetelerinin,
- takım ömürlerinin

Naswood OS içerisinde nasıl yönetileceğini tanımlar.

Bu modül yalnızca bakım amacıyla değil;

- kalite,
- maliyet,
- üretim planlama,
- reçete yönetimi,
- yapay zekâ analizleri

için kullanılacaktır.

---

# 2. Kapsam

Bu modül aşağıdaki ekipmanları kapsar.

- Profil Bıçakları
- Planya Bıçakları
- Testere Diskleri
- Finger Joint Frezeleri
- Frezeler
- Matkaplar
- CNC Takımları
- Elmas Takımlar
- Kesici Takımlar

---

# 3. Temel Kavramlar

## Tool

Tek bir kesici ekipman.

Örnek

- Profil Bıçağı
- Testere
- Freze

---

## Cutter Head

Bıçakların bağlandığı top.

---

## Tool Assembly

Bir top üzerindeki tüm takım dizilimi.

---

## Recipe

Belirli bir ürünü üretebilmek için gerekli;

- takım dizilimi
- makine ayarları
- ilerleme hızı
- devir
- kalite parametreleri

kombinasyonu.

---

# 4. Takım Kartı

Her takım aşağıdaki bilgilerle tanımlanacaktır.

## Kimlik

- Tool Code
- Tool Name
- Barcode / QR
- Üretici
- Model
- Seri No

---

## Teknik Bilgiler

- Takım Tipi
- Malzeme
- Çap
- Kalınlık
- Delik Çapı
- Kesme Açısı
- Boşluk Açısı
- Kama Açısı
- Toplam Açı

---

## Kullanım

- Kullanıldığı Makineler
- Kullanıldığı Operasyonlar
- Kullanıldığı Ürünler

---

## Yaşam Döngüsü

- İlk Kullanım Tarihi
- Toplam Çalışma Süresi
- Toplam Metraj
- Toplam Parça Sayısı
- Bileme Sayısı
- Maksimum Bileme
- Son Bileme
- Tahmini Kalan Ömür

---

## Stok

- Depo
- Raf
- Mevcut Adet
- Minimum Stok
- Kritik Seviye

---

# 5. Cutter Head Yönetimi

Her top sistemde kayıtlı olacaktır.

## Bilgiler

- Head Code
- Çap
- Uzunluk
- Maksimum Devir
- Makine Uyumluluğu

---

## Dizilim

Her top üzerinde;

- Spacer
- Pul
- Bıçak
- Sağ Bıçak
- Sol Bıçak
- Alt Bıçak

sırası kayıt altına alınacaktır.

---

# 6. Profil Reçeteleri

Her profil için bir reçete oluşturulur.

## Genel Bilgiler

- Recipe Code
- Recipe Name
- Ürün Kodu
- Revizyon No
- Hazırlayan
- Onaylayan
- Durum

---

## Kullanılan Makine

- Profil Makinesi

---

## Kullanılan Takımlar

- Tool Listesi

---

## Kullanılan Toplar

- Head Listesi

---

## Makine Ayarları

- Devir
- İlerleme Hızı
- Baskı Ayarları
- Besleme Hızı

---

## Ahşap Bilgileri

- Ağaç Türü
- Nem
- Yoğunluk

---

## Kalite

- Toleranslar
- Ölçüler
- Yüzey Kalitesi

---

# 7. Bileme Yönetimi

Her bileme kayıt altına alınacaktır.

## Bilgiler

- Tarih
- Operatör
- Taşlama Firması
- Alınan Malzeme
- Verilen Malzeme
- Ölçü Kaybı
- Sonraki Bileme

---

# 8. Takım Değişimi

Her takım değişikliği kayıt altına alınacaktır.

## Bilgiler

- Makine
- Tarih
- Operatör
- Sebep
- Eski Takım
- Yeni Takım
- Üretim Siparişi

---

# 9. Kalite İlişkisi

Her takım aşağıdaki kalite verileri ile ilişkilendirilecektir.

- Fire Oranı
- Ölçü Sapması
- Yüzey Kalitesi
- Çatlak
- Yanık
- Lif Kalkması

---

# 10. Yapay Zekâ

AI aşağıdaki analizleri yapacaktır.

- Takım Ömür Tahmini
- Bileme Önerisi
- Fire Analizi
- Kalite Analizi
- Reçete Karşılaştırması
- En Verimli Takım Seti
- Operatör Performansı
- Ağaç Türüne Göre Optimum Kesme Açısı

---

# 11. Dijital Arşiv

Her takım için aşağıdaki dosyalar saklanabilir.

- Teknik Resim
- DXF
- STEP
- PDF
- Fotoğraf
- Montaj Şeması
- Bileme Talimatı

---

# 12. Gelecek Geliştirmeler

İlerleyen sürümlerde sisteme aşağıdaki özellikler eklenecektir.

- Takım RFID Takibi
- Otomatik Takım Tanıma
- CNC Entegrasyonu
- Profil Simülasyonu
- 3D Takım Görselleştirme
- Dijital Twin
- CAM Entegrasyonu
- ERP Entegrasyonu
