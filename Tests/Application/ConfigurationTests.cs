using com.etsoo.CoreFramework.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Application
{
    /// <summary>
    /// Application configuration tests
    /// </summary>
    [TestClass]
    public class ConfigurationTests
    {
        /// <summary>
        /// Build with null PrivateKey should throw exception test
        /// </summary>
        [TestMethod]
        public void Build_NullPrivateKey_Exception()
        {
            // Arrange, Act and assert
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                Configuration.Builder.Build();
            });
        }

        /// <summary>
        /// Build with default values test
        /// </summary>
        [TestMethod]
        public void Build_DefaultValues_Validation()
        {
            // Arrange
            var configuration = Configuration.Builder.PrivateKey("test").Build();

            // Assert default Languages, should be an empty array
            Assert.IsTrue(configuration.Languages.Length == 0);

            // Assert default ModelValidated, should be false
            Assert.IsFalse(configuration.ModelValidated);
        }
    }
}
