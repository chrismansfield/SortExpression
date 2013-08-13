using System.Collections.Generic;
using System.Linq;
using Core.Linq;
using Core.Tests.Linq.Dummys;
using Core.Tests.Linq.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Linq
{
    [TestClass]
    public class SortExpressionTests
    {
        private List<SortDummy> _dummies;

        [TestInitialize]
        public void TestInitialize()
        {
            _dummies = new List<SortDummy>
				{
					new SortDummy {Integer = 2, String = "B"},
					new SortDummy {Integer = 4, String = "D"},
					new SortDummy {Integer = 1, String = "A"},
					new SortDummy {Integer = 3, String = "C"},
				};
        }

        [TestMethod]
        public void FromString_PassedSimpleExpressionString_ParsesAsAscending()
        {
            var result = SortExpression.FromString("Dummy.Dummy");

            Assert.AreEqual(SortDirections.Ascending, result.SortDirection);
        }

        [TestMethod]
        public void FromString_PassedSimpleExpressionString_ParsesCommand()
        {
            var result = SortExpression.FromString("Dummy.Dummy");

            Assert.AreEqual("Dummy.Dummy", result.Command);
        }

        [TestMethod]
        public void FromString_PassedSimpleExpressionString_ParsesAsNotSortExpressionDefinition()
        {
            var result = SortExpression.FromString("Dummy.Dummy");

            Assert.IsFalse(result.IsSortExpressionDefinition);
        }

        [TestMethod]
        public void FromString_PassedDescString_ParsesAsDescending()
        {
            var result = SortExpression.FromString("Dummy.Dummy DESC");

            Assert.AreEqual(SortDirections.Descending, result.SortDirection);
        }

        [TestMethod]
        public void FromString_PassedLowerCaseDescString_ParsesAsDescending()
        {
            var result = SortExpression.FromString("Dummy.Dummy desc"); //Lower case

            Assert.AreEqual(SortDirections.Descending, result.SortDirection);
        }

        [TestMethod]
        public void FromString_PassedSortExpression_ParsesAsSortExpressionDefinition()
        {
            var result = SortExpression.FromString("sortexpr: Dummy");

            Assert.IsTrue(result.IsSortExpressionDefinition);
        }

        [TestMethod]
        public void FromString_PassedSortExpression_ParsesCommand()
        {
            var result = SortExpression.FromString("sortexpr: Dummy");

            Assert.AreEqual("Dummy", result.Command);
        }

        [TestMethod]
        public void FromString_PassedSortExpressionWithParameters_ParsesParameters()
        {
            var result = SortExpression.FromString("sortexpr: Dummy DummyParam1 DummyParam2");

            Assert.IsTrue(result.Parameters.All(p => p == "DummyParam1" || p == "DummyParam2"));
        }

        [TestMethod]
        public void FromString_PassedSortExpressionWithParametersAndDesc_ParsesAsDescending()
        {
            var result = SortExpression.FromString("sortexpr: Dummy DummyParam1 DummyParam2 DESC");

            Assert.AreEqual(SortDirections.Descending, result.SortDirection);
        }

        [TestMethod]
        public void FromString_PassedSortExpressionWithParametersAndAsc_ParsesAsAscending()
        {
            var result = SortExpression.FromString("sortexpr: Dummy DummyParam1 DummyParam2 ASC");

            Assert.AreEqual(SortDirections.Ascending, result.SortDirection);
        }

        [TestMethod]
        public void FromString_PassedSortExpressionWithParametersAndDesc_DoesntParseDescAsParameter()
        {
            var result = SortExpression.FromString("sortexpr: Dummy DummyParam1 DummyParam2 DESC");

            Assert.IsTrue(result.Parameters.All(p => p == "DummyParam1" || p == "DummyParam2"));
        }

        [TestMethod]
        public void FromString_PassedSortExpressionWithParametersAndAsc_DoesntParseAscAsParameter()
        {
            var result = SortExpression.FromString("sortexpr: Dummy DummyParam1 DummyParam2 ASC");

            Assert.IsTrue(result.Parameters.All(p => p == "DummyParam1" || p == "DummyParam2"));
        }

        [TestMethod]
        public void Sort_UsingSimpleExpression_SortsCorrectly()
        {
            var sortExpression = SortExpression.FromString("Integer");
            var result = _dummies.AsQueryable().OrderBy(sortExpression).ToList();

            AssertOrder(result, 1, 2, 3, 4);
        }

        [TestMethod]
        public void Sort_UsingSimpleExpression_SortsCorrectly2()
        {
            var sortExpression = SortExpression.FromString("String");
            var result = _dummies.AsQueryable().OrderBy(sortExpression).ToList();

            AssertOrder(result, 1, 2, 3, 4);
        }

        [TestMethod]
        public void Sort_UsingSimpleExpressionWithDesc_SortsCorrectly()
        {
            var sortExpression = SortExpression.FromString("Integer DESC");
            var result = _dummies.AsQueryable().OrderBy(sortExpression).ToList();

            AssertOrder(result, 4, 3, 2, 1);
        }

        [TestMethod]
        public void Sort_UsingSortExpression_ConstructsProviderWithProperGenerics()
        {
            bool invoked = false;
            SortProviderFake<SortDummy>.Constructed += obj => { invoked = true; };

            _dummies.AsQueryable().OrderBy(SortExpression.FromString("sortexpr: SortDummy"));

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void Sort_UsingSortExpression_ConstructsProviderWithProperGenerics2()
        {
            bool invoked = false;
            //Using the non-generic version
            NonGenericSortProviderFake.Constructed += obj => { invoked = true; };

            _dummies.AsQueryable().OrderBy(SortExpression.FromString("sortexpr: NonGenericSortDummy"));

            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void Sort_UsingSortExpression_SortsCorrectly()
        {
            var result = _dummies.AsQueryable().OrderBy(SortExpression.FromString("sortexpr: SortDummy")).ToList();

            //The fake SortProvider we use order ascending
            AssertOrder(result, 1, 2, 3, 4);
        }

        public void AssertOrder(List<SortDummy> collection, params int[] expectedOrder)
        {
            for (int i = 0; i < collection.Count(); i++)
            {
                var sortDummy = collection[i];
                var expected = expectedOrder[i];
                if (sortDummy.Integer != expected)
                    Assert.Fail("Expected {0} at element {1}. Found {2}", expected, i, sortDummy.Integer);
            }
        }
    }
}
