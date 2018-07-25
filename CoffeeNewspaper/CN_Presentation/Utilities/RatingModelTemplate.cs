using System.Diagnostics;
using CN_Presentation.ViewModel.Controls.Design;

namespace CN_Presentation.ViewModel.Controls
{
    public static class RatingModelTemplate
    {
        public static RatingViewModel GetNewModel(this RatingControlType controlType)
        {
            switch (controlType)
            {
                case RatingControlType.Priority:
                    return new RatingDesignModel2();
                case RatingControlType.Urgency:
                    return new RatingDesignModel();
                default:
                    Debugger.Break();
                    return null;
            }
        }
    }
}