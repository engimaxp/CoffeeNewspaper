using System;
using System.Collections;
using System.Collections.Generic;
using CN_Presentation.Input;

namespace CN_Presentation.Utilities
{
    public class TimeRangeExplainer : IDateTimeV2Explainer
    {
        public IEnumerable<DateTimeSuggestButtonViewModel> CreateDateTimeSuggestButtonViewModel(IEnumerable enumerable,DateTimeEntryViewModel parentModel)
        {
            var results = new List<DateTimeSuggestButtonViewModel>();
            if (enumerable == null) return results;
            foreach (var element in enumerable)
            {
                if (element is Dictionary<string, string> dict)
                {
                    var time = dict["end"];
                    results.Add(new DateTimeSuggestButtonViewModel
                    {
                        ValueDateTime = Convert.ToDateTime($"{DateTime.Now.ToShortDateString()} {time}"),
                        Title = time,
                        ParentModel = parentModel
                    });
                }
            }

            return results;
        }
    }
}