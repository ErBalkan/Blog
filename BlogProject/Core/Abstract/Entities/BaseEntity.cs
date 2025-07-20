namespace Core.Abstract.Entities;

public abstract class BaseEntity : IEntity
{
    public int Id { get; set; } // Tüm varlıkların sahip olacağı benzersiz kimlik. 
    public DateTime CreatedDate { get; set; } // Kaydın oluşturulduğu tarih.
    public DateTime? UpdatedDate { get; set; } // Kaydın son güncellendiği tarih (null olabilir).
    public bool IsDeleted { get; set; } // Kaydın silinip silinmediğini belirtir (Soft Delete için).

    // Constructor: Yeni bir varlık oluşturulduğunda varsayılan değerleri ayarlar.
    public BaseEntity()
    {
        CreatedDate = DateTime.Now; // Oluşturulma tarihi o anki zaman olarak ayarlanır.
        IsDeleted = false; // Varsayılan olarak silinmemiş kabul edilir.
    }
}

/*
abstract: Bu sınıfın doğrudan nesnesinin oluşturulamayacağı anlamına gelir. 
Sadece ondan türeyen sınıflar kullanılabilir.

Id (int): Tüm varlıklar için benzersiz tanımlayıcı. 
Genellikle birincil anahtar (Primary Key) olarak kullanılır.

CreatedDate (DateTime): Varlığın ne zaman oluşturulduğunu kaydeder. 
DateTime.Now ile varsayılan olarak o anki zamanı alır.

UpdatedDate (DateTime?): Varlığın en son ne zaman güncellendiğini kaydeder. 
DateTime? kullanımı, bu alanın null olabileceği anlamına gelir. 
Yani, varlık hiç güncellenmemişse bu alan boş kalabilir.

IsDeleted (bool): Soft Delete (Yumuşak Silme) denilen bir desen için kullanılır. 
Bir kaydı veritabanından tamamen silmek yerine, bu bayrağı true yaparak mantıksal olarak 
silinmiş gibi işaretleriz. Bu, veri kaybını önler ve geçmiş verilere erişim ihtiyacında faydalıdır.
*/