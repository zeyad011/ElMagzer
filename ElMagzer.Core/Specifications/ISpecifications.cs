using ElMagzer.Core.Models;
using System.Linq.Expressions;

namespace ElMagzer.Core.Specifications
{
    public interface ISpecifications<T> where T : BaseModel
    {
        public Expression<Func<T, bool>> Criteria { get; set; } 
        public List<Expression<Func<T, Object>>> Includes { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; } 
        public bool IsPagingationEnabled { get; set; }
    }
}
