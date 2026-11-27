# Nexus Chat

**TCP/IP tabanlı, C# ve Windows Forms ile geliştirilmiş masaüstü sohbet uygulaması.**

Nexus Chat, **YMH219 – Nesneye Dayalı Programlama** dersi kapsamında geliştirilen bir istemci-sunucu (Client-Server) uygulamasıdır. İki uygulama örneğinin TCP bağlantısı kurarak gerçek zamanlı metin mesajları gönderip almasını sağlar.

> **Güncel durum:** Temel bire bir mesajlaşma akışı uygulanmış ve aynı bilgisayarda iki uygulama örneğiyle manuel olarak test edilmiştir. Bu sürüm eğitim amaçlıdır; grup sohbeti, kullanıcı kimlik doğrulaması ve şifreli iletişim kapsam dışındadır.

## Özellikler

- TCP/IP üzerinden istemci-sunucu iletişimi
- Uygulama içinden sunucuyu başlatma ve durdurma
- Sunucu IP adresi ve port bilgileriyle istemci bağlantısı kurma
- Kullanıcı adı belirleme
- Gerçek zamanlı mesaj gönderme ve alma
- Mesajlarda gönderen adını ve saat bilgisini görüntüleme
- Mesaj verilerini JSON formatında serileştirme ve çözümleme
- Asenkron ağ işlemleri (`async / await`)
- Bağlantı durumunu arayüzde gösterme ve bağlantıyı sonlandırma
- **Enter** ile mesaj gönderme
- **Shift + Enter** ile mesaj içinde yeni satır oluşturma
- Koyu temalı, yuvarlatılmış bileşenlere sahip Windows Forms arayüzü

## Kullanım kapsamı

Mevcut sürüm, **bir sunucu ve bir istemci arasında bire bir sohbet** senaryosunu hedefler. Aynı bilgisayarda uygulama iki kez açılarak yerel test yapılabilir.

Aşağıdaki özellikler mevcut sürümde bulunmamaktadır:

- Birden fazla istemciyle grup sohbeti
- Kullanıcı hesabı, parola veya kimlik doğrulama
- Mesajları veritabanına ya da dosyaya kalıcı olarak kaydetme
- TLS veya uygulama düzeyinde mesaj şifreleme
- Dosya gönderimi ve sesli/görüntülü görüşme

## Kullanılan teknolojiler

| Teknoloji | Kullanım amacı |
|---|---|
| C# | Uygulama programlama dili |
| .NET 10 | Uygulama platformu |
| Windows Forms | Masaüstü kullanıcı arayüzü |
| TCP/IP | İstemci-sunucu iletişimi |
| `System.Net.Sockets` | TCP sunucu ve istemci bağlantıları |
| `System.Text.Json` | Mesajların JSON serileştirmesi |
| `async / await` | Asenkron bağlantı ve iletişim işlemleri |
| Git & GitHub | Sürüm kontrolü ve kaynak kod yönetimi |
| Visual Studio 2026 | Geliştirme ortamı |

## Mimari ve mesaj akışı

Uygulama, kullanıcı arayüzü ile ağ iletişimi sorumluluklarını ayrı sınıflarda tutan basit bir Client-Server yapısı kullanır.

1. **Server**, seçilen port üzerinden TCP bağlantısı bekler.
2. **Client**, girilen IP adresi ve port bilgileriyle sunucuya bağlanır.
3. Mesaj, gönderen adı, benzersiz kimliği, UTC zaman damgası ve metin içeriğiyle birlikte bir `ChatMessage` nesnesi olarak oluşturulur.
4. Mesaj `System.Text.Json` ile JSON biçimine dönüştürülür ve TCP bağlantısı üzerinden aktarılır.
5. Alıcı taraf JSON verisini çözümler ve mesajı sohbet arayüzünde görüntüler.

Mesaj aktarımında UTF-8 metin akışı ve satır sonu ayıracı kullanılır. TCP bağlantısı **şifrelenmemiş düz metin (plaintext)** taşır; bu nedenle uygulama hassas bilgi göndermek için uygun değildir.

## Proje yapısı

```text
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
```

### Sınıfların sorumlulukları

