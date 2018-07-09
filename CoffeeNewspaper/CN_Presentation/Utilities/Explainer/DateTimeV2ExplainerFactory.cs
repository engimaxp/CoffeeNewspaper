namespace CN_Presentation.Utilities
{
    public static class DateTimeV2ExplainerFactory
    {

        public static IDateTimeV2Explainer CreateExplaner(this string typeName)
        {
            switch (typeName)
            {
                case "datetimeV2.date": 
                    return new DateExplainer();
                case "datetimeV2.time":
                    return new TimeExplainer();
                case "datetimeV2.datetime":
                    return new DateTimeExplainer();
                case "datetimeV2.duration":
                    return new DurationExplainer();
                case "datetimeV2.daterange":
                    return new DateRangeExplainer();
                case "datetimeV2.timerange":
                    return new TimeRangeExplainer();
                case "datetimeV2.datetimerange":
                    return new DateTimeRangeExplainer();
                default:
                    return null;
            }
        }
    }
}