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

        [Test]
        public void DictionaryString_GetIntArray_Test()
        {
            // Arrange
            var dic = new StringKeyDictionaryString(new Dictionary<string, string?>()
            {
                { "array1", "1, a,3,4" },
                { "array2", "[1, 3, 4]" }
            });

            var array1 = dic.GetArray<int>("array1");
            var array2 = dic.GetArray<int>("array2");

            Assert.Multiple(() =>
            {
                Assert.That(array1.Count, Is.EqualTo(3));
                Assert.That(array2.Count, Is.EqualTo(3));
            });
        }

        [Test]
        public void DictionaryString_GetStringArray_Test()
        {
            // Arrange
            var dic = new StringKeyDictionaryString(new Dictionary<string, string?>()
            {
                { "array1", "1, a, 3,4" },
                { "array2", "[\"1\",\"3\",\"4\"]" }
            });

            var array1 = dic.GetArray("array1");
            var array2 = dic.GetArray("array2");

            Assert.Multiple(() =>
            {
                Assert.That(array1, Is.Not.Null);
                Assert.That(array2, Is.Not.Null);
                Assert.That(array1!.Count, Is.EqualTo(4));
                Assert.That(array2!.Count, Is.EqualTo(3));
            });
        }

        [Test]
        public void DictionaryObject_GetIntArray_Test()
        {
            // Arrange
            var dic = new StringKeyDictionaryObject(new Dictionary<string, object?>()
            {
                { "array1", "1, a,3,4" },
                { "array2", "[1, 3, 4]" }
            });

            var array1 = dic.GetArray<int>("array1");
            var array2 = dic.GetArray<int>("array2");

            Assert.Multiple(() =>
            {
                Assert.That(array1.Count, Is.EqualTo(3));
                Assert.That(array2.Count, Is.EqualTo(3));
            });
        }

        [Test]
        public void DictionaryObject_GetStringArray_Test()
        {
            // Arrange
            var dic = new StringKeyDictionaryObject(new Dictionary<string, object?>()
            {
                { "array1", "1, a, 3,4" },
                { "array2", "[\"1\",\"3\",\"4\"]" }
            });

            var array1 = dic.GetArray("array1");
            var array2 = dic.GetArray("array2");

            Assert.Multiple(() =>
            {
                Assert.That(array1, Is.Not.Null);
                Assert.That(array2, Is.Not.Null);
                Assert.That(array1!.Count, Is.EqualTo(4));
                Assert.That(array2!.Count, Is.EqualTo(3));
            });
        }
    }
}
