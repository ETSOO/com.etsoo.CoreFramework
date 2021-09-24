﻿using com.etsoo.CoreFramework.Authentication;
using com.etsoo.CoreFramework.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Tests.CoreFramework
{
    [TestFixture]
    public class JwtServiceTests
    {
        readonly JwtService service;

        public JwtServiceTests()
        {
            // Arrange
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(@"{
                ""Jwt"": {
                    ""Issuer"": ""etsoo"",
                    ""Audience"": ""all""
                }
            }"));
            var section = new ConfigurationBuilder().AddJsonStream(stream).Build().GetSection("Jwt");

            service = new JwtService(new ServiceCollection(), false, section, Encoding.UTF8.GetBytes("SecurityKeyShouldBeLongerThan128"));
        }

        [Test]
        public void CreateAccessToken_Tests()
        {
            // Arrange
            var user = new CurrentUser("1", null, "Etsoo", 1, IPAddress.Parse("127.0.0.1"), CultureInfo.CurrentCulture, "CN", null);

            // Act
            var token = service.CreateAccessToken(user);

            // Assert
            Assert.AreEqual(3, token.Split('.').Length);
        }

        [Test]
        public void ValidateToken_Tests()
        {
            // Arrange
            var token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkV0c29vIiwibmFtZWlkIjoiMSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2xvY2FsaXR5IjoiemgtQ04iLCJyb2xlIjoiQWRtaW4iLCJpcGFkZHJlc3MiOiIxMjcuMC4wLjEiLCJuYmYiOjE2MjI4NzIzOTIsImV4cCI6MTYyMjg3MjM5MywiaWF0IjoxNjIyODcyMzkyLCJpc3MiOiJldHNvbyIsImF1ZCI6IlJlZnJlc2hUb2tlbiJ9.TYIba_fU9wamEHCv7UJu9V8hEN1zoMkq8UW70oy7hNNSbjrUkb-kMwn3JFh7akM7ceOBnaGoDRp4jqluvrVzNQ";

            // Act
            var result = service.ValidateRefreshToken(token, out var expired);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(true, expired);
        }
    }
}
