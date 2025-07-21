using BlogProject.Entities;
using FluentValidation;

namespace BlogProject.Business.ValidationRules;
    public class PostValidator : AbstractValidator<Post>
    {
    public PostValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("Başlık boş olamaz.")
            .NotNull().WithMessage("Başlık boş olamaz.")
            .MinimumLength(5).WithMessage("Başlık en az 5 karakter olmalıdır.")
            .MaximumLength(200).WithMessage("Başlık en fazla 200 karakter olmalıdır.");

        RuleFor(p => p.Content)
            .NotEmpty().WithMessage("İçerik boş olamaz.")
            .NotNull().WithMessage("İçerik boş olamaz.")
            .MinimumLength(20).WithMessage("İçerik en az 20 karakter olmalıdır."); // Daha uzun bir içerik bekliyoruz.
            
        RuleFor(p => p.CategoryId)
            .NotEmpty().WithMessage("Kategori seçimi zorunludur.")
            .NotNull().WithMessage("Kategori seçimi zorunludur.")
            .GreaterThan(0).WithMessage("Geçerli bir kategori seçilmelidir.");

        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage("Yazar bilgisi zorunludur.")
            .NotNull().WithMessage("Yazar bilgisi zorunludur.")
            .GreaterThan(0).WithMessage("Geçerli bir yazar seçilmelidir.");

        RuleFor(p => p.ImageUrl)
            .MaximumLength(500).WithMessage("Görsel URL'si en fazla 500 karakter olmalıdır.");

        RuleFor(p => p.ViewCount)
            .GreaterThanOrEqualTo(0).WithMessage("Görüntülenme sayısı negatif olamaz.");

        RuleFor(p => p.PublishDate)
            .NotEmpty().WithMessage("Yayın tarihi boş olamaz.")
            .NotNull().WithMessage("Yayın tarihi boş olamaz.")
            .Must(date => date != default(DateTime)).WithMessage("Geçerli bir yayın tarihi girilmelidir."); // Tarihin varsayılan değer olmamasını kontrol eder.
    }
}