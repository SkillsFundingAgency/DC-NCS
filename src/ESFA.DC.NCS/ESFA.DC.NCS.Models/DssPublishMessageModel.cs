using System;

namespace ESFA.DC.NCS.Models
{
    [Serializable]
    public class DssPublishMessageModel
    {
        /// <summary>
        /// Unique job id of job.
        /// </summary>
        public Guid JobId { get; set; }


        /// <summary>
        /// The status of the job (expecting Success)
        /// </summary>
        public string Status { get; set; }

    }
}
