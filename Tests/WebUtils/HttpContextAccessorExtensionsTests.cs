﻿using com.etsoo.CoreFramework.Authentication;
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
                new(ClaimTypes.Name, "Etsoo"),
                new(ClaimTypes.Role, role.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act
            var roleValue = principal.GetEnumClaim<UserRole>(ClaimTypes.Role);

            // Assert
            Assert.That(roleValue, Is.Not.Null);
            Assert.That(roleValue.GetValueOrDefault().HasFlag(UserRole.Operator), Is.True);
        }
    }
}
