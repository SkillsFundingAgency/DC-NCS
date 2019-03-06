using System;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.Service;

namespace ESFA.DC.NCS.Service.Helpers
{
    public class MessageHelper : IMessageHelper
    {
        // TODO: method should call off to a service to get collection dates rather than constants
        public DateTime CalculateFundingYearStart(DateTime messageSubmissionDate)
        {
            if (messageSubmissionDate >= CollectionYearConstants.CollectionYearStart1819 &&
                messageSubmissionDate < CollectionYearConstants.CollectionYearEnd1819)
            {
                return new DateTime(CollectionYearConstants.CollectionYearStart1819.Year, 04, 01);
            }

            if (messageSubmissionDate >= CollectionYearConstants.CollectionYearStart1920 &&
                messageSubmissionDate < CollectionYearConstants.CollectionYearEnd1920)
            {
                return new DateTime(CollectionYearConstants.CollectionYearStart1920.Year, 04, 01);
            }

            throw new Exception($"Submission date:{messageSubmissionDate} does not fall between any known collection year.");
        }
    }
}
