using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.Interfaces;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ESFA.DC.NCS.DataService.Tests
{
    public class FundingValueServiceTests
    {
        [Fact]
        public async Task PersistAsync_SuccessSingle()
        {
            var fundingValues = new List<FundingValue>
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
                    Period = "April"
                }
            };

            var options = new DbContextOptionsBuilder<NcsContext>()
                .UseInMemoryDatabase(databaseName: "PersistAsync_SuccessSingle_FundingValues")
                .Options;

            using (var context = new NcsContext(options))
            {
                await NewService().PersistAsync(context, fundingValues, CancellationToken.None);
            }

            using (var context = new NcsContext(options))
            {
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
            }
        }

        [Fact]
        public async Task PersistAsync_SuccessMultiple()
        {
            var fundingValues = new List<FundingValue>
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
                    Period = "April"
                },
                new FundingValue()
                {
                    Ukprn = 123456789,
                    TouchpointId = "0000000000",
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomePriorityCustomer = 1,
                    Value = 10,
                    Period = "April"
                }
            };

            var options = new DbContextOptionsBuilder<NcsContext>()
                .UseInMemoryDatabase(databaseName: "PersistAsync_SuccessMultiple_FundingValues")
                .Options;

            using (var context = new NcsContext(options))
            {
                await NewService().PersistAsync(context, fundingValues, CancellationToken.None);
            }

            using (var context = new NcsContext(options))
            {
                context.FundingValues.Should().NotBeEmpty();
                context.FundingValues.Should().HaveCount(fundingValues.Count);
            }
        }

        [Fact]
        public async Task DeleteByTouchpointAsync_SuccessSingle()
        {
            var touchpointId = "9999999999";

            var fundingValues = new List<FundingValue>
            {
                new FundingValue()
                {
                    Ukprn = 123456789,
                    TouchpointId = touchpointId,
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomePriorityCustomer = 1,
                    Value = 10,
                    Period = "April"
                }
            };

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);

            var options = new DbContextOptionsBuilder<NcsContext>()
                .UseInMemoryDatabase(databaseName: "DeleteByTouchpointAsync_SuccessSingle_FundingValues")
                .Options;

            using (var context = new NcsContext(options))
            {
                await NewService().PersistAsync(context, fundingValues, CancellationToken.None);
            }

            using (var context = new NcsContext(options))
            {
                context.FundingValues.Should().NotBeEmpty();
                context.FundingValues.Should().ContainSingle();

                await NewService().DeleteByTouchpointAsync(context, ncsJobContextMessage.Object, CancellationToken.None);

                context.FundingValues.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task DeleteByTouchpointAsync_NoMatch()
        {
            var touchpointId = "9999999999";

            var fundingValues = new List<FundingValue>
            {
                new FundingValue()
                {
                    Ukprn = 123456789,
                    TouchpointId = "5",
                    CustomerId = new Guid("5ef69cda-1193-4c80-8427-e79544ddb46e"),
                    ActionPlanId = new Guid("1e35c9f6-39b2-4620-87d6-9dcf276b37e7"),
                    OutcomeId = new Guid("91b85df7-6313-4b56-bce0-80133a7b2559"),
                    OutcomeType = 1,
                    OutcomeEffectiveDate = new DateTime(2019, 04, 01),
                    OutcomePriorityCustomer = 1,
                    Value = 10,
                    Period = "April"
                }
            };

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);

            var options = new DbContextOptionsBuilder<NcsContext>()
                .UseInMemoryDatabase(databaseName: "DeleteByTouchpointAsync_NoMatch_FundingValues")
                .Options;

            using (var context = new NcsContext(options))
            {
                await NewService().PersistAsync(context, fundingValues, CancellationToken.None);
            }

            using (var context = new NcsContext(options))
            {
                context.FundingValues.Should().NotBeEmpty();
                context.FundingValues.Should().ContainSingle();

                await NewService().DeleteByTouchpointAsync(context, ncsJobContextMessage.Object, CancellationToken.None);

                context.FundingValues.Should().NotBeEmpty();
            }
        }

        private FundingValueService NewService()
        {
            return new FundingValueService();
        }
    }
}
