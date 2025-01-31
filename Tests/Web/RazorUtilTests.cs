using com.etsoo.Web;
using NUnit.Framework;

namespace Tests.Web
{
    public record TestModelItem(string Name);

    public record TestModel
    {
        public string? Name { get; init; }

        public IEnumerable<TestModelItem>? Items { get; init; }
    }

    [TestFixture]
    public class RazorUtilTests
    {
        [Test]
        public async Task RenderAsync_Test()
        {
            // Arrange
            var template = """
                @{
                  string AddHello(string Name)
                  {
                      return $"Hello, {Name}";
                  }

                  var model = @Model as Tests.Web.TestModel;
                }

                <p>@AddHello(model.Name)</p>@foreach(var item in model.Items){<h1>@item.Name</h1>}
            """;

            var model = new TestModel
            {
                Name = "ETSOO",
                Items = [new TestModelItem("Item 1"), new TestModelItem("Item 2")]
            };

            // Act
            var result = (await RazorUtils.RenderAsync(nameof(RenderAsync_Test), template, model)).Trim();

            // Assert
            Assert.That(result, Is.EqualTo("<p>Hello, ETSOO</p><h1>Item 1</h1><h1>Item 2</h1>"));
        }

        [Test]
        public async Task HtmlSafeRenderAsync_Test()
        {
            // Arrange
            var template = """
                @{
                  var model = @Model as Tests.Web.TestModel;
                }
                <p>@model.Name, @Html.Raw(model.Name)</p>
            """;

            var model = new TestModel
            {
                Name = "<b>ETSOO</b>"
            };

            // Act
            var result = (await RazorUtils.RenderAsync(nameof(HtmlSafeRenderAsync_Test), template, model)).Trim();

            // Assert
            Assert.That(result, Is.EqualTo("<p>&lt;b&gt;ETSOO&lt;/b&gt;, <b>ETSOO</b></p>"));
        }
    }
}
