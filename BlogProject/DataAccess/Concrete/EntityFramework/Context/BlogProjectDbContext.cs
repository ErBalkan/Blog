using BlogProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.DataAccess.Concrete.EntityFramework.Context;
    // BlogProjectDbContext sınıfımız, EF Core'un ana sınıfı olan DbContext'ten türetilmiştir.
    // Bu sınıf, uygulamamızın veritabanı ile nasıl etkileşime gireceğini tanımlar.
    public class BlogProjectDbContext : DbContext
    {
        
        // OnConfiguring metodu, DbContext örneği oluşturulduğunda çağrılır.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // PostgreSQL bağlantı dizenizi buraya ekleyin.
                // Bu bağlantı dizesi, appsettings.json'daki ile aynı olmalıdır.
                // Örnek bir PostgreSQL bağlantı dizesi formatı:
                // "Host=localhost;Port=5432;Database=BlogProjectDb;Username=myuser;Password=mypassword;"
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=BlogProjectDb;Username=postgres;Password=12345;");
            }
        }
    // Constructor (Yapıcı Metot):
    // Bu metot, DbContext'imiz oluşturulurken kullanılır.
    // DbContextOptions<BlogProjectDbContext> parametresi, uygulamanın başlangıcında (örneğin WebAPI'de Program.cs'te)
    // veritabanı bağlantı dizesi gibi yapılandırma bilgilerini almamızı sağlar.
    public BlogProjectDbContext(DbContextOptions<BlogProjectDbContext> options) : base(options)
    {
        // Database.EnsureCreated():
        // Bu satır çok önemlidir ve bizim senaryomuzda migration kullanmadan veritabanını otomatik oluşturma isteğimize hizmet eder.
        // Uygulama ilk kez çalıştığında, eğer bağlantı dizesinde belirtilen isimde bir veritabanı yoksa,
        // EF Core bu metodu kullanarak veritabanını ve OnModelCreating'de tanımladığımız şemayı temel alarak tabloları otomatik olarak oluşturur.
        // Dikkat: Eğer veritabanı zaten varsa, bu metot hiçbir şey yapmaz; şema değişikliklerini kontrol etmez veya güncellemez.
        Database.EnsureCreated();
    }
        
        // 2. Parametresiz public constructor (EfEntityRepositoryBase için)
        // Bu constructor, EfEntityRepositoryBase sınıfındaki 'new TContext()' çağrısı tarafından kullanılır.
        // Burada, OptionsBuilder kullanarak bağlantı dizesini manuel olarak yapılandırmamız gerekir.
        // NOT: Connection string burada sabit kodlanmış. Daha iyi bir yaklaşım için Configuration'dan çekmek gerekir.
        // Ancak EfEntityRepositoryBase'in TContext kısıtlamasını karşılamak için en basit çözüm budur.
        public BlogProjectDbContext()
        {
            // Veritabanı bağlantı dizesini doğrudan burada belirtebilirsiniz.
            // Bu, yalnızca EfEntityRepositoryBase'in DbContext'i kendi içinde oluşturduğu durumlar için geçerlidir.
            // Gerçek uygulamada, bu bağlantı dizesini appsettings.json'dan veya başka bir yapılandırma kaynağından almak daha güvenlidir.
            // Örneğin: optionsBuilder.UseSqlServer("Server=YOUR_SERVER_NAME;Database=BlogProjectDb;Trusted_Connection=True;TrustServerCertificate=True");
        }


        // DbSet<TEntity> Özellikleri:
        // Bu özellikler, C# varlık sınıflarımızın (entity classes) veritabanındaki hangi tablolara karşılık geldiğini EF Core'a söyler.
        // Her bir DbSet, belirli bir varlık türü için veritabanı sorguları yapmamızı sağlayan bir koleksiyon gibidir.
        public DbSet<Post> Posts { get; set; } // Veritabanındaki "Posts" tablosunu temsil eder.
        public DbSet<Category> Categories { get; set; } // Veritabanındaki "Categories" tablosunu temsil eder.
        public DbSet<Comment> Comments { get; set; } // Veritabanındaki "Comments" tablosunu temsil eder.
        public DbSet<User> Users { get; set; } // Veritabanındaki "Users" tablosunu temsil eder.

        // OnModelCreating Metodu:
        // Bu metot, EF Core'un varlıklar ve veritabanı arasındaki eşlemeyi (mapping) ve şema konfigürasyonlarını
        // daha detaylı bir şekilde yapmamızı sağlayan merkezi yerdir.
        // Burada Fluent API adı verilen bir dizi metod zinciri kullanarak varsayılan EF Core davranışlarını özelleştiririz.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Category (Kategori) Varlığı İçin Konfigürasyonlar ---
            // modelBuilder.Entity<Category>(entity => { ... }):
            // EF Core'a Category sınıfının veritabanında nasıl eşleşeceğini yapılandırmaya başladığımızı söyler.
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id); 
                // HasKey(c => c.Id):
                // Category varlığının birincil anahtarının (Primary Key) "Id" özelliği olduğunu açıkça belirtiriz.
                // EF Core genellikle "Id" veya "CategoryId" gibi isimleri otomatik olarak Primary Key olarak tanır,
                // ancak açıkça belirtmek niyetimizi netleştirir ve kodun okunabilirliğini artırır.

                entity.HasIndex(c => c.Name).IsUnique(); 
                // HasIndex(c => c.Name):
                // "Name" (kategori adı) özelliği üzerinde bir veritabanı indeksi oluşturulacağını belirtir.
                // İndeksler, veritabanı sorgularının (özellikle arama ve sıralama işlemlerinin) performansını artırır.
                // IsUnique(): Bu indeksi "benzersiz" kılar. Yani veritabanında aynı "Name" değerine sahip iki Category kaydı olamaz.
                // Bu, veri bütünlüğünü veritabanı seviyesinde sağlayan kritik bir kısıtlamadır.

                // FluentValidation kullanacağımız için, string alanların IsRequired() (NOT NULL) ve HasMaxLength() (maksimum uzunluk)
                // gibi basit doğrulama kısıtlamalarını burada yapmadık. Bunları iş katmanında FluentValidation ile yöneteceğiz.

                // HasQueryFilter(c => !c.IsDeleted):
                // Bu, "soft delete" (yumuşak silme) stratejimiz için hayati bir konfigürasyondur.
                // Bu filtre, Category DbSet'i üzerinden yapılan *her sorguya* otomatik olarak "WHERE IsDeleted = FALSE" koşulunu ekler.
                // Yani, IsDeleted özelliği true olan (mantıksal olarak silinmiş) kategoriler, siz özel olarak belirtmedikçe
                // sorgu sonuçlarında görünmeyecektir. Bu, kod tekrarını önler ve veri tutarlılığını sağlar.
                entity.HasQueryFilter(c => !c.IsDeleted);
            });

            // --- Post (Makale) Varlığı İçin Konfigürasyonlar ---
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.Id); 

                // Property(p => p.PublishDate).HasColumnType("timestamp without time zone"):
                // "PublishDate" özelliğinin veritabanında PostgreSQL'e özgü "timestamp without time zone"
                // veri tipiyle eşleştirilmesini sağlar. Bu, tarih ve saat verilerinin doğru bir şekilde
                // saklanması ve işlenmesi için önemlidir.
                entity.Property(p => p.PublishDate).HasColumnType("timestamp without time zone"); 

                // Property(p => p.ViewCount).HasDefaultValue(0):
                // "ViewCount" özelliğine, veritabanında yeni bir Post kaydı eklendiğinde
                // varsayılan olarak 0 değerinin atanmasını sağlar.
                entity.Property(p => p.ViewCount).HasDefaultValue(0); 
                
                // --- Foreign Key (Yabancı Anahtar) İlişkileri ---

                // HasOne(p => p.Category).WithMany(c => c.Posts):
                // Bu satır, Post ve Category arasındaki "Bire Çok" (One-to-Many) ilişkiyi yapılandırır.
                // "Bir Post'un bir Category'si vardır" (HasOne) ve "Bir Category'nin birden fazla Post'u olabilir" (WithMany).
                entity.HasOne(p => p.Category) 
                      .WithMany(c => c.Posts) 
                      .HasForeignKey(p => p.CategoryId) // Post sınıfındaki "CategoryId" özelliğinin yabancı anahtar olduğunu belirtir.
                      .OnDelete(DeleteBehavior.Restrict); 
                      // OnDelete(DeleteBehavior.Restrict): Silme davranışını belirler.
                      // "Restrict" (Kısıtla) seçeneği, eğer bir Category'ye bağlı (ilişkili) Post'lar varsa,
                      // o Category'nin silinmesini engeller. Bu, veri bütünlüğünü korur.

                // HasOne(p => p.User).WithMany(u => u.Posts):
                // Post ve User arasındaki "Bire Çok" ilişkiyi yapılandırır ("Bir Post'un bir User'ı (yazarı) vardır").
                entity.HasOne(p => p.User) 
                      .WithMany(u => u.Posts) 
                      .HasForeignKey(p => p.UserId) // Post sınıfındaki "UserId" özelliğinin yabancı anahtar olduğunu belirtir.
                      .OnDelete(DeleteBehavior.Restrict); 
                      // Kullanıcı silindiğinde ilişkili Post'ların silinmesini kısıtlar.

                // Global sorgu filtresi (Soft Delete).
                entity.HasQueryFilter(p => !p.IsDeleted);
            });

            // --- Comment (Yorum) Varlığı İçin Konfigürasyonlar ---
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id); 
                entity.Property(c => c.CommentDate).HasColumnType("timestamp without time zone"); 

                // HasOne(c => c.Post).WithMany(p => p.Comments):
                // Comment ve Post arasındaki "Bire Çok" ilişkiyi yapılandırır ("Bir Comment'in bir Post'u vardır").
                entity.HasOne(c => c.Post)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(c => c.PostId)
                      .OnDelete(DeleteBehavior.Cascade); 
                      // OnDelete(DeleteBehavior.Cascade): Silme davranışını belirler.
                      // "Cascade" (Basamaklı) seçeneği, bir Post silindiğinde,
                      // o Post'a bağlı tüm Comment'lerin de otomatik olarak silinmesini sağlar.
                      // Bu, mantıksal bir veri temizliğidir.

                // HasOne(c => c.User).WithMany(u => u.Comments):
                // Comment ve User arasındaki ilişkiyi yapılandırır ("Bir Comment'in bir User'ı olabilir").
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .IsRequired(false) 
                      // IsRequired(false): "UserId" özelliğinin veritabanında null kabul edilebilir olduğunu belirtir.
                      // Bu, anonim yorumların yapılabilmesi için önemlidir (yani bir yorumun kullanıcısı olmayabilir).
                      .OnDelete(DeleteBehavior.SetNull); 
                      // OnDelete(DeleteBehavior.SetNull): Silme davranışını belirler.
                      // Eğer yorumu yapan User silinirse, o yoruma ait "UserId" değeri veritabanında null olarak ayarlanır.
                      // Yorumun kendisi silinmez, sadece yorumcu bilgisi kaldırılır.
                
                // Global sorgu filtresi (Soft Delete).
                entity.HasQueryFilter(c => !c.IsDeleted);
            });

            // --- User (Kullanıcı) Varlığı İçin Konfigürasyonlar ---
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id); 
                entity.HasIndex(u => u.Email).IsUnique(); // E-posta adresinin benzersiz olmasını sağlar.
                entity.HasIndex(u => u.Username).IsUnique(); // Kullanıcı adının benzersiz olmasını sağlar.

                // Global sorgu filtresi (Soft Delete).
                entity.HasQueryFilter(u => !u.IsDeleted);
            });

            // base.OnModelCreating(modelBuilder):
            // Bu satırın çağrılması önemlidir. DbContext'in temel sınıfındaki OnModelCreating metodunu çağırır.
            // Bu, EF Core'un kendi varsayılan kurallarını uygulamasını ve başka bir yerden (örneğin NuGet paketlerinden)
            // gelen ek konfigürasyonları uygulamasını sağlar. Genellikle bu satır en sonda yer alır.
            base.OnModelCreating(modelBuilder);
        }
    }

