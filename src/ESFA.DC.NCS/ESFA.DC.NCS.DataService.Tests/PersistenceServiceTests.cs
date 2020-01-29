using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.NCS.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ESFA.DC.NCS.DataService.Tests
{
    public class PersistenceServiceTests
    {
        [Fact]
        public async Task PersistSubmissionAndFundingValuesAsync_SuccessNoPreviousData()
        {
            var touchpointId = "9999999999";

            var fundingValues = GetFundingValuesSet1();

            var submissions = GetNcsSubmissionSet1();

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);

            var loggerMock = new Mock<ILogger>();

            var options = new DbContextOptionsBuilder<NcsContext>()
                .UseInMemoryDatabase(databaseName: "PersistSubmissionAndFundingValuesAsync_SuccessNoPreviousData")
                .Options;

            Func<INcsContext> ncsContext = () => new NcsContext(options);

            using (var context = ncsContext())
            {
                await NewService(ncsContext, loggerMock.Object).PersistSubmissionAndFundingValuesAsync(submissions, fundingValues, ncsJobContextMessage.Object, CancellationToken.None);
            }

            using (var context = new NcsContext(options))
            {
                // Funding Values Assertion
                context.FundingValues.Should().NotBeEmpty();
                context.FundingValues.Should().ContainSingle();
                context.FundingValues.Should().Contain(x => x.Ukprn.Equals(123456789));
                context.FundingValues.Should().Contain(x => x.TouchpointId.Equals("9999999999"));
                context.FundingValues.Should().Contain(x => x.CustomerId.Equals(new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e")));
                context.FundingValues.Should().Contain(x => x.ActionPlanId.Equals(new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7")));
                context.FundingValues.Should().Contain(x => x.OutcomeId.Equals(new Guid("91b85df7-6313-4b56-bce0-80133a7b2559")));
                context.FundingValues.Should().Contain(x => x.OutcomeType.Equals(1));
                context.FundingValues.Should().Contain(x => x.OutcomeEffectiveDate.Equals(new DateTime(2019, 04, 01)));
                context.FundingValues.Should().Contain(x => x.OutcomePriorityCustomer.Equals(1));
                context.FundingValues.Should().Contain(x => x.Value.Equals(10));
                context.FundingValues.Should().Contain(x => x.Period.Equals("April"));

                // Submission Assertion
                context.NcsSubmissions.Should().NotBeEmpty();
                context.NcsSubmissions.Should().ContainSingle();
                context.NcsSubmissions.Should().Contain(x => x.ActionPlanId.Equals(new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7")));
                context.NcsSubmissions.Should().Contain(x => x.AdviserName.Equals("Adviser Name"));
                context.NcsSubmissions.Should().Contain(x => x.CustomerId.Equals(new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e")));
                context.NcsSubmissions.Should().Contain(x => x.DateOfBirth.Equals(new DateTime(2000, 01, 01)));
                context.NcsSubmissions.Should().Contain(x => x.HomePostCode.Equals("XXX XXX"));
                context.NcsSubmissions.Should().Contain(x => x.OutcomeEffectiveDate.Equals(new DateTime(2019, 04, 01)));
                context.NcsSubmissions.Should().Contain(x => x.OutcomeId.Equals(new Guid("91b85df7-6313-4b56-bce0-80133a7b2559")));
                context.NcsSubmissions.Should().Contain(x => x.OutcomePriorityCustomer.Equals(1));
                context.NcsSubmissions.Should().Contain(x => x.OutcomeType.Equals(1));
                context.NcsSubmissions.Should().Contain(x => x.SubContractorId.Equals("Subcontractor ID"));
                context.NcsSubmissions.Should().Contain(x => x.SessionDate.Equals(new DateTime(2019, 03, 01)));
                context.NcsSubmissions.Should().Contain(x => x.Ukprn.Equals(123456789));
                context.NcsSubmissions.Should().Contain(x => x.TouchpointId.Equals("9999999999"));
                context.NcsSubmissions.Should().Contain(x => x.DssJobId.Equals(new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab")));
                context.NcsSubmissions.Should().Contain(x => x.DssTimestamp.Equals(new DateTime(2019, 04, 01)));
                context.NcsSubmissions.Should().Contain(x => x.CollectionYear.Equals(1819));
            }
        }

        [Fact]
        public async Task PersistSubmissionAndFundingValuesAsync_SuccessWithPreviousData()
        {
            var touchpointId = "9999999999";

            var fundingValuesSet1 = GetFundingValuesSet1();
            var fundingValuesSet2 = GetFundingValuesSet2();

            var submissionsSet1 = GetNcsSubmissionSet1();
            var submissionsSet2 = GetNcsSubmissionSet2();

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);
            ncsJobContextMessage.Setup(m => m.CollectionYear).Returns(1819);

            var loggerMock = new Mock<ILogger>();

            var options = new DbContextOptionsBuilder<NcsContext>()
                .UseInMemoryDatabase(databaseName: "PersistSubmissionAndFundingValuesAsync_SuccessWithPreviousData")
                .Options;

            Func<INcsContext> ncsContext = () => new NcsContext(options);

            using (var context = ncsContext())
            {
                await NewService(ncsContext, loggerMock.Object).PersistSubmissionAndFundingValuesAsync(submissionsSet1, fundingValuesSet1, ncsJobContextMessage.Object, CancellationToken.None);
            }

            using (var context = new NcsContext(options))
            {
                // Funding Values Assertion
                context.FundingValues.Should().NotBeEmpty();
                context.FundingValues.Should().ContainSingle();
                context.FundingValues.Should().Contain(x => x.Ukprn.Equals(123456789));
                context.FundingValues.Should().Contain(x => x.TouchpointId.Equals("9999999999"));
                context.FundingValues.Should().Contain(x => x.CustomerId.Equals(new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e")));
                context.FundingValues.Should().Contain(x => x.ActionPlanId.Equals(new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7")));
                context.FundingValues.Should().Contain(x => x.OutcomeId.Equals(new Guid("91b85df7-6313-4b56-bce0-80133a7b2559")));
                context.FundingValues.Should().Contain(x => x.OutcomeType.Equals(1));
                context.FundingValues.Should().Contain(x => x.OutcomeEffectiveDate.Equals(new DateTime(2019, 04, 01)));
                context.FundingValues.Should().Contain(x => x.OutcomePriorityCustomer.Equals(1));
                context.FundingValues.Should().Contain(x => x.Value.Equals(10));
                context.FundingValues.Should().Contain(x => x.Period.Equals("April"));

                // Submission Assertion
                context.NcsSubmissions.Should().NotBeEmpty();
                context.NcsSubmissions.Should().ContainSingle();
                context.NcsSubmissions.Should().Contain(x => x.ActionPlanId.Equals(new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7")));
                context.NcsSubmissions.Should().Contain(x => x.AdviserName.Equals("Adviser Name"));
                context.NcsSubmissions.Should().Contain(x => x.CustomerId.Equals(new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e")));
                context.NcsSubmissions.Should().Contain(x => x.DateOfBirth.Equals(new DateTime(2000, 01, 01)));
                context.NcsSubmissions.Should().Contain(x => x.HomePostCode.Equals("XXX XXX"));
                context.NcsSubmissions.Should().Contain(x => x.OutcomeEffectiveDate.Equals(new DateTime(2019, 04, 01)));
                context.NcsSubmissions.Should().Contain(x => x.OutcomeId.Equals(new Guid("91b85df7-6313-4b56-bce0-80133a7b2559")));
                context.NcsSubmissions.Should().Contain(x => x.OutcomePriorityCustomer.Equals(1));
                context.NcsSubmissions.Should().Contain(x => x.OutcomeType.Equals(1));
                context.NcsSubmissions.Should().Contain(x => x.SubContractorId.Equals("Subcontractor ID"));
                context.NcsSubmissions.Should().Contain(x => x.SessionDate.Equals(new DateTime(2019, 03, 01)));
                context.NcsSubmissions.Should().Contain(x => x.Ukprn.Equals(123456789));
                context.NcsSubmissions.Should().Contain(x => x.TouchpointId.Equals("9999999999"));
                context.NcsSubmissions.Should().Contain(x => x.DssJobId.Equals(new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab")));
                context.NcsSubmissions.Should().Contain(x => x.DssTimestamp.Equals(new DateTime(2019, 04, 01)));
                context.NcsSubmissions.Should().Contain(x => x.CollectionYear.Equals(1819));
            }

            using (var context = ncsContext())
            {
                await NewService(ncsContext, loggerMock.Object).PersistSubmissionAndFundingValuesAsync(submissionsSet2, fundingValuesSet2, ncsJobContextMessage.Object, CancellationToken.None);
            }

            using (var context = new NcsContext(options))
            {
                // Funding Values Assertion
                context.FundingValues.Should().NotBeEmpty();
                context.FundingValues.Should().ContainSingle();
                context.FundingValues.Should().Contain(x => x.Value.Equals(50));

                // Submission Assertion
                context.NcsSubmissions.Should().NotBeEmpty();
                context.NcsSubmissions.Should().ContainSingle();               
                context.NcsSubmissions.Should().Contain(x => x.DssJobId.Equals(new Guid("6d639b8f-d5e2-4732-b2c7-e390a8d233b1")));
            }
        }

        private List<NcsSubmission> GetNcsSubmissionSet1()
        {
            return new List<NcsSubmission>()
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
        }

        private List<NcsSubmission> GetNcsSubmissionSet2()
        {
            return new List<NcsSubmission>()
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
                    DssJobId = new Guid("6d639b8f-d5e2-4732-b2c7-e390a8d233b1"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1819
                }
            };
        }

        private List<FundingValue> GetFundingValuesSet1()
        {
            return new List<FundingValue>
            {
                new FundingValue()
                {
                    Ukprn = 123456789,
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
        }

        private List<FundingValue> GetFundingValuesSet2()
        {
            return new List<FundingValue>
            {
                new FundingValue()
                {
                    Ukprn = 123456789,
                    TouchpointId = "9999999999",
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomePriorityCustomer = 1,
                    Value = 50,
                    Period = "April",
                    CollectionYear = 1819
                }
            };
        }

        private PersistenceService NewService(Func<INcsContext> ncsContext = null, ILogger logger = null, IDateTimeProvider dateTimeProvider = null)
        {
            return new PersistenceService(ncsContext, logger, dateTimeProvider);
        }
    }
}
