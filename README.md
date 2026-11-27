# Nexus Chat

**C# ve Windows Forms ile geliştirilmiş, TCP tabanlı masaüstü sohbet uygulaması.**

Nexus Chat, YMH219 Nesneye Dayalı Programlama dersi kapsamında geliştirilen bir istemci-sunucu (Client-Server) uygulamasıdır. Uygulama, TCP/IP üzerinden iki kullanıcı arasında doğrudan mesaj gönderip alınmasını sağlar.

## Proje Hakkında

Bu projenin amacı; C# ile Windows Forms arayüzü geliştirmeyi, TCP soket programlamayı, JSON tabanlı veri aktarımını ve asenkron programlama yaklaşımını uygulamalı olarak kullanmaktır.

Uygulama iki çalışma biçimini destekler:

- **Sunucu (Server):** Belirlenen port üzerinden bağlantı bekler.
- **İstemci (Client):** Sunucunun IP adresi ve port bilgilerini kullanarak bağlantı kurar.

Bağlantı kurulduktan sonra kullanıcılar gerçek zamanlı olarak mesaj gönderip alabilir.

## Özellikler

- TCP/IP üzerinden istemci-sunucu bağlantısı
- Sunucu başlatma ve sunucuya bağlanma
- Kullanıcı adı belirleme
- Gerçek zamanlı mesaj gönderme ve alma
- Mesajlarda gönderen adını ve gönderim saatini gösterme
- Mesajların JSON formatında serileştirilmesi ve çözümlenmesi
- Asenkron bağlantı ve mesajlaşma işlemleri (async / await)
- Bağlantı durumunu arayüzde görüntüleme
- Bağlantıyı sonlandırma
- **Enter** tuşuyla mesaj gönderme
- **Shift + Enter** ile mesaj içinde yeni satıra geçme
- Koyu temalı, yuvarlatılmış bileşenlere sahip Windows Forms arayüzü

## Kullanılan Teknolojiler

| Teknoloji | Kullanım Amacı |
|---|---|
| C# | Uygulama programlama dili |
| .NET 10 | Uygulama platformu |
| Windows Forms | Masaüstü kullanıcı arayüzü |
| TCP/IP | İstemci-sunucu iletişimi |
| System.Net.Sockets | TCP bağlantılarının yönetimi |
| JSON (System.Text.Json) | Mesaj verilerinin serileştirilmesi |
| async / await | Asenkron ağ işlemleri |
| Git & GitHub | Sürüm kontrolü ve proje paylaşımı |
| Visual Studio 2026 | Geliştirme ortamı |

## Proje Yapısı

~~~text
YMH219-ChatApp/
├── .gitignore
└── YMH219_ChatApp/
    ├── ChatClient.cs
    ├── ChatConnection.cs
    ├── ChatMessage.cs
    ├── ChatProtocol.cs
    ├── ChatServer.cs
    ├── YMH219_ChatApp.slnx
    └── YMH219_ChatApp/
        ├── Form1.cs
        ├── Form1.Designer.cs
        ├── Form1.resx
        ├── Program.cs
        └── YMH219_ChatApp.csproj
~~~

### Sınıfların Görevleri

| Dosya | Açıklama |
|---|---|
| ChatMessage.cs | Mesaj kimliği, gönderen, zaman bilgisi ve mesaj metnini tutar. |
| ChatServer.cs | TCP sunucusunu başlatır ve istemci bağlantısını kabul eder. |
| ChatClient.cs | Sunucuya TCP istemci bağlantısı kurar. |
| ChatConnection.cs | Kurulan bağlantı üzerinden mesaj gönderme ve alma işlemlerini yönetir. |
| ChatProtocol.cs | Mesajların JSON serileştirme ve serileştirmeden çıkarma işlemlerini yürütür. |
| Form1.cs | Kullanıcı arayüzünü ve arayüz ile ağ katmanı arasındaki işlemleri yönetir. |
| Program.cs | Windows Forms uygulamasının başlangıç noktasıdır. |

## Kurulum ve Çalıştırma

### Gereksinimler

- Windows işletim sistemi
- Visual Studio 2026
- .NET 10 SDK

### 1. Depoyu klonlayın

Terminal veya PowerShell üzerinden:

~~~powershell
git clone https://github.com/yaseminu0/YMH219-ChatApp.git
~~~

### 2. Çözüm dosyasını açın

Klonlanan klasörde aşağıdaki dizine gidin:

~~~text
YMH219-ChatApp/YMH219_ChatApp/
~~~

Visual Studio ile YMH219_ChatApp.slnx dosyasını açın.

### 3. Uygulamayı çalıştırın

Visual Studio'nun gerekli NuGet ve .NET bileşenlerini yüklemesini bekleyin. Ardından **Build > Build Solution** seçeneğiyle projeyi derleyin ve **F5** tuşuyla çalıştırın.

## Kullanım

Yerel bilgisayarda iki kullanıcıyla test etmek için uygulamayı iki kez çalıştırabilirsiniz.

1. İlk uygulama penceresinde kullanıcı adını girin.
2. Port numarasını belirleyin. Varsayılan port **5055**'tir.
3. İlk pencerede **Sunucuyu Başlat** düğmesine tıklayın.
4. İkinci uygulama penceresinde farklı bir kullanıcı adı girin.
5. Sunucu ile aynı bilgisayarda test yapıyorsanız IP adresi olarak 127.0.0.1 kullanın.
6. Aynı port numarasını girin ve **Bağlan** düğmesine tıklayın.
7. Bağlantı kurulduğunda iki pencere arasında mesaj gönderip alabilirsiniz.
8. Oturumu bitirmek için **Bağlantıyı Kes** düğmesini kullanın.

> **Not:** 127.0.0.1 yalnızca aynı bilgisayardaki yerel test içindir. Farklı bilgisayarlardan bağlantı kurmak için sunucunun ağ IP adresi kullanılmalıdır.

## Testler

Geliştirme sırasında aşağıdaki temel senaryolar kontrol edilmiştir:

- Sunucunun başlatılması ve istemcinin bağlanması
- İki uygulama penceresi arasında karşılıklı mesaj gönderimi
- Mesaj gönderen adının ve zaman bilgisinin görüntülenmesi
- **Shift + Enter** ile yeni satır oluşturulması
- Bağlantının kesilmesi ve bağlantı durumunun güncellenmesi

## Mevcut Kapsam ve Sınırlamalar

- Mevcut sürüm, **bir sunucu ve bir istemci arasında bire bir sohbet** için tasarlanmıştır.
- Grup sohbeti, kullanıcı hesabı doğrulama ve mesaj geçmişini kalıcı olarak saklama özellikleri bu sürümde bulunmamaktadır.
- TCP bağlantısı üzerinden aktarılan mesajlar için TLS veya uygulama düzeyinde şifreleme uygulanmamıştır. Bu nedenle uygulama hassas veya gizli bilgilerin aktarılması için tasarlanmamıştır.

## Geliştirici

**Yasemin Ubeyd**

GitHub: [@yaseminu0](https://github.com/yaseminu0)

---

Bu proje, YMH219 Nesneye Dayalı Programlama dersi kapsamında eğitim ve öğrenme amacıyla geliştirilmiştir.
