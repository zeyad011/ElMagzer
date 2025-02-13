using ElMagzer.Core.Models;
using ElMagzer.Core.Repositories;
using ElMagzer.Core.Specifications;
using ElMagzer.Repository.Data;
using Microsoft.EntityFrameworkCore;


namespace ElMagzer.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        private readonly ElMagzerContext context;

        public GenericRepository(ElMagzerContext _context) {
            context = _context;
        }

        public async Task<T> AddAsync(T entity)
        {
            await context.AddAsync(entity);
            context.SaveChanges();
            return entity;
        }
        public T Delete(T entity)
        {
            context.Remove(entity);
            context.SaveChanges();
            return entity;
        }
        public async Task<IReadOnlyList<T>> GetAllAysnc()
        {
            return await context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAysnc(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public T Update(T entity)
        {
            context.Update(entity);
            context.SaveChanges();
            return entity;
        }
        public async Task<IReadOnlyList<T>> GetAllAysncWithspec(ISpecifications<T> spec)
        {
           return await ApplySpecfiication(spec).ToListAsync();
        }
        public async Task<T> GetByIDAysncWithspec(ISpecifications<T> spec)
        {
            return await ApplySpecfiication(spec).FirstOrDefaultAsync();
        }

        private IQueryable<T> ApplySpecfiication(ISpecifications<T> spec)
        {
            return SpecificationEvalutor<T>.GetQuery(context.Set<T>(), spec);
        }
    }
}
