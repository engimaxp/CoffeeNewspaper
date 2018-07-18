using System;
using System.Collections;
using System.Collections.Generic;

namespace CN_Presentation.Utilities
{
    public abstract class BaseDateTimeV2Explainer
    {
        protected abstract string FieldName { get; set; }

        protected abstract DateTime StringToDateTime(string time);

        protected abstract int StringToTimeRangeSecondsCount(string time);

        protected virtual string GetTitle(string time)
        {
            return time;
        }

        public IEnumerable<SuggestDataDto> CreateDTOModelDateTime(IEnumerable enumerable,Type valueType)
        {

            var results = new List<SuggestDataDto>();
            if (enumerable == null) return results;
            foreach (var element in enumerable)
            {
                if (element is Dictionary<string, string> dict)
                {
                    var time = dict[FieldName];
                    results.Add(new SuggestDataDto
                    {
                        Value = ValueConverter(time,valueType),
                        Title = GetTitle(time),
                    });
                }
            }
            return results;
        }
        private object ValueConverter(string value,Type valueType)
        {
            if (valueType == typeof(DateTime))
            {
                return StringToDateTime(value);
            }
            else
            {
                return StringToTimeRangeSecondsCount(value);
            }
        }
    }
}