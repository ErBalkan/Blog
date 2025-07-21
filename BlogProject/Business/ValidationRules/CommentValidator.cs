using BlogProject.Entities;
using FluentValidation;

namespace BlogProject.Business.ValidationRules;
public class CommentValidator : AbstractValidator<Comment>
{
    public CommentValidator()
    {
        RuleFor(c => c.Text)
            .NotEmpty().WithMessage("Yorum içeriği boş olamaz.")
            .NotNull().WithMessage("Yorum içeriği boş olamaz.")
            .MinimumLength(5).WithMessage("Yorum en az 5 karakter olmalıdır.")
            .MaximumLength(1000).WithMessage("Yorum en fazla 1000 karakter olmalıdır.");

        RuleFor(c => c.PostId)
            .NotEmpty().WithMessage("Yorum yapılacak makale seçimi zorunludur.")
            .NotNull().WithMessage("Yorum yapılacak makale seçimi zorunludur.")
            .GreaterThan(0).WithMessage("Geçerli bir makale seçilmelidir.");

        // UserId opsiyonel olduğu için NotNull/NotEmpty kullanmıyoruz, ancak girilirse pozitif bir sayı olmalı.
        RuleFor(c => c.UserId)
            .GreaterThan(0).When(c => c.UserId.HasValue).WithMessage("Geçerli bir kullanıcı Id'si girilmelidir.");

        RuleFor(c => c.CommentDate)
            .NotEmpty().WithMessage("Yorum tarihi boş olamaz.")
            .NotNull().WithMessage("Yorum tarihi boş olamaz.")
            .Must(date => date != default(DateTime)).WithMessage("Geçerli bir yorum tarihi girilmelidir.");
    }
}