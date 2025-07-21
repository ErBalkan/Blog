using BlogProject.Entities;
using FluentValidation;

namespace BlogProject.Business.ValidationRules;
// Category sınıfı için doğrulama kuralları tanımlar.
public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        // Category'nin Name özelliği için kurallar:
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Kategori adı boş olamaz.") // Boş olmamalı
            .NotNull().WithMessage("Kategori adı boş olamaz.") // Null olmamalı
            .MinimumLength(3).WithMessage("Kategori adı en az 3 karakter olmalıdır.") // En az 3 karakter
            .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olmalıdır."); // En fazla 100 karakter

        // Category'nin Description özelliği için kurallar:
        RuleFor(c => c.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olmalıdır."); // En fazla 500 karakter (opsiyonel olduğu için NotEmpty/NotNull yok)
            
        // Id için doğrulama (genellikle Add operasyonunda Id 0 olur, Update'te ise > 0 olmalı)
        RuleFor(c => c.Id)
            .GreaterThanOrEqualTo(0).WithMessage("Id 0'dan küçük olamaz.");
    }
}