using System;
using ESFA.DC.NCS.Interfaces;
using ESFA.DC.NCS.Interfaces.Constants;
using ESFA.DC.NCS.Interfaces.Service;

namespace ESFA.DC.NCS.Service.Helpers
{
    public class MessageHelper : IMessageHelper
    {
        public DateTime CalculateFundingYearStart(INcsJobContextMessage ncsJobContextMessage)
        {
            if (ncsJobContextMessage.CollectionYear == CollectionYearConstants.CollectionYear1920)
            {
                return new DateTime(2019, 04, 01);
            }

            if (ncsJobContextMessage.CollectionYear == CollectionYearConstants.CollectionYear2021)
            {
                return new DateTime(2020, 04, 01);
            }

            throw new Exception($"Collection year:{ncsJobContextMessage.CollectionYear} unknown.");
        }
    }
}
