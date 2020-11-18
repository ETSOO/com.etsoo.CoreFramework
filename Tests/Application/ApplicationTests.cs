using com.etsoo.CoreFramework.Application;
using com.etsoo.CoreFramework.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Tests.Application
{
    [TestClass]
    public class ApplicationTests
    {
        /// <summary>
        /// Build with null PrivateKey should throw exception test
        /// </summary>
        [TestMethod]
        public void Build_NullCases_Exception()
        {
            // Null configuration exception
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                CoreApplication.Builder.Build();
            });

            // Null db connection exception
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                CoreApplication.Builder.Configuration(Configuration.Builder.PrivateKey("test").Build()).Build();
            });
        }

        /// <summary>
        /// Build with default storage should be the local storage
        /// </summary>
        [TestMethod]
        public void Build_DefaultStorage_ShouldBeLocalStorage()
        {
            // Arrange
            var configuration = Configuration.Builder.PrivateKey("test").Build();
            Func<IDbConnection> dbConnection = () =>
            {
                return new SqlConnection();
            };

            // Act
            var app = CoreApplication.Builder.Configuration(configuration).UseDbConnection(dbConnection).Build();

            // Assert
            Assert.IsInstanceOfType(app.UseStorage(), typeof(LocalStorage));
        }
    }
}
