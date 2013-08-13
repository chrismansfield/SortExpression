using System.Collections.Generic;
using System.Linq;

namespace Core.Linq
{
    public abstract class SortExpressionProviderImplBase<T> : ISortExpressionProvider<T>
    {
        protected SortExpressionProviderImplBase()
        {
            Parameters = new List<string>();
        }

        public virtual string Expression { get; set; }
        public virtual ICollection<string> Parameters { get; private set; }
        public abstract IOrderedQueryable<T> Sort(IQueryable<T> source, SortDirections sortDirection);
    }
}