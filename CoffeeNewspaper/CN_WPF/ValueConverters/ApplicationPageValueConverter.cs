using System;
using System.Diagnostics;
using System.Globalization;
using CN_Presentation;
using CN_Presentation.ViewModel.Application;

namespace CN_WPF
{
        public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
        {
            public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                switch ((ApplicationPage)value)
            {
                case ApplicationPage.WorkSpace:
                    return new WorkSpacePage(null as WorkSpaceViewModel);

                case ApplicationPage.MemoList:
                    return new MemoListPage(null as MemoListViewModel);

                case ApplicationPage.Settings:
                    return new SettingsPage(null as SettingsViewModel);

                case ApplicationPage.Statistic:
                    return new StatisticPage(null as StatisticViewModel);

                case ApplicationPage.TagReview:
                    return new TagReviewPage(null as TagReviewViewModel);

                case ApplicationPage.TasksList:
                    return new TaskListPage(null as TasksListPageViewModel);

                default:
                    {
                        Debugger.Break();
                        return null;
                    }
                }
            }

            public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
}