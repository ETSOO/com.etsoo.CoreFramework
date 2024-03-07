using com.etsoo.Utils.String;
using NUnit.Framework;

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

            Assert.Multiple(() =>
            {
                // Act & assert
                Assert.That(dic.GetItem("null"), Is.Null);
                Assert.That(dic.GetItem("ok"), Is.Not.Null);
            });

            var key = dic.GetItem("key");
            Assert.That(key, Is.Null);
        }

        /// <summary>
        /// Integer dictionary GetItem test
        /// </summary>
        [Test]
        public void GetItem_Int_Test()
        {
            // Arrange
            var dic = new StringKeyDictionary<int?>(new Dictionary<string, int?>() { { "null", null }, { "ok", 123 } });

            Assert.Multiple(() =>
            {
                // Act & assert
                Assert.That(dic.GetItem("null"), Is.Null);
                Assert.That(dic.GetItem("ok"), Is.Not.Null);
            });

            var key = dic.GetItem("key");
            Assert.That(key, Is.Null);
        }

        // Arrange
        private readonly StringKeyDictionaryObject dic = new(new Dictionary<string, object?>()
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
            Assert.Multiple(() =>
            {
                Assert.That(dic.Get("null"), Is.Null);
                Assert.That(dic.Get<bool>("null"), Is.Null);
            });
        }

        /// <summary>
        /// StringKeyDictionaryDynamic bool test
        /// </summary>
        [Test]
        public void DictionaryDynamic_GetBool_Test()
        {
            Assert.That(dic.Get<bool>("bool"), Is.EqualTo(true));
        }

        /// <summary>
        /// StringKeyDictionaryDynamic decimal test
        /// </summary>
        [Test]
        public void DictionaryDynamic_GetDecimal_Test()
        {
            Assert.That(dic.Get<decimal>("money"), Is.EqualTo(12.8M));
        }

        /// <summary>
        /// StringKeyDictionaryDynamic decimal from string test
        /// </summary>
        [Test]
        public void DictionaryDynamic_GetDecimalFromString_Test()
        {
            Assert.That(dic.Get<decimal>("string"), Is.EqualTo(12.8M));
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

            Assert.Multiple(() =>
            {
                Assert.That(dic.GetItem("null"), Is.Null);
                Assert.That(dic.Get<bool>("null"), Is.Null);
                Assert.That(dic.Get<decimal>("string"), Is.EqualTo(12.8M));
            });
        }
    }
}
