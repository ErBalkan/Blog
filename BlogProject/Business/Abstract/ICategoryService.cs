using BlogProject.Entities;
using Core.Utilities.Results.Abstract;

namespace BlogProject.Business.Abstract;

// Kategori işlemleri için servis arayüzü
public interface ICategoryService
{
    // Tek bir kategoriyi Id'ye göre asenkron olarak getirir.
    Task<IDataResult<Category>> GetByIdAsync(int categoryId);

    // Tüm kategorileri asenkron olarak getirir.
    Task<IDataResult<List<Category>>> GetAllAsync();

    // Yeni bir kategori ekler.
    Task<IMyResult> AddAsync(Category category);

    // Mevcut bir kategoriyi günceller.
    Task<IMyResult> UpdateAsync(Category category);

    // Bir kategoriyi siler (soft delete).
    Task<IMyResult> DeleteAsync(int categoryId); // Id ile silme, Business katmanında daha yaygındır.
}