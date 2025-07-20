namespace Core.Abstract.Dtos;

// Mevcut bir kaynağı güncellemek için kullanılacak DTO'lar bu arayüzden türemelidir.
public interface IUpdateDto : IDto
{
    int Id { get; set; } // Güncellenecek kaynağın Id'si mutlaka olmalıdır.
}

// Id özelliği, hangi kaydın güncelleneceğini belirtmek için zorunludur.

