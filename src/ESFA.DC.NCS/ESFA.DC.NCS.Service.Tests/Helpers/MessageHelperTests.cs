using System;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Service.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.NCS.Service.Tests.Helpers
{
    public class MessageHelperTests
    {
        [Fact]
        public void CalculateFundingYearStart_ShouldBe2019()
        {
            var collectionYear = 1920;
            var expectedDate = new DateTime(2019, 04, 01);
            var messageHelper = new MessageHelper();

            var messageMock = new Mock<INcsJobContextMessage>();
            messageMock.Setup(m => m.CollectionYear).Returns(collectionYear);

            var result = messageHelper.CalculateFundingYearStart(messageMock.Object);

            result.Should().Be(expectedDate);
        }

        [Fact]
        public void CalculateFundingYearStart_ShouldBe2020()
        {
            var collectionYear = 2021;
            var expectedDate = new DateTime(2020, 04, 01);
            var messageHelper = new MessageHelper();

            var messageMock = new Mock<INcsJobContextMessage>();
            messageMock.Setup(m => m.CollectionYear).Returns(collectionYear);

            var result = messageHelper.CalculateFundingYearStart(messageMock.Object);

            result.Should().Be(expectedDate);
        }

        [Fact]
        public void CalculateFundingYearStart_ShouldThrowException()
        {
            var collectionYear = 2122;
            var messageHelper = new MessageHelper();

            var messageMock = new Mock<INcsJobContextMessage>();
            messageMock.Setup(m => m.CollectionYear).Returns(collectionYear);

            Action act = () => messageHelper.CalculateFundingYearStart(messageMock.Object);

            act.Should()
                .Throw<Exception>()
                .WithMessage($"Collection year:{collectionYear} unknown.");
        }
    }
}
