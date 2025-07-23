using BlogProject.Business.Abstract;
using BlogProject.Business.ValidationRules;
using BlogProject.Entities;
using Core.Abstract.DataAccess;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.Error;
using Core.Utilities.Results.Concrete.Success;

namespace BlogProject.Business.Concrete;

public class UserManager : IUserService
{
    private readonly IEntityRepository<User> _userRepository;

    public UserManager(IEntityRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IMyResult> AddAsync(User user)
    {
        // 1. Doğrulama:
        var validator = new UserValidator();
        var validationResult = await validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            return new ErrorResult(validationResult.Errors[0].ErrorMessage);
        }

        // 2. İş Kuralı Kontrolü: E-posta veya kullanıcı adı zaten mevcut mu? (Aktif kayıtlar arasında)
        var existingUserByEmail = await _userRepository.GetAsync(u => u.Email.ToLower() == user.Email.ToLower() && !u.IsDeleted);
        if (existingUserByEmail != null)
        {
            return new ErrorResult("Bu e-posta adresi zaten kullanımda.");
        }

        var existingUserByUsername = await _userRepository.GetAsync(u => u.Username.ToLower() == user.Username.ToLower() && !u.IsDeleted);
        if (existingUserByUsername != null)
        {
            return new ErrorResult("Bu kullanıcı adı zaten kullanımda.");
        }

        // 3. Parola Hashleme:
        // Parolayı veritabanına kaydetmeden önce güvenli bir şekilde hash'liyoruz.
        // BCrypt.Net-Next paketi, güvenli parola hash'leme için kullanılır.
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        // 4. Ekleme işlemi:
        user.CreatedDate = DateTime.Now;
        user.IsDeleted = false; // Yeni kullanıcı varsayılan olarak silinmemiş olmalı.

        await _userRepository.AddAsync(user);
        return new SuccessResult("Kullanıcı başarıyla kaydedildi.");
    }

// Kullanıcı kimlik doğrulama işlemi (Giriş)
public async Task<IDataResult<bool>> AuthenticateUserAsync(string username, string password)
{
    // Kullanıcı adını kullanarak aktif kullanıcıyı veritabanından getir.
    // _userRepository.GetAsync: Veritabanından tek bir kullanıcıyı asenkron olarak getirir.
    // u => u.Username.ToLower() == username.ToLower() && !u.IsDeleted: 
    //   - Veritabanındaki kullanıcı adını (u.Username) ve metoda gelen kullanıcı adını (username)
    //     küçük harflere dönüştürerek karşılaştırır. Bu, büyük/küçük harf duyarsız arama yapar.
    //   - Ayrıca, sadece 'IsDeleted' alanı 'false' olan, yani silinmemiş (aktif) kullanıcıları ararız.
    var user = await _userRepository.GetAsync(u => u.Username.ToLower() == username.ToLower() && !u.IsDeleted);

    // Eğer kullanıcı bulunamazsa veya silinmişse kimlik doğrulama başarısız.
    // user == null kontrolü, belirtilen kullanıcı adıyla bir kayıt bulunup bulunmadığını gösterir.
    if (user == null)
    {
        // Kullanıcı bulunamadığı için bir hata sonucu döndürüyoruz.
        // Success: false (zaten ErrorDataResult olduğu için), Data: null (veya default(bool) yani false)
        // Message: "Kullanıcı bulunamadı."
        return new ErrorDataResult<bool>("Kullanıcı bulunamadı.");
    }

    // Sağlanan parolayı, veritabanındaki hashlenmiş parola ile karşılaştır.
    // BCrypt.Net.BCrypt.Verify: BCrypt kütüphanesinin ana doğrulama metodudur.
    //   - İlk parametre (password): Kullanıcının girdiği düz metin parola.
    //   - İkinci parametre (user.PasswordHash): Veritabanında saklanan, hashlenmiş parola.
    //   - Bu metot, düz metin parolayı hash'ler ve veritabanındaki hash ile eşleşip eşleşmediğini güvenli bir şekilde kontrol eder.
    //   - Parolalar eşleşiyorsa 'true', eşleşmiyorsa 'false' döner.
    bool result = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    
    // BCrypt.Verify metodunun sonucuna göre geri dönüş değeri oluşturulur.
    if (result == true)
    {
        // Parolalar eşleştiyse başarılı bir sonuç döndürülür.
        // Success: true, Data: true, Message: "Parola eşleşti."
        return new SuccessDataResult<bool>(true, "Parola eşleşti."); 
    }
    else
    {
        // Parolalar eşleşmediyse hata sonucu döndürülür.
        // Success: false, Data: false, Message: "Parola eşleşmiyor."
        return new ErrorDataResult<bool>(false, "Parola eşleşmiyor."); 
    }
}

