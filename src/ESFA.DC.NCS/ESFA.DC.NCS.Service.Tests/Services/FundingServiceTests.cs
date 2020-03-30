using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.DataService;
using ESFA.DC.NCS.Service.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.NCS.Service.Tests.Services
{
    public class FundingServiceTests
    {
        [Theory]
        [InlineData(1, 45)]
        [InlineData(2, 50)]
        [InlineData(3, 70)]
        [InlineData(4, 70)]
        [InlineData(5, 70)]
        public void CalculateFunding_PriorityRate(int outcomeType, int expectedRate)
        {
            var submissions = new List<NcsSubmission>()
            {
                new NcsSubmission()
                {
                    ActionPlanId = new Guid("d01b4976-bc3a-4731-9ef0-e41baa2f0619"),
                    AdviserName = "Adviser Name",
                    CustomerId = new Guid("dd4cdc23-486c-4ec6-aec6-39ff71207ce5"),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomeId = new Guid("854f9d3b-8729-4bee-82c2-fa7f24af5d50"),
                    OutcomePriorityCustomer = OutcomeRatesConstants.Priority,
                    OutcomeType = outcomeType,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    UKPRN = 123456789,
                    TouchpointId = "000000001",
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1819
                }
            };

            var priorityOutcome = GetPriorityRate();
            var nonPriorityOutcome = GetNonPriorityRate();

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.DssTimeStamp).Returns(new DateTime(2019, 04, 01));

            var outcomeRateServiceMock = new Mock<IOutcomeRateQueryService>();

            outcomeRateServiceMock.Setup(orsm => orsm.GetOutcomeRateByPriorityAndDeliveryAsync(OutcomeRatesConstants.Priority, OutcomeRatesConstants.Community, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(priorityOutcome);

            outcomeRateServiceMock.Setup(orsm => orsm.GetOutcomeRateByPriorityAndDeliveryAsync(OutcomeRatesConstants.NonPriority, OutcomeRatesConstants.Community, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(nonPriorityOutcome);

            var dateTimeProviderServiceMock = new Mock<IDateTimeProvider>();
            dateTimeProviderServiceMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2000, 01, 01));

            var fundingValues = NewService(outcomeRateServiceMock.Object, dateTimeProviderServiceMock.Object).CalculateFundingAsync(submissions, ncsJobContextMessage.Object, CancellationToken.None).Result;

            fundingValues.Should().NotBeEmpty();
            fundingValues.Should().ContainSingle();
            fundingValues.Should().Contain(x => x.Value.Equals(expectedRate));
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 10)]
        [InlineData(3, 20)]
        [InlineData(4, 20)]
        [InlineData(5, 20)]
        public void CalculateFunding_NonPriorityRate(int outcomeType, int expectedRate)
        {
            var submissions = new List<NcsSubmission>()
            {
                new NcsSubmission()
                {
                    ActionPlanId = new Guid("d01b4976-bc3a-4731-9ef0-e41baa2f0619"),
                    AdviserName = "Adviser Name",
                    CustomerId = new Guid("dd4cdc23-486c-4ec6-aec6-39ff71207ce5"),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomeId = new Guid("854f9d3b-8729-4bee-82c2-fa7f24af5d50"),
                    OutcomePriorityCustomer = OutcomeRatesConstants.NonPriority,
                    OutcomeType = outcomeType,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    UKPRN = 123456789,
                    TouchpointId = "000000001",
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1819
                }
            };

            var priorityOutcome = GetPriorityRate();
            var nonPriorityOutcome = GetNonPriorityRate();

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.DssTimeStamp).Returns(new DateTime(2019, 04, 01));

            var outcomeRateServiceMock = new Mock<IOutcomeRateQueryService>();

            outcomeRateServiceMock.Setup(orsm => orsm.GetOutcomeRateByPriorityAndDeliveryAsync(OutcomeRatesConstants.Priority, OutcomeRatesConstants.Community, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(priorityOutcome);

            outcomeRateServiceMock.Setup(orsm => orsm.GetOutcomeRateByPriorityAndDeliveryAsync(OutcomeRatesConstants.NonPriority, OutcomeRatesConstants.Community, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(nonPriorityOutcome);

            var dateTimeProviderServiceMock = new Mock<IDateTimeProvider>();
            dateTimeProviderServiceMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2000, 01, 01));

            var fundingValues = NewService(outcomeRateServiceMock.Object, dateTimeProviderServiceMock.Object).CalculateFundingAsync(submissions, ncsJobContextMessage.Object, CancellationToken.None).Result;

            fundingValues.Should().NotBeEmpty();
            fundingValues.Should().ContainSingle();
            fundingValues.Should().Contain(x => x.Value.Equals(expectedRate));
        }

        [Fact]
        public void CalculateFunding_PriorityRateInvalid()
        {
            var outcomePriority = 5;

            var submissions = new List<NcsSubmission>()
            {
                new NcsSubmission()
                {
                    ActionPlanId = new Guid("d01b4976-bc3a-4731-9ef0-e41baa2f0619"),
                    AdviserName = "Adviser Name",
                    CustomerId = new Guid("dd4cdc23-486c-4ec6-aec6-39ff71207ce5"),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomeId = new Guid("854f9d3b-8729-4bee-82c2-fa7f24af5d50"),
                    OutcomePriorityCustomer = outcomePriority,
                    OutcomeType = 1,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    UKPRN = 123456789,
                    TouchpointId = "000000001",
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1819
                }
            };

            var priorityOutcome = GetPriorityRate();
            var nonPriorityOutcome = GetNonPriorityRate();

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.DssTimeStamp).Returns(new DateTime(2019, 04, 01));

            var outcomeRateServiceMock = new Mock<IOutcomeRateQueryService>();

            outcomeRateServiceMock.Setup(orsm => orsm.GetOutcomeRateByPriorityAndDeliveryAsync(OutcomeRatesConstants.Priority, OutcomeRatesConstants.Community, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(priorityOutcome);

            outcomeRateServiceMock.Setup(orsm => orsm.GetOutcomeRateByPriorityAndDeliveryAsync(OutcomeRatesConstants.NonPriority, OutcomeRatesConstants.Community, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(nonPriorityOutcome);

            var dateTimeProviderServiceMock = new Mock<IDateTimeProvider>();
            dateTimeProviderServiceMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2000, 01, 01));

            Func<Task> act = async () => await NewService(outcomeRateServiceMock.Object, dateTimeProviderServiceMock.Object).CalculateFundingAsync(submissions, ncsJobContextMessage.Object, CancellationToken.None);

            act.Should()
                .Throw<Exception>()
                .WithMessage($"The outcome priority customer {outcomePriority}, is not a valid value");
        }

        [Fact]
        public void CalculateFunding_OutcomeTypeInvalid()
        {
            var outcomeType = 0;

            var submissions = new List<NcsSubmission>()
            {
                new NcsSubmission()
                {
                    ActionPlanId = new Guid("d01b4976-bc3a-4731-9ef0-e41baa2f0619"),
                    AdviserName = "Adviser Name",
                    CustomerId = new Guid("dd4cdc23-486c-4ec6-aec6-39ff71207ce5"),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomeId = new Guid("854f9d3b-8729-4bee-82c2-fa7f24af5d50"),
                    OutcomePriorityCustomer = 1,
                    OutcomeType = outcomeType,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    UKPRN = 123456789,
                    TouchpointId = "000000001",
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1819
                }
            };

            var priorityOutcome = GetPriorityRate();
            var nonPriorityOutcome = GetNonPriorityRate();

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.DssTimeStamp).Returns(new DateTime(2019, 04, 01));

            var outcomeRateServiceMock = new Mock<IOutcomeRateQueryService>();

            outcomeRateServiceMock.Setup(orsm => orsm.GetOutcomeRateByPriorityAndDeliveryAsync(OutcomeRatesConstants.Priority, OutcomeRatesConstants.Community, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(priorityOutcome);

            outcomeRateServiceMock.Setup(orsm => orsm.GetOutcomeRateByPriorityAndDeliveryAsync(OutcomeRatesConstants.NonPriority, OutcomeRatesConstants.Community, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(nonPriorityOutcome);

            var dateTimeProviderServiceMock = new Mock<IDateTimeProvider>();
            dateTimeProviderServiceMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2000, 01, 01));

            Func<Task> act = async () => await NewService(outcomeRateServiceMock.Object, dateTimeProviderServiceMock.Object).CalculateFundingAsync(submissions, ncsJobContextMessage.Object, CancellationToken.None);

            act.Should()
                .Throw<Exception>()
                .WithMessage($"The outcome type {outcomeType}, doesn't correspond with an outcome rate");
        }

        [Theory]
        [InlineData("2019/01/01", "January")]
        [InlineData("2019/02/01", "February")]
        [InlineData("2019/03/01", "March")]
        [InlineData("2019/04/01", "April")]
        [InlineData("2019/05/01", "May")]
        [InlineData("2019/06/01", "June")]
        [InlineData("2019/07/01", "July")]
        [InlineData("2019/08/01", "August")]
        [InlineData("2019/09/01", "September")]
        [InlineData("2019/10/01", "October")]
        [InlineData("2019/11/01", "November")]
        [InlineData("2019/12/01", "December")]
        public void CalculateFunding_Periods(string outcomeDate, string expectedPeriod)
        {
            var outcomeEffectiveDate = DateTime.Parse(outcomeDate);

            var submissions = new List<NcsSubmission>()
            {
                new NcsSubmission()
                {
                    ActionPlanId = new Guid("d01b4976-bc3a-4731-9ef0-e41baa2f0619"),
                    AdviserName = "Adviser Name",
                    CustomerId = new Guid("dd4cdc23-486c-4ec6-aec6-39ff71207ce5"),
                    DateOfBirth = new DateTime(2000, 01, 01),
                    HomePostCode = "XXX XXX",
                    OutcomeEffectiveDate = outcomeEffectiveDate,
                    OutcomeId = new Guid("854f9d3b-8729-4bee-82c2-fa7f24af5d50"),
                    OutcomePriorityCustomer = OutcomeRatesConstants.NonPriority,
                    OutcomeType = 1,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    UKPRN = 123456789,
                    TouchpointId = "000000001",
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1819
                }
            };

            var priorityOutcome = GetPriorityRate();
            var nonPriorityOutcome = GetNonPriorityRate();

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.DssTimeStamp).Returns(new DateTime(2019, 04, 01));

            var outcomeRateServiceMock = new Mock<IOutcomeRateQueryService>();

            outcomeRateServiceMock.Setup(orsm => orsm.GetOutcomeRateByPriorityAndDeliveryAsync(OutcomeRatesConstants.Priority, OutcomeRatesConstants.Community, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(priorityOutcome);

            outcomeRateServiceMock.Setup(orsm => orsm.GetOutcomeRateByPriorityAndDeliveryAsync(OutcomeRatesConstants.NonPriority, OutcomeRatesConstants.Community, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(nonPriorityOutcome);

            var dateTimeProviderServiceMock = new Mock<IDateTimeProvider>();
            dateTimeProviderServiceMock.Setup(x => x.GetNowUtc()).Returns(new DateTime(2000, 01, 01));

            var fundingValues = NewService(outcomeRateServiceMock.Object, dateTimeProviderServiceMock.Object).CalculateFundingAsync(submissions, ncsJobContextMessage.Object, CancellationToken.None).Result;

            fundingValues.Should().NotBeEmpty();
            fundingValues.Should().ContainSingle();
            fundingValues.Should().Contain(x => x.Period.Equals(expectedPeriod));
        }

        private OutcomeRate GetPriorityRate()
        {
            return new OutcomeRate()
            {
                OutcomePriorityCustomer = OutcomeRatesConstants.Priority,
                CustomerSatisfaction = 45,
                CareerManagement = 50,
                JobsAndLearning = 70,
                EffectiveFrom = new DateTime(2018, 10, 01)
            };
        }

        private OutcomeRate GetNonPriorityRate()
        {
            return new OutcomeRate()
            {
                OutcomePriorityCustomer = OutcomeRatesConstants.NonPriority,
                CustomerSatisfaction = 10,
                CareerManagement = 10,
                JobsAndLearning = 20,
                EffectiveFrom = new DateTime(2018, 10, 01)
            };
        }

        private FundingService NewService(IOutcomeRateQueryService outcomeRateService = null, IDateTimeProvider dateTimeProvider = null)
        {
            return new FundingService(outcomeRateService, dateTimeProvider);
        }
    }
}
