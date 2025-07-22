using System.Linq.Expressions;
using Core.Abstract.Entities;

namespace Core.Abstract.DataAccess;
    // TEntity: Bu repository'nin hangi varlık tipi için kullanılacağını belirtir.
    // TEntity, bir IEntity olmalı ve new() ile örneklenebilir olmalı (concrete sınıf olmalı).
    public interface IEntityRepository<TEntity> where TEntity : class, IEntity
    {
        // Tek bir varlığı Id'ye göre asenkron olarak getirir.
        Task<TEntity> GetByIdAsync(int id);

        // Belirli bir koşula uyan tek bir varlığı asenkron olarak getirir.
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter);

        // Belirli bir koşula uyan tüm varlıkları asenkron olarak getirir (liste).
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);

        // Yeni bir varlık ekler (asenkron olabilir ancak EF Core Add metodu genelde void veya non-async Task döndürür).
        // Ancak SaveChangesAsync() metodu mutlaka çağrılacağı için burada async bir Task döndürmek mantıklı.
        Task AddAsync(TEntity entity);

        // Mevcut bir varlığı günceller.
        Task UpdateAsync(TEntity entity);

        // Bir varlığı siler (genellikle IsDeleted bayrağını true yaparız).
        Task DeleteAsync(TEntity entity);

        // LINQ sorgularını doğrudan çalıştırabilmek için IQueryable döndürür.
        // IQueryable doğrudan veritabanı sorgusunu tetiklemediği için bu metodu asenkron yapmaya gerek yoktur.
        // Ancak ToListAsync() veya FirstOrDefaultAsync() gibi metodlar çağrılırken asenkronluk devreye girer.
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? filter = null);
    }


/*
    - where TEntity : class, IEntity, new(): Bu bir kısıtlamadır (constraint). 
    TEntity'nin ne olması gerektiğini belirtir:
    ********************************************************************************
    
    - class: TEntity bir referans tipi olmalıdır.
    
    - IEntity: TEntity mutlaka IEntity arayüzünü uygulamalıdır 
    (bu, onun bir veritabanı varlığı olduğunu garantiler).

    - new(): TEntity parametresiz bir yapıcıya (constructor) sahip olmalıdır 
    (bu, Entity Framework gibi ORM'lerin varlıkları örneklemesi için gereklidir).
    
    ********************************************************************************

    - Expression<Func<TEntity, bool>> filter: Bu, LINQ sorgularında kullanılan filtre 
    ifadelerini temsil eder. Örneğin, u => u.Email == "test@example.com" gibi 
    lambda ifadelerini alabilir. Bu sayede repository'mize dinamik filtreleme yeteneği 
    kazandırırız. filter = null ise tüm kayıtları getireceği anlamına gelir.
    
    ********************************************************************************

    - IQueryable<TEntity> Query(...): Bu metot, doğrudan bir List döndürmek yerine bir 
    IQueryable döndürür. IQueryable sayesinde, veritabanına sorgu gönderilmeden önce 
    (yani "deferred execution" - ertelenmiş yürütme prensibi ile) sorguya daha fazla filtre, 
    sıralama veya dahil etme (include) işlemi ekleyebiliriz. Bu, esneklik sağlar ve gereksiz 
    veri çekmeyi önler.
    */

// ******************************************************************************************
// ******************************************************************************************
// ******************************************************************************************
                            // ASENKRON YAPI // 
/*
    - Task<T> dönüş tipi: Veritabanı işlemleri uzun sürebileceği için, 
    metotlar artık işlemi temsil eden bir Task nesnesi döndürüyor. 
    Task<T> ise T tipinde bir değer döndürecek asenkron bir işlemi temsil eder.

    - Async soneki: Metot isimlerinin sonuna Async ekledik (GetById yerine GetByIdAsync). 
    Bu, C# dilinde asenkron metotlar için yaygın bir isimlendirme kuralıdır ve kodun daha 
    okunabilir olmasını sağlar.

    - AddAsync, UpdateAsync, DeleteAsync: Bu metotlar da artık Task döndürüyor. 
    Entity Framework Core'da Add, Update gibi metotlar genellikle senkrondur çünkü 
    bu metotlar sadece entity'yi Change Tracker'a ekler; asıl veritabanı işlemi 
    SaveChangesAsync() metodu ile gerçekleşir. Ancak, bu repository metotlarını çağıran 
    üst katmanın (örneğin Business katmanının) tutarlı bir asenkron imza beklentisi olacağından,
    burada Task döndürmek ve implementasyonda await _context.SaveChangesAsync() kullanmak 
    daha doğru bir yaklaşımdır.

    - Query metodu: Query metodu, bir IQueryable döndürüyor. IQueryable doğrudan veritabanı 
    işlemini tetiklemez; sorgu tanımını oluşturur. Bu nedenle kendisi asenkron olmak 
    zorunda değildir. Asenkronluk, bu IQueryable üzerinde ToListAsync(), 
    FirstOrDefaultAsync() gibi metotlar çağrıldığında devreye girer.
    */