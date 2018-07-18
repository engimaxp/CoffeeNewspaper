using System;

namespace CN_Presentation.Utilities
{
    public class TimeExplainer : BaseDateTimeV2Explainer
    {
        protected override string FieldName { get; set; } = "value";

        protected override DateTime StringToDateTime(string time)
        {
            return Convert.ToDateTime($"{DateTime.Now.ToShortDateString()} {time}");
        }

        protected override int StringToTimeRangeSecondsCount(string time)
        {
            throw new NotImplementedException();
        }
    }
}