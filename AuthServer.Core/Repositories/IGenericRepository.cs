using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetAsync(int id);

        IQueryable<TEntity> GetAllAsync();

        IQueryable<TEntity> Where(Expression<Func<TEntity,bool>> predicate);

        Task AddAsync(TEntity entity); // memory'e bir data eklediğinden dolayı bir asenkron metodu vardır. daha sonra SaveChanges methodu çağırılarak memorydeki entityler kontrol edilir ve veri tabanına kaydedilir.
        void Remove(int id); // entity'nin state'ini 'deleted' olarak işaretler bu yüzden asenkron metodu yok

        TEntity Update(TEntity entity);
    }
}
