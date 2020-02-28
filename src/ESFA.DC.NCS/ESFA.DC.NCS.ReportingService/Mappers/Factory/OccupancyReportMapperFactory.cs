using CsvHelper.Configuration;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Service;
using ESFA.DC.NCS.Models.Reports;

namespace ESFA.DC.NCS.ReportingService.Mappers.Factory
{
    public class OccupancyReportMapperFactory : IClassMapFactory<OccupancyReportModel>
    {
        public ClassMap<OccupancyReportModel> Build(INcsJobContextMessage ncsJobContextMessage)
        {
            return new OccupancyReportMapper(ncsJobContextMessage.CollectionYear);
        }
    }
}
