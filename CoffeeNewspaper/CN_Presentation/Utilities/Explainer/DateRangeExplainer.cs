using System;

namespace CN_Presentation.Utilities
{
    public class DateRangeExplainer : BaseDateTimeV2Explainer
    {
        protected override string FieldName { get; set; } = "end";

        protected override DateTime StringToDateTime(string time)
        {
            return Convert.ToDateTime(time);
        }

        protected override int StringToTimeRangeSecondsCount(string time)
        {
            throw new NotImplementedException();
        }
    }
}