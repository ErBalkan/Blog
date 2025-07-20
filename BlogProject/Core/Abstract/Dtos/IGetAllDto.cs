namespace Core.Abstract.Dtos;

// Tüm kaynakları getirmek için kullanılacak DTO'lar bu arayüzden türemelidir.
// Genellikle sayfalama, sıralama ve filtreleme bilgileri içerir.
public interface IGetAllDto : IDto
{
    // Örneğin, Pagination ve Sorting için özellikler eklenebilir.
    // int PageNumber { get; set; }
    // int PageSize { get; set; }
    // string SortBy { get; set; }
    // bool SortOrderAscending { get; set; }
}
