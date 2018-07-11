using System;

namespace CN_Presentation.Utilities
{
    public class TimeRangeExplainer : BaseDateTimeV2Explainer
    {
        protected override string FieldName { get; set; } = "end";

        protected override DateTime StringToDateTime(string time)
        {
            return Convert.ToDateTime($"{DateTime.Now.ToShortDateString()} {time}");
        }
    }
}