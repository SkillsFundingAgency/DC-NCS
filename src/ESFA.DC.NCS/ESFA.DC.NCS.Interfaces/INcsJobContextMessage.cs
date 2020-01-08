﻿using System;

namespace ESFA.DC.NCS.Interfaces
{
    public interface INcsJobContextMessage
    {
        int Ukprn { get; }

        int JobId { get; }

        string Username { get; }

        Guid DssJobId { get; }

        string TouchpointId { get; }

        DateTime DssTimeStamp { get; }

        string ReportFileName { get; }

        string CollectionName { get; }

        int CollectionYear { get; }
    }
}
