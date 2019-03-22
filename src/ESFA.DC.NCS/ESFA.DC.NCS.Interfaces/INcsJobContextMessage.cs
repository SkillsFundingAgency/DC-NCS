using System;

namespace ESFA.DC.NCS.Interfaces
{
    public interface INcsJobContextMessage
    {
        int Ukprn { get; }

        string Username { get; }

        Guid DssJobId { get; }

        string TouchpointId { get; }

        DateTime DssTimeStamp { get; }

        string DssContainer { get; }

        string ReportFileName { get; }

        string CollectionName { get; }

        int ReturnPeriod { get; }

        int CollectionYear { get; }
    }
}
