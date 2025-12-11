using com.etsoo.Utils.String;

namespace Tests.Utils
{
    [TestClass]
    public class StringKeyDictionaryTests
    {
        /// <summary>
        /// String dictionary GetItem test
        /// </summary>
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
        public void DictionaryDynamic_GetNull_Test()
        {
            Assert.IsNull(dic.Get("null"));
            Assert.IsNull(dic.Get<bool>("null"));
        }

        /// <summary>
        /// StringKeyDictionaryDynamic bool test
        /// </summary>
        [TestMethod]
        public void DictionaryDynamic_GetBool_Test()
        {
            Assert.IsTrue(dic.Get<bool>("bool"));
        }

        /// <summary>
        /// StringKeyDictionaryDynamic decimal test
        /// </summary>
        [TestMethod]
        public void DictionaryDynamic_GetDecimal_Test()
        {
            Assert.AreEqual(12.8M, dic.Get<decimal>("money"));
        }

        /// <summary>
        /// StringKeyDictionaryDynamic decimal from string test
        /// </summary>
        [TestMethod]
        public void DictionaryDynamic_GetDecimalFromString_Test()
        {
            Assert.AreEqual(12.8M, dic.Get<decimal>("string"));
        }

        /// <summary>
        /// StringKeyDictionaryString test
        /// </summary>
        [TestMethod]
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
            Assert.AreEqual(12.8M, dic.Get<decimal>("string"));
        }

        [TestMethod]
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

            Assert.AreEqual(3, array1.Count());
            Assert.AreEqual(3, array2.Count());
        }

        [TestMethod]
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

            Assert.IsNotNull(array1);
            Assert.IsNotNull(array2);
            Assert.AreEqual(4, array1.Count());
            Assert.AreEqual(3, array2.Count());
        }

        [TestMethod]
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

            Assert.AreEqual(3, array1.Count());
            Assert.AreEqual(3, array2.Count());
        }

        [TestMethod]
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

            Assert.IsNotNull(array1);
            Assert.IsNotNull(array2);
            Assert.AreEqual(4, array1.Count());
            Assert.AreEqual(3, array2.Count());
        }
    }
}
