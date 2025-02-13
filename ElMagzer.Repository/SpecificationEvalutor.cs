using ElMagzer.Core.Models;
using ElMagzer.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ElMagzer.Repository
{
    public class SpecificationEvalutor<TEntity> where TEntity : BaseModel
    {

        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,ISpecifications<TEntity> spec)
        {
            var query = inputQuery;
            if(spec.Criteria is not null)
                query = query.Where(spec.Criteria);

            query = spec.Includes.Aggregate(query, (currentQuery, IncludeExpression) => currentQuery.Include(IncludeExpression));


            return query;
        }
    }
}
