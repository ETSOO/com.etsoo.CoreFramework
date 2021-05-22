using com.etsoo.Web;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Web
{
    public record TestModelItem (string Name);

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
            var template = @"
                @{
                  string AddHello(string Name)
                  {
                      return $""Hello, {Name}"";
                  }

                  var model = @Model as Tests.Web.TestModel;
                }

                <p>@AddHello(model.Name)</p>@foreach(var item in model.Items){<h1>@item.Name</h1>}
            ";

            var model = new TestModel {
                Name = "ETSOO",
                Items = new List<TestModelItem> { new TestModelItem("Item 1"), new TestModelItem("Item 2") }
            };

            // Act
            var result = (await RazorUtils.RenderAsync("test", template, model)).Trim();

            // Assert
            Assert.IsTrue(result.Equals("<p>Hello, ETSOO</p><h1>Item 1</h1><h1>Item 2</h1>"), result);
        }
    }
}
