using System.Diagnostics;
using CN_Presentation.ViewModel.Controls.Design;

namespace CN_Presentation.ViewModel.Controls
{
    public static class RatingModelTemplate
    {
        /// <summary>
        /// Generate a New Model
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="selectedValue"></param>
        /// <param name="rateChangedEvent">child button click will fire the event</param>
        /// <returns></returns>
        public static RatingViewModel GetNewModel(this RatingControlType controlType,int selectedValue, RateChangedSubsriber rateChangedEvent = null)
        {
            switch (controlType)
            {
                case RatingControlType.Priority:
                    return new RatingDesignModel2(selectedValue, rateChangedEvent);
                case RatingControlType.Urgency:
                    return new RatingDesignModel(selectedValue, rateChangedEvent);
                default:
                    Debugger.Break();
                    return null;
            }
        }
    }
}