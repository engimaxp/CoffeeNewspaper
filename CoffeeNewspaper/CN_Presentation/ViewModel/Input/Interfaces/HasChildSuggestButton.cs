using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CN_Presentation.Utilities;
using CN_Presentation.ViewModel.Base;
using Microsoft.Recognizers.Text;

namespace CN_Presentation.Input
{
    public abstract class HasChildSuggestButton<TChild>:BaseViewModel where TChild : BaseViewModel, new()
    {
        private const string Key_Values = "values";

        protected IEnumerable<TChild> CollectDateTimeSuggestButtonViewModels<TParent>(
            IEnumerable<ModelResult> suggestions, TParent parentModel) where TParent : HasChildSuggestButton<TChild>
        {
            var result = new List<TChild>();
            foreach (var modelResult in suggestions)
            {
                BaseDateTimeV2Explainer explainer = modelResult.TypeName.CreateExplaner();
                if (explainer != null && modelResult.Resolution != null && modelResult.Resolution.Count > 0)
                {
                    modelResult.Resolution.TryGetValue(Key_Values, out var resolutions);
                    if (resolutions is IEnumerable enumerable)
                    {
                        var btn = parentModel.CreateListUseModelLists(explainer.CreateDTOModelDateTime(enumerable, parentModel.GetTypeForValue()));
                        var suggestButtonViewModels = btn as TChild[] ?? btn.ToArray();
                        if (suggestButtonViewModels.Any())
                            result.AddRange(suggestButtonViewModels);
                    }
                }
            }

            return result;
        }

        protected abstract Type GetTypeForValue();
        protected abstract IEnumerable<TChild> CreateListUseModelLists(IEnumerable<SuggestDataDto> sdDtos);
    }
}