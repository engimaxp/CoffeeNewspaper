﻿using System;
using System.Collections;
using System.Collections.Generic;
using CN_Presentation.Input;
using Newtonsoft.Json;

namespace CN_Presentation.Utilities
{
    public class DateTimeExplainer : IDateTimeV2Explainer
    {
        public IEnumerable<DateTimeSuggestButtonViewModel> CreateDateTimeSuggestButtonViewModel(IEnumerable enumerable,DateTimeEntryViewModel parentModel)
        {
            var results = new List<DateTimeSuggestButtonViewModel>();
            if (enumerable == null) return results;
            foreach (var element in enumerable)
            {
                if (element is Dictionary<string, string> dict)
                {
                    var time = dict["value"];
                    results.Add(new DateTimeSuggestButtonViewModel
                    {
                        ValueDateTime = Convert.ToDateTime(time),
                        Title = time,
                        ParentModel = parentModel
                    });
                }
            }

            return results;
        }
    }
}