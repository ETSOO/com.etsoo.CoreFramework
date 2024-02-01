using com.etsoo.Utils;
using com.etsoo.Utils.Models;
using com.etsoo.Utils.Serialization;
using NUnit.Framework;
using System.Text;
using System.Text.Json;

namespace Tests.Utils
{
    class Data
    {
        public int Id { get; set; }

        public string Name { get; }

        public Data(string name)
        {
            Name = name;
        }
    }

    [TestFixture]
    public class SharedUtilTests
    {
        [Test]
        public void GetAccordingValueTests()
        {
            var fields = new List<string> { "日期", "美元", "欧元", "日元", "港元", "英镑", "林吉特", "卢布", "澳元", "加元", "新西兰元", "新加坡元", "瑞士法郎", "兰特", "韩元", "迪拉姆", "里亚尔", "福林", "兹罗提", "丹麦克朗", "瑞典克朗", "挪威克朗", "里拉", "比索", "泰铢" };
            var values = new List<string> { "2022-02-24", "632.8", "715.14", "5.5079", "81.079", "856.99", "66.144", "1283.55", "457.16", "496.96", "428.31", "470.01", "689.73", "239.05", "18889.0", "58.039", "59.287", "5046.66", "64.149", "104.02", "148.4", "140.58", "218.486", "319.98", "509.91" };
            var value = SharedUtils.GetAccordingValue<decimal>(fields, values, "港元", 4);
            Assert.AreEqual(81.079, value);
        }

