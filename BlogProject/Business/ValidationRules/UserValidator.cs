using BlogProject.Entities;
using FluentValidation;

namespace BlogProject.Business.ValidationRules;
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty().WithMessage("Ad boş olamaz.")
            .NotNull().WithMessage("Ad boş olamaz.")
            .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olmalıdır.");

        RuleFor(u => u.LastName)
            .NotEmpty().WithMessage("Soyad boş olamaz.")
            .NotNull().WithMessage("Soyad boş olamaz.")
            .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olmalıdır.");

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("E-posta boş olamaz.")
            .NotNull().WithMessage("E-posta boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girilmelidir.") // E-posta formatı kontrolü
            .MaximumLength(100).WithMessage("E-posta en fazla 100 karakter olmalıdır.");

        RuleFor(u => u.Username)
            .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
            .NotNull().WithMessage("Kullanıcı adı boş olamaz.")
            .MinimumLength(4).WithMessage("Kullanıcı adı en az 4 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olmalıdır.");

        RuleFor(u => u.PasswordHash) // Bu alana hash değeri geleceği için içeriğini kontrol etmiyoruz, sadece null/boş olmamasını.
            .NotEmpty().WithMessage("Parola boş olamaz.") // Parola hash'i boş olamaz.
            .NotNull().WithMessage("Parola boş olamaz.")
            .MinimumLength(6).WithMessage("Parola en az 6 karakter olmalıdır."); // Hash'in uzunluğu değil, orijinal parolanın uzunluğu gibi düşünülebilir.

        RuleFor(u => u.ProfilePictureUrl)
            .MaximumLength(500).WithMessage("Profil resmi URL'si en fazla 500 karakter olmalıdır.");
    }
}