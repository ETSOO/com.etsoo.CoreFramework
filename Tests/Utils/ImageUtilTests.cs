using com.etsoo.Utils.Image;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.Utils
{
    [TestFixture]
    public class ImageUtilTests
    {
        private static IEnumerable<TestCaseData> GetCodecInfoBulkTestData
        {
            get
            {
                yield return new TestCaseData(".jpg", "image/jpeg");
                yield return new TestCaseData(".tiff", "image/tiff");
                yield return new TestCaseData(".txt", null);
            }
        }

        [Test, TestCaseSource(nameof(GetCodecInfoBulkTestData))]
        public void GetCodecInfo_BulkTests(string path, string? mimeType)
        {
            // Arrange & act
            var result = ImageUtils.GetCodecInfo(path);

            // Assert
            Assert.AreEqual(result?.MimeType, mimeType);
        }
    }
}
