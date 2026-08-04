
# Machine Catalog

**Project:** Naswood OS  
**Document:** Machine Catalog  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Amaç

Bu doküman Naswood üretim tesislerinde kullanılan tüm üretim makinelerini tanımlar.

Her makine;

- teknik özellikleri,
- gerçekleştirdiği operasyonlar,
- işlediği malzemeler,
- oluşturduğu çıktılar,
- kapasitesi,
- kalite kontrol noktaları,
- bakım gereksinimleri,
- performans göstergeleri

ile birlikte tanımlanır.

Makine bilgileri yalnızca envanter amacıyla değil, üretim planlama, bakım yönetimi, maliyet hesaplama ve yapay zekâ analizleri için de kullanılacaktır.

---

# 2. Makine Sınıfları

## 2.1 Tomruk İşleme

- Tomruk Soyma
- Arabalı Tomruk Kesim
- Canter
- Çoklu Dilimleme
- Yan Alma

---

## 2.2 Kurutma

- Kereste Kurutma Fırını
- Thermowood Fırını

---

## 2.3 Hazırlık

- Boylama
- Ön Silim
- Opticut
- 4 Taraf Planya

---

## 2.4 Panel Üretimi

- Finger Joint
- Tutkal Hattı
- Panel Presi
- Kalibrasyon
- Ebatlama

---

## 2.5 Profil Üretimi

- Profil Makinesi

---

## 2.6 Paketleme

- Shrink Paketleme
- Etiketleme
- Paketleme Hattı

---

## 2.7 Yan Ürün

- Kırıcı
- Öğütücü
- Talaş Kurutucu
- Pelet Presi

---

# 3. Standart Makine Kartı

Her makine aşağıdaki bilgilerle tanımlanacaktır.

## Genel Bilgiler

- Makine Kodu
- Makine Adı
- Üretici
- Model
- Seri Numarası
- Üretim Yılı
- Lokasyon
- Üretim Hattı
- Durumu (Aktif / Pasif / Bakım)

---

## Teknik Bilgiler

- Maksimum Kapasite
- Minimum Kapasite
- Çalışma Hızı
- Motor Gücü
- Elektrik Tüketimi
- Hava Tüketimi
- Hidrolik Sistem
- PLC Markası
- Yazılım Versiyonu

---

## Operasyon Bilgileri

- Operasyon Adı
- Operasyon Tipi
- İşlediği Malzemeler
- Oluşturduğu Çıktılar
- Oluşturduğu Yan Ürünler
- Oluşturduğu Fireler

---

## Kalite

- Kontrol Noktaları
- Ölçülen Parametreler
- Kabul Kriterleri

---

## Operasyon

- Operatör Sayısı
- Ortalama Çevrim Süresi
- Ortalama Kurulum Süresi
- Vardiya Bilgisi

---

## Bakım

- Günlük Kontrol
- Haftalık Bakım
- Aylık Bakım
- Yıllık Bakım

---

## Yapay Zeka

- İzlenecek KPI'lar
- Anomali Tespiti
- Tahmini Bakım
- Fire Analizi

---

# 4. Makine Kartları

---

# MC-001 Tomruk Soyma Makinesi

## Amaç

Tomruk üzerindeki kabuğun soyulması.

## Operasyon

Soyma

## Girdi

- Tomruk

## Çıktı

- Soyulmuş Tomruk

## Yan Ürün

- Kabuk

## Sonraki Operasyon

Arabalı Kesim

---

# MC-002 Arabalı Tomruk Kesim

## Amaç

Tomruğun ilk kesiminin yapılması.

## Girdi

- Soyulmuş Tomruk

## Çıktı

- Kesilmiş Tomruk

## Sonraki Operasyon

Canter

---

# MC-003 Canter

## Amaç

Tomruğun prizma ve keresteye dönüştürülmesi.

## Girdi

- Tomruk

## Çıktılar

- Prizma
- Ham Kereste

## Yan Ürün

- Talaş
- Kabuk
- Yonga

## Sonraki Operasyonlar

- Boylama
- Kurutma
- Satış

