using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CN_Presentation.Input;
using Microsoft.Recognizers.Text;

namespace CN_Presentation.Utilities
{
    public static class RecognizeTextExtensions
    {
        private const string Key_Values = "values";

        public static IEnumerable<DateTimeSuggestButtonViewModel> CollectDateTimeSuggestButtonViewModels(
            this IEnumerable<ModelResult> suggestions, DateTimeEntryViewModel parentModel)
        {
            var result = new List<DateTimeSuggestButtonViewModel>();
            foreach (var modelResult in suggestions)
            {
                var explainer = modelResult.TypeName.CreateExplaner();
                if (explainer!=null && modelResult.Resolution != null && modelResult.Resolution.Count > 0)
                {
                    modelResult.Resolution.TryGetValue(Key_Values, out var resolutions);
                    if (resolutions is IEnumerable enumerable)
                    {
                        var btn = explainer.CreateDateTimeSuggestButtonViewModel(enumerable,parentModel);
                        var dateTimeSuggestButtonViewModels = btn as DateTimeSuggestButtonViewModel[] ?? btn.ToArray();
                        if(dateTimeSuggestButtonViewModels.Any())
                            result.AddRange(dateTimeSuggestButtonViewModels);
                    }
                }
            }

            return result;
        }
    }
}