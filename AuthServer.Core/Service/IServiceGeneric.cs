using SharedLibrary.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Service
{
    public interface IServiceGeneric<TEntity, TDto> where TEntity : class where TDto : class
    {
        Task<Response<TDto>> GetByIdAsync(int id); // geriye dto nesneleri dönelim

        Task<Response<IEnumerable<TDto>>> GetAllAsync();

        Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate); //where, (product=>product.ıd>5) gibi

        Task<Response<TDto>> AddAsync(TDto entity); // Db'de oluşturduğumuz entity'yi dto olarak geri dönderelim
        Task<Response<NoDataDto>> Remove(int id); // artık void döndüremeyiz, bu yüzden geriye boş nesne gönderelim

        Task<Response<NoDataDto>> Update(TDto entity,int id);
    }
}
