using System;

namespace Core
{
	public static class ActionExtensions
	{
		 public static Func<int> AsFunc(this Action action)
		 {
			 return () =>
				 {
					 action();
					 return 0;
				 };
		 }
	}
}