using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;

namespace ESFA.DC.NCS.Service.Tasks
{
    public class StorageTask : INcsDataTask
    {
        public string TaskName => TaskNameConstants.StorageTaskName;

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
