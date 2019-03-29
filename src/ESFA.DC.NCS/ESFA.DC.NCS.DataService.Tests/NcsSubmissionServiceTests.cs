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
    public class NcsSubmissionServiceTests
    {
        [Fact]
        public async Task PersistAsync_SuccessSingle()
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
                    OutcomePriorityCustomer = 1,
                    OutcomeType = 1,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    Ukprn = 123456789,
                    TouchpointId = "000000001",
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1819
                }
            };

            var options = new DbContextOptionsBuilder<NcsContext>()
                .UseInMemoryDatabase(databaseName: "PersistAsync_SuccessSingle_NcsSubmission")
                .Options;

            using (var context = new NcsContext(options))
            {
                await NewService().PersistAsync(context, submissions, CancellationToken.None);
            }

            using (var context = new NcsContext(options))
            {
                context.NcsSubmissions.Should().NotBeEmpty();
                context.NcsSubmissions.Should().ContainSingle();                
                context.NcsSubmissions.Should().Contain(x => x.ActionPlanId.Equals(new Guid("d01b4976-bc3a-4731-9ef0-e41baa2f0619")));
                context.NcsSubmissions.Should().Contain(x => x.AdviserName.Equals("Adviser Name"));
                context.NcsSubmissions.Should().Contain(x => x.CustomerId.Equals(new Guid("dd4cdc23-486c-4ec6-aec6-39ff71207ce5")));
                context.NcsSubmissions.Should().Contain(x => x.DateOfBirth.Equals(new DateTime(2000, 01, 01)));
                context.NcsSubmissions.Should().Contain(x => x.HomePostCode.Equals("XXX XXX"));
                context.NcsSubmissions.Should().Contain(x => x.OutcomeEffectiveDate.Equals(new DateTime(2019, 04, 01)));
                context.NcsSubmissions.Should().Contain(x => x.OutcomeId.Equals(new Guid("854f9d3b-8729-4bee-82c2-fa7f24af5d50")));
                context.NcsSubmissions.Should().Contain(x => x.OutcomePriorityCustomer.Equals(1));
                context.NcsSubmissions.Should().Contain(x => x.OutcomeType.Equals(1));
                context.NcsSubmissions.Should().Contain(x => x.SubContractorId.Equals("Subcontractor ID"));
                context.NcsSubmissions.Should().Contain(x => x.SessionDate.Equals(new DateTime(2019, 03, 01)));
                context.NcsSubmissions.Should().Contain(x => x.Ukprn.Equals(123456789));
                context.NcsSubmissions.Should().Contain(x => x.TouchpointId.Equals("000000001"));
                context.NcsSubmissions.Should().Contain(x => x.DssJobId.Equals(new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab")));
                context.NcsSubmissions.Should().Contain(x => x.DssTimestamp.Equals(new DateTime(2019, 04, 01)));
                context.NcsSubmissions.Should().Contain(x => x.CollectionYear.Equals(1819));
            }
        }

        [Fact]
        public async Task PersistAsync_SuccessMultiple()
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
                    OutcomePriorityCustomer = 1,
                    OutcomeType = 1,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    Ukprn = 123456789,
                    TouchpointId = "000000001",
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1819
                },
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
                    OutcomeType = 1,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    Ukprn = 111111111,
                    TouchpointId = "000000001",
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1819
                }
            };

            var options = new DbContextOptionsBuilder<NcsContext>()
                .UseInMemoryDatabase(databaseName: "PersistAsync_SuccessMultiple_NcsSubmission")
                .Options;

            using (var context = new NcsContext(options))
            {
                await NewService().PersistAsync(context, submissions, CancellationToken.None);
            }

            using (var context = new NcsContext(options))
            {
                context.NcsSubmissions.Should().NotBeEmpty();
                context.NcsSubmissions.Should().HaveCount(submissions.Count);
            }
        }

        [Fact]
        public async Task DeleteByTouchpointAsync_SuccessSingle()
        {
            var touchpointId = "9999999999";

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
                    OutcomeType = 1,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    Ukprn = 123456789,
                    TouchpointId = touchpointId,
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1819
                }
            };

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);

            var options = new DbContextOptionsBuilder<NcsContext>()
                .UseInMemoryDatabase(databaseName: "DeleteByTouchpointAsync_SuccessSingle_NcsSubmission")
                .Options;

            using (var context = new NcsContext(options))
            {
                await NewService().PersistAsync(context, submissions, CancellationToken.None);
            }

            using (var context = new NcsContext(options))
            {
                context.NcsSubmissions.Should().NotBeEmpty();
                context.NcsSubmissions.Should().ContainSingle();

                await NewService().DeleteByTouchpointAsync(context, ncsJobContextMessage.Object, CancellationToken.None);

                context.NcsSubmissions.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task DeleteByTouchpointAsync_NoMatch()
        {
            var touchpointId = "9999999999";

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
                    OutcomeType = 1,
                    SubContractorId = "Subcontractor ID",
                    SessionDate = new DateTime(2019, 03, 01),
                    Ukprn = 123456789,
                    TouchpointId = "5",
                    DssJobId = new Guid("0d2c8ffe-0b54-4e44-b67c-e1d7915257ab"),
                    DssTimestamp = new DateTime(2019, 04, 01),
                    CollectionYear = 1819
                }
            };

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);

            var options = new DbContextOptionsBuilder<NcsContext>()
                .UseInMemoryDatabase(databaseName: "DeleteByTouchpointAsync_NoMatch_NcsSubmission")
                .Options;

            using (var context = new NcsContext(options))
            {
                await NewService().PersistAsync(context, submissions, CancellationToken.None);
            }

            using (var context = new NcsContext(options))
            {
                context.NcsSubmissions.Should().NotBeEmpty();
                context.NcsSubmissions.Should().ContainSingle();

                await NewService().DeleteByTouchpointAsync(context, ncsJobContextMessage.Object, CancellationToken.None);

                context.NcsSubmissions.Should().NotBeEmpty();
            }
        }

        private NcsSubmissionService NewService()
        {
            return new NcsSubmissionService();
        }
    }
}
