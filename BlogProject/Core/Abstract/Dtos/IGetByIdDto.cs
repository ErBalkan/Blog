namespace Core.Abstract.Dtos;
// Belirli bir kaynağı Id'ye göre getirmek için kullanılacak DTO'lar bu arayüzden türemelidir.
public interface IGetByIdDto : IDto
{
    int Id { get; set; } // Getirilecek kaynağın Id'si mutlaka olmalıdır.
}

