﻿using System;

namespace ESFA.DC.NCS.Interfaces.Service
{
    public interface IMessageHelper
    {
        DateTime CalculateFundingYearStart(DateTime submissionDate);
    }
}
