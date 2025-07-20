using Core.Abstract.Entities;

namespace BlogProject.Entities;

    public class Post : BaseEntity
    {
        public required string Title { get; set; } // Makale başlığı
        public required string Content { get; set; } // Makale içeriği
        public required string ImageUrl { get; set; } // Makale resmi için URL (isteğe bağlı)
        public int ViewCount { get; set; } // Görüntülenme sayısı
        public DateTime PublishDate { get; set; } // Makalenin yayınlandığı tarih

        // Foreign Key: Post'un ait olduğu Category'nin Id'si
        public int CategoryId { get; set; }
        // Navigation Property: Post'un ait olduğu Category nesnesi
        public required Category Category { get; set; } // Bir Post'un bir Category'si vardır.

        // Foreign Key: Post'u yazan User'ın Id'si
        public int UserId { get; set; }
        // Navigation Property: Post'u yazan User nesnesi
        public required User User { get; set; } // Bir Post'un bir yazarı (User) vardır.

        // Navigation Property: Bir Post'un birden fazla yorumu olabilir.
        public ICollection<Comment> Comments { get; set; } // Bu yazıya yapılan tüm yorumlar

        // Constructor: Yeni bir Post oluşturulduğunda varsayılan değerleri ayarlar.
        public Post()
        {
            PublishDate = DateTime.Now; // Yayın tarihi o anki zaman olarak ayarlanır.
            ViewCount = 0; // Başlangıçta görüntülenme sayısı 0.
            Comments = new HashSet<Comment>(); // Yorumlar listesini başlatırız
        }
    }

/*

* Title, Content, ImageUrl, ViewCount, PublishDate: Makaleye özgü özellikler.

* CategoryId, Category: Foreign Key ve Navigation Property'dir. 
Bir gönderinin bir kategoriye ait olduğunu belirtir (Çoktan Bire - Many-to-One ilişki).

* UserId, User: Foreign Key ve Navigation Property'dir. 
Bir gönderinin bir kullanıcıya (yazar) ait olduğunu belirtir.

* ICollection<Comment> Comments: Navigation Property'dir. 
Bir gönderiye ait birden fazla yorum olabileceğini gösterir.

* Post() Constructor'ında PublishDate ve ViewCount için başlangıç 
değerleri atadık. Ayrıca Comments koleksiyonunu HashSet<Comment>() ile başlattık. 
HashSet genellikle performanslı arama ve benzersiz öğe saklama için tercih edilir.    
*/