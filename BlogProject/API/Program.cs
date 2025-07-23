using BlogProject.Business.DependencyResolvers;
using BlogProject.Business.Concrete;
// FluentValidation için gerekli using'ler
using FluentValidation.AspNetCore; // Eski metot için
using FluentValidation; // Yeni metotlar için
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Controller'ları ekle ve FluentValidation entegrasyonunu yapılandır.
builder.Services.AddControllers(); // .AddFluentValidation() kısmı buradan kaldırılacak

// --- FluentValidation Yapılandırması: Güncellenmiş Bölüm ---
// FluentValidation için iki yeni extension metodunu kullanıyoruz.
// AddFluentValidationAutoValidation(): Sunucu tarafı otomatik doğrulamayı etkinleştirir.
// AddFluentValidationClientsideAdapters(): İstemci tarafı doğrulama adaptörlerini etkinleştirir (eğer kullanılıyorsa).
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

// Doğrulayıcıları (validators) kaydetmek için ayrı bir çağrı yapıyoruz.
// typeof(CategoryManager).Assembly: Business katmanındaki doğrulayıcıları kaydeder.
// Assembly.GetExecutingAssembly(): Web API projesindeki doğrulayıcıları kaydeder (eğer varsa).
builder.Services.AddValidatorsFromAssembly(typeof(CategoryManager).Assembly);
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
// --- Güncellenmiş Bölüm Sonu ---


// Swagger/OpenAPI yapılandırması
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// İş katmanı servislerini ekle
builder.Services.AddBusinessServices(builder.Configuration);

var app = builder.Build();

// İstek işleme hattını yapılandırma
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();