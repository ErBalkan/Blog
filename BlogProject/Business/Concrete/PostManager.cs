using BlogProject.Business.Abstract;
using BlogProject.Business.ValidationRules;
using BlogProject.Entities;
using Core.Abstract.DataAccess;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.Error;
using Core.Utilities.Results.Concrete.Success;

namespace BlogProject.Business.Concrete;
    public class PostManager : IPostService
    {
        private readonly IEntityRepository<Post> _postRepository;
        private readonly ICategoryService _categoryService; // Kategori varlığını kontrol etmek için
        private readonly IUserService _userService; // Kullanıcı varlığını kontrol etmek için

        // Yapıcı metot (Constructor) ile bağımlılıkların enjeksiyonu.
        public PostManager(IEntityRepository<Post> postRepository, ICategoryService categoryService, IUserService userService)
        {
            _postRepository = postRepository;
            _categoryService = categoryService;
            _userService = userService;
        }

        public async Task<IResult> AddAsync(Post post)
        {
            // 1. Doğrulama:
            var validator = new PostValidator();
            var validationResult = await validator.ValidateAsync(post);
            if (!validationResult.IsValid)
            {
                return new ErrorResult(validationResult.Errors[0].ErrorMessage);
            }

            // 2. İş Kuralı Kontrolü: Category ve User var mı? (Aktif olduklarını da kontrol ederiz)
            var categoryResult = await _categoryService.GetByIdAsync(post.CategoryId);
            if (!categoryResult.Success || categoryResult.Data == null)
            {
                return new ErrorResult("Belirtilen kategori bulunamadı veya aktif değil.");
            }

            var userResult = await _userService.GetByIdAsync(post.UserId);
            if (!userResult.Success || userResult.Data == null)
            {
                return new ErrorResult("Belirtilen yazar (kullanıcı) bulunamadı veya aktif değil.");
            }

            // 3. Ekleme işlemi:
            post.CreatedDate = DateTime.Now;
            post.PublishDate = DateTime.Now; // Yayın tarihi otomatik olarak şimdiki zaman olarak ayarlanır.
            post.ViewCount = 0; // Görüntülenme sayısı varsayılan olarak 0 başlar.
            post.IsDeleted = false; // Yeni makale varsayılan olarak silinmemiş olmalı.
            
            await _postRepository.AddAsync(post);
            return new SuccessResult("Makale başarıyla eklendi.");
        }

        public async Task<IResult> DeleteAsync(int postId)
        {
            var postToDelete = await _postRepository.GetByIdAsync(postId);
            if (postToDelete == null)
            {
                return new ErrorResult("Silinecek makale bulunamadı.");
            }

            // Soft delete
            await _postRepository.DeleteAsync(postToDelete); 
            return new SuccessResult("Makale başarıyla silindi.");
        }

        public async Task<IDataResult<List<Post>>> GetAllAsync()
        {
            var posts = await _postRepository.GetAllAsync();
            return new SuccessDataResult<List<Post>>(posts, "Makaleler başarıyla listelendi.");
        }

        public async Task<IDataResult<Post>> GetByIdAsync(int postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return new ErrorDataResult<Post>(null, "Makale bulunamadı.");
            }
            return new SuccessDataResult<Post>(post, "Makale başarıyla getirildi.");
        }

        public async Task<IDataResult<List<Post>>> GetPostsByCategoryIdAsync(int categoryId)
        {
            // İlgili kategorinin varlığını kontrol edebiliriz
            var categoryResult = await _categoryService.GetByIdAsync(categoryId);
            if (!categoryResult.Success || categoryResult.Data == null)
            {
                return new ErrorDataResult<List<Post>>(null, "Belirtilen kategori bulunamadı veya aktif değil.");
            }

            var posts = await _postRepository.GetAllAsync(p => p.CategoryId == categoryId);
            return new SuccessDataResult<List<Post>>(posts, $"'{categoryResult.Data.Name}' kategorisine ait makaleler listelendi.");
        }

        public async Task<IDataResult<List<Post>>> GetPostsByUserIdAsync(int userId)
        {
            // İlgili kullanıcının varlığını kontrol edebiliriz
            var userResult = await _userService.GetByIdAsync(userId);
            if (!userResult.Success || userResult.Data == null)
            {
                return new ErrorDataResult<List<Post>>(null, "Belirtilen yazar (kullanıcı) bulunamadı veya aktif değil.");
            }

            var posts = await _postRepository.GetAllAsync(p => p.UserId == userId);
            return new SuccessDataResult<List<Post>>(posts, $"'{userResult.Data.Username}' kullanıcısına ait makaleler listelendi.");
        }

        public async Task<IResult> UpdateAsync(Post post)
        {
            // 1. Doğrulama:
            var validator = new PostValidator();
            var validationResult = await validator.ValidateAsync(post);
            if (!validationResult.IsValid)
            {
                return new ErrorResult(validationResult.Errors[0].ErrorMessage);
            }

            // 2. İş Kuralı Kontrolü: Category ve User var mı?
            var categoryResult = await _categoryService.GetByIdAsync(post.CategoryId);
            if (!categoryResult.Success || categoryResult.Data == null)
            {
                return new ErrorResult("Belirtilen kategori bulunamadı veya aktif değil.");
            }

            var userResult = await _userService.GetByIdAsync(post.UserId);
            if (!userResult.Success || userResult.Data == null)
            {
                return new ErrorResult("Belirtilen yazar (kullanıcı) bulunamadı veya aktif değil.");
            }

            // 3. Güncelleme işlemi:
            post.UpdatedDate = DateTime.Now;
            await _postRepository.UpdateAsync(post);
            return new SuccessResult("Makale başarıyla güncellendi.");
        }
    }