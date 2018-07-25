using System.Diagnostics;
using CN_Presentation.ViewModel.Controls.Design;

namespace CN_Presentation.ViewModel.Controls
{
    public static class RatingModelTemplate
    {
        public static RatingViewModel GetNewModel(this RatingControlType controlType,int selectedValue)
        {
            switch (controlType)
            {
                case RatingControlType.Priority:
                    return new RatingDesignModel2(selectedValue);
                case RatingControlType.Urgency:
                    return new RatingDesignModel(selectedValue);
                default:
                    Debugger.Break();
                    return null;
            }
        }
    }
}