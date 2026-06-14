using com.etsoo.Utils;

namespace Tests.Utils
{
    [TestClass]
    public class NumUtilsTests
    {
        [TestMethod]
        public void GetMonthPeriod_WithValidDate_ReturnsCorrectPeriod()
        {
            // Arrange
            var year = 2024;
            var month = 3;
            var date = new DateTimeOffset(year, month, 15, 0, 0, 0, TimeSpan.Zero);

            // Act
            var result = NumUtils.GetMonthPeriod(date);

            // Assert
            Assert.AreEqual(202403, result);
        }

        [TestMethod]
        public void GetMonthPeriod_WithValidYearAndMonth_ReturnsCorrectPeriod()
        {
            // Arrange
            var year = 2024;
            var month = 3;

            // Act
            var result = NumUtils.GetMonthPeriod(year, month);

            // Assert
            Assert.AreEqual(202403, result);
        }

        [TestMethod]
        public void GetMonthPeriod_WithJanuary_ReturnsCorrectPeriod()
        {
            // Arrange
            var year = 2024;
            var month = new DateTime(year, 1, 12).Month;

            // Act
            var result = NumUtils.GetMonthPeriod(year, month);

            // Assert
            Assert.AreEqual(202401, result);
        }

        [TestMethod]
        public void GetMonthPeriod_WithDecember_ReturnsCorrectPeriod()
        {
            // Arrange
            var year = 2024;
            var month = 12;

            // Act
            var result = NumUtils.GetMonthPeriod(year, month);

            // Assert
            Assert.AreEqual(202412, result);
        }

        [TestMethod]
        public void GetMonthPeriodRange_WithValidYear_ReturnsCorrectRange()
        {
            // Arrange
            var year = 2024;

            // Act
            var (monthStart, monthEnd) = NumUtils.GetMonthPeriodRange(year);

            // Assert
            Assert.AreEqual(202401, monthStart);
            Assert.AreEqual(202412, monthEnd);
        }

        [TestMethod]
        public void GetQuarterPeriod_WithFirstQuarter_ReturnsCorrectPeriod()
        {
            // Arrange
            var year = 2024;
            var month = 1;

            // Act
            var result = NumUtils.GetQuarterPeriod(year, month);

            // Assert
            Assert.AreEqual(20241, result);
        }

        [TestMethod]
        public void GetQuarterPeriod_WithSecondQuarter_ReturnsCorrectPeriod()
        {
            // Arrange
            var year = 2024;
            var month = 4;

            // Act
            var result = NumUtils.GetQuarterPeriod(year, month);

            // Assert
            Assert.AreEqual(20242, result);
        }

        [TestMethod]
        public void GetQuarterPeriod_WithThirdQuarter_ReturnsCorrectPeriod()
        {
            // Arrange
            var year = 2024;
            var month = 7;

            // Act
            var result = NumUtils.GetQuarterPeriod(year, month);

            // Assert
            Assert.AreEqual(20243, result);
        }

        [TestMethod]
        public void GetQuarterPeriod_WithFourthQuarter_ReturnsCorrectPeriod()
        {
            // Arrange
            var year = 2024;
            var month = 10;

            // Act
            var result = NumUtils.GetQuarterPeriod(year, month);

            // Assert
            Assert.AreEqual(20244, result);
        }

        [TestMethod]
        public void GetQuarterPeriod_WithEndOfQuarter_ReturnsCorrectPeriod()
        {
            // Arrange & Act & Assert
            Assert.AreEqual(20241, NumUtils.GetQuarterPeriod(2024, 3));
            Assert.AreEqual(20242, NumUtils.GetQuarterPeriod(2024, 6));
            Assert.AreEqual(20243, NumUtils.GetQuarterPeriod(2024, 9));
            Assert.AreEqual(20244, NumUtils.GetQuarterPeriod(2024, 12));
        }

        [TestMethod]
        public void GetQuarterPeriodRange_WithValidYear_ReturnsCorrectRange()
        {
            // Arrange
            var year = 2024;

            // Act
            var (quarterStart, quarterEnd) = NumUtils.GetQuarterPeriodRange(year);

            // Assert
            Assert.AreEqual(20241, quarterStart);
            Assert.AreEqual(20244, quarterEnd);
        }

        [TestMethod]
        public void GetMonthPeriod_WithDifferentYears_ReturnsDifferentPeriods()
        {
            // Arrange & Act
            var period2023 = NumUtils.GetMonthPeriod(2023, 6);
            var period2024 = NumUtils.GetMonthPeriod(2024, 6);

            // Assert
            Assert.AreEqual(202306, period2023);
            Assert.AreEqual(202406, period2024);
            Assert.AreNotEqual(period2023, period2024);
        }

        [TestMethod]
        public void GetQuarterPeriod_WithDifferentYears_ReturnsDifferentPeriods()
        {
            // Arrange & Act
            var period2023 = NumUtils.GetQuarterPeriod(2023, 6);
            var period2024 = NumUtils.GetQuarterPeriod(2024, 6);

            // Assert
            Assert.AreEqual(20232, period2023);
            Assert.AreEqual(20242, period2024);
            Assert.AreNotEqual(period2023, period2024);
        }
    }
}
