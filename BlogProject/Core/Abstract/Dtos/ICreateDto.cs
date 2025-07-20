namespace Core.Abstract.Dtos;

// Yeni bir kaynak oluşturmak için kullanılacak DTO'lar bu arayüzden türemelidir.
public interface ICreateDto : IDto
{
    // Create DTO'lar genellikle Id içermez, çünkü Id veritabanı tarafından atanır.
}
