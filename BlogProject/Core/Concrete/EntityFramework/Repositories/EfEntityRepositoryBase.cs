using System.Linq.Expressions;
using Core.Abstract.DataAccess;
using Core.Abstract.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Concrete.EntityFramework.Repositories;

    // EfEntityRepositoryBase: IEntityRepository arayüzünü Entity Framework Core kullanarak uygular.
    // TEntity: Veritabanı varlığıdır, IEntity'den türemeli ve bir sınıf olmalıdır.
    // TContext: Bir DbContext'ten türemeli ve parametresiz bir yapıcı metoda sahip olmalıdır.
    public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
        where TEntity : class, IEntity // TEntity bir sınıf olmalı ve IEntity'yi uygulamalı.
        where TContext : DbContext, new() // TContext bir DbContext olmalı ve parametresiz yapıcı metodu olmalı.
    {
        // AddAsync: Yeni bir varlık ekler.
        public async Task AddAsync(TEntity entity)
        {
            // 'using' bloğu, DbContext örneğinin işi bittiğinde doğru şekilde dispose edilmesini sağlar.
            using (var context = new TContext())
            {
                // Eğer varlık BaseEntity'den türemişse, CreatedDate ve IsDeleted alanlarını set et.
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.CreatedDate = DateTime.Now;
                    baseEntity.IsDeleted = false; // Varsayılan olarak silinmemiş olsun
                }
                // DbContext'in ilgili DbSet'ine varlığı ekle.
                await context.Set<TEntity>().AddAsync(entity);
                // Değişiklikleri veritabanına kaydet.
                await context.SaveChangesAsync();
            }
        }

        // UpdateAsync: Mevcut bir varlığı günceller.
        public async Task UpdateAsync(TEntity entity)
        {
            using (var context = new TContext())
            {
                // Eğer varlık BaseEntity'den türemişse, UpdatedDate alanını set et.
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.UpdatedDate = DateTime.Now;
                }
                // DbContext'in ilgili DbSet'indeki varlığı 'Modified' olarak işaretle.
                context.Set<TEntity>().Update(entity);
                await context.SaveChangesAsync();
            }
        }

        // DeleteAsync: Bir varlığı veritabanından siler (bizim mimarimizde soft delete).
        public async Task DeleteAsync(TEntity entity)
        {
            using (var context = new TContext())
            {
                // Eğer varlık BaseEntity'den türemişse, 'IsDeleted' özelliğini true yaparak soft delete yap.
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.IsDeleted = true;
                    // Entity durumunu 'Modified' olarak ayarlayarak sadece IsDeleted alanının güncellenmesini sağla.
                    context.Entry(baseEntity).State = EntityState.Modified;
                }
                else
                {
                    // BaseEntity'den türemeyenler için doğrudan silme.
                    context.Set<TEntity>().Remove(entity);
                }
                await context.SaveChangesAsync();
            }
        }

        // GetAsync: Belirli bir filtreye uyan tek bir varlık getirir.
        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter)
        {
            using (var context = new TContext())
            {
                // SingleOrDefaultAsync: Filtreye uyan tek bir varlık veya null döndürür.
                return await context.Set<TEntity>().SingleOrDefaultAsync(filter);
            }
        }

        // GetAllAsync: Belirli bir filtreye göre tüm varlıkları veya tüm varlıkları getirir.
        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            using (var context = new TContext())
            {
                // Eğer filtre null ise tüm varlıkları, aksi takdirde filtreye uyanları getir.
                return filter == null
                    ? await context.Set<TEntity>().ToListAsync()
                    : await context.Set<TEntity>().Where(filter).ToListAsync();
            }
        }

        // GetByIdAsync: Id'ye göre tek bir varlık getirir.
        public async Task<TEntity?> GetByIdAsync(int id)
        {
            using (var context = new TContext())
            {
                // FindAsync: Primary key'e göre doğrudan varlığı bulur.
                return await context.Set<TEntity>().FindAsync(id);
            }
        }

        // Query: LINQ sorguları için IQueryable döndürür.
        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? filter = null)
        {
            // DbContext'i burada oluşturmak, sorgunun repository dışında (örneğin Business katmanında)
            // inşa edilmesine ve daha sonra execute edilmesine olanak tanır.
            var context = new TContext(); 

            return filter == null
                ? context.Set<TEntity>().AsQueryable()
                : context.Set<TEntity>().Where(filter).AsQueryable();
        }
    }