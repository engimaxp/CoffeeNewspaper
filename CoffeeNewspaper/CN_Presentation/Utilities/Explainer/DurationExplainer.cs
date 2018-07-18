using System;
using CN_Core.Utilities;

namespace CN_Presentation.Utilities
{
    public class DurationExplainer : BaseDateTimeV2Explainer
    {
        protected override string FieldName { get; set; } = "value";

        protected override DateTime StringToDateTime(string time)
        {
            return Convert.ToDateTime(DateTime.Now.AddSeconds(Convert.ToDouble(time)));
        }

        protected override int StringToTimeRangeSecondsCount(string time)
        {
            throw new NotImplementedException();
        }

        protected override string GetTitle(string time)
        {
            return new TimeSpan(Convert.ToInt64(time) * 1000).GetTimeSpanLeftInfo();
        }
    }
}