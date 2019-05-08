using System;
using System.Collections.Generic;
using System.Linq;
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
    public class NcsSubmissionQueryServiceTests
    {
        [Theory]
        [InlineData("9999999999", 1920)]
        [InlineData("1111111111", 1819)]
        public async Task GetNcsSubmissionsAsync_NoResults(string touchpointId, int collectionYear)
        {
            var ncsSubmissions = new List<NcsSubmission>
            {
                new NcsSubmission()
                {
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    AdviserName = "Adviser Name",
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomePriorityCustomer = 1,
                    OutcomeType = 1,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    Ukprn = 123456789,
                    TouchpointId = "9999999999",
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1819
                }
            };

            var ncsDbMock = ncsSubmissions.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(ns => ns.NcsSubmissions).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);
            ncsJobContextMessage.Setup(m => m.CollectionYear).Returns(collectionYear);

            var result = await NewService(ncsContext).GetNcsSubmissionsAsync(ncsJobContextMessage.Object, CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetNcsSubmissionsAsync_OneResult()
        {
            var touchpointId = "9999999999";
            var collectionYear = 1819;
            var ncsSubmissions = new List<NcsSubmission>
            {
                new NcsSubmission()
                {
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    AdviserName = "Adviser Name",
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomePriorityCustomer = 1,
                    OutcomeType = 1,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    Ukprn = 123456789,
                    TouchpointId = touchpointId,
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = collectionYear
                },
                new NcsSubmission()
                {
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    AdviserName = "Adviser Name",
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomePriorityCustomer = 1,
                    OutcomeType = 1,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    Ukprn = 123456789,
                    TouchpointId = "1111111111",
                    DssJobId = new Guid("6d639b8f-d5e2-4732-b2c7-e390a8d233b1"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1920
                }
            };

            var ncsDbMock = ncsSubmissions.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(ns => ns.NcsSubmissions).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);
            ncsJobContextMessage.Setup(m => m.CollectionYear).Returns(collectionYear);

            var result = await NewService(ncsContext).GetNcsSubmissionsAsync(ncsJobContextMessage.Object, CancellationToken.None);

            result.Should().NotBeEmpty();
            result.Count().Should().Be(1);
            result.Should().Contain(x => x.TouchpointId.Equals(touchpointId));
            result.Should().Contain(x => x.CollectionYear.Equals(collectionYear));
        }

        [Fact]
        public async Task GetNcsSubmissionsAsync_MultipleResults()
        {
            var touchpointId = "9999999999";
            var collectionYear = 1819;
            var ncsSubmissions = new List<NcsSubmission>
            {
                new NcsSubmission()
                {
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    AdviserName = "Adviser Name",
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomePriorityCustomer = 1,
                    OutcomeType = 1,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    Ukprn = 123456789,
                    TouchpointId = touchpointId,
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = collectionYear
                },
                new NcsSubmission()
                {
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    AdviserName = "Adviser Name",
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomePriorityCustomer = 1,
                    OutcomeType = 1,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    Ukprn = 123456789,
                    TouchpointId = touchpointId,
                    DssJobId = new Guid("6d639b8f-d5e2-4732-b2c7-e390a8d233b1"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = collectionYear
                }
            };

            var ncsDbMock = ncsSubmissions.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(ns => ns.NcsSubmissions).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);
            ncsJobContextMessage.Setup(m => m.CollectionYear).Returns(collectionYear);

            var result = await NewService(ncsContext).GetNcsSubmissionsAsync(ncsJobContextMessage.Object, CancellationToken.None);

            result.Should().NotBeEmpty();
            result.Count().Should().Be(2);
        }

        private NcsSubmissionQueryService NewService(Func<INcsContext> ncsContext = null)
        {
            return new NcsSubmissionQueryService(ncsContext);
        }
    }
}
