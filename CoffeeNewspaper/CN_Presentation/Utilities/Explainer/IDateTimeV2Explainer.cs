using System.Collections;
using System.Collections.Generic;
using CN_Presentation.Input;

namespace CN_Presentation.Utilities
{
    public interface IDateTimeV2Explainer
    {
        IEnumerable<DateTimeSuggestButtonViewModel> CreateDateTimeSuggestButtonViewModel(IEnumerable enumerable,DateTimeEntryViewModel parentModel);
    }
}