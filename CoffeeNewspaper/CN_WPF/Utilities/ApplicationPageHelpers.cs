using System.Diagnostics;
using CN_Presentation;
using CN_Presentation.ViewModel.Application;

namespace CN_WPF
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public static class ApplicationPageHelpers
    {
        /// <summary>
        /// Takes a <see cref="ApplicationPage"/> and a view model, if any, and creates the desired page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static BasePage ToBasePage(this ApplicationPage page, object viewModel = null)
        {
            // Find the appropriate page
            switch (page)
            {
                case ApplicationPage.WorkSpace:
                    return new WorkSpacePage(viewModel as WorkSpaceViewModel);

                case ApplicationPage.MemoList:
                    return new MemoListPage(viewModel as MemoListViewModel);

                case ApplicationPage.Settings:
                    return new SettingsPage(viewModel as SettingsViewModel);

                case ApplicationPage.Statistic:
                    return new StatisticPage(viewModel as StatisticViewModel);

                case ApplicationPage.TagReview:
                    return new TagReviewPage(viewModel as TagReviewViewModel);

                case ApplicationPage.TasksList:
                    return new TaskListPage(viewModel as TasksListPageViewModel);

                default:
                    Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// Converts a <see cref="BasePage"/> to the specific <see cref="ApplicationPage"/> that is for that type of page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static ApplicationPage ToApplicationPage(this BasePage page)
        {
            // Find application page that matches the base page
            if (page is WorkSpacePage)
                return ApplicationPage.WorkSpace;

            if (page is MemoListPage)
                return ApplicationPage.MemoList;

            if (page is SettingsPage)
                return ApplicationPage.Settings;
            if (page is StatisticPage)
                return ApplicationPage.Statistic;
            if (page is TagReviewPage)
                return ApplicationPage.TagReview;
            if (page is TaskListPage)
                return ApplicationPage.TasksList;

            // Alert developer of issue
            Debugger.Break();
            return default(ApplicationPage);
        }
    }
}