/*
-------------------------------------------------------------------------
-------------------------------------------------------------------------
-------------------------------------------------------------------------
Neden Bu Konfigürasyonlar Bu Kadar Detaylı ve Önemli?
-------------------------------------------------------------------------
1. Veri Bütünlüğü (Data Integrity):

- IsUnique() ve OnDelete() davranışları gibi kısıtlamalar, 
veritabanı seviyesinde verilerin tutarlılığını sağlar. ü
Bu, uygulamanızda veya harici bir araçtan hatalı veri girişini engeller.
-------------------------------------------------------------------------
2. Performans:

- HasIndex() kullanımı, sıkça sorgulanan sütunlarda 
(örneğin arama veya filtreleme yapılan alanlarda) 
veritabanı performansını önemli ölçüde artırır.
-------------------------------------------------------------------------
3. Soft Delete (Yumuşak Silme):

- HasQueryFilter() özelliği, silinmiş gibi işaretlediğimiz kayıtların 
otomatik olarak sorgu sonuçlarından dışlanmasını sağlar. 
Bu, iş mantığınızı daha basit ve hataya daha az yatkın hale getirir.
-------------------------------------------------------------------------
4. Esneklik ve Sürdürülebilirlik:

- Bu konfigürasyonlar, entity sınıflarınızı temiz tutarken 
(EF Core'a özgü attribute'lar olmadan), veritabanı şemasının 
detaylarını merkezi bir yerde yönetmenizi sağlar. Projeniz büyüdükçe 
veya veritabanı şeması değiştikçe bu esneklik çok değerlidir.
-------------------------------------------------------------------------
5. Veritabanı Sağlayıcısına Özgü Ayarlar:

- HasColumnType("timestamp without time zone") gibi ayarlar, 
PostgreSQL gibi belirli veritabanı sistemleri ile uyumluluğu sağlar.
-------------------------------------------------------------------------
-------------------------------------------------------------------------
-------------------------------------------------------------------------
*/