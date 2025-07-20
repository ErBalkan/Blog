namespace Core.Abstract.Dtos;

// Bir kaynağı silmek için kullanılacak DTO'lar bu arayüzden türemelidir.
public interface IDeleteDto : IDto
{
    int Id { get; set; } // Silinecek kaynağın Id'si mutlaka olmalıdır.
}

