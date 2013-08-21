using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SortExpression.Reflection;

namespace SortExpression
{
    public class SortExpression
    {
        private readonly string _expression;
        private static readonly ConcurrentDictionary<string, ProviderEntry> CachedProviders;

        static SortExpression()
        {
            CachedProviders = new ConcurrentDictionary<string, ProviderEntry>();
        }

        private SortExpression(string sortExpression)
        {
            _expression = sortExpression;
            ParseExpression();
        }

        public static SortExpression FromString(string sortExpression)
        {
            return new SortExpression(sortExpression);
        }

        public SortDirections SortDirection { get; private set; }

        public bool IsSortExpressionDefinition { get; private set; }

        public string Command { get; set; }

        public string[] Parameters { get; set; }

        private void ParseExpression()
        {
            SortDirection = ResolveSortDirection();
            IsSortExpressionDefinition = VerifyIsSortExpressionDefinition();

            var expression = RemoveTrailingDirectionModifiers(_expression);
            if (IsSortExpressionDefinition)
            {
                var command = expression.Substring("sortexpr:".Length).Trim().Split(' ');
                Command = command[0];
                Parameters = command.Skip(1).ToArray();
            }
            else
                Command = expression;
        }

        private SortDirections ResolveSortDirection()
        {
            return _expression.EndsWith("desc", StringComparison.OrdinalIgnoreCase)
                       ? SortDirections.Descending
                       : SortDirections.Ascending;
        }

        private bool VerifyIsSortExpressionDefinition()
        {
            return _expression.StartsWith("sortexpr:", StringComparison.OrdinalIgnoreCase);
        }

        private string RemoveTrailingDirectionModifiers(string expression)
        {
            return expression.Remove(StringComparison.OrdinalIgnoreCase, " asc", " desc");
        }

        internal IQueryable<T> Sort<T>(IQueryable<T> source)
        {
            if (String.IsNullOrEmpty(Command)) return source;

            if (IsSortExpressionDefinition)
                return SortUsingProvider(source);

            return SortUsingExpressions(source);
        }

        private IOrderedQueryable<T> SortUsingProvider<T>(IQueryable<T> source)
        {
            var provider = GetSortExpressionProvider<T>();
            provider.Expression = Command;
            Array.ForEach(Parameters, p => provider.Parameters.Add(p));

            return provider.Sort(source, SortDirection);
        }

        private ISortExpressionProvider<T> GetSortExpressionProvider<T>()
        {
            var entry = CachedProviders.GetOrAdd(Command, cmd =>
                {
                    var baseType = (from t in AllTypes.Everywhere()
                                    where t.Implements(typeof(ISortExpressionProvider<>))
                                          &&
                                          t.Attribute((SortExpressionProviderAttribute attr) => attr.CommandName) == cmd
                                    select t).Single();

                    if (baseType.IsGenericTypeDefinition)
                        return new ProviderEntry(typeof(T), baseType.MakeGenericType(typeof(T)));
                    return new ProviderEntry(baseType);
                });


            var providerType = entry.GetOrAdd<T>(t => t.MakeGenericType(typeof(T)));

            return Activator.CreateInstance(providerType) as ISortExpressionProvider<T>;
        }

        private IOrderedQueryable<T> SortUsingExpressions<T>(IQueryable<T> source)
        {
            var properties = Command.Split('.');
            var parameter = Expression.Parameter(typeof(T));
            var accessor = GetAccessor(properties, parameter);
            var lambda = Expression.Lambda(accessor, parameter);
            var method = GetOrderByMethod(typeof(T), accessor.Type);

            return (IOrderedQueryable<T>)method.Invoke(null, new object[] { source, lambda });
        }

        private MemberExpression GetAccessor(IEnumerable<string> properties, ParameterExpression parameter)
        {
            return (MemberExpression)properties.Aggregate<string, Expression>(parameter, Expression.PropertyOrField);

        }

        private MethodInfo GetOrderByMethod(Type queryableType, Type memberType)
        {
            string methodInfoName = SortDirection == SortDirections.Ascending ? "OrderBy" : "OrderByDescending";
            return
                typeof(Queryable).GetMethods()
                                  .Single(m => m.Name == methodInfoName && m.GetParameters().Count() == 2)
                                  .MakeGenericMethod(queryableType, memberType);
        }

        public static implicit operator SortExpression(string expression)
        {
            return new SortExpression(expression);
        }

    }

    public enum SortDirections
    {
        Ascending,
        Descending
    }
}