using System;

namespace CN_Presentation.Utilities
{
    public class DurationExplainer : BaseDateTimeV2Explainer
    {
        protected override string FieldName { get; set; } = "value";

        protected override DateTime StringToDateTime(string time)
        {
            return Convert.ToDateTime(DateTime.Now.AddSeconds(Convert.ToDouble(time)));
        }
    }
}