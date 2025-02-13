using ElMagzer.Core.Models;
using System.Linq.Expressions;

namespace ElMagzer.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseModel
    {
        private ISpecifications<T> _specificationsImplementation;
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPagingationEnabled { get; set; }

        public BaseSpecifications()
        {
        }

        public BaseSpecifications(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;
        }

        public void ApplyPagination(int skip, int take)
        {
            IsPagingationEnabled = true;
            Skip = skip;
            Take = take;
        }
    }
}