---

# MC-004 Çoklu Dilimleme

## Amaç

Prizmanın hedef ölçülerde keresteye dönüştürülmesi.

## Girdi

- Prizma

## Çıktı

- Kereste

## Fire

- Talaş

---

# MC-005 Boylama

## Amaç

Malzemeyi sipariş veya üretim ölçülerine hazırlamak.

## Girdi

- Kereste
- Lata

## Çıktı

- Boylanmış Malzeme

---

# MC-006 Kurutma Fırını

## Amaç

Malzemenin hedef nem değerine düşürülmesi.

## Girdi

- Yaş Kereste
- Yaş Lata

## Çıktı

- Fırınlı Kereste

## Kalite

- Nem
- Eğrilik
- Çatlak

---

# MC-007 Thermowood Fırını

## Amaç

Isıl modifikasyon işlemi.

## Girdi

- Fırınlı Kereste
- Kuru Lata

## Çıktı

- Thermowood Kereste

## Yakıt

Thermowood Talaşı

---

# MC-008 Ön Silim

## Amaç

Yüzey düzeltme.

## Girdi

- Fırınlı Kereste
- Thermowood Kereste

## Çıktı

- Düzgün Yüzey

---

# MC-009 Profil Makinesi

## Amaç

Nihai profil geometrisinin oluşturulması.

## Girdi

- Thermowood Kereste
- Sert Ağaç Kereste

## Çıktı

- Thermowood Profil
- Deck
- Lambri
- Cephe Profili
- Özel Profiller

---

# MC-010 Opticut

## Amaç

Budak ve kusurlu bölgelerin otomatik kesilmesi.

## Girdi

- Ön Silim Görmüş Lamel

## Çıktı

- Temiz Parçalar
- Fire

---

# MC-011 Finger Joint

## Amaç

Temiz kısa parçaların panel boyunda lamel haline getirilmesi.

## Girdi

- Temiz Kısa Parçalar

## Çıktı

- Finger Joint Lamel

---

# MC-012 4 Taraf Planya

## Amaç

Pres öncesi son ölçü hazırlığı.

## Girdi

- Solid Lamel
- Finger Joint Lamel

## Çıktı

- Pres Hazır Lamel

---

# MC-013 Tutkal Hattı

## Amaç

Lamellerin tutkallanması.

## Çıktı

- Tutkallı Lamel

---

# MC-014 Panel Presi

## Amaç

Lamellerin panel haline getirilmesi.

## Girdi

- Tutkallı Lamel

## Çıktı

- Masif Panel

---

# MC-015 Kalibrasyon

## Amaç

Panel kalınlığını hassas toleranslara getirmek.

---

# MC-016 Ebatlama

## Amaç

Sipariş ölçülerinde kesim.

---

# MC-017 Paketleme

## Amaç

Ürünün sevkiyata hazırlanması.

---

# MC-018 Kırıcı

## Amaç

Odun parçalarının küçültülmesi.

## Çıktı

Pelet Hammaddesi

---

# MC-019 Öğütücü

## Amaç

Pelet öncesi ince öğütme.

---

# MC-020 Talaş Kurutucu

## Amaç

Yaş talaşın kurutulması.

---

# MC-021 Pelet Presi

## Amaç

Pelet üretimi.

## Girdi

- Kuru Talaş
- Öğütülmüş Ahşap

## Çıktı

Pelet

---

# 5. Makine Kimlik Kuralları

Her makine benzersiz bir kod ile tanımlanacaktır.

Örnek:

- MC-001
- MC-002
- MC-003

Makine kodu sistem içerisinde değiştirilemez.

---

# 6. Gelecek Genişletmeler

İlerleyen sürümlerde her makine için aşağıdaki bilgiler de sisteme eklenecektir.

- PLC bağlantısı
- IoT sensörleri
- Enerji tüketimi
- OEE hesapları
- Duruş nedenleri
- Alarm kayıtları
- Tahmini bakım modeli
- Yapay zekâ destekli performans analizi
- Dijital ikiz entegrasyonu
