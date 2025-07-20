using Core.Abstract.Entities;

namespace BlogProject.Entities;

    public class User : BaseEntity
    {
        public required string FirstName { get; set; } // Kullanıcının adı
        public required string LastName { get; set; } // Kullanıcının soyadı
        public required string Email { get; set; } // E-posta adresi (benzersiz olmalı)
        public required string PasswordHash { get; set; } // Parolanın hash'lenmiş hali (güvenlik için)
        public required string Username { get; set; } // Kullanıcı adı (benzersiz olmalı)
        public required string ProfilePictureUrl { get; set; } // Profil resmi URL'si (isteğe bağlı)

        // Navigation Property: Bir kullanıcının birden fazla Post'u olabilir.
        public ICollection<Post> Posts { get; set; } // Bu kullanıcının yazdığı tüm yazılar

        // Navigation Property: Bir kullanıcının birden fazla Comment'i olabilir.
        public ICollection<Comment> Comments { get; set; } // Bu kullanıcının yaptığı tüm yorumlar

        // Constructor: Yeni bir User oluşturulduğunda varsayılan değerleri ayarlar.
        public User()
        {
            Posts = new HashSet<Post>(); // Yazılar listesini başlatırız
            Comments = new HashSet<Comment>(); // Yorumlar listesini başlatırız
        }
    }