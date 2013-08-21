using System.Linq;

namespace SortExpression
{
    public static class SortExpressionExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, SortExpression sortExpression)
        {
            return sortExpression.Sort(source);
        }
    }
}