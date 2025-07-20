using Core.Abstract.Entities;

namespace BlogProject.Entities;
    public class Comment : BaseEntity
    {
        public required string Text { get; set; } // Yorum içeriği
        public DateTime CommentDate { get; set; } // Yorumun yapıldığı tarih

        // Foreign Key: Yorumun ait olduğu Post'un Id'si
        public int PostId { get; set; }
        // Navigation Property: Yorumun ait olduğu Post nesnesi
        public required Post Post { get; set; } // Bir yorumun bir Post'u vardır.

        // Foreign Key: Yorumu yapan User'ın Id'si (Anonim yorumlar için null olabilir)
        public int? UserId { get; set; } // ? işareti, UserId'nin null olabileceğini belirtir.
        // Navigation Property: Yorumu yapan User nesnesi
        public required User User { get; set; } // Bir yorumun bir yazarı (User) olabilir (isteğe bağlı).

        // Constructor: Yeni bir Comment oluşturulduğunda varsayılan değerleri ayarlar.
        public Comment()
        {
            CommentDate = DateTime.Now; // Yorum tarihi o anki zaman olarak ayarlanır.
        }
    }