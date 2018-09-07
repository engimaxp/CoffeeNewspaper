using System;
using System.Threading.Tasks;
using CN_Core;
using CN_Presentation.Utilities;

namespace CN_Presentation.ViewModel.Controls
{
    public abstract class BubbleSortButtonBase 
    {
        protected abstract Func<CNTask, CNTask, int> compareFunc { get; set; }
        protected abstract FontFamilyType IconFontFamily { get; set; }
        
        protected abstract IconType IconFontText { get; set; }
        protected abstract string ButtonText { get; set; }
        protected abstract bool IsSelected { get; set; }


        public BubbleMenuButtonViewModel GetButton(Action<BubbleMenuButtonViewModel,Func<CNTask, CNTask, int>> callbackAction)
        {
            return new BubbleMenuButtonViewModel()
            {
                IconFontFamily = this.IconFontFamily,
                IconFontText = this.IconFontText,
                ButtonText = this.ButtonText,
                IsSelected = this.IsSelected,
                ClickHandler = async btn =>
                {
                    btn.IsSelected = true;
                    callbackAction(btn, compareFunc);
                    await Task.Delay(1);
                },
            };
        }
    }

    public class BubbleSortDefaultButton : BubbleSortButtonBase
    {
        protected override Func<CNTask, CNTask, int> compareFunc { get; set; } = (x, y) =>
        {
            if (x.TaskId > y.TaskId) return 1;
            else if (x.TaskId == y.TaskId) return 0;
            else return -1;
        };

        protected override FontFamilyType IconFontFamily { get; set; } = FontFamilyType.FontAwesomeSolid;
        protected override IconType IconFontText { get; set; } = IconType.SortAmount;
        protected override string ButtonText { get; set; } = "Default Order";
        protected override bool IsSelected { get; set; }
    }

    public class BubbleSortByCreateTimeButton : BubbleSortButtonBase
    {
        protected override Func<CNTask, CNTask, int> compareFunc { get; set; } = (x, y) =>
        {
            if (x.CreateTime > y.CreateTime) return 1;
            else if (x.CreateTime == y.CreateTime) return 0;
            else return -1;
        };

        protected override FontFamilyType IconFontFamily { get; set; } = FontFamilyType.CNFont;
        protected override IconType IconFontText { get; set; } = IconType.SortCreateTime;
        protected override string ButtonText { get; set; } = "By CreateTime";
        protected override bool IsSelected { get; set; }
    }
    public class BubbleSortCreateTimeReverseButton : BubbleSortButtonBase
    {
        protected override Func<CNTask, CNTask, int> compareFunc { get; set; } = (x, y) =>
        {
            if (x.CreateTime > y.CreateTime) return -1;
            else if (x.CreateTime == y.CreateTime) return 0;
            else return 1;
        };

        protected override FontFamilyType IconFontFamily { get; set; } = FontFamilyType.CNFont;
        protected override IconType IconFontText { get; set; } = IconType.SortCreateTimeReverse;
        protected override string ButtonText { get; set; } = "By CreateTime Reverse";
        protected override bool IsSelected { get; set; }
    }
    public class BubbleSortByRecentlyButton : BubbleSortButtonBase
    {
        protected override Func<CNTask, CNTask, int> compareFunc { get; set; } = (x, y) =>
        {
            if (x.EndTime == null && y.EndTime != null) return 1;
            if (x.EndTime != null && y.EndTime == null) return -1;
            if (x.EndTime > y.EndTime) return -1;
            else if (x.EndTime == y.EndTime) return 0;
            else return 1;
        };

        protected override FontFamilyType IconFontFamily { get; set; } = FontFamilyType.FontAwesomeRegular;
        protected override IconType IconFontText { get; set; } = IconType.Clock;
        protected override string ButtonText { get; set; } = "By Recently used";
        protected override bool IsSelected { get; set; }
    }
    public class BubbleSortByUrgencyImportanceButton : BubbleSortButtonBase
    {
        protected override Func<CNTask, CNTask, int> compareFunc { get; set; } = (x, y) =>
        {
            var a = x.MapFourQuadrantTaskUrgency();
            var b = y.MapFourQuadrantTaskUrgency();
            if (a > b) return -1;
            else if (a == b) return 0;
            else return 1;
        };

        protected override FontFamilyType IconFontFamily { get; set; } = FontFamilyType.FontAwesomeSolid;
        protected override IconType IconFontText { get; set; } = IconType.Trophy;
        protected override string ButtonText { get; set; } = "By Importance and Urgency";
        protected override bool IsSelected { get; set; }
    }
}