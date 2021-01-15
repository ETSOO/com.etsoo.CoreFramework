using com.etsoo.Utils.String;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.Utils
{
    [TestFixture]
    public class StringKeyDictionaryTests
    {
        /// <summary>
        /// String dictionary GetItem test
        /// </summary>
        [Test]
        public void GetItem_String_Test()
        {
            // Arrange
            var dic = new StringKeyDictionary<string>(new Dictionary<string, string?>() { { "null", null }, { "ok", "ok" } });

            // Act & assert
            Assert.IsNull(dic.GetItem("null"));
            Assert.IsNotNull(dic.GetItem("ok"));

            var key = dic.GetItem("key");
            Assert.IsNull(key);
        }

        /// <summary>
        /// Integer dictionary GetItem test
        /// </summary>
        [Test]
        public void GetItem_Int_Test()
        {
            // Arrange
            var dic = new StringKeyDictionary<int?>(new Dictionary<string, int?>() { { "null", null }, { "ok", 123 } });

            // Act & assert
            Assert.IsNull(dic.GetItem("null"));
            Assert.IsNotNull(dic.GetItem("ok"));

            var key = dic.GetItem("key");
            Assert.IsNull(key);
        }

        // Arrange
        private readonly StringKeyDictionaryObject dic = new (new Dictionary<string, object?>()
            {
                { "null", null },
                { "bool", true },
                { "money", 12.8M },
                { "string", "12.8" }
            });

        /// <summary>
        /// StringKeyDictionaryDynamic null test
        /// </summary>
        [Test]
        public void DictionaryDynamic_GetNull_Test()
        {
            Assert.IsNull(dic.Get("null"));
            Assert.IsNull(dic.Get<bool>("null"));
        }

        /// <summary>
        /// StringKeyDictionaryDynamic bool test
        /// </summary>
        [Test]
        public void DictionaryDynamic_GetBool_Test()
        {
            Assert.IsTrue(dic.Get<bool>("bool") == true);
        }

        /// <summary>
        /// StringKeyDictionaryDynamic decimal test
        /// </summary>
        [Test]
        public void DictionaryDynamic_GetDecimal_Test()
        {
            Assert.IsTrue(dic.Get<decimal>("money") == 12.8M);
        }

        /// <summary>
        /// StringKeyDictionaryDynamic decimal from string test
        /// </summary>
        [Test]
        public void DictionaryDynamic_GetDecimalFromString_Test()
        {
            Assert.IsTrue(dic.Get<decimal>("string") == 12.8M);
        }

        /// <summary>
        /// StringKeyDictionaryString test
        /// </summary>
        [Test]
        public void DictionaryString_Get_Test()
        {
            // Arrange
            var dic = new StringKeyDictionaryString(new Dictionary<string, string?>()
            {
                { "null", null },
                { "string", "12.8" }
            });

            Assert.IsNull(dic.GetItem("null"));
            Assert.IsNull(dic.Get<bool>("null"));
            Assert.IsTrue(dic.Get<decimal>("string") == 12.8M);
        }
    }
}
