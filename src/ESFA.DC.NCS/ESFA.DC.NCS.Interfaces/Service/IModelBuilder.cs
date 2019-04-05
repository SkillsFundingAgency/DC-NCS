using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.NCS.EF;
using ESFA.DC.NCS.Models;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IModelBuilder
    {
        IEnumerable<NcsSubmission> BuildNcsSubmission(IEnumerable<DssDataModel> dssData, INcsJobContextMessage ncsJobContextMessage);
    }
}
