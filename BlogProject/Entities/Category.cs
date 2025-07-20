using Core.Abstract.Entities;

namespace BlogProject.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; } // Kategori adı (örneğin "Yazılım")
    public required string Description { get; set; } // Kategori açıklaması

    // Navigation Property: Bir kategorinin birden fazla Post'u olabilir.
    // Bu, Entity Framework Core tarafından ilişkileri yönetmek için kullanılır.
    public ICollection<Post>? Posts { get; set; } // Bu kategorideki tüm yazılar
    // Bir kategoriye ait belki hiç yazı olmayabilir. o yüzden null olabilir olarak işaretlendi.
    }

/*
    - Name: Kategorinin adını tutar.
    - Description: Kategorinin kısa açıklamasını tutar.
    - ICollection<Post> Posts: Bu bir Navigation Property'dir. 
    Bir kategoriye ait birden fazla gönderi olabileceğini gösterir 
    (Bire Çok - One-to-Many ilişki). Entity Framework Core, bu sayede 
    ilişkili verileri kolayca getirmemizi sağlar.
*/