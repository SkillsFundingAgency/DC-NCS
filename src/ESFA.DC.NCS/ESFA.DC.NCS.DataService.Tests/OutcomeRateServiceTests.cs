using ESFA.DC.Logging.Interfaces;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.Interfaces.Constants;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.NCS.DataService.Tests
{
    public class OutcomeRateServiceTests
    {
        [Theory]
        [InlineData(1, OutcomeRatesConstants.Community)]
        [InlineData(0, OutcomeRatesConstants.Community)]
        public async Task GetOutcomeRateByPriorityAndDeliveryAsync_Success(int priorityGroup, string delivery)
        {
            var submissionDate = new DateTime(2018, 12, 01);

            IEnumerable<OutcomeRate> outcomeRates = new List<OutcomeRate>
            {
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 0,
                    CustomerSatisfaction = 10,
                    CareerManagement = 10,
                    JobsAndLearning = 20,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null,
                    Delivery = OutcomeRatesConstants.Community
                },
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 1,
                    CustomerSatisfaction = 45,
                    CareerManagement = 50,
                    JobsAndLearning = 70,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null,
                    Delivery = OutcomeRatesConstants.Community
                }
            };

            var ncsDbMock = outcomeRates.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(ors => ors.OutcomeRates).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var result = await NewService(ncsContext: ncsContext).GetOutcomeRateByPriorityAndDeliveryAsync(priorityGroup, delivery, submissionDate, CancellationToken.None);

            result.Should().NotBeNull();
            result.OutcomePriorityCustomer.Should().Be(priorityGroup);
            result.Delivery.Should().Be(delivery);
        }

        [Fact]
        public async Task GetOutcomeRateByPriorityAndDeliveryAsync_SuccessWithExpiredOutcomes()
        {
            var priorityGroup = 1;
            var delivery = OutcomeRatesConstants.Community;
            var submissionDate = new DateTime(2018, 12, 01);

            IEnumerable<OutcomeRate> outcomeRates = new List<OutcomeRate>
            {
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 0,
                    CustomerSatisfaction = 10,
                    CareerManagement = 10,
                    JobsAndLearning = 20,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null,
                    Delivery = OutcomeRatesConstants.Community
                },
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 1,
                    CustomerSatisfaction = 45,
                    CareerManagement = 50,
                    JobsAndLearning = 70,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null,
                    Delivery = OutcomeRatesConstants.Community
                },
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 0,
                    CustomerSatisfaction = 10,
                    CareerManagement = 10,
                    JobsAndLearning = 20,
                    EffectiveFrom = new DateTime(2017, 10, 01),
                    EffectiveTo = new DateTime(2018, 09, 30),
                    Delivery = OutcomeRatesConstants.Community
                },
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 1,
                    CustomerSatisfaction = 45,
                    CareerManagement = 50,
                    JobsAndLearning = 70,
                    EffectiveFrom = new DateTime(2017, 10, 01),
                    EffectiveTo = new DateTime(2018, 09, 30),
                    Delivery = OutcomeRatesConstants.Community
                }
            };

            var ncsDbMock = outcomeRates.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(ors => ors.OutcomeRates).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var result = await NewService(ncsContext: ncsContext).GetOutcomeRateByPriorityAndDeliveryAsync(priorityGroup, delivery, submissionDate, CancellationToken.None);

            result.Should().NotBeNull();
            result.OutcomePriorityCustomer.Should().Be(priorityGroup);
            result.Delivery.Should().Be(delivery);
            result.CustomerSatisfaction.Should().Be(45);
            result.CareerManagement.Should().Be(50);
            result.JobsAndLearning.Should().Be(70);
        }

        [Fact]
        public async Task GetOutcomeRateByPriorityAndDeliveryAsync_NoResults()
        {
            var priorityGroup = 1;
            var delivery = OutcomeRatesConstants.Community;
            var submissionDate = new DateTime(2018, 09, 01);

            IEnumerable<OutcomeRate> outcomeRates = new List<OutcomeRate>
            {
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 0,
                    CustomerSatisfaction = 10,
                    CareerManagement = 10,
                    JobsAndLearning = 20,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null,
                    Delivery = OutcomeRatesConstants.Community
                },
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 1,
                    CustomerSatisfaction = 45,
                    CareerManagement = 50,
                    JobsAndLearning = 70,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null,
                    Delivery = OutcomeRatesConstants.Community
                }
            };

            var ncsDbMock = outcomeRates.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(ors => ors.OutcomeRates).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            Func<Task> act = async () => await NewService(ncsContext: ncsContext).GetOutcomeRateByPriorityAndDeliveryAsync(priorityGroup, delivery, submissionDate, CancellationToken.None);

            act.Should()
                .Throw<Exception>()
                .WithMessage($"OutcomeRates table does not contain an outcome rate for the values: OutcomePriorityCustomer-{priorityGroup}, Delivery-{delivery} and date-{submissionDate}");
        }

        [Fact]
        public async Task GetOutcomeRateByPriorityAndDeliveryAsync_MoreThanOneOutcomeReturned()
        {
            var priorityGroup = 1;
            var delivery = OutcomeRatesConstants.Community;
            var submissionDate = new DateTime(2018, 12, 01);

            IEnumerable<OutcomeRate> outcomeRates = new List<OutcomeRate>
            {
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 0,
                    CustomerSatisfaction = 10,
                    CareerManagement = 10,
                    JobsAndLearning = 20,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null,
                    Delivery = OutcomeRatesConstants.Community
                },
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 1,
                    CustomerSatisfaction = 45,
                    CareerManagement = 50,
                    JobsAndLearning = 70,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null,
                    Delivery = OutcomeRatesConstants.Community
                },
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 1,
                    CustomerSatisfaction = 60,
                    CareerManagement = 90,
                    JobsAndLearning = 80,
                    EffectiveFrom = new DateTime(2018, 11, 01),
                    EffectiveTo = null,
                    Delivery = OutcomeRatesConstants.Community
                }
            };

            var ncsDbMock = outcomeRates.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(ors => ors.OutcomeRates).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            Func<Task> act = async () => await NewService(ncsContext: ncsContext).GetOutcomeRateByPriorityAndDeliveryAsync(priorityGroup, delivery, submissionDate, CancellationToken.None);

            act.Should()
                .Throw<Exception>()
                .WithMessage($"OutcomeRates table contains more than one rate for the values: OutcomePriorityCustomer-{priorityGroup}, Delivery-{delivery} and date-{submissionDate}");
        }

        private OutcomeRateService NewService(ILogger logger = null, Func<INcsContext> ncsContext = null)
        {
            return new OutcomeRateService(logger, ncsContext);
        }
    }
}

