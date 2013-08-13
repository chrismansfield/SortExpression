using System;
using System.Collections.Generic;
using System.Linq;
using Core.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Reflection
{
	[TestClass]
	public class ReflectionExtensionsTests
	{
		[TestMethod]
		public void Implements_PassedClassThatImplementsInterface_ReturnsTrue()
		{
			var result = typeof(DisposableImpl).Implements<IDisposable>();

			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Implements_PassedClassThatDoesNotImplementInterface_ReturnsFalse()
		{
			var result = typeof(InvalidImpl).Implements<IDisposable>();

			Assert.IsFalse(result);
		}

		[TestMethod]
		public void Implements_PassedInterfaceThatExtendsInterface_ReturnsTrue()
		{
			var result = typeof(IDummy).Implements<IDisposable>();

			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Inherits_PassedClassThatInheritsCorrectClass_ReturnsTrue()
		{
			var result = typeof(DisposableImplSubClass).Inherits<DisposableImpl>();

			Assert.IsTrue(result);
		}

		[TestMethod]
		public void Inherits_PassedClassThatDoesNotInheritsCorrectClass_ReturnsFalse()
		{
			//This is false
			var result = typeof(DisposableImplSubClass).Inherits<Exception>();

			Assert.IsFalse(result);
		}

		[TestMethod]
		public void Implements_PassedCorrectGenericDefinition_ReturnsTrue()
		{
			var result = typeof(List<string>).Implements(typeof(IEnumerable<>));

			Assert.IsTrue(result);
		}

		/// <summary>
		/// Make sure that even if the supplied class does IMPLEMENT the provided interface the method returns false,
		/// since the interface is not INHERITED, but IMPLEMENTED or EXTENDED
		/// </summary>
		[TestMethod]
		public void Inherits_PassedClassThatInheritsCorrectInterface_ReturnsFalse()
		{
			var result = typeof(DisposableImplSubClass).Inherits<IDisposable>();

			Assert.IsFalse(result);
		}

		/// <summary>
		/// Make sure that even if the supplied type is several levels down from the base, the method returns true
		/// </summary>
		[TestMethod]
		public void Inherits_PassedClassThatInheritsCorrectClassThroughOtherClasses_ReturnsTrue()
		{
			var result = typeof(DisposableImplSeveralSubLevelsAway).Inherits<DisposableImpl>();

			Assert.IsTrue(result);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void InvokeGeneric_PassedVoidMethod_ThrowsInvalidOperationException()
		{
			GetType().GetMethod("DummyVoidMethod").Invoke<object>(this);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void InvokeStatic_PassedNonStaticMethod_ThrowsInvalidOperationException()
		{
			GetType().GetMethod("DummyVoidMethod").InvokeStatic();
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void InvokeStaticGeneric_PassedNonStaticMethod_ThrowsInvalidOperationException()
		{
			GetType().GetMethod("DummyVoidMethod").InvokeStatic<object>();
		}

		[TestMethod]
		public void Attribute_PassedMemberWithSpecifiedAttribute_ReturnsAttribute()
		{
			var result = typeof(AttributeTestHelperObject).Attribute<DummyAttribute>();

			Assert.AreEqual(expected: "DummyClass", actual: result.DummyString);
		}

		[TestMethod]
		public void Attribute_PassedMemberWithSpecifiedAttribute_ReturnsAttribute2()
		{
			var result = typeof(AttributeTestHelperObject).GetMember("DummyProperty").Single().Attribute<DummyAttribute>();

			Assert.AreEqual(expected: "DummyMember", actual: result.DummyString);
		}

		[TestMethod]
		public void Attribute_PassedMemberWithoutSepcifiedAttribute_ReturnsNull()
		{
			//DisposableImlp has no attributes
			var result = typeof(DisposableImpl).Attribute<DummyAttribute>();

			Assert.IsNull(result);
		}

		[TestMethod]
		public void Attribute_PassedMemberWithSpecifiedAttributeAndSelector_ReturnsValueFromAttribute()
		{
			var result = typeof(AttributeTestHelperObject).Attribute((DummyAttribute da) => da.DummyString);

			Assert.AreEqual(expected: "DummyClass", actual: result);
		}

		[TestMethod]
		public void Attribute_PassedMemberWithSpecifiedAttributeAndSelector_ReturnsValueFromAttribute2()
		{
			var result = typeof(AttributeTestHelperObject).GetMember("DummyProperty").Single().Attribute((DummyAttribute da) => da.DummyString);

			Assert.AreEqual(expected: "DummyMember", actual: result);
		}

		[TestMethod]
		public void Attribute_PassedMemberWithoutSepcifiedAttributeAndSelector_ReturnsDefaultValue()
		{
			var result = typeof(DisposableImpl).Attribute((DummyAttribute da) => da.DummyString);

			Assert.AreEqual(expected: null, actual: result);
		}

		public void DummyVoidMethod()
		{}

		interface IDummy : IDisposable { }

		class DisposableImpl : IDisposable
		{
			public void Dispose()
			{ }
		}

		class InvalidImpl { }

		class DisposableImplSubClass : DisposableImpl
		{ }

		class DisposableImplSeveralSubLevelsAway : DisposableImplSubClass
		{ }

		public class DummyMethodContainer
		{
			private int _invocationCount;

			public string DummyMethod(int? arg0 = null)
			{
				_invocationCount++;
				return "Dummy";
			}

			internal bool Verify(Func<int, bool> allowedInvocations)
			{
				return allowedInvocations(_invocationCount);
			}
		}

		[Dummy("DummyClass")]
		public class AttributeTestHelperObject
		{
			[Dummy("DummyMember")]
			public string DummyProperty { get; set; }
		}

		[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
		private sealed class DummyAttribute : Attribute
		{
			internal readonly string DummyString;

			public DummyAttribute(string dummyString)
			{
				DummyString = dummyString;
			}
		}
	}
}