    public async Task<IMyResult> DeleteAsync(int userId)
    {
        var userToDelete = await _userRepository.GetByIdAsync(userId);
        if (userToDelete == null)
        {
            return new ErrorResult("Silinecek kullanıcı bulunamadı.");
        }

        // İş Kuralı: Kullanıcıya bağlı aktif makaleler veya yorumlar varsa silmeyi engelleyebiliriz.
        // Bunun için ilgili servisleri (PostService, CommentService) enjekte etmemiz gerekebilir.
        // Örneğin:
        // var postsOfUserResult = await _postService.GetPostsByUserIdAsync(userId);
        // if (postsOfUserResult.Data != null && postsOfUserResult.Data.Any()) {
        //     return new ErrorResult("Bu kullanıcıya ait aktif makaleler var, silinemez.");
        // }

        await _userRepository.DeleteAsync(userToDelete); // Soft delete
        return new SuccessResult("Kullanıcı başarıyla silindi.");
    }

    public async Task<IDataResult<List<User>>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return new SuccessDataResult<List<User>>(users, "Kullanıcılar başarıyla listelendi.");
    }

    public async Task<IDataResult<User>> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetAsync(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted);
        if (user == null)
        {
            return new ErrorDataResult<User>(null, "E-posta adresiyle kullanıcı bulunamadı.");
        }
        return new SuccessDataResult<User>(user, "Kullanıcı başarıyla getirildi.");
    }

    public async Task<IDataResult<User>> GetByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return new ErrorDataResult<User>(null, "Kullanıcı bulunamadı.");
        }
        return new SuccessDataResult<User>(user, "Kullanıcı başarıyla getirildi.");
    }

    public async Task<IDataResult<User>> GetByUsernameAsync(string username)
    {
        var user = await _userRepository.GetAsync(u => u.Username.ToLower() == username.ToLower() && !u.IsDeleted);
        if (user == null)
        {
            return new ErrorDataResult<User>(null, "Kullanıcı adıyla kullanıcı bulunamadı.");
        }
        return new SuccessDataResult<User>(user, "Kullanıcı başarıyla getirildi.");
    }

    public async Task<IMyResult> UpdateAsync(User user)
    {
        // 1. Doğrulama:
        var validator = new UserValidator();
        var validationResult = await validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            return new ErrorResult(validationResult.Errors[0].ErrorMessage);
        }

        // 2. İş Kuralı Kontrolü: Güncellenen e-posta veya kullanıcı adı başka bir aktif kullanıcıya ait mi?
        var existingUserByEmail = await _userRepository.GetAsync(u => u.Email.ToLower() == user.Email.ToLower() && u.Id != user.Id && !u.IsDeleted);
        if (existingUserByEmail != null)
        {
            return new ErrorResult("Bu e-posta adresi zaten kullanımda.");
        }

        var existingUserByUsername = await _userRepository.GetAsync(u => u.Username.ToLower() == user.Username.ToLower() && u.Id != user.Id && !u.IsDeleted);
        if (existingUserByUsername != null)
        {
            return new ErrorResult("Bu kullanıcı adı zaten kullanımda.");
        }

        // Parola güncellenirken özel bir dikkat gerekir. Eğer parola alanı değişiyorsa hash'lemeliyiz.
        // Genellikle parola güncelleme ayrı bir API endpoint'i veya Business metodu ile yapılır.
        // Burada basitçe, eğer PasswordHash alanı değişmişse, tekrar hash'leyelim.
        var existingUser = await _userRepository.GetByIdAsync(user.Id);
        if (existingUser != null && !string.IsNullOrEmpty(user.PasswordHash) && existingUser.PasswordHash != user.PasswordHash)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        }
        else if (existingUser != null)
        {
            // Eğer parola değişmediyse, mevcut hash'i koru.
            user.PasswordHash = existingUser.PasswordHash;
        }

        user.UpdatedDate = DateTime.Now;
        await _userRepository.UpdateAsync(user);
        return new SuccessResult("Kullanıcı başarıyla güncellendi.");
    }

}

/*
UserManager parola yönetimi: Parolaların hash'lenerek saklanması 
ve doğrulama sırasında BCrypt.Net.BCrypt.Verify kullanılması 
güvenlik için kritik öneme sahiptir. Güncelleme işleminde 
parolanın değişip değişmediğini kontrol eden basit bir mantık da eklendi.
*/