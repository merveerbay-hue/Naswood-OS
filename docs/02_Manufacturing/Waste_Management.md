# Waste Management

**Project:** Naswood OS  
**Document:** Waste Management  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Amaç

Bu doküman üretim sırasında oluşan;

- fireleri,
- yan ürünleri,
- geri kazanılabilir malzemeleri,
- bertaraf edilecek atıkları

tanımlar.

Naswood OS'un amacı fireyi yalnızca kayıt altına almak değil, mümkün olan her malzemeyi tekrar üretime veya ekonomik değere dönüştürmektir.

---

# 2. Temel Prensip

Naswood'da oluşan her ahşap parçası aşağıdaki dört gruptan birine girer.

- Nihai Ürün
- Yarı Mamul
- Yan Ürün
- Fire

Fire oluştuğunda sistem bunun;

- neden oluştuğunu,
- miktarını,
- hangi operasyonda oluştuğunu,
- ekonomik olarak değerlendirilebilir olup olmadığını

kayıt altına alır.

---

# 3. Atık Sınıfları

## 3.1 Geri Kazanılabilir

- Kısa Parçalar
- Kusursuz Kesilmiş Parçalar
- Talaş
- Yonga
- Kabuk

---

## 3.2 Geri Kazanılamayan

- Çürük
- Yanık
- Küf
- Kirlenmiş Atık
- Kullanılamaz Ahşap

---

# 4. Yan Ürün Yönetimi

Üretim sırasında oluşan ekonomik değeri olan malzemeler yan ürün olarak değerlendirilir.

Örnekler

- Talaş
- Kabuk
- Yonga
- Kısa Parça

Yan ürünler stok olarak takip edilir.

---

# 5. Recovery Yönetimi

Recovery; üretim sırasında oluşan uygun parçaların tekrar üretime alınmasıdır.

Örnek

Solid Panel

↓

Kısa Parça

↓

Finger Joint

↓

Yeni Lamel

↓

Panel

Bu işlem fire değil, geri kazanımdır.

---

# 6. Talaş Yönetimi

Standart talaş;

- stoklanır,
- kurutulur (gerekiyorsa),
- pelet üretiminde kullanılır.

Her talaş oluşumu kayıt altına alınır.

---

# 7. Thermowood Talaşı

Thermowood talaşı pelet üretiminde kullanılmaz.

Sebep

Isıl işlem görmüş ahşabın pelet kalitesini olumsuz etkilemesi.

Kullanım

Thermowood fırınlarında yakıt olarak değerlendirilir.

---

# 8. Kabuk Yönetimi

Kabuk;

işletme kararına bağlı olarak;

- yakıt,
- pelet hammaddesi,
- satış

amacıyla değerlendirilebilir.

---

# 9. Yonga Yönetimi

Yonga;

- pelet,
- yakıt,
- diğer endüstriyel kullanımlar

için stoklanabilir.

---

# 10. Fire Nedenleri

Her fire aşağıdaki nedenlerden biri ile ilişkilendirilir.

- Budak
- Çatlak
- Eğrilik
- Dönüklük
- Lif Kalkması
- Ölçü Hatası
- Operatör Hatası
- Makine Ayarsızlığı
- Yanlış Reçete
- Yanlış Nem
- Diğer

---

# 11. Fire Kaydı

Her fire kaydında aşağıdaki bilgiler tutulacaktır.

- Fire ID
- Material ID
- Operation
- Machine
- Operator
- Quantity
- Unit
- Reason
- Recoverable
- Recovery Process
- Date

---

# 12. Fire Maliyeti

Her fire maliyet hesabına dahil edilir.

Sistem;

- malzeme maliyeti,
- işçilik,
- enerji,
- operasyon maliyeti

ile ilişkilendirerek gerçek fire maliyetini hesaplar.

---

# 13. KPI'lar

Sistem aşağıdaki göstergeleri hesaplar.

- Fire %
- Recovery %
- Yield %
- Pelet Verimi
- Recovery Kazancı
- Fire Maliyeti
- Fire Nedeni Dağılımı

---

# 14. AI Analizleri

AI aşağıdaki analizleri yapacaktır.

- Fire Tahmini
- Makine Bazlı Fire Analizi
- Operatör Bazlı Fire Analizi
- Recovery Optimizasyonu
- Fire Sebep Analizi
- Üretim Verim Analizi

---

# 15. Temel Kurallar

- Her fire kayıt altına alınır.
- Recoverable malzemeler ayrı sınıflandırılır.
- Recovery işlemleri Parent → Child ilişkisini korur.
- Thermowood talaşı pelet hattına gönderilemez.
- Fire stoktan kaybolamaz.
- Yan ürünler ekonomik değer taşıyorsa stok olarak izlenir.
- Tüm fire hareketleri izlenebilirlik zincirinin bir parçasıdır.
