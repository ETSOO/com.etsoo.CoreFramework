using com.etsoo.CoreFramework.Authentication;
using com.etsoo.WebUtils;
using NUnit.Framework;
using System.Security.Claims;

namespace Tests.WebUtils
{
    [TestFixture]
    public class HttpContextAccessorExtensionsTests
    {
        [Test]
        public void GetEnumClaimTest()
        {
            // Arrange
            var role = (short)(UserRole.Finance | UserRole.Operator);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Etsoo"),
                new Claim(ClaimTypes.Role, role.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act
            var roleValue = principal.GetEnumClaim<UserRole>(ClaimTypes.Role);

            // Assert
            Assert.NotNull(roleValue);
            Assert.IsTrue(roleValue.GetValueOrDefault().HasFlag(UserRole.Operator));
        }
    }
}