| Dosya | Sorumluluk |
|---|---|
| `ChatMessage.cs` | Mesaj kimliği, gönderen, UTC zaman damgası ve metin içeriğini tutar; temel mesaj doğrulamalarını yapar. |
| `ChatServer.cs` | TCP sunucusunu başlatır, istemci bağlantısını kabul eder ve sunucuyu durdurur. |
| `ChatClient.cs` | Belirtilen IP adresi ve port üzerinden TCP istemci bağlantısı kurar. |
| `ChatConnection.cs` | Kurulan bağlantı üzerinden mesaj gönderme ve alma işlemlerini yönetir. |
| `ChatProtocol.cs` | Mesajların JSON serileştirme/çözümleme ve UTF-8 akış işlemlerini yürütür. |
| `Form1.cs` | Windows Forms arayüzünü ve kullanıcı işlemlerini yönetir. |
| `Program.cs` | Windows Forms uygulamasının başlangıç noktasıdır. |

## Kurulum ve çalıştırma

### Gereksinimler

- Windows işletim sistemi
- .NET 10 SDK
- Visual Studio 2026 (Windows Forms desteğiyle)

### 1. Depoyu klonlayın

PowerShell veya terminalde:

```powershell
git clone https://github.com/yaseminu0/YMH219-ChatApp.git
```

### 2. Çözüm klasörüne geçin

```powershell
cd YMH219-ChatApp/YMH219_ChatApp
```

### 3. Projeyi derleyin

```powershell
dotnet build YMH219_ChatApp.slnx -c Release
```

### 4. Uygulamayı çalıştırın

```powershell
dotnet run --project YMH219_ChatApp/YMH219_ChatApp.csproj
```

Alternatif olarak `YMH219_ChatApp.slnx` dosyasını Visual Studio ile açıp **Build > Build Solution** komutunu çalıştırabilir ve uygulamayı **F5** ile başlatabilirsiniz.

## Yerel kullanım ve test

Aynı bilgisayarda iki kullanıcıyı test etmek için uygulamayı iki ayrı pencerede çalıştırın:

1. İlk pencerede kullanıcı adını girin.
2. Port alanına varsayılan **5055** değerini bırakın veya kullanılabilir başka bir port seçin.
3. İlk pencerede **Sunucuyu Başlat** düğmesine tıklayın.
4. İkinci pencerede farklı bir kullanıcı adı girin.
5. Aynı bilgisayarda test ediyorsanız IP adresi olarak **127.0.0.1** yazın.
6. İkinci pencerede aynı port numarasını girip **Bağlan** düğmesine tıklayın.
7. Bağlantı kurulduğunda iki pencere arasında karşılıklı mesaj gönderin.
8. Bağlantıyı bitirmek için **Bağlantıyı Kes** düğmesini kullanın.

**Kontrol edilen temel senaryolar:**

- Sunucunun başlatılması ve istemcinin bağlanması
- İki uygulama penceresi arasında karşılıklı mesaj gönderimi
- Mesaj gönderen adının ve zaman bilgisinin görüntülenmesi
- **Shift + Enter** ile mesaj içinde yeni satır oluşturulması
- Bağlantının kesilmesi ve bağlantı durumunun arayüzde güncellenmesi

Bu kontroller geliştirme sırasında iki yerel uygulama örneğiyle manuel olarak yapılmıştır. README'de otomatik test paketi veya kapsamlı ağ/cihaz testi yapıldığı iddia edilmemektedir.

## Ağ ve güvenlik notları

- **127.0.0.1**, yalnızca aynı bilgisayardaki yerel testler içindir.
- Farklı bilgisayarlardan bağlantı için sunucu bilgisayarın uygun yerel ağ IP adresi kullanılmalıdır.
- İşletim sistemi güvenlik duvarı bağlantıyı engelleyebilir; ağ ve güvenlik duvarı ayarları kullanıcı tarafından ayrıca yapılandırılmalıdır.
- TCP iletişimi TLS ile korunmaz ve kullanıcı adları doğrulanmaz.
- Uygulamayı yalnızca yerel ortamda veya bilinçli olarak yapılandırılmış güvenilir bir ağda deneyin. Gerçek kullanıcı verileri veya gizli bilgiler göndermeyin.

## Geliştirici

**Yasemin Ubeyd**

GitHub: [@yaseminu0](https://github.com/yaseminu0)

Repository: [yaseminu0/YMH219-ChatApp](https://github.com/yaseminu0/YMH219-ChatApp)

---

Bu proje, Fırat Üniversitesi YMH219 – Nesneye Dayalı Programlama dersi kapsamında eğitim ve öğrenme amacıyla geliştirilmiştir.
