using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Reflection
{
	public static class AllTypes
	{
		private static IDictionary<Assembly, IEnumerable<Type>> _typeCache;
		private static IEnumerable<KeyValuePair<Assembly, IEnumerable<Type>>> TypeCache
		{
			get
			{
				if (_typeCache == null)
					LoadTypeCache();
				return _typeCache;
			}
		}

		private static void LoadTypeCache()
		{
			_typeCache = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                          select asm).ToDictionary(a => a, a => a.GetTypes().AsEnumerable());
		}

		/// <summary>
		/// Gets all public types in every assembly loaded in the current app domain
		/// </summary>
		public static IEnumerable<Type> Everywhere()
		{
			return TypeCache.SelectMany(tc => tc.Value);
		}
	}
}
