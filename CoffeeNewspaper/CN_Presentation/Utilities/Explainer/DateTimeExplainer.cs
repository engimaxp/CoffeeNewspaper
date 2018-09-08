using System;

namespace CN_Presentation.Utilities
{
    public class DateTimeExplainer : BaseDateTimeV2Explainer
    {
        protected override string FieldName { get; set; } = "value";

        protected override DateTime StringToDateTime(string time)
        {
            return Convert.ToDateTime(time);
        }

        protected override long StringToTimeRangeSecondsCount(string time)
        {
            var timeForThis = Convert.ToDateTime(time);
            if (timeForThis <= DateTime.Now) throw new ArgumentException("ignoreable,dump time which less than now");
            return Convert.ToInt64((timeForThis - DateTime.Now).TotalSeconds);
        }
    }
}