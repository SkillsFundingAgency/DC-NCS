using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.ReportingService.Mappers.Factory
{
    public class OccupancyReportMapperFactory : IClassMapFactory<OccupancyReportMapper, OccupancyReportModel>
    {
        public OccupancyReportMapper Build(INcsJobContextMessage ncsJobContextMessage)
        {
            return new OccupancyReportMapper(ncsJobContextMessage.CollectionYear);
        }
    }
}
