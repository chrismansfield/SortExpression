using System;

namespace SortExpression
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class SortExpressionProviderAttribute : Attribute
    {
        internal readonly string CommandName;

        public SortExpressionProviderAttribute(string commandName)
        {
            CommandName = commandName;
        }
    }
}