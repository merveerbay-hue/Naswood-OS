# Factory Overview

**Proje:** Naswood OS  
**Belge:** Factory Overview  
**Sürüm:** V1.0  
**Durum:** Aktif Geliştirme

---

# 1. Amaç

Bu doküman, Naswood'un üretim yapısını, üretim felsefesini ve malzeme akış mantığını tanımlar.

Bu belge yazılım dokümanı değildir.

Bu belge fabrikanın çalışma mantığını açıklayan ana referans dokümandır.

Veritabanı, API, kullanıcı ekranları ve yapay zekâ modelleri bu doküman referans alınarak geliştirilecektir.

---

# 2. Naswood Hakkında

Naswood; masif ahşap, Thermowood, masif panel, finger joint panel, pelet ve lamine ahşap ürünleri üreten, üretim süreçlerini dijitalleştirmeyi hedefleyen entegre bir ahşap üretim şirketidir.

Şirket, yalnızca ürün üretmeyi değil; tüm üretim süreçlerini, malzeme hareketlerini ve kalite verilerini dijital ortamda yönetmeyi hedeflemektedir.

---

# 3. Fabrikalar

## Bucak Fabrikası

Ana üretim tesisidir.

Başlıca üretim faaliyetleri:

- Tomruk İşleme
- Kereste Üretimi
- Kurutma
- Thermowood
- Profil Üretimi
- Solid Panel
- Finger Joint Panel
- Pelet Üretimi

---

## Antalya Fabrikası

İkinci üretim tesisidir.

Başlıca faaliyetler:

- CLT / CF / FJ üretimi (gelecekte genişletilecek)
- Lamine Üretimi
- CNC İşleme
- Boyama
- Nihai Ürün Hazırlama

Her iki fabrika aynı Naswood OS altyapısını kullanacaktır.

---

# 4. Üretim Felsefesi

Naswood'da üretim doğrusal bir hat değildir.

Her malzeme;

- satış ürünü olabilir,
- yarı mamul olabilir,
- başka bir prosese yönlendirilebilir,
- tekrar üretime alınabilir,
- farklı üretim rotalarından geçebilir.

Bu nedenle sistem ürün odaklı değil, **malzeme odaklı** tasarlanacaktır.

---

# 5. Malzeme Yaşam Döngüsü

Naswood OS'un temel prensibi;

> **Her malzeme sisteme bir kez girer ve fabrikadan çıkıncaya kadar sürekli izlenir.**

Malzeme;

- dönüşebilir,
- bölünebilir,
- birleşebilir,
- tekrar üretime alınabilir,
- farklı ürünlere dönüşebilir.

Hiçbir fiziksel hareket kayıt dışı değildir.

---

# 6. Üretim Yaklaşımı

Üretim süreçleri sabit değildir.

Aynı malzeme farklı üretim rotalarını izleyebilir.

Örneğin;

- Fırınlı kereste satılabilir.
- Thermowood hattına gönderilebilir.
- Solid panel üretiminde kullanılabilir.
- Finger Joint üretimine yönlendirilebilir.

Üretim rotası yazılım tarafından zorlanmaz.

Karar, üretim planlama ve üretim sorumluları tarafından verilir.

Naswood OS yalnızca izin verilen üretim rotalarını yönetir ve kayıt altına alır.

---

# 7. Ürün Grupları

Başlıca ürün grupları:

- Thermowood Kereste
- Thermowood Profil
- Thermowood Panel
- Fırınlı Kereste
- Solid Panel
- Finger Joint Panel
- Lamine Ürünler
- Pelet
- Kesilmiş Lata

---

# 8. Hammadde Grupları

Fabrikaya aşağıdaki hammaddeler girebilir.

- Tomruk
- Yaş Kereste
- Fırınlı Kereste
- Yaş Lata
- Kuru Lata
- İthal Kereste
- İthal Lata

Her hammadde sisteme "Material Lot" olarak kaydedilir.

---

# 9. Temel Üretim Prensipleri

Naswood üretim sisteminde aşağıdaki prensipler geçerlidir.

- Bir malzeme hem satış ürünü hem yarı mamul olabilir.
- Aynı malzeme birden fazla üretim hattında kullanılabilir.
- Üretim sırasında rota değişebilir.
- Fire en aza indirilmeye çalışılır.
- Yan ürünler mümkün olduğunca geri kazanılır.
- Üretim kararları verim odaklı alınır.

---

# 10. İzlenebilirlik

Naswood OS'un en önemli amacı tam izlenebilirlik sağlamaktır.

Her malzeme için aşağıdaki sorular cevaplanabilmelidir.

- Hangi tedarikçiden geldi?
- Hangi lota ait?
- Hangi makinede işlendi?
- Hangi operatör çalıştı?
- Hangi kalite kontrolünden geçti?
- Ne kadar fire oluştu?
- Hangi müşteriye sevk edildi?

---

# 11. Fire ve Geri Kazanım

Naswood'da oluşan her fire kayıt altına alınacaktır.

Fireler;

- Talaş
- Kabuk
- Yonga
- Kısa Parça
- Kusurlu Lamel
- Diğer Ahşap Atıkları

olarak sınıflandırılır.

Geri kazanım prensipleri:

- Kısa parçalar Finger Joint üretiminde değerlendirilir.
- Standart talaş pelet üretiminde kullanılır.
- Thermowood talaşı pelet üretiminde kullanılmaz; Thermowood fırınlarında yakıt olarak değerlendirilir.
- Yan ürünlerin tamamı stok olarak izlenir.

---

# 12. Dijital Dönüşüm Vizyonu

Naswood OS'un amacı yalnızca stok takibi yapmak değildir.

Sistem;

- üretim süreçlerini dijitalleştirmeyi,
- tam izlenebilirlik sağlamayı,
- verim analizleri yapmayı,
- fireleri azaltmayı,
- maliyetleri doğru hesaplamayı,
- kaliteyi standartlaştırmayı,
- yapay zekâ destekli karar mekanizmaları oluşturmayı hedeflemektedir.

Uzun vadede Naswood OS, fabrikanın dijital ikizi (Digital Twin) olarak çalışacaktır.
