using NUnit.Framework;

namespace Tests.ActionResult
{
    [TestFixture]
    public class CreateFromTests
    {
        [Test]
        public void CreateFromExceptionTest()
        {
            var message = "System Exception";
            var exception = new SystemException(message);
            exception.Data.Add("HasData", true);
            var result = com.etsoo.Utils.Actions.ActionResult.From(exception);
            Assert.Multiple(() =>
            {
                Assert.That(result.Title, Is.EqualTo(message));
                Assert.That(result.Data.Get<bool>("HasData"), Is.True);
            });
        }
    }
}
