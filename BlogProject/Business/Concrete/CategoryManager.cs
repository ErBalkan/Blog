using BlogProject.Business.Abstract;
using BlogProject.Business.ValidationRules;
using BlogProject.Entities;
using Core.Abstract.DataAccess;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.Error;
using Core.Utilities.Results.Concrete.Success;

namespace BlogProject.Business.Concrete;

    // CategoryManager sınıfı, ICategoryService arayüzünü uygular.
    // İş kuralları ve doğrulama mantığı burada yer alır.
    public class CategoryManager : ICategoryService
    {
        // _categoryRepository, veritabanı işlemlerini yapmak için kullanılan IEntityRepository arayüzüdür.
        // Dependency Injection (Bağımlılık Enjeksiyonu) ile dışarıdan enjekte edilecek.
        private readonly IEntityRepository<Category> _categoryRepository;

        // Yapıcı metot (Constructor):
        // Program.cs'te yapılandıracağımız Dependency Injection mekanizması sayesinde,
        // CategoryManager örneği oluşturulurken ona bir IEntityRepository<Category> örneği sağlanır.
        public CategoryManager(IEntityRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // --- CRUD (Create, Read, Update, Delete) Operasyonları ve İş Mantığı ---

        // Asenkron olarak yeni bir kategori ekler.
        public async Task<IMyResult> AddAsync(Category category)
        {
            // 1. FluentValidation ile Gelen Veriyi Doğrulama:
            // CategoryValidator sınıfından bir örnek oluşturulur.
            var validator = new CategoryValidator();
            // Validate metodu, category nesnesini CategoryValidator'daki kurallara göre kontrol eder.
            var validationResult = validator.Validate(category);

            // Eğer doğrulama başarısız olursa (yani kurallardan en az biri ihlal edilmişse):
            if (!validationResult.IsValid)
            {
                // Bir ErrorResult döndürülür. İlk doğrulama hatasının mesajı kullanılır.
                // Gerçek bir uygulamada, tüm hata mesajlarını içeren bir DataResult veya özel bir ValidationResult tipi döndürülebilir.
                return new ErrorResult(validationResult.Errors[0].ErrorMessage); 
            }

            // 2. İş Kuralı Kontrolü: Aynı isimde kategori var mı?
            // Veritabanında aynı "Name" değerine sahip başka bir kategori olup olmadığı kontrol edilir.
            var existingCategory = await _categoryRepository.GetAsync(c => c.Name.ToLower() == category.Name.ToLower() && !c.IsDeleted);
            if (existingCategory != null)
            {
                // Eğer varsa, bir hata mesajı ile ErrorResult döndürülür.
                return new ErrorResult("Bu isimde bir kategori zaten mevcut.");
            }

            // 3. Başarılı ise Kategori Ekleme İşlemi:
            // Kategoriye otomatik olarak oluşturulma tarihi atanır.
            category.CreatedDate = DateTime.Now; 
            category.IsDeleted = false; // Yeni kategori varsayılan olarak silinmemiş olmalı

            // Repository aracılığıyla kategori veritabanına eklenir.
            await _categoryRepository.AddAsync(category);
            // Başarılı bir sonuç mesajı ile SuccessResult döndürülür.
            return new SuccessResult("Kategori başarıyla eklendi.");
        }

        // Asenkron olarak tüm kategorileri listeler.
        public async Task<IDataResult<List<Category>>> GetAllAsync()
        {
            // Repository'den tüm kategoriler getirilir. Soft delete filtresi DbContext'te uygulandığı için
            // burada manuel olarak IsDeleted kontrolü yapmaya gerek yoktur.
            var categories = await _categoryRepository.GetAllAsync();
            // Başarılı bir sonuç ve kategori listesi ile SuccessDataResult döndürülür.
            return new SuccessDataResult<List<Category>>(categories, "Kategoriler başarıyla listelendi.");
        }

        // Asenkron olarak belirli bir Id'ye sahip kategoriyi getirir.
        public async Task<IDataResult<Category>> GetByIdAsync(int categoryId)
        {
            // Repository'den Id'ye göre kategori getirilir.
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            // Eğer kategori bulunamazsa:
            if (category == null)
            {
                // Bir ErrorDataResult döndürülür (veri null, hata mesajı var).
                return new ErrorDataResult<Category>(null,"Kategori bulunamadı.");
            }
            // Kategori bulunursa:
            // Başarılı bir sonuç ve kategori nesnesi ile SuccessDataResult döndürülür.
            return new SuccessDataResult<Category>(category, "Kategori başarıyla getirildi.");
        }

        // Asenkron olarak mevcut bir kategoriyi günceller.
        public async Task<IMyResult> UpdateAsync(Category category)
        {
            // 1. FluentValidation ile Gelen Veriyi Doğrulama:
            var validator = new CategoryValidator();
            var validationResult = validator.Validate(category);

            if (!validationResult.IsValid)
            {
                return new ErrorResult(validationResult.Errors[0].ErrorMessage);
            }

            // 2. İş Kuralı Kontrolü: Güncellenen kategori adı başka bir aktif kategoriye ait mi?
            // Güncellenen kategorinin Id'si hariç, aynı isme sahip başka bir aktif kategori olup olmadığı kontrol edilir.
            var existingCategory = await _categoryRepository.GetAsync(c => c.Name.ToLower() == category.Name.ToLower() && c.Id != category.Id && !c.IsDeleted);
            if (existingCategory != null)
            {
                return new ErrorResult("Bu isimde başka bir kategori zaten mevcut.");
            }

            // 3. Kategori Güncelleme İşlemi:
            // Kategoriye otomatik olarak güncellenme tarihi atanır (BaseEntity'de otomatik set edilecek ama yine de örnek).
            category.UpdatedDate = DateTime.Now; 

            // Repository aracılığıyla kategori güncellenir.
            await _categoryRepository.UpdateAsync(category);
            // Başarılı bir sonuç mesajı ile SuccessResult döndürülür.
            return new SuccessResult("Kategori başarıyla güncellendi.");
        }

        // Asenkron olarak belirli bir Id'ye sahip kategoriyi siler (soft delete).
        public async Task<IMyResult> DeleteAsync(int categoryId)
        {
            // Silinecek kategori Id'ye göre bulunur.
            var categoryToDelete = await _categoryRepository.GetByIdAsync(categoryId);
            // Eğer kategori bulunamazsa:
            if (categoryToDelete == null)
            {
                return new ErrorResult("Silinecek kategori bulunamadı.");
            }

            // İş Kuralı Kontrolü (Örnek): Kategoriye bağlı aktif post'lar varsa silinmesini engelle.
            // Bu kontrol için PostManager'a veya doğrudan PostRepository'e ihtiyacımız olur.
            // Şimdilik sadece konsepti göstermek için örnek bir yorum olarak bırakıyorum.
            // Gerçek projede:
            // var postsInThisCategory = await _postService.GetPostsByCategoryIdAsync(categoryId);
            // if (postsInThisCategory.Data != null && postsInThisCategory.Data.Any()) {
            //     return new ErrorResult("Bu kategoriye bağlı aktif makaleler var, silinemez.");
            // }

            // Repository aracılığıyla kategori silinir (soft delete işlemi).
            await _categoryRepository.DeleteAsync(categoryToDelete);
            // Başarılı bir sonuç mesajı ile SuccessResult döndürülür.
            return new SuccessResult("Kategori başarıyla silindi.");
        }
    }