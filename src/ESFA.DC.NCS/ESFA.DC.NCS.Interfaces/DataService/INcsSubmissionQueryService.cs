﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.EF;

namespace ESFA.DC.NCS.Interfaces.DataService
{
    public interface INcsSubmissionQueryService
    {
        Task<IEnumerable<NcsSubmission>> GetNcsSubmissionsAsync(INcsJobContextMessage ncsJobContextMessage, CancellationToken cancellationToken);
    }
}
