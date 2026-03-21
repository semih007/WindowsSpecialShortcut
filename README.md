# WindowsSpecialShortcut

Akıllı Kısayol Yöneticisi

Bu proje, Windows'ta servis + exe eşleştirmesini yönetmek, doğru sıralama ile "Başlat" ve "Durdur" iş akışlarını güvenli şekilde çalıştırmak için tasarlanmıştır.

## Temel Özellikler

- Servis + exe eşleştirmeleri ekleme/silme (config.json) - Bir exe ile birden fazla servis desteklenir.
- Senkronize Başlatma: servisler Running olana kadar 5sn boyunca bekleme, sonra exe başlatma
- Senkronize Durdurma: exe süreçlerini önce nazik kapatma, gerekirse zorla kill, sonra servisleri ters sırada durdurma
- Masaüstüne .lnk kısayol oluşturma (toggle: açık ise kapat, kapalı ise aç)
- Yönetici (Administrator) yetkisi zorunlu, UAC manifest açıklamalı
- Çevrimdışı (offline-only), telemetri/yazılım güncelleme yok

## Yapı

- `SmartShortcutManager/SmartShortcutManager.csproj`
- `SmartShortcutManager/Program.cs`
- `SmartShortcutManager/Form1.cs`
- `SmartShortcutManager/Form1.Designer.cs`
- `SmartShortcutManager/app.manifest`
- `SmartShortcutManager/ShortcutConfig.cs`

## Nasıl Çalıştırılır

1. Visual Studio ile çözümü açın.
2. `SmartShortcutManager` projesini hedefleyin.
3. Uygulamayı yönetici haklarıyla çalıştırın (required).
4. Servis adlarını virgülle ayrılmış girin (ör: pangpg, başkaServis), exe yolu girip "Eşleştirme Ekle".
5. Seçili satır üzerinden "Başlat", "Durdur", "Kısayol Oluştur".
6. Masaüstü kısayolu toggle olarak çalışır: açık ise kapatır, kapalı ise açar.

## Özel Test Hedefi

- Uygulama: `globalproject.exe`
- Servis: `pangpg`

## GitHub Actions

Aşağıdaki dosya oluşturuldu:
- `.github/workflows/dotnet-build.yml`

CI: `dotnet build` ve otomatik çözüm doğrulama.
