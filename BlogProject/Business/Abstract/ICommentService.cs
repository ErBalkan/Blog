using BlogProject.Entities;
using Core.Utilities.Results.Abstract;

namespace BlogProject.Business.Abstract;
// Yorum işlemleri için servis arayüzü
public interface ICommentService
{
    Task<IDataResult<Comment>> GetByIdAsync(int commentId);
    Task<IDataResult<List<Comment>>> GetAllAsync();
    Task<IMyResult> AddAsync(Comment comment);
    Task<IMyResult> UpdateAsync(Comment comment);
    Task<IMyResult> DeleteAsync(int commentId);

    // Bir makaleye ait tüm yorumları getirme
    Task<IDataResult<List<Comment>>> GetCommentsByPostIdAsync(int postId);
}