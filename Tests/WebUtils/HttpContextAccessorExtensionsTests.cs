using com.etsoo.CoreFramework.Authentication;
using com.etsoo.WebUtils;
using System.Security.Claims;

namespace Tests.WebUtils
{
    [TestClass]
    public class HttpContextAccessorExtensionsTests
    {
        [TestMethod]
        public void GetEnumClaimTest()
        {
            // Arrange
            var role = (short)(UserRole.Finance | UserRole.Operator);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "Etsoo"),
                new(ClaimTypes.Role, role.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act
            var roleValue = principal.GetEnumClaim<UserRole>(ClaimTypes.Role);

            // Assert
            Assert.IsNotNull(roleValue);
            Assert.IsTrue(roleValue.GetValueOrDefault().HasFlag(UserRole.Operator));
        }
    }
}
