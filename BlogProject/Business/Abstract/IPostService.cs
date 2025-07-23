using BlogProject.Entities;
using Core.Utilities.Results.Abstract;

namespace BlogProject.Business.Abstract;
// Makale işlemleri için servis arayüzü
public interface IPostService
{
    Task<IDataResult<Post>> GetByIdAsync(int postId);
    Task<IDataResult<List<Post>>> GetAllAsync();
    Task<IMyResult> AddAsync(Post post);
    Task<IMyResult> UpdateAsync(Post post);
    Task<IMyResult> DeleteAsync(int postId);
        
    // Kategoriye göre makaleleri getirme gibi özel iş operasyonları eklenebilir.
    Task<IDataResult<List<Post>>> GetPostsByCategoryIdAsync(int categoryId);
    // Kullanıcıya göre makaleleri getirme
    Task<IDataResult<List<Post>>> GetPostsByUserIdAsync(int userId);
}