using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.NCS.Interfaces;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ESFA.DC.NCS.DataService.Tests
{
    public class FundingValueQueryServiceTests
    {
        [Theory]
        [InlineData("9999999999", 1920)]
        [InlineData("1111111111", 1819)]
        public async Task GetFundingValuesAsync_NoResults(string touchpointId, int collectionYear)
        {
            var fundingValues = new List<FundingValue>
            {
                new FundingValue()
                {
                    UKPRN = 123456789,
                    TouchpointId = "9999999999",
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomePriorityCustomer = 1,
                    Value = 10,
                    Period = "April",
                    CollectionYear = 1819
                }
            };

            var ncsDbMock = fundingValues.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(fv => fv.FundingValues).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);
            ncsJobContextMessage.Setup(m => m.CollectionYear).Returns(collectionYear);

            var result = await NewService(ncsContext).GetFundingValuesAsync(ncsJobContextMessage.Object, CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetFundingValuesAsync_OneResult()
        {
            var touchpointId = "9999999999";
            var collectionYear = 1819;
            var fundingValues = new List<FundingValue>
            {
                new FundingValue()
                {
                    UKPRN = 123456789,
                    TouchpointId = touchpointId,
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomePriorityCustomer = 1,
                    Value = 10,
                    Period = "April",
                    CollectionYear = collectionYear
                },
                new FundingValue()
                {
                    UKPRN = 123456789,
                    TouchpointId = "1111111111",
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomePriorityCustomer = 1,
                    Value = 50,
                    Period = "April",
                    CollectionYear = 1920
                }
            };

            var ncsDbMock = fundingValues.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(fv => fv.FundingValues).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);
            ncsJobContextMessage.Setup(m => m.CollectionYear).Returns(collectionYear);

            var result = await NewService(ncsContext).GetFundingValuesAsync(ncsJobContextMessage.Object, CancellationToken.None);

            result.Should().NotBeEmpty();
            result.Count().Should().Be(1);
            result.Should().Contain(x => x.TouchpointId.Equals(touchpointId));
            result.Should().Contain(x => x.CollectionYear.Equals(collectionYear));
        }

        [Fact]
        public async Task GetFundingValuesAsync_MultipleResults()
        {
            var touchpointId = "9999999999";
            var collectionYear = 1819;
            var fundingValues = new List<FundingValue>
            {
                new FundingValue()
                {
                    UKPRN = 123456789,
                    TouchpointId = touchpointId,
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomePriorityCustomer = 1,
                    Value = 10,
                    Period = "April",
                    CollectionYear = collectionYear
                },
                new FundingValue()
                {
                    UKPRN = 123456789,
                    TouchpointId = touchpointId,
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomePriorityCustomer = 1,
                    Value = 50,
                    Period = "April",
                    CollectionYear = collectionYear
                }
            };

            var ncsDbMock = fundingValues.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(fv => fv.FundingValues).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);
            ncsJobContextMessage.Setup(m => m.CollectionYear).Returns(collectionYear);

            var result = await NewService(ncsContext).GetFundingValuesAsync(ncsJobContextMessage.Object, CancellationToken.None);

            result.Should().NotBeEmpty();
            result.Count().Should().Be(2);
        }

        private FundingValueQueryService NewService(Func<INcsContext> ncsContext = null)
        {
            return new FundingValueQueryService(ncsContext);
        }
    }
}
