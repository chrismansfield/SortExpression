using System;
using System.Linq;
using System.Text;

namespace Core
{
	public static class StringExtensions
	{
		public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
		{
			var sb = new StringBuilder();

			int previousIndex = 0,
				index = str.IndexOf(oldValue, comparison);
			while (index != -1)
			{
				sb.Append(str.Substring(previousIndex, index - previousIndex));
				sb.Append(newValue);
				index += oldValue.Length;

				previousIndex = index;
				index = str.IndexOf(oldValue, index, comparison);
			}
			sb.Append(str.Substring(previousIndex));

			return sb.ToString();
		}

        public static string Remove(this string str, string valueToRemove)
        {
            return str.Replace(valueToRemove, String.Empty);
        }

        public static string Remove(this string str, string valueToRemove, StringComparison comparison)
        {
            return str.Replace(valueToRemove, "", comparison);
        }

        public static string Remove(this string str, params string[] valuesToRemove)
        {
            return valuesToRemove.Aggregate(str, Remove);
        }

        public static string Remove(this string str, StringComparison comparison, params string[] valuesToRemove)
        {
            return valuesToRemove.Aggregate(str, (current, next) => current.Remove(next, comparison));
        }
	}
}
