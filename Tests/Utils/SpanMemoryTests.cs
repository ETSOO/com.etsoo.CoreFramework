using com.etsoo.Utils.SpanMemory;
using NUnit.Framework;
using System.Text;

namespace Tests.Utils
{
    [TestFixture]
    internal class SpanMemoryTests
    {
        [Test]
        public void ToBase64BytesTests()
        {
            // Arrange
            var input = "In this scenario, the external client will give you the structure of JWT, normally with custom claims that they expect and provide you with an RSA private key to sign the token. The token will then be used to construct a Uri that will be sent to users and allowing them to invoke the external client endpoints.";
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));

            // Act
            var bytes1 = Convert.FromBase64String(base64);
            var bytes2 = base64.AsSpan().ToBase64Bytes().ToArray();

            // Assert
            Assert.That(bytes2, Is.EqualTo(bytes1));

            // Invalid Base64 string
            var bytes3 = "Hello, world!".AsSpan().ToBase64Bytes().ToArray();
            Assert.That(bytes3, Is.Empty);
        }
    }
}
