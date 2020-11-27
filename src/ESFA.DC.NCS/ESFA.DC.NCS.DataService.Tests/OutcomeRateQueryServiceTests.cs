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
using FluentAssertions;
using Xunit;

namespace ESFA.DC.NCS.DataService.Tests
{
    public class OutcomeRateQueryServiceTests
    {

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public async Task GetOutcomeRateByPriorityAsync_Success(int priorityGroup)
        {
            IEnumerable<OutcomeRate> outcomeRates = new List<OutcomeRate>
            {
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 0,
                    CustomerSatisfaction = 10,
                    CareerManagement = 10,
                    Jobs = 20,
                    Learning = 20,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null
                },
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 1,
                    CustomerSatisfaction = 45,
                    CareerManagement = 50,
                    Jobs = 70,
                    Learning = 70,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null
                }
            };

            var ncsDbMock = outcomeRates.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(ors => ors.OutcomeRates).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var result = await NewService(ncsContext: ncsContext).GetOutcomeRatesByPriorityAsync(priorityGroup, CancellationToken.None);

            result.Should().NotBeNull();
            result.Count().Should().Be(1);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public async Task GetOutcomeRateByPriorityAndDeliveryAsync_SuccessMultipleRates(int priorityGroup)
        {
            IEnumerable<OutcomeRate> outcomeRates = new List<OutcomeRate>
            {
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 0,
                    CustomerSatisfaction = 10,
                    CareerManagement = 10,
                    Jobs = 20,
                    Learning = 20,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null
                },
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 1,
                    CustomerSatisfaction = 45,
                    CareerManagement = 50,
                    Jobs = 70,
                    Learning = 70,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null
                },
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 0,
                    CustomerSatisfaction = 10,
                    CareerManagement = 10,
                    Jobs = 20,
                    Learning = 20,
                    EffectiveFrom = new DateTime(2017, 10, 01),
                    EffectiveTo = new DateTime(2018, 09, 30)
                },
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 1,
                    CustomerSatisfaction = 45,
                    CareerManagement = 50,
                    Jobs = 70,
                    Learning = 70,
                    EffectiveFrom = new DateTime(2017, 10, 01),
                    EffectiveTo = new DateTime(2018, 09, 30)
                }
            };

            var ncsDbMock = outcomeRates.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(ors => ors.OutcomeRates).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var result = await NewService(ncsContext: ncsContext).GetOutcomeRatesByPriorityAsync(priorityGroup,CancellationToken.None);

            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetOutcomeRateByPriorityAndDeliveryAsync_NoResults()
        {
            var priorityGroup = 1;

            IEnumerable<OutcomeRate> outcomeRates = new List<OutcomeRate>
            {
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 2,
                    CustomerSatisfaction = 10,
                    CareerManagement = 10,
                    Jobs = 20,
                    Learning = 20,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null
                },
                new OutcomeRate()
                {
                    OutcomePriorityCustomer = 3,
                    CustomerSatisfaction = 45,
                    CareerManagement = 50,
                    Jobs = 70,
                    Learning = 70,
                    EffectiveFrom = new DateTime(2018, 10, 01),
                    EffectiveTo = null
                }
            };

            var ncsDbMock = outcomeRates.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(ors => ors.OutcomeRates).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            Func<Task> act = async () => await NewService(ncsContext: ncsContext).GetOutcomeRatesByPriorityAsync(priorityGroup, CancellationToken.None);

            act.Should()
                .Throw<Exception>()
                .WithMessage($"OutcomeRates table does not contain an outcome rate for the values: OutcomePriorityCustomer-{priorityGroup}");
        }

        private OutcomeRateQueryService NewService(Func<INcsContext> ncsContext = null)
        {
            return new OutcomeRateQueryService(ncsContext);
        }
    }
}

