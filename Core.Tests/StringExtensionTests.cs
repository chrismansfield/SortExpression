using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests
{
    [TestClass]
    public class StringExtensionTests
    {
        [TestMethod]
        public void Remove_PassedStringWithMatch_RemovesMatch()
        {
            var value = "This is a string with a VALUE to be removed";

            var result = value.Remove("VALUE");

            Assert.AreEqual("This is a string with a  to be removed", result);
        }

        [TestMethod]
        public void Remove_PassedStringWithMultipleMatches_RemovesAllMatches()
        {
            var value = "VALUE1 VALUE2 VALUE3";

            var result = value.Remove("VALUE");

            Assert.AreEqual("1 2 3", result);
        }

        [TestMethod]
        public void Remove_PassedStringWithouMatch_DoesntModifyString()
        {
            var value = "VALUE1 VALUE2 VALUE3";

            var result = value.Remove("DUMMY");

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Remove_PassedStringWithInvalidCaseMatchAndCaseSensitiveComparison_DoesntModifyString()
        {
            var value = "VALUE1 VALUE2 VALUE3";

            var result = value.Remove("value", StringComparison.Ordinal); //different case

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void Remove_PassedStringWithInvalidCaseMatchAndCaseInsensitiveComparison_DoesntModifyString()
        {
            var value = "VALUE1 VALUE2 VALUE3";

            var result = value.Remove("value", StringComparison.OrdinalIgnoreCase); //different case

            Assert.AreEqual("1 2 3", result);
        }

        [TestMethod]
        public void Remove_PassedStringWithMultipleDifferentMatches_RemovesAllMatches()
        {
            var value = "VALUE1 VALUE2 VALUE3";

            var result = value.Remove("VALUE1", "VALUE2", " ");

            Assert.AreEqual("VALUE3", result);
        }
        
        [TestMethod]
        public void Remove_PassedStringWithMultipleDifferentMatchesCaseInsensitive_RemovesAllMatches()
        {
            var value = "VALUE1 VALUE2 VALUE3";

            var result = value.Remove(StringComparison.OrdinalIgnoreCase, "Value1", "Value2", " ");

            Assert.AreEqual("VALUE3", result);
        }

    }
}
