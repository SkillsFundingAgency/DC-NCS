using System;
using System.Collections.Generic;

namespace ESFA.DC.NCS.EF
{
    public partial class FamFunding
    {
        public int Id { get; set; }
        public string TouchpointId { get; set; }
        public string Area { get; set; }
        public string PrimeContractor { get; set; }
        public int UKPRN { get; set; }
        public decimal? April { get; set; }
        public decimal? May { get; set; }
        public decimal? June { get; set; }
        public decimal? July { get; set; }
        public decimal? August { get; set; }
        public decimal? September { get; set; }
        public decimal? October { get; set; }
        public decimal? November { get; set; }
        public decimal? December { get; set; }
        public decimal? January { get; set; }
        public decimal? February { get; set; }
        public decimal? March { get; set; }
        public int CollectionYear { get; set; }
    }
}
