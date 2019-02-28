using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;

namespace ESFA.DC.NCS.Service.Tasks
{
    public class ReportingTask : INcsDataTask
    {
        public string TaskName => TaskNameConstants.ReportingTaskName;

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
