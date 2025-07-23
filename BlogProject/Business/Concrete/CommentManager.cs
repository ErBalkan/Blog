using BlogProject.Business.Abstract;
using BlogProject.Business.ValidationRules;
using BlogProject.Entities;
using Core.Abstract.DataAccess;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.Error;
using Core.Utilities.Results.Concrete.Success;

namespace BlogProject.Business.Concrete;

    public class CommentManager : ICommentService
    {
        private readonly IEntityRepository<Comment> _commentRepository;
        private readonly IPostService _postService; // Yorum yapılacak Post'un varlığını kontrol etmek için
        private readonly IUserService _userService; // Yorum yapan User'ın varlığını kontrol etmek için

        public CommentManager(IEntityRepository<Comment> commentRepository, IPostService postService, IUserService userService)
        {
            _commentRepository = commentRepository;
            _postService = postService;
            _userService = userService;
        }

        public async Task<IMyResult> AddAsync(Comment comment)
        {
            // 1. Doğrulama:
            var validator = new CommentValidator();
            var validationResult = await validator.ValidateAsync(comment);
            if (!validationResult.IsValid)
            {
                return new ErrorResult(validationResult.Errors[0].ErrorMessage);
            }

            // 2. İş Kuralı Kontrolü: Yorum yapılacak Post var mı?
            var postResult = await _postService.GetByIdAsync(comment.PostId);
            if (!postResult.Success || postResult.Data == null)
            {
                return new ErrorResult("Yorum yapılacak makale bulunamadı veya aktif değil.");
            }

            // 3. İş Kuralı Kontrolü: Yorum yapan User var mı? (opsiyonel olduğu için sadece varsa kontrol)
            if (comment.UserId.HasValue) // Eğer UserId değeri verilmişse
            {
                var userResult = await _userService.GetByIdAsync(comment.UserId.Value);
                if (!userResult.Success || userResult.Data == null)
                {
                    return new ErrorResult("Yorumu yapan kullanıcı bulunamadı veya aktif değil.");
                }
            }

            // 4. Ekleme işlemi:
            comment.CreatedDate = DateTime.Now;
            comment.CommentDate = DateTime.Now; // Yorum tarihi otomatik olarak şimdiki zaman olarak ayarlanır.
            comment.IsDeleted = false; // Yeni yorum varsayılan olarak silinmemiş olmalı.

            await _commentRepository.AddAsync(comment);
            return new SuccessResult("Yorum başarıyla eklendi.");
        }

        public async Task<IMyResult> DeleteAsync(int commentId)
        {
            var commentToDelete = await _commentRepository.GetByIdAsync(commentId);
            if (commentToDelete == null)
            {
                return new ErrorResult("Silinecek yorum bulunamadı.");
            }

            await _commentRepository.DeleteAsync(commentToDelete); // Soft delete
            return new SuccessResult("Yorum başarıyla silindi.");
        }

        public async Task<IDataResult<List<Comment>>> GetAllAsync()
        {
            var comments = await _commentRepository.GetAllAsync();
            return new SuccessDataResult<List<Comment>>(comments, "Yorumlar başarıyla listelendi.");
        }

        public async Task<IDataResult<Comment>> GetByIdAsync(int commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
            {
                return new ErrorDataResult<Comment>(null, "Yorum bulunamadı.");
            }
            return new SuccessDataResult<Comment>(comment, "Yorum başarıyla getirildi.");
        }

        public async Task<IDataResult<List<Comment>>> GetCommentsByPostIdAsync(int postId)
        {
            // İlgili makalenin varlığını kontrol et
            var postResult = await _postService.GetByIdAsync(postId);
            if (!postResult.Success || postResult.Data == null)
            {
                return new ErrorDataResult<List<Comment>>(null, "Belirtilen makale bulunamadı veya aktif değil.");
            }

            var comments = await _commentRepository.GetAllAsync(c => c.PostId == postId);
            return new SuccessDataResult<List<Comment>>(comments, $"'{postResult.Data.Title}' başlıklı makaleye ait yorumlar listelendi.");
        }

        public async Task<IMyResult> UpdateAsync(Comment comment)
        {
            // 1. Doğrulama:
            var validator = new CommentValidator();
            var validationResult = await validator.ValidateAsync(comment);
            if (!validationResult.IsValid)
            {
                return new ErrorResult(validationResult.Errors[0].ErrorMessage);
            }

            // 2. İş Kuralı Kontrolü: Yorum yapılacak Post var mı?
            var postResult = await _postService.GetByIdAsync(comment.PostId);
            if (!postResult.Success || postResult.Data == null)
            {
                return new ErrorResult("Yorum yapılacak makale bulunamadı veya aktif değil.");
            }

            // 3. İş Kuralı Kontrolü: Yorum yapan User var mı?
            if (comment.UserId.HasValue)
            {
                var userResult = await _userService.GetByIdAsync(comment.UserId.Value);
                if (!userResult.Success || userResult.Data == null)
                {
                    return new ErrorResult("Yorumu yapan kullanıcı bulunamadı veya aktif değil.");
                }
            }

            // 4. Güncelleme işlemi:
            comment.UpdatedDate = DateTime.Now;
            await _commentRepository.UpdateAsync(comment);
            return new SuccessResult("Yorum başarıyla güncellendi.");
        }
    }