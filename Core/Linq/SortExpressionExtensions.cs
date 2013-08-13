using System.Linq;

namespace Core.Linq
{
    public static class SortExpressionExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, SortExpression sortExpression)
        {
            return sortExpression.Sort(source);
        }
    }
}