using System;
using System.Linq;
using System.Reflection;

namespace Core.Reflection
{
	public static class ReflectionExtensions
	{
		/// <summary>
		/// Checks if a type implements an interface.
		/// </summary>
		/// <typeparam name="TInterface">Type of the interface that should be implemented. If this type is not an interface, the method will always return false</typeparam>
		/// <param name="type">The type to check. This type can be both a class, and an interface</param>
		/// <returns></returns>
		public static bool Implements<TInterface>(this Type type)
		{
			return type.Implements(typeof(TInterface));
		}

		/// <summary>
		/// Checks if a type implements an interface.
		/// </summary>
		/// <param name="type">The type to check. This type can be both a class, and an interface</param>
		/// <param name="interface">Type of the interface that should be implemented.</param>
		/// <returns></returns>
		public static bool Implements(this Type type, Type @interface)
		{
			if (@interface.IsGenericTypeDefinition)
			{
				return type.GetInterfaces().Select(t => t.IsGenericType ? t.GetGenericTypeDefinition() : t).Contains(@interface);
			}
			return type.GetInterfaces().Contains(@interface);
		}

		/// <summary>
		/// Checks if a certain type inherits another type
		/// </summary>
		/// <typeparam name="TBase">The type that should be implemented. If this type is an interface, the method will always return false</typeparam>
		/// <param name="type">The type to check. If this type is an interface, the method will always return false</param>
		/// <returns></returns>
		public static bool Inherits<TBase>(this Type type)
		{
			return type.Inherits(typeof(TBase));
		}

		/// <summary>
		/// Checks if a certain type inherits another type
		/// </summary>
		/// <param name="type">The type to check. If this type is an interface, the method will always return false</param>
		/// <param name="base">The type that should be implemented. If this type is an interface, the method will always return false</param>
		/// <returns></returns>
		public static bool Inherits(this Type type, Type @base)
		{
			if (type.BaseType != null)
			{
				return type.BaseType == @base || type.BaseType.Inherits(@base);
			}
			return false;
		}

		/// <summary>
		/// Check if a type either inherits the class, or implements the interface provided
		/// </summary>
		public static bool InheritsOrImplements<TBase>(this Type type)
		{
			return type.InheritsOrImplements(typeof(TBase));
		}

		/// <summary>
		/// Check if a type either inherits the class, or implements the interface provided
		/// </summary>
		public static bool InheritsOrImplements(this Type type, Type @base)
		{
			return type.Inherits(@base) || type.Implements(@base);
		}

		/// <summary>
		/// Performs an invocation of a method on the specified object, passing the specified arguments in the order they're supplied
		/// </summary>
		public static object Invoke(this MethodInfo method, object @object, params object[] args)
		{
			return method.Invoke(@object, args);
		}

		/// <summary>
		/// Performs an invocation of a method on the specified object and casts the result to the specified type,
		///  passing the specified arguments in the order they're supplied
		/// </summary>
		public static T Invoke<T>(this MethodInfo method, object @object, params object[] args)
		{
			if (method.ReturnType == typeof (void))
				throw new InvalidOperationException(
					"Cannot perform a typed invoke on a void method. Please use the non-generic version of Invoke for this purpose");

			return (T) method.Invoke(@object, args);
		}

		/// <summary>
		/// Performs an invocation of a static method, passing the specified arguments in the order they're supplied
		/// </summary>
		public static object InvokeStatic(this MethodInfo method, params object[] args)
		{
			AssertMethodIsStatic(method);
			return method.Invoke(null, args);
		}

		/// <summary>
		/// Performs an invocation of a static method and casts the result to the specified type, passing the specified arguments in the order they're supplied
		/// </summary>
		public static T InvokeStatic<T>(this MethodInfo method, params object[] args)
		{
			AssertMethodIsStatic(method);
			return method.Invoke<T>(null, args);
		}

		private static void AssertMethodIsStatic(MethodInfo method)
		{
			if (!method.IsStatic)
				throw new InvalidOperationException("The method is not static. Please use the Invoke method instead");
		}

		/// <summary>
		/// Gets the specified attribute from the MemberInfo
		/// </summary>
		public static T Attribute<T>(this MemberInfo memberInfo) where T : Attribute
		{
			object[] attribute = memberInfo.GetCustomAttributes(typeof(T), false);

			if (attribute.OfType<T>().Any())
			{
				return (T)attribute.First();
			}

			return null;
		}

		/// <summary>
		/// Selects a value from the single attribute of the specified type, using the specified selector
		/// </summary>
		/// <remarks>
		/// To avoid having to specify both generic parameters, the best way of calling this method is by explicitly
		/// define the type of the lambda parameter as in the example below
		/// </remarks>
		/// <code>
		///	MethodInfo methodInfo;
		///	methodInfo.Attribute((MyAttribute attr) => attr.MyNamedProperty);
		/// </code>
		public static TValue Attribute<TAttribute, TValue>(this MemberInfo memberInfo, Func<TAttribute, TValue> selector)
			where TAttribute : Attribute
		{
			var attribute = memberInfo.Attribute<TAttribute>();

			if (attribute != null)
			{
				return selector(attribute);
			}
			return default(TValue);
		}
	}
}