        [Test]
        public async Task JsonSerializeAsyncTests()
        {
            // Arrange
            var data = new Data("Etsoo");

            // Act
            using var stream = SharedUtils.GetStream();
            await SharedUtils.JsonSerializeAsync(stream, data, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            var result = Encoding.UTF8.GetString(stream.GetReadOnlySequence());

            // Assert
            Assert.AreEqual("""{"id":0,"name":"Etsoo"}""", result);
        }

        [Test]
        public async Task ObjectToDictionaryAsyncTests()
        {
            // Arrange
            var data = new Data("Etsoo");

            // Act
            var dic = await SharedUtils.ObjectToDictionaryAsync(data);

            // Assert
            Assert.AreEqual(dic.Keys.Count, 2);
            Assert.IsTrue(dic.ContainsKey("Name"));
        }

        [Test]
        public async Task StreamToSequenceAsyncTests()
        {
            // Arrange
            var source = "MIIEpAIBAAKCAQEAvJdWUjJubQxN0LGiaaSlxfhHnecAN2yQphCt5HYW3T5m6fSaQVIJ57eFqxw4+D5CTdmLcCVHpghNKa7/FF+NH85a4i30+EMXjlJbRzwd5Tz4RXeiixDexRG9LOx1q31jFuKfo9/9L94rvQvUphHEomqk6zVjak0wVmwUu/WQbAR16Aa62pXibBZVQoQ8z4gz/gUm7aABf7nXmmH2Wm9cLSBo5nfe01qcRKUY8rTpVJeLUzYpsbddyHcJogj2RnuaS1H+KfKkhX0re0i/NFdwQBhyTPxdXWoMOO+L3unOK9eJLcp36GEL4vsA4C4FFYwhhtbTEqOHKAqSbRGDQCGViQIDAQABAoIBADm4L8k0eiuR6ncHBuhCZiIzHOgs/Rn5dkP9MtuLcPAB20mbfWKkkzeCKbz3BFCl7XEaNdz66/Ta8ZLiZlt76xti8tnqquEY16rNdfZVZej1Qh8wwGTDowq5pSaMsG7uD1e/wJBNS5ZM7yIK7uhs47u53API8UZlnXe12Jq2S59jLwnkgeStZT0yatWvblpeMBtiitW6W/SuULZuWjZL0TLGLoxsXFUUBwgCi28gdYP2ra0mlcUN7WBy0+ug/TZ8YOXiZZX8SQN8mSUK+KV5uOghri37Nxoyx3aQOwUxFZqdCPbdhFFhUlvJsRNJu/QQq3p5FT6VMJ26yA0ArNT9o2kCgYEAyHC43XvlDXWINVieCGyxucCwAvklWPLL7eJLqsnmj5YNpheP/xausTSbLQIIeAlcfpVTcJMTiodhoBoBi2e+yLNyLOZCKSLFsJLxUO83S/C7wWdajpDXybaNG7MvWugaPAA7RvywhHtZLOutDwqJTre1UfNbHQ2bdJn5Wl1gSMMCgYEA8N3L9zz0WWJhm2Gp6Ws6nB41R/6FaKvWfORwCUp3FzHr+FqfkyQFidv4n0Oo7/LH2wt7NBvCZlsKe37uHEhEZSFKI89/LbaGALGcw2S41VWiS96YOMPRQyFdPHZqfiR8zX+RcqN7LtdOwWKcy0og+bi8yFGDL3vf143o40FSo8MCgYAt9LpN/cQMi/AI2yKQp+svvaAdbmZDuJdNGV9j7xqvvSWv+SMIx3iSJI+XiCnM68iLNU2GOBJ45oVZodzMy6KQfaQl6z0sFU7iJy6w8cfp324M79dxbIAtPW+o9DJdU24AZ8Uvh2wpU+akR/zLwAyvQauO+I7hYGdOGqdzMomK5QKBgQCaNBURyudQlkiQ9pyWAH08V6aa2drFIUYnHQSRHihSJDbDABmrVONq1/Y62FE+lPrYRGhy+tahOuXiHGgKmUWYTRCvDneIZ5MwvIT1HvWqNrG5yt8/cDX3uVN8kv8olOmFkocmkn0ZhuQ3sI9bIrErztallHHdI3wx/vs7CqYCiwKBgQCsjt2JsC9ceqjcMoj+oBztH4axMMUgOK6vDC3tPaAyznoYLUlMeWFfvVZn7CK6GP6Rcns6jouPSqQEHEv+w4RNOeA7cF0EJ2i+h8jzLZpItBwTTU4a8bFxNpyFu2Hq7lXzzWMAb5z6giecM4kM4hdvQ4CzAgL9JsHGK5vet0KFfA==";
            var stream = SharedUtils.GetStream(source);
            var result = await SharedUtils.StreamToStringAsync(stream);

            Assert.AreEqual(source, result);
        }

        [Test]
        public void TruncateDateTimeTests()
        {
            var now = DateTime.Now;
            var now1 = SharedUtils.TruncateDateTime(now);
            Assert.AreEqual(0, now1.Millisecond);

            var utc = DateTime.UtcNow;
            var utc1 = SharedUtils.TruncateDateTime(utc);
            Assert.AreEqual(0, utc1.Millisecond);
            Assert.AreEqual(DateTimeKind.Utc, utc1.Kind);
        }

        [Test]
        public async Task JsonSerializeAsync_SourceGeneratorTests()
        {
            var model = new UserModel
            {
                Id = 1001,
                Name = "Admin 1",
                Valid = true,
                DecimalValue = 3.14M,
                Date = DateTime.UtcNow
            };

            var json = await SharedUtils.JsonSerializeAsync(model, new JsonSerializerOptions { IncludeFields = true }, ["Id", "name", "Valid", "Date"]);
            Assert.IsTrue(json.Contains("Name"));
            Assert.IsTrue(json.Contains("Valid"));
            Assert.IsFalse(json.Contains("DecimalValue"));
        }

        [Test]
        public async Task JsonSerializeAsync_TextJsonTests()
        {
            var model = new UserUpdateModule
            {
                Id = 1001,
                Name = "Admin 1"
            };

            var json = await SharedUtils.JsonSerializeAsync(model, new JsonSerializerOptions(), ["Id"]);
            Assert.IsTrue(json.Contains("Id"));
            Assert.IsFalse(json.Contains("Name"));
        }

        [Test]
        public async Task JsonSerializeAsync_TypeInfoTests()
        {
            var model = new GuidItem
            {
                Id = Guid.NewGuid(),
                Label = "Guid Label"
            };

            var json = await SharedUtils.JsonSerializeAsync(model, CommonJsonSerializerContext.Default.GuidItem, ["id"]);
            Assert.IsTrue(json.Contains("id"));
            Assert.IsFalse(json.Contains("label"));
        }

        [Test]
        public async Task JoinAsAuditJsonTests()
        {
            var oldData = new UserModel
            {
                Id = 1001,
                Name = "Admin 1", // Name is a field, not a property, so should be ignored
                Valid = true,
                DecimalValue = 3.14M,
                Date = DateTime.UtcNow
            };

            var newData = new UserUpdateModule
            {
                Id = 1001,
                Name = "Admin 2"
            };

            var json = await SharedUtils.JoinAsAuditJsonAsync(oldData, newData, ["Id", "Name"]);
            Assert.AreEqual("{\"oldData\":{\"id\":1001},\"newData\":{\"name\":\"Admin 2\",\"id\":1001}}", json);
        }
    }
}
