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

        protected override long StringToTimeRangeSecondsCount(string time)
        {
            var newtime = Convert.ToDateTime($"{DateTime.Now.ToShortDateString()} {time}");
            if (newtime <= DateTime.Now)
            {
                newtime = newtime.AddDays(1);
            }
            return Convert.ToInt64((newtime - DateTime.Now).TotalSeconds);
        }
    }
}