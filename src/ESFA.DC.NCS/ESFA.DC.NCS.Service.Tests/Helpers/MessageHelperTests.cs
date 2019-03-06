using System;
using ESFA.DC.NCS.Service.Helpers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.NCS.Service.Tests.Helpers
{
    public class MessageHelperTests
    {
        [Theory]
        [InlineData("2018-04-12")]
        [InlineData("2019-04-09")]
        public void CalculateFundingYearStart_ShouldBe2018(string testDate)
        {
            // Arrange
            var messageHelper = new MessageHelper();
            var submissionDate = DateTime.Parse(testDate);
            var expectedDate = new DateTime(2018, 04, 01);

            // Act
            var result = messageHelper.CalculateFundingYearStart(submissionDate);

            // Assert
            result.Should().Be(expectedDate);
        }

        [Theory]
        [InlineData("2019-04-12")]
        [InlineData("2020-04-09")]
        public void CalculateFundingYearStart_ShouldBe2019(string testDate)
        {
            // Arrange
            var messageHelper = new MessageHelper();
            var submissionDate = DateTime.Parse(testDate);
            var expectedDate = new DateTime(2019, 04, 01);

            // Act
            var result = messageHelper.CalculateFundingYearStart(submissionDate);

            // Assert
            result.Should().Be(expectedDate);
        }

        [Fact]
        public void CalculateFundingYearStart_ShouldThrowException()
        {
            // Arrange
            var messageHelper = new MessageHelper();
            var submissionDate = new DateTime(2050, 01, 01);

            // Act
            Action act = () => messageHelper.CalculateFundingYearStart(submissionDate);

            // Assert
            act.Should()
                .Throw<Exception>()
                .WithMessage($"Submission date:{submissionDate} does not fall between any known collection year.");
        }
    }
}
