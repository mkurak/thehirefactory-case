<div align="center">
	<h1>TheHireFactory E-Commerce Case</h1>
	<p>Modern, katmanlı mimariye sahip, .NET tabanlı e-ticaret API projes (case)i</p>
</div>

---

# İçindekiler

+ [Proje Hakkında](#proje-hakkında)
    + [Amaç](#amaç)
    + [Kapsam](#kapsam)
    + [Kullanılan Teknolojiler](#kullanılan-teknolojiler)
    + [Mimari Yaklaşım](#mimari-yaklaşım)
+ [Dizin Yapısı](#dizin-yapısı)
    + [API](#api)
    + [Domain](#domain)
    + [Infrastructure](#infrastructure)
    + [Tests](#tests)
+ [Kurulum](#kurulum)
    + [Gereksinimler](#gereksinimler)
    + [Repo Clone](#repo-clone)
    + [Dev Container](#dev-container)
    + [Docker Compose](#docker-compose)
        + [Watch Özellikli Compose](#watch-özellikli-compose)
    + [İlk Derleme](#ilk-derleme)
+ [Veritabanı İşlemleri](#veritabanı-işlemleri)
    + [Migration](#migration)
    + [Seed](#seed)
+ [Çalıştırma](#çalıştırma)
    + [API Start](#api-start)
        + [HTTPS](#https)
    + [Swagger Erişimi](#swagger-erişimi)
    + [Health-Check](#health-check)
+ [Testler](#testler)
    + [Integration Test Yaklaşım](#integration-test-yaklaşım)
    + [Unit Test Yaklaşımı](#unit-test-yaklaşımı)
+ [Logging ve Exception Handling](#logging-ve-exception-handling)
    + [Serilog](#serilog)
    + [ProblemDetails Middleware](#problemdetails-middleware)
+ [Swagger / API Dokümantasyonu](#swagger-api-dokümantasyonu)
    + [Swagger UI Erişimi](#swagger-ui-erişimi)
    + [Endpoint Örnekleri](#endpoint-örnekleri)
+ [Faydalı Komutlar](#faydalı-komutlar)
+ [Katkı ve Geliştirme Notları](#katkı-ve-geliştirme-notları)

# Proje Hakkında

## Amaç
Bu proje, temel bir **E-Ticaret Mikroservis Uygulaması** geliştirme amacıyla hazırlanmıştır.  
Hedef; ürün, kategori ve sipariş gibi e-ticaretin çekirdek alanlarını kapsayan,  
geliştirilebilir ve test edilebilir bir altyapı sunmaktır.  

Case senaryosunun amacı ise:  
- Adayın **.NET 8**, **Entity Framework Core**, **integration testing** ve **clean architecture** yaklaşımlarına hakimiyetini ölçmek,  
- Kod kalitesini, dokümantasyon alışkanlıklarını ve kurumsal proje standartlarına uyumunu değerlendirmektir.  

## Kapsam
Proje şu bileşenleri kapsar:  
- **API Katmanı (TheHireFactory.ECommerce.Api)**  
  - Ürün CRUD işlemleri, kategori yönetimi, health-check endpoint’i.  
  - Swagger UI üzerinden otomatik dokümantasyon.  
- **Domain Katmanı (TheHireFactory.ECommerce.Domain)**  
  - Temel entity tanımları (Product, Category vb.)  
  - Repository pattern için kullanılan arayüzler.  
- **Infrastructure Katmanı (TheHireFactory.ECommerce.Infrastructure)**  
  - EF Core tabanlı veritabanı erişimi.  
  - Migration ve seed mekanizması.  
- **Tests Katmanı (TheHireFactory.ECommerce.Tests)**  
  - Unit testler (domain logic)  
  - Integration testler (in-memory SQLite, custom `WebApplicationFactory`)  

Bu sayede proje hem lokal geliştirme, hem de CI/CD süreçlerine uygun olacak şekilde kurgulanmıştır.  

## Kullanılan Teknolojiler
- **.NET 8** – Modern, hızlı ve cross-platform API geliştirme.  
- **Entity Framework Core 8 (EF Core)** – ORM, migration ve LINQ desteği.  
- **SQLite (In-Memory)** – Integration testlerde hafif ve hızlı veritabanı çözümü.  
- **MSSQL** – Gerçek geliştirme ortamı için varsayılan veritabanı.  
- **xUnit & FluentAssertions** – Test framework ve assertion kütüphanesi.  
- **Serilog** – Loglama çözümü (console sink).  
- **Swagger (Swashbuckle)** – API dokümantasyonu ve test arayüzü.  
- **FluentValidation** – API DTO’ları için validation kuralları.  

## Mimari Yaklaşım
Proje **Clean Architecture** ve **katmanlı mimari** prensipleri esas alınarak geliştirilmiştir:  

- **Domain**: Saf iş kurallarını içerir, hiçbir bağımlılığı yoktur.  
- **Infrastructure**: EF Core implementasyonu ve dış bağımlılıkları barındırır.  
- **API**: Uygulamanın giriş noktasıdır. Controller’lar, DTO mapping ve middleware burada bulunur.  
- **Tests**: Hem unit hem de integration testleri içerir.  

Katmanlar arası bağımlılık yönü daima **Domain → Infrastructure → API** şeklinde korunur.  
Böylece yüksek test edilebilirlik ve düşük bağımlılık sağlanır.  

# Dizin Yapısı

Proje **katmanlı mimari** yapısına uygun olacak şekilde organize edilmiştir.  
Her bir katman bağımsızdır ve sorumlulukları ayrıştırılmıştır.

```bash
source/
├── TheHireFactory.ECommerce.Api/                       # API katmanı
│   ├── Controllers/                                    # REST API Controller sınıfları
│   ├── Dtos/                                           # API giriş/çıkış modelleri (DTO)
│   ├── Mappings/                                       # AutoMapper profilleri
│   ├── Middlewares/                                    # Global Exception Handler vb.
│   ├── Program.cs                                      # Uygulamanın başlangıç noktası
│   └── ...
│
├── TheHireFactory.ECommerce.Domain/                    # Domain katmanı (iş kuralları)
│   ├── Abstractions/                                   # Repository arayüzleri
│   ├── Entities/                                       # Domain entity sınıfları
│   └── ...
│
├── TheHireFactory.ECommerce.Infrastructure/            # Altyapı katmanı
│   ├── Data/                                           # DbContext, Migration, Seed
│   ├── Repositories/                                   # Repository implementasyonları
│   ├── DependencyInjection.cs                          # Service registration (AddInfrastructure)
│   └── ...
│
└── Tests/                                              # Test projeleri
    ├── TheHireFactory.ECommerce.Tests.Unit/            # Unit testler
    └── TheHireFactory.ECommerce.Tests.Integration/     # Integration testler
        ├── TestWebApplicationFactory.cs                # Custom WebApplicationFactory
        └── ProductApiTests.cs                          # Örnek integration testleri
```

## API

- HTTP endpoint’lerini barındırır.
- Controller → DTO → Service/Repository zinciri üzerinden çalışır.
- Swagger dokümantasyonu bu katmanda üretilir.

## Domain

- İş kuralları ve entity tanımları bulunur.
- Hiçbir dış bağımlılık içermez.
- Örneğin: Product, Category entity’leri ve IProductRepository arayüzü.

## Infrastructure

- Veritabanı erişimi ve repository implementasyonları burada yer alır.
- ECommerceDbContext, migration işlemleri ve seed verileri bu katmandadır.
- AddInfrastructure extension metodu ile bağımlılıklar API katmanına eklenir.

## Tests

- **Unit Tests:** Domain katmanındaki iş kurallarını test eder.
- **Integration Tests:**
    - Custom WebApplicationFactory ile API gerçekçi şekilde ayağa kaldırılır.
    - EF Core SQLite in-memory DB kullanılır.
    - Endpoint’lerin uçtan uca davranışı test edilir.

# Kurulum

Proje geliştirme ortamı **VSCode Dev Container** üzerine kurgulanmıştır.  
Yerel cihazınızda sadece **Git**, **Docker** ve **VSCode** bulunması yeterlidir.  
Diğer tüm bağımlılıklar container içinde sağlanır.

## Gereksinimler

- [Git](https://git-scm.com/downloads)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Visual Studio Code](https://code.visualstudio.com/)
- [Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)

## Repo Clone

```bash
git clone https://github.com/mkurak/thehirefactory-case.git
cd thehirefactory-case
```

## Dev Container

VSCode üzerinden:
- F1 → Dev Containers: Reopen in Container seçin
- İlk açılışta bağımlılıklar otomatik olarak container içinde kurulacaktır
    - .NET SDK
    - EF Core CLI

> Not: Veritabanı container dışındaki docker-compose.yml dosyası ile ayağa kaldırılır.

```bash
docker-compose up -d
```

## Docker Compose

Bağımlılıklar için projede bir Docker Compose dosyası bulunmaktadır. Bu dosya projenin derlenmiş sürümünü çalıştırır ve içerisinde MSSQL Server bulunmaktadır. Şuan ki yapılandırılmış dosya, compose'un dev container dışında çalıştırılacağı şekilde ayarlanmıştır. Böylece dev container dışında da kullanılabilmektedir.

Eğer compose dosyasını container içerisinde çalıştırmak isterseniz muhakkak adresleri güncellemeli ve örneğin "localhost" yerine servis ismini kullanmalısınız (compose içerisindeki servis adı).

Compose dosyasını aşağıdaki komutla çalıştırabilirsiniz.

```bash
docker-compose up -d --build
```

Daha önce ayağa kaldırılmış compose'u kapatmak için;

```bash
docker-compose down
```

### Watch Özellikli Compose

Normal compose dosyası olan "***docker-compose.yml***" dosyası projenin build edilmiş yapısını çalıştırır. Ancak geliştirme yaparken yapılan değişikliklerin hemen yansıması gerekiyorsa (aktif geliştirme yaparken muhakkak gerekli olacaktır) içerisindeki watch özelliği ile hot reload özelliği sağlayan "***docker-compose.watch.yml***" dosyasını kullanabilirsiniz.

```bash
docker-compose -f docker-compose.watch.yml up -d --build
```

Eğer bundan önce compose dosyasını çalıştırmışsanız (watch özelliği olmayan normal compose) kapatarak watch özellikli olanına geçmek için;

```bash
# önce ayakta olan compose'u kapat
docker-compose down

# sonra watch özelliği olanını ayağa kaldır
docker-compose -f docker-compose.watch.yml up -d --build
```

> Mevcut compose dosyalarında sadece MSSQL Server bulunmaktadır. Kullanıcı bilgileri ise "sa" ve şifresi de şuan için "user123TEST." şeklindedir.

<a id="ilk-derleme"></a>
## İlk Derleme

Container içerisindeyken projeyi derlemek için aşağıdaki komutları kullanabilirsiniz.

> Projenin SLN dosyası kök dizinde olmadığı için bu komutların ***./source*** içerisinde çalıştırılması gerekmektedir.

```bash
# SLN dosyasının olduğu dizine git
cd source

# paketleri yükle
dotnet restore

# projeyi derle
dotnet build
```

<a id="veritabanı-işlemleri"></a>
# Veritabanı İşlemleri

Proje Entity Framework Core kullanarak MSSQL üzerinde çalışır. Veritabanı şeması **migration** mekanizması ile yönetilir ve başlangıç verileri (ör. kategoriler) için **seed** işlemi yapılır.

## Migration

Migration’lar veritabanı şemasındaki değişiklikleri yönetmek için kullanılır.

Yeni bir migration eklemek için:

```bash
dotnet ef migrations add <MigrationName> 
    --project source/TheHireFactory.ECommerce.Infrastructure 
    --startup-project source/TheHireFactory.ECommerce.Api
```

Migration’ı veritabanına uygulamak için:

```bash
dotnet ef database update 
    --project source/TheHireFactory.ECommerce.Infrastructure 
    --startup-project source/TheHireFactory.ECommerce.Api
```

Migration’ları silmek isterseniz:

```bash
dotnet ef migrations remove 
    --project source/TheHireFactory.ECommerce.Infrastructure 
    --startup-project source/TheHireFactory.ECommerce.Api
```

> Not: Migration komutlarının çalışması için dotnet-ef tool’unun yüklü olması gerekir:

```bash
dotnet tool install --global dotnet-ef
```

## Seed

İlk kurulumda temel veriler (ör. “Default” kategorisi) otomatik olarak eklenir.
Bu işlem Program.cs içindeki SeedData.EnsureSeedAsync metodu üzerinden çalışır.

Eğer seed işlemini kapatmak isterseniz appsettings.json veya environment variable üzerinden:

```bash
"DisableSeed": true
```

veya doğrudan migration sonrası uygulama açıldığında otomatik çalışacaktır.

# Çalıştırma

Proje geliştirme ortamında **Dev Container** içerisinde çalıştırılır. API ayağa kalktığında Swagger arayüzü ve health-check endpoint’i üzerinden doğrulama yapılabilir.

## API Start

API’yi başlatmak için:

```bash
dotnet run --project source/TheHireFactory.ECommerce.Api
```

Eğer Dev Container içinde çalışıyorsanız, servisler ayağa kalktıktan sonra otomatik olarak http://localhost:8080 adresinden erişebilirsiniz.

> Dikkat: MSSQL server'ın ayağa kalkması biraz zaman alabilir. Bu nedenle compose'u up yaptıktan sonra bir süre bekleyin. Çünkü MSSQL ayağa kalkmadan uygulama da ayağa kalkmayacak ve bekleyecektir. Çünkü uygulama başlangıcında otomatik migration işlemi yapılmaktadır ve bunun için aktif MSSQL bağlantısına ihtiyaç duyar.

Port ayarlaması şuan ki yapılandırma için compose dosyasında tutulmaktadır;

```yml
name: thehirefactory
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    platform: linux/amd64
    container_name: thehirefactory-sql
    environment:
      ACCEPT_EULA: "Y"
      SA_PASSWORD: "user123TEST."
      MSSQL_PID: "Developer"
    ports:
      - "1433:1433"
    volumes:
      - mssql_data:/var/opt/mssql
    healthcheck:
      test: [ "CMD", "/opt/mssql-tools18/bin/sqlcmd", "-S", "localhost", "-U", "sa", "-P", "user123TEST.", "-Q", "SELECT 1", "-C" ]
      interval: 5s
      timeout: 3s
      retries: 30

  api:
    build:
      context: ./source
      dockerfile: TheHireFactory.ECommerce.Api/Dockerfile
    container_name: thehirefactory-api
    environment:
      ASPNETCORE_URLS: "http://+:8080" # <-- burada port belirtiliyor.
      DB_CONNECTION: "Server=sqlserver;Database=ECommerceDb;User Id=sa;Password=user123TEST.;TrustServerCertificate=True;"
    ports:
      - "8080:8080" # <-- burada da port dışarıya açılıyor (container için)
    depends_on:
      sqlserver:
        condition: service_healthy

volumes:
  mssql_data:
```

Eğer port bilgisini değiştirmek isterseniz buradaki bilgileri değiştirebilirsiniz (aynı şekilde watch compose'da da). Fakat başka alternatifler de vardır;

```bash
Varsayılan olarak API şu portlarda açılır:
- HTTP: http://localhost:8080

Port değiştirmek için:
- `ASPNETCORE_URLS` environment variable’ını ayarlayın:

```bash
export ASPNETCORE_URLS="http://+:8080"
dotnet run --project source/TheHireFactory.ECommerce.Api
```

> veya Properties/launchSettings.json dosyasında "applicationUrl" alanını güncelleyin.

### HTTPS

Şuan ki projede HTTPS aktif değil.
Eğer aktifleştirmek isterseniz şu adımları uygulamanız gerekmektedir;

İlk olarak compose dosyasında(larda) aşağıdaki değişiklikleri yapmanız gerekmektedir;

```yml
name: thehirefactory
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    platform: linux/amd64
    container_name: thehirefactory-sql
    environment:
      ACCEPT_EULA: "Y"
      SA_PASSWORD: "user123TEST."
      MSSQL_PID: "Developer"
    ports:
      - "1433:1433"
    volumes:
      - mssql_data:/var/opt/mssql
    healthcheck:
      test: [ "CMD", "/opt/mssql-tools18/bin/sqlcmd", "-S", "localhost", "-U", "sa", "-P", "user123TEST.", "-Q", "SELECT 1", "-C" ]
      interval: 5s
      timeout: 3s
      retries: 30

  api:
    build:
      context: ./source
      dockerfile: TheHireFactory.ECommerce.Api/Dockerfile
    container_name: thehirefactory-api
    environment:
      # HTTP + HTTPS listener’lar
      ASPNETCORE_URLS: "http://+:8080;https://+8443"

      # Kestrel default sertifika ayarı
      ASPNETCORE_Kestrel__Certificates__Default__Path: "/https/thf-dev.pfx"
      ASPNETCORE_Kestrel__Certificates__Default__Password: "thfpass"

      DB_CONNECTION: "Server=sqlserver;Database=ECommerceDb;User Id=sa;Password=user123TEST.;TrustServerCertificate=True;"
    ports:
      - "8080:8080" # HTTP
      - "8443:8443" # HTTPS
    depends_on:
      sqlserver:
        condition: service_healthy

volumes:
  mssql_data:
```

Bu değişiklikler ile Kestrel'e yeni bir port bilgisi vermiş olursunuz (ASPNETCORE_URLS). Ayrıca oluşturulacak olan container'ın 8443 portunu da dışarıya açmış olursunuz. Bunu yapmazsanız cihazınızdan bu container'ın bu portuna erişemezsiniz (***- "8443:8443" # HTTPS***)

Sonraki adımda da yukarıda belirttiğiniz sertifikayı oluşturmanız gerekmektedir.
Bunu dotnet üzerinden üretebilirsiniz.

```bash
mkdir -p certs
# .NET dev sertifikasını PFX olarak export et
dotnet dev-certs https -ep ./certs/thf-dev.pfx -p thfpass
# (Mac/Windows’ta güvenmek için)
dotnet dev-certs https --trust
```

> PFX adı/parolası örnek: thf-dev.pfx / thfpass (istediğin gibi değiştirebilirsin).

> ⚠️ Yukarıdaki komutları kök dizinde çalıştırmanız gerekmektedir. Çünkü compose içerisindeki yol kök dizine bakmaktadır.

Son olarak aşağıdaki kodu ***Program.cs*** içerisine ekleyebilirsiniz fakt zorunlu değildir. Çünkü Kestrel yukarıdaki environment anahtarlarını otomatik okur ve 8443 portunda TLS'i açar.

```csharp
// HTTPS listener’ı da açtıysan (8443), istersen redirect ekle:
app.UseHttpsRedirection();
```

## Swagger Erişimi

Swagger arayüzü proje çalıştığında otomatik olarak aktiftir.
Tarayıcıdan aşağıdaki adrese giderek API endpointlerini inceleyebilirsiniz:

```bash
curl http://localhost:5000/swagger
```

Burada:

- Tüm controller ve action metodları listelenir,
- Request/Response örnekleri görülür,
- Parametreler ve DTO yapıları test edilebilir.

## Health-Check

API’nin ayakta olup olmadığını doğrulamak için health endpoint kullanılabilir:

```bash
curl http://localhost:8080/health

# eğer https aktifse
curl http://localhost:8443/health
```

> ⚠️ Yukarıdaki adreslerdeki port mevcut yapılandırmaya göre verilmiştir (https şuan aktif değildir). Port bilgisini değiştirdiğinizde adresleri de buna göre değerlendirmelisiniz.

Sistem ayaktaysa health-check çıktısı şu şekilde olacaktır;

```json
{
  "status": "ok",
  "time": "2025-09-16T21:25:00+00:00"
}
```

# Testler

Bu projede iki düzeyde test yaklaşımı benimsendi: integration (uçtan uca API + EF Core + gerçek DI kablolarıyla) ve unit (tekil sınıfların izole testi). Aşağıda her birinin amacı, mimarisi, bağımlılıkları ve çalıştırma komutları yer alıyor.

## Integration Test Yaklaşım

***Amaç?***

Gerçek uygulama varlıklarını (routing, middleware, filtreler, DI grafiği, AutoMapper, FluentValidation, ProblemDetails, Serilog vb.) kullanarak HTTP seviyesinde davranışı doğrulamak. EF Core için SQLite in-memory kullanarak veritabanı etkileşimini de senaryoya dahil etmek.

***Neden SQLite (In-Memory)?:***

- EF Core’un UseInMemoryDatabase sağlayıcısı relational özellikleri (foreign key, unique index, transaction kapsamı, gerçek migration davranışı) birebir simüle etmez.
- SQLite in-memory ise ilişkisel kısıtları uygular ve prod’daki SQL Server’a daha yakın davranış verir (dialect farklı olsa da).
- Testler daha gerçekçi; “prod’a yakınlık” artar.

***Özet Mimari***

- TestWebApplicationFactory : WebApplicationFactory<Program> sınıfı, test host’u Testing environment’ında ayağa kaldırır.
- Program.cs içinde, builder.Environment.IsEnvironment("Testing") ise DB kablolaması ve migrate/seed devre dışı bırakılır (test, kendi bağlayacak).
- ConfigureWebHost içinde:
    - Prod’daki DbContextOptions<ECommerceDbContext> kayıtları çözer.
    - Tek bir açık SqliteConnection("DataSource=:memory:") oluşturulur (bağlantı açık kaldığı sürece DB yaşar).
    - services.AddDbContext<ECommerceDbContext>(opt => opt.UseSqlite(_connection)) ile test DB’si bağlanır.
    - db.Database.EnsureCreated() (veya migrate) çalıştırılır.
    - Minimum seed (ör. Category { Id=1, Name="Default" }) yapılır.
    - DisableSeed=true konfig ile Program.cs içindeki retry’li migrate+seed bloğu kapatılır.
- ProductApiTests sınıfı IClassFixture<TestWebApplicationFactory> ile HttpClient üretir ve /health, /api/product endpoint'lerini gerçek HTTP isteğiyle test eder.
- Testing ortamında ayrıntılı hata görmek için app.UseDeveloperExceptionPage() aktif.

***Dosyalar (Integration)***

- source/Tests/TheHireFactory.ECommerce.Tests.Integration/TestWebApplicationFactory.cs
- source/Tests/TheHireFactory.ECommerce.Tests.Integration/ProductApiTests.cs

***Başlıca Senaryolar:***

- GET /health → 200
- GET /api/product → boşken 200 + []
- POST /api/product → 201 + created body
- GET /api/product/{id} → olmayan için 404

***Çalıştırma Komutları***

```bash
# Tüm testler
dotnet test

# Yalnızca integration test projesi
dotnet test source/Tests/TheHireFactory.ECommerce.Tests.Integration

# Belirli bir test sınıfı
dotnet test source/Tests/TheHireFactory.ECommerce.Tests.Integration \
  --filter "FullyQualifiedName~TheHireFactory.ECommerce.Tests.Integration.ProductApiTests"

# Belirli bir test adı
dotnet test source/Tests/TheHireFactory.ECommerce.Tests.Integration \
  --filter "TestName=PostProduct_Should_Create_And_Return_Product"

# Çıkış ayrıntılı
dotnet test source/Tests/TheHireFactory.ECommerce.Tests.Integration -v m
```

> ⚠️ Yukarıdaki komutlar /source dizininde çalıştırılmalıdır.

***Sık Karşılaşılan Hatalar & İpuçları:***

- InvalidOperationException: Unable to resolve service for type 'IProductRepository':
    Test host’unda DI grafiğinin eksik kalması demektir. Genelde Program.cs içinde Testing ortamı nedeniyle AddInfrastructure atlandığında, repo ve DbContext’i TestWebApplicationFactory tarafında yeniden kayıt ettiğimizden emin olun. (DbContext’i zaten ediyoruz; repo kayıtları AddInfrastructure içinde ise onu Testing’de atlamayacak şekilde ayarladık ya da testte repo’yu da ekleyebilirsiniz.)
- TypeLoadException / EF Sqlite sürümü:
Paket sürümlerinin .csproj dosyalarında EF Core 8.x ile uyumlu olduğundan emin olun. Karışıklıkta dotnet restore ve bin/obj temizliği işe yarar.

```bash
dotnet clean
dotnet restore
```

- 500 dönerse, testte ITestOutputHelper ile response body’yi loglayarak ProblemDetails içindeki hatayı görün:

```bash
var body = await res.Content.ReadAsStringAsync();
_output.WriteLine(body);
```

## Unit Test Yaklaşımı

***Amaç:***
Tek bir sınıfın/komponentin (ör. domain entity mantığı, mapper profili, validator, basit servis) izole test edilmesi; hızlı ve deterministik doğrulama.

***Ne Test Edilir (Örnekler):***

- FluentValidation kuralı: ProductCreateDtoValidator (ör. Name boş olamaz, Price > 0, Stock >= 0, CategoryId zorunlu).
- AutoMapper Profili: MappingProfile → DTO ↔ Entity eşleştirmeleri, kritik alan dönüşümleri.
- Basit Domain Mantığı: Entity factory metotları, hesaplama/iş kuralları içeriyorsa.
- Repository’nin “iş” tarafı yoksa (yalnızca EF geçişi) unit test yerine integration’da kapsanması daha anlamlıdır. EF erişimini sahtelemek yerine integration’da uçtan uca test etmek tercih edilmiştir.

Bunların yanı sıra diğer her türlü metod için de birim testleri yazabilisiniz. Önemli olan; bu metotların herhangi bir bağımlılık içermemesidir (randomPassword gibi mesela). Eğer bağımlılık içeriyorsa da, mock'lanmalıdır bu bağımlılıklar.

***Nasıl İzole Edilir:***

- Saf sınıflar için doğrudan instantiate edip test edilir.
- Harici bağımlılıklar (saat, GUID üreteci, basit cache vb.) varsa interface ile soyutlayıp mock (Moq/NSubstitute) ile izole etmek tercih edilir.
- EF bağımlılığı olan logic’i ayırmak (örneğin domain service) unit test edilebilir kılar.

***Örnek Komutlar***

```bash
# Tüm testler (unit + integration)
dotnet test

# Sadece unit test projesi (varsa)
dotnet test source/Tests/TheHireFactory.ECommerce.Tests.Unit
```

> ⚠️ Yukarıdaki komutlar /source dizininde çalıştırılmalıdır.

***Notlar:***

- Şu an odak integration testlerde. Unit testler için hedef sınıflar (validator, mapper) belirlendiğinde xUnit + FluentAssertions ile kısa ve net testler eklenebilir.
- "Validation" testlerinde valid ve invalid dataset’leri ayrı ayrı senaryolarla beslemek (Theory + InlineData / MemberData) hız kazandırır.

AutoMapper için:

```bash
var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
config.AssertConfigurationIsValid(); // mapping hatalarını erken yakalar
```

***Özet:***

- Integration testler projede ana güvence: API davranışı + EF Core erişimini gerçekçi biçimde doğrular.
- Unit testler “mantık içeren” küçük parçaları hızlı ve izole şekilde doğrular.
- Komutlar basit: dotnet test (tamamı) veya proje/filtre bazında hedefli çalıştırma.
- Test ortamı Testing; DB işlerini TestWebApplicationFactory yönetir; prod seed/migrate blokları bu ortamda çalışmaz.


# Logging ve Exception Handling

Bu projede gözlemlenebilirlik iki ana mekanizmayla sağlanır:

1- **Serilog** ile yapılandırılmış, yapısal (structured) loglama
2- **ProblemDetails** tabanlı global hata yakalama (custom GlobalExceptionHandler)
    - Testing ortamında ise hızlı teşhis için **Developer Exception Page**

Aşağıda nasıl çalıştıklarını, nerede devreye girdiklerini ve nasıl ayarlayabileceğini bulacaksın.

## Serilog

***Nerede devreye giriyor?***

Program.cs başında:

```csharp
builder.Host.UseSerilog((ctx, lc) => lc
    .Enrich.WithEnvironmentName()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));
```

- **Console sink aktif:** container/log akışında anında görünür.
- **Enrichment:** EnvironmentName, RequestId, TraceIdentifier gibi bağlam özellikleri loglara eklenir.
- **ReadFrom.Configuration:** appsettings*.json içinde Serilog ayarı varsa otomatik yüklenir (yoksa koddaki varsayılanlar çalışır).

***Middleware seviyesinde istek günlükleme***

Program.cs pipeline’ında:

```csharp
app.UseSerilogRequestLogging(); // Her HTTP isteği için özet log
app.UseHttpLogging();           // ASP.NET Core built-in ayrıntılı HTTP loglama
```

- UseSerilogRequestLogging() her isteğin yolu, durum kodu, süre gibi alanlarını tek satırda özetler.
- UseHttpLogging() ise header/body boyutları gibi ayrıntıları (gerektiğinde) standardize eder.

Log örneği (konsol):

```bash
[INF] HTTP GET /api/product responded 200 in 12.34 ms
```

***Log seviyesini değiştirme***

- Geçici olarak: ASPNETCORE_ENVIRONMENT=Development iken minimal seviyeler geniş olur.
- Kalıcı olarak (öneri): appsettings.Development.json içine Serilog bölümünü ekle:

```json
{
  "Serilog": {
    "MinimumLevel": { "Default": "Information", "Override": { "Microsoft": "Warning", "System": "Warning" } },
    "WriteTo": [ { "Name": "Console" } ],
    "Enrich": [ "FromLogContext", "WithEnvironmentName" ]
  }
}
```

***Koddan log yazmak***

Controller/service sınıflarında ctor ile **ILogger< T>** alabilirsin:

```csharp
public class ProductController(ILogger<ProductController> logger, IProductRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        logger.LogInformation("Listing products");
        var list = await repo.ListWithCategoryAsync();
        return Ok(list);
    }
}
```

***Bağlamsal alan eklemek***

Gelişmiş senaryolarda (örn. siparişId) scope kullan:

```sharp
using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = HttpContext.TraceIdentifier }))
{
    logger.LogInformation("Processing request");
}
```

***Container’da logları görmek***

```bash
docker compose logs -f api
```

## ProblemDetails Middleware

**Hedef:** API hatalarını standart bir JSON formunda döndürmek (RFC 7807) ve ayrıntıyı ortama göre ayarlamak.

***Bileşenler***

- GlobalExceptionHandler : IExceptionHandler
    - Pipeline’daki yakalanmamış hataları yakalar, ProblemDetails üretir, application/problem+json döner.
- ProblemDetailsExtensions.ToProblemDetails(Exception)
    - Varsayılan başlık, detay ve durum kodunu şekillendirir.

***Kayıt ve kullanım (Program.cs):***

```csharp
// Servis kaydı
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Pipeline
if (app.Environment.IsEnvironment("Testing"))
{
    app.UseDeveloperExceptionPage(); // testte hızlı teşhis
}
else
{
    app.UseExceptionHandler();       // prod/dev: ProblemDetails + 5xx
}
```

***Örnek 500 cevabı (kısaltılmış):***

```json
{
  "type": "about:blank",
  "title": "Beklenmeyen bir hata oluştu.",
  "status": 500,
  "detail": "Hatanın mesajı burada...",
  "traceId": "00-...-00"
}
```

> Testing ortamında DeveloperExceptionPage etkin; stack trace ekranda görünür (hızlı teşhis için). Diğer ortamlarda UseExceptionHandler() devrede ve ProblemDetails döndürülür.

***404/400 gibi durumlar***

- **404:** Controller’dan return NotFound(); döndürdüğünde HTTP 404 ve gövde boş/ProblemDetails (ASP.NET davranışına bağlı) döner.
- **400:** FluentValidation kurallarına takılan isteklerde otomatik 400 ve validation problem details döner (çünkü AddFluentValidationAutoValidation() etkin).
- **500:** Yakalanmamış tüm hataları GlobalExceptionHandler tek tip ProblemDetails haline getirir.

***Hata teşhisi ipuçları***

- Integration testlerde ITestOutputHelper ile response.Content.ReadAsStringAsync() çıktısını yazdır; ProblemDetails içeriği hızlıca yönlendirir.
- Üretimde ayrıntıyı azalt (güvenlik), geliştirme/testte ayrıntıyı artır (teşhis). Bizim kurguda bu ayrım ortam üzerinden zaten yapılıyor.

***Özet***

- Serilog + Request/HTTP logging ile istek/yanıt akışı net kayıt altına alınır.
- GlobalExceptionHandler + ProblemDetails ile tutarlı hata formatı sağlanır.
- Testing ortamında geliştirici sayfası (Developer Exception Page) açık; diğerlerinde ProblemDetails standardı kullanılır.
- Log seviyesi ve sink’ler appsettings ile konfigüre edilebilir; container’da docker compose logs -f api ile canlı takip mümkündür.

<a id="swagger-api-dokümantasyonu"></a>
# Swagger / API Dokümantasyonu

Projenin API’leri, Swagger (OpenAPI) desteği ile dökümante edilmiştir. Bu sayede hem geliştiriciler hem de test eden ekipler API uç noktalarını kolayca keşfedebilir ve deneyebilir.

- **Lokal (dev-container dışı):** http://localhost:8080/swagger
- **Dev Container içinde:** Container port yönlendirmesi yapıldığı için yine http://localhost:8080/swagger adresinden erişim sağlanır.

> Varsayılan port: 8080 (docker-compose içinde tanımlı). HTTPS desteği eklenirse erişim adresi: https://localhost:8443/swagger gibi konfigüre edilebilir.

<a id="swagger-ui-erişimi"></a>
## Swagger UI Erişimi

- Tüm endpoint’ler listelenir.
- Request/response gövdeleri, model şemaları ve örnekleri görüntülenir.
- Doğrudan browser üzerinden API çağrıları yapılabilir.

## Endpoint Örnekleri

Proje minimal bir e-ticaret API’sidir. Başlıca uç noktalar:

Health-Check

```bash
GET /health
```

API’nin canlı olduğunu test etmek için kullanılır.

Örnek yanıt:

```json
{
  "status": "ok",
  "time": "2025-09-16T10:00:00Z"
}
```

***Ürünler***

Tüm ürünleri döner.

```bash
GET /api/product
```

ID’ye göre ürün getirir. Bulunamazsa 404 NotFound döner.

```bash
GET /api/product/{id}
```

Yeni ürün oluşturur.

Örnek request body:

```json
{
  "name": "Laptop",
  "price": 19999.90,
  "stock": 25,
  "categoryId": 1
}
```

Örnek response (201 Created):

```json
{
  "id": 42,
  "name": "Laptop",
  "price": 19999.90,
  "stock": 25,
  "categoryId": 1
}
```

***Kategoriler***

Tüm kategorileri listeler.

```bash
GET /api/category
```

Yeni kategori oluşturur.

```bash
POST /api/category
```

Request:

```json
{ "name": "Elektronik" }
```

***Ek Özellikler***

- **FluentValidation Rules → Swagger:** Parametre doğrulama kuralları Swagger UI üzerinde otomatik gösterilir. Örn. price alanı negatif olamaz gibi kurallar doğrudan schema’ya yansır.
- **Swagger Examples:** ProductCreateExample gibi sınıflar sayesinde request örnekleri otomatik doldurulur.
- **Custom Operation Filters:** Varsayılan hata yanıtları (400, 404, 500 → ProblemDetails formatı) her endpoint için otomatik eklenir.

***Özet***

Swagger, bu projede canlı bir API dokümantasyonu işlevi görür. Tüm endpoint’ler, parametreler, örnek istek/yanıtlar ve hata durumları Swagger UI üzerinden görülebilir ve test edilebilir.

# Faydalı Komutlar

Bu bölüm, geliştirme sırasında sık kullanılan komutların bir özetini içerir.
Tüm komutlar **dev-container** içinde çalıştırılmalıdır *(aksi belirtilmedikçe)*.

***Docker / Container***

```bash
# Servisleri ayağa kaldır
docker-compose up -d

# Servisleri durdur
docker-compose down

# Logları takip et
docker-compose logs -f api
```

***Docker / Container (watch)***

```bash
# Servisleri ayağa kaldır
docker-compose -f docker-compose.override.yml up -d 

# Servisleri durdur
docker-compose -f docker-compose.override.yml down

# Logları takip et
docker-compose -f docker-compose.override.yml logs -f api
```

***.NET Genel***

```bash
# Restore bağımlılıklar
dotnet restore

# Build (tüm solution)
dotnet build source/TheHireFactory.ECommerce.sln

# API’yi çalıştır
dotnet run --project source/TheHireFactory.ECommerce.Api
```

***Migration & Veritabanı***

```bash
# Yeni migration oluştur
dotnet ef migrations add InitialCreate -p source/TheHireFactory.ECommerce.Infrastructure -s source/TheHireFactory.ECommerce.Api

# Migration’ı veritabanına uygula
dotnet ef database update -p source/TheHireFactory.ECommerce.Infrastructure -s source/TheHireFactory.ECommerce.Api

# Migration geri al (son eklenen)
dotnet ef migrations remove -p source/TheHireFactory.ECommerce.Infrastructure -s source/TheHireFactory.ECommerce.Api
```

> ⚠️ Not: Migration komutları için önce dotnet tool install --global dotnet-ef çalıştırılmış olmalıdır.

***Testler***

```bash
# Tüm testleri çalıştır
dotnet test

# Sadece integration testleri çalıştır
dotnet test source/Tests/TheHireFactory.ECommerce.Tests.Integration
```

***Format / Lint***

```bash
# Kodları formatla (pre-commit hook’ta da çalışır)
dotnet format
```

***Health & Swagger***

```bash
# Health endpoint kontrolü
curl http://localhost:8080/health

# Swagger’a tarayıcıdan erişim
http://localhost:8080/swagger
```

Bu komutlar, günlük geliştirme sürecinde en sık ihtiyaç duyulan işlemleri kapsar.

Ekstra durumlarda (örn. seed verilerini resetlemek, veritabanını sıfırlamak) komutlar README’ye eklenebilir.

# Katkı ve Geliştirme Notları

## Kodlama Standartları
- **C# 12 / .NET 8** kullanılmaktadır. Tüm kod bu sürümlerle uyumlu olmalıdır.
- **Adlandırma Kuralları**:
  - Sınıf isimleri: `PascalCase`
  - Metotlar: `PascalCase`
  - Değişkenler ve parametreler: `camelCase`
  - Sabitler: `UPPER_SNAKE_CASE`
- Gereksiz `using` ifadeleri temizlenmelidir.
- Kod mümkün olduğunca **SOLID prensipleri** gözetilerek yazılmalıdır.
- Tüm public API’ler XML doc comment ile açıklanmalıdır.

## Pull Request Süreci
1. Yeni branch oluşturun:
   ```bash
   git checkout -b feature/ozellik-adi
   ```
2. Geliştirmeyi yapın ve testleri çalıştırın:
   ```bash
   dotnet test
   ```
3. Kodunuzu formatlayın:
   ```bash
   dotnet format
   ```
4. Commit mesaj formatı (Conventional Commits):
   - `feat:` → Yeni özellik
   - `fix:` → Hata düzeltme
   - `docs:` → Sadece dokümantasyon
   - `test:` → Test ekleme/düzeltme
   - `refactor:` → Davranışı değiştirmeyen refactor
   - `chore:` → Yapılandırma, araç güncellemesi vb.
   ```bash
   git commit -m "feat: ürün API’sine kategori filtreleme eklendi"
   ```

## Pre-commit Hook (Otomatik Formatlama)
Kod stil sorunlarını commit aşamasında engellemek için `pre-commit` hook eklenmiştir:
```bash
#!/bin/sh
dotnet format
RESULT=$?
if [ $RESULT -ne 0 ]; then
  echo "❌ Format hataları bulundu. Commit iptal edildi."
  exit 1
fi
```
> Bu dosya `.git/hooks/pre-commit` içine eklenmelidir.  

## Test Zorunluluğu
- Yeni eklenen her özellik için **Unit veya Integration test** yazılması zorunludur.
- Mevcut testler kırık durumda PR gönderilmemelidir.

## Dokümantasyon
- Swagger dokümantasyonu güncel tutulmalıdır.
- README.md, yapılan değişikliklere göre güncellenmelidir.

## Katkı Sağlamak
1. Repo’yu fork edin
2. Yeni branch açın (`feature/xyz`)
3. Değişiklikleri yapın ve testleri çalıştırın
4. Pull request gönderin
