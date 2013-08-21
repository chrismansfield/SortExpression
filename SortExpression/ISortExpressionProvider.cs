using System.Collections.Generic;
using System.Linq;

namespace SortExpression
{
    public interface ISortExpressionProvider<T>
    {
        string Expression { get; set; }

        ICollection<string> Parameters { get; }

        IOrderedQueryable<T> Sort(IQueryable<T> source, SortDirections sortDirection);
    }
}