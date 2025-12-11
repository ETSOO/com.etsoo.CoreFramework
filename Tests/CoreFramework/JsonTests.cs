using com.etsoo.CoreFramework.Json;
using com.etsoo.CoreFramework.Models;
using System.Text.Json;

namespace Tests.CoreFramework
{
    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public async Task CustomFieldSchemaSuccessTest()
        {
            // Arrange
            var schema = CustomFieldSchema.Create();

            var jsonText = @"[{
                ""type"": ""checkbox"",
                ""name"": ""name"",
                ""space"": ""five"",
                ""options"": [
                    {
                        ""id"": 1,
                        ""title"": ""Name 1""
                    },
                    {
                        ""id"": ""b"",
                        ""title"": ""Name B""
                    }
                ]
            }]";

            var json = JsonElement.Parse(jsonText);

            // Act
            var result = schema.Evaluate(json);

            // For plaintext
            var source = JsonSerializer.Serialize(schema);

            // Assert
            Assert.Contains("\"oneOf\"", source);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task CustomFieldSchemaFailedWithSpaceTest()
        {
            // Arrange
            var schema = CustomFieldSchema.Create();

            var jsonText = @"[{
                ""type"": ""checkbox"",
                ""name"": ""name"",
                ""space"": ""five1"",
                ""options"": [
                    {
                        ""id"": 1,
                        ""title"": ""Name 1""
                    },
                    {
                        ""id"": ""b"",
                        ""title"": ""Name B""
                    }
                ]
            }]";

            var json = JsonElement.Parse(jsonText);

            // Act
            var result = schema.Evaluate(json);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public async Task CustomFieldSchemaFailedWithOptionsTest()
        {
            // Arrange
            var schema = CustomFieldSchema.Create();

            var jsonText = @"[{
                ""type"": ""checkbox"",
                ""name"": ""name"",
                ""space"": ""five"",
                ""options"": [
                    {
                        ""id"": 1,
                        ""title"": ""Name 1""
                    },
                    {
                        ""id"": ""b""
                    }
                ]
            }]";

            var json = JsonElement.Parse(jsonText);

            // Act
            var result = schema.Evaluate(json);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public async Task EmailTemplateSchemaSuccessTest()
        {
            // Arrange
            var schema = EmailTemplateSchema.Create();

            var template = new EmailTemplateDto
            {
                Subject = "Test",
                Template = "Test",
                Cc = ["info@etsoo.com", "ETSOO Sales <sales@etsoo.com>"],
                Bcc = ["ETSOO Support <support@etsoo.com>"]
            };

            var jsonText = JsonSerializer.Serialize(template, ModelJsonSerializerContext.Default.EmailTemplateDto);

            var json = JsonElement.Parse(jsonText);

            // Act
            var result = schema.Evaluate(json);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task EmailTemplateSchemaEmailFailedTest()
        {
            // Arrange
            var schema = EmailTemplateSchema.Create();

            var template = new EmailTemplateDto
            {
                Subject = "Test",
                Template = "Test",
                IsRazor = true,
                Cc = ["info@etsoo."],
                Bcc = ["abc"]
            };

            var jsonText = JsonSerializer.Serialize(template, ModelJsonSerializerContext.Default.EmailTemplateDto);

            var json = JsonElement.Parse(jsonText);

            // Act
            var result = schema.Evaluate(json);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public async Task EmailTemplateSchemaDuplicateCcFailedTest()
        {
            // Arrange
            var schema = EmailTemplateSchema.Create();

            var template = new EmailTemplateDto
            {
                Subject = "Test",
                Template = "Test",
                IsRazor = true,
                Cc = ["info@etsoo.com", "info@etsoo.com"]
            };

            var jsonText = JsonSerializer.Serialize(template, ModelJsonSerializerContext.Default.EmailTemplateDto);

            var json = JsonElement.Parse(jsonText);

            // Act
            var result = schema.Evaluate(json);

            // Assert
            Assert.IsFalse(result.IsValid);
        }
    }
}
