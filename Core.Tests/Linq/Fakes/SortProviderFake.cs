using System;
using System.Linq;
using Core.Linq;
using Core.Tests.Linq.Dummys;

namespace Core.Tests.Linq.Fakes
{
	[SortExpressionProvider("SortDummy")]
	public class SortProviderFake<T> : SortExpressionProviderImplBase<T>
		where T : SortDummy
	{
		public static event Action<SortProviderFake<T>> Constructed;

		private static void OnConstructed(SortProviderFake<T> obj)
		{
			var handler = Constructed;
			if (handler != null) handler(obj);
		}

		public SortProviderFake()
		{
			OnConstructed(this);
		}

		public override IOrderedQueryable<T> Sort(IQueryable<T> source, SortDirections sortDirection)
		{
			return source.OrderBy(s => s.Integer);
		}
	}

	[SortExpressionProvider("NonGenericSortDummy")]
	public class NonGenericSortProviderFake : SortProviderFake<SortDummy>
	{

	}


}