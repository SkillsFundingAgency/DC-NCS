using System;
using System.Collections.Generic;

namespace ESFA.DC.NCS.EF
{
    public partial class Source
    {
        public int SourceId { get; set; }
        public int Ukprn { get; set; }
        public string TouchpointId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public Guid DssJobId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
