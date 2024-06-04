using com.etsoo.CoreFramework.Json;
using com.etsoo.CoreFramework.Models;
using com.etsoo.Utils;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Tests.CoreFramework
{
    [TestFixture]
    public class JsonTests
    {
        [Test]
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
            var json = await JsonNode.ParseAsync(SharedUtils.GetStream(jsonText));

            // Act
            var result = schema.Evaluate(json);

            // For plaintext
            var source = JsonSerializer.Serialize(schema);

            // Assert
            Assert.That(source, Does.Contain("\"oneOf\""));
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
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
            var json = await JsonNode.ParseAsync(SharedUtils.GetStream(jsonText));

            // Act
            var result = schema.Evaluate(json);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
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
            var json = await JsonNode.ParseAsync(SharedUtils.GetStream(jsonText));

            // Act
            var result = schema.Evaluate(json);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public async Task EmailTemplateSchemaSuccessTest()
        {
            // Arrange
            var schema = EmailTemplateSchema.Create();

            var template = new EmailTemplateDto
            {
                Subject = "Test",
                Template = "Test",
                Cc = ["info@etsoo.com", "ETSOO Sales <sales@etsoo.com>"]
            };

            var jsonText = JsonSerializer.Serialize(template, ModelJsonSerializerContext.Default.EmailTemplateDto);

            var json = await JsonNode.ParseAsync(SharedUtils.GetStream(jsonText));

            // Act
            var result = schema.Evaluate(json);

            // Assert
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
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

            var json = await JsonNode.ParseAsync(SharedUtils.GetStream(jsonText));

            // Act
            var result = schema.Evaluate(json);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }
    }
}
