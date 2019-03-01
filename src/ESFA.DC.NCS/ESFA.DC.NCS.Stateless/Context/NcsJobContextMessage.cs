using System;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.NCS.Interfaces;

namespace ESFA.DC.NCS.Stateless.Context
{
    public class NcsJobContextMessage : INcsJobContextMessage
    {
        private readonly IJobContextMessage _jobContextMessage;

        public NcsJobContextMessage(IJobContextMessage jobContextMessage)
        {
            _jobContextMessage = jobContextMessage;
        }

        public int Ukprn => int.Parse(_jobContextMessage.KeyValuePairs[JobContextMessageKey.UkPrn].ToString());

        public string Username => _jobContextMessage.KeyValuePairs[JobContextMessageKey.Username].ToString();

        public Guid DssJobId => Guid.Parse(_jobContextMessage.KeyValuePairs["ExternalJobId"].ToString());

        public string TouchpointId => _jobContextMessage.KeyValuePairs["TouchpointId"].ToString();

        public DateTime DssTimeStamp => DateTime.Parse(_jobContextMessage.KeyValuePairs["ExternalTimestamp"].ToString());

        public string DssContainer => _jobContextMessage.KeyValuePairs[JobContextMessageKey.Container].ToString();

        public string ReportFileName => _jobContextMessage.KeyValuePairs[JobContextMessageKey.Filename].ToString();
    }
}
