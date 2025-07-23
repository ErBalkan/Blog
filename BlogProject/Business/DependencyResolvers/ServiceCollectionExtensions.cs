using BlogProject.Business.Abstract; // Servis arayüzleri için
using BlogProject.Business.Concrete; // Servis implementasyonları için
using Core.Abstract.DataAccess; // IEntityRepository için
using Core.Concrete.EntityFramework.Repositories; // <-- EfEntityRepositoryBase'in yeni konumu
using BlogProject.DataAccess.Concrete.EntityFramework.Context; // BlogProjectDbContext için
using BlogProject.Entities; // Entity sınıfları için
using Microsoft.EntityFrameworkCore; // AddDbContext için
using Microsoft.Extensions.Configuration; // IConfiguration için
using Microsoft.Extensions.DependencyInjection; // IServiceCollection için (using Microsoft.Extensions.DependencyInjection;)
using System.Reflection; // Assembly için

namespace BlogProject.Business.DependencyResolvers;


    // ServiceCollectionExtensions: 'static' olarak tanımlanır çünkü içinde sadece extension metotları barındıracaktır.
    public static class ServiceCollectionExtensions
    {
        // AddBusinessServices: IServiceCollection tipini genişleten bir extension metottur.
        // 'this IServiceCollection services' ifadesi, bu metodun bir extension metot olduğunu ve
        // 'services' parametresinin metodu çağıran IServiceCollection nesnesini temsil ettiğini belirtir.
        // IConfiguration configuration: appsettings.json gibi yapılandırma dosyalarındaki bilgilere (örneğin bağlantı dizeleri) erişmek için kullanılır.
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            // --- DbContext Yapılandırması ---
            // BlogProjectDbContext'i uygulamanın servis konteynerine ekler.
            // options => options.UseNpgsql(...): PostgreSQL veritabanı sağlayıcısını kullanacağımızı belirtir.
            // configuration.GetConnectionString("DefaultConnection"): appsettings.json dosyasındaki "ConnectionStrings" bölümündeki "DefaultConnection" adlı bağlantı dizesini okur.
            services.AddDbContext<BlogProjectDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // --- Repository Katmanı Bağımlılıkları ---
            // 'AddScoped': Servis yaşam döngüsünü 'Scoped' olarak ayarlar. Bu, her HTTP isteği için bir kez oluşturulacağı ve
            // o istek boyunca aynı örneğin kullanılacağı anlamına gelir. Bu, genellikle veritabanı işlemleri için en uygun yaşam döngüsüdür.
            // IEntityRepository<T>: Genel repository arayüzümüz.
            // EfEntityRepositoryBase<T, BlogProjectDbContext>: Bu arayüzün Entity Framework Core ile yapılan somut implementasyonu.
            // Uygulama herhangi bir yerde IEntityRepository<Category> istediğinde, ona EfEntityRepositoryBase<Category, BlogProjectDbContext> örneği sağlanır.
            services.AddScoped<IEntityRepository<Category>, EfEntityRepositoryBase<Category, BlogProjectDbContext>>();
            services.AddScoped<IEntityRepository<Post>, EfEntityRepositoryBase<Post, BlogProjectDbContext>>();
            services.AddScoped<IEntityRepository<Comment>, EfEntityRepositoryBase<Comment, BlogProjectDbContext>>();
            services.AddScoped<IEntityRepository<User>, EfEntityRepositoryBase<User, BlogProjectDbContext>>();

            // --- Business Katmanı Servis Bağımlılıkları ---
            // Business katmanındaki servis arayüzlerini ve onların somut implementasyonlarını kaydeder.
            // Aynı şekilde 'Scoped' yaşam döngüsü kullanılır.
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<IPostService, PostManager>();
            services.AddScoped<ICommentService, CommentManager>();
            services.AddScoped<IUserService, UserManager>();

            // Metot zincirlemesini (method chaining) sağlamak için 'services' nesnesini geri döndürüyoruz.
            return services;
        }
    }