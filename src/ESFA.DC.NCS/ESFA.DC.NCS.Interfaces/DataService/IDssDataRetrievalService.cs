using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.NCS.Models;

namespace ESFA.DC.NCS.Interfaces.DataService
{
    public interface IDssDataRetrievalService
    {
        Task<IEnumerable<DssDataModel>> GetDataForTouchpoint(string touchpointId, DateTime dssSubmissionDateTime, DateTime fundingYearStart);
    }
}
