using ElMagzer.Core.Models;
using ElMagzer.Core.Specifications;
namespace ElMagzer.Core.Repositories
{
    public interface IGenericRepository<T> where T : BaseModel
    {
        Task<IReadOnlyList<T>> GetAllAysnc();
        Task<T> GetByIdAysnc(int id);
        Task<IReadOnlyList<T>> GetAllAysncWithspec(ISpecifications<T> spec);
        Task<T> GetByIDAysncWithspec(ISpecifications<T> spec);

        Task<T> AddAsync(T entity);
        T Update(T entity);
        T Delete(T entity);

    }
}
