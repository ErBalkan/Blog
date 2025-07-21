using BlogProject.Entities;
using Core.Utilities.Results.Abstract;

namespace BlogProject.Business.Abstract;
    // Kullanıcı işlemleri için servis arayüzü
public interface IUserService
{
    Task<IDataResult<User>> GetByIdAsync(int userId);
    Task<IDataResult<User>> GetByEmailAsync(string email); // E-posta ile kullanıcı bulma
    Task<IDataResult<User>> GetByUsernameAsync(string username); // Kullanıcı adı ile kullanıcı bulma
    Task<IDataResult<List<User>>> GetAllAsync();
    Task<IResult> AddAsync(User user);
    Task<IResult> UpdateAsync(User user);
    Task<IResult> DeleteAsync(int userId);

    // Kullanıcı girişi gibi özel iş operasyonları eklenebilir.
    // Asenkron bir görev döndüren ve bool türünde bir sonuç içeren bir metot.
    // bool değeri, kimlik doğrulamanın başarılı olup olmadığını gösterir.
    Task<IDataResult<bool>> AuthenticateUserAsync(string username, string password); 
}