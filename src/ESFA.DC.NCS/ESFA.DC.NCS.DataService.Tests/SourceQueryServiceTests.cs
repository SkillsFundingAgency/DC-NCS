using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.NCS.Interfaces;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ESFA.DC.NCS.DataService.Tests
{
    public class SourceQueryServiceTests
    {
        [Fact]
        public void GetLastNcsSubmissionDate_Null()
        {
            var touchpointId = "9999999999";
            var sources = new List<Source>
            {
                new Source()
                {
                    UKPRN = 123456789,
                    TouchpointId = "11111111111",
                    SubmissionDate = new DateTime(2019, 05, 01),
                    DssJobId = Guid.NewGuid(),
                    CreatedOn = DateTime.Now
                }
            };

            var ncsDbMock = sources.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(s => s.Sources).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);

            var result = NewService(ncsContext).GetLastNcsSubmissionDate(ncsJobContextMessage.Object, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public void GetLastNcsSubmissionDate_Success()
        {
            var touchpointId = "9999999999";
            var submissionDate = new DateTime(2019, 05, 01);
            var sources = new List<Source>
            {
                new Source()
                {
                    UKPRN = 123456789,
                    TouchpointId = touchpointId,
                    SubmissionDate = submissionDate,
                    DssJobId = Guid.NewGuid(),
                    CreatedOn = DateTime.Now
                }
            };

            var ncsDbMock = sources.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(s => s.Sources).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);

            var result = NewService(ncsContext).GetLastNcsSubmissionDate(ncsJobContextMessage.Object, CancellationToken.None);

            result.Should().Be(submissionDate);
        }

        [Fact]
        public void GetLastNcsSubmissionDate_SuccessLatest()
        {
            var touchpointId = "9999999999";
            var submissionDate = new DateTime(2019, 05, 01);
            var latestSubmissionDate = new DateTime(2019, 06, 01);
            var sources = new List<Source>
            {
                new Source()
                {
                    UKPRN = 123456789,
                    TouchpointId = touchpointId,
                    SubmissionDate = submissionDate,
                    DssJobId = Guid.NewGuid(),
                    CreatedOn = DateTime.Now
                },
                new Source()
                {
                    UKPRN = 123456789,
                    TouchpointId = touchpointId,
                    SubmissionDate = latestSubmissionDate,
                    DssJobId = Guid.NewGuid(),
                    CreatedOn = DateTime.Now
                }
            };

            var ncsDbMock = sources.AsQueryable().BuildMockDbSet();

            var contextMock = new Mock<INcsContext>();

            contextMock.Setup(s => s.Sources).Returns(ncsDbMock.Object);

            Func<INcsContext> ncsContext = () => contextMock.Object;

            var ncsJobContextMessage = new Mock<INcsJobContextMessage>();
            ncsJobContextMessage.Setup(m => m.TouchpointId).Returns(touchpointId);

            var result = NewService(ncsContext).GetLastNcsSubmissionDate(ncsJobContextMessage.Object, CancellationToken.None);

            result.Should().Be(latestSubmissionDate);
        }

        private SourceQueryService NewService(Func<INcsContext> ncsContext = null)
        {
            return new SourceQueryService(ncsContext);
        }
    }
}
