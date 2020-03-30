using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace ESFA.DC.NCS.EF.Console.DesignTime.Naming
{
    public class DataCandidateNamingService : CandidateNamingService
    {
        public override string GenerateCandidateIdentifier(DatabaseTable originalTable)
        {
            return originalTable.Name;
        }

        public override string GenerateCandidateIdentifier(DatabaseColumn originalColumn)
        {
            return originalColumn.Name;
        }
    }
}
