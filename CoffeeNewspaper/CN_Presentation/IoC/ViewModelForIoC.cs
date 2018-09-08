using System.Collections.Generic;
using CN_Presentation.ViewModel.Application;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Controls;
using CN_Presentation.ViewModel.Controls.StatusBar;
using Ninject;

namespace CN_Presentation
{
    public static class ViewModelForIoC
    {
        /// <summary>
        ///     Helper Method: if a ViewModel is already binded then return directly else bind it
        /// </summary>
        /// <param name="Kernel"></param>
        /// <param name="model"></param>
        private static void BindViewModel<TViewModel>(this IKernel Kernel, TViewModel model)
            where TViewModel : BaseViewModel
        {
            // create a default dbContext object
            Kernel?.Bind<TViewModel>().ToConstant(model);
        }

        /// <summary>
        ///     Bind the ViewModel
        ///     shall do this at the beginning of app start
        /// </summary>
        public static void BindInitialViewModel(this IKernel Kernel)
        {
            Kernel.BindViewModel(new TaskListViewModel());
            Kernel.BindViewModel(new HeadMenuViewModel
            {
                NavButtonItems = new List<HeadMenuButtonViewModel>
                {
                    new HeadMenuButtonViewModel()
                    {
                        FontCode = IconType.BreifCase,
                        TargetPage = ApplicationPage.WorkSpace,
                    },
                    new HeadMenuButtonViewModel()
                    {
                        FontCode = IconType.Tasks,
                        TargetPage = ApplicationPage.TasksList
                    },
                    new HeadMenuButtonViewModel()
                    {
                        FontCode = IconType.Notes,
                        TargetPage = ApplicationPage.MemoList
                    },
                    new HeadMenuButtonViewModel()
                    {
                        FontCode = IconType.ChartArea,
                        TargetPage = ApplicationPage.Statistic
                    },
                    new HeadMenuButtonViewModel()
                    {
                        FontCode = IconType.GraduationCap,
                        TargetPage = ApplicationPage.TagReview
                    },
                    new HeadMenuButtonViewModel()
                    {
                        FontCode = IconType.Cog,
                        TargetPage = ApplicationPage.Settings
                    },
                }
            });
            Kernel.BindViewModel(new StatusBarViewModel());
            Kernel.BindViewModel(new TasksListPageViewModel());
            Kernel.BindViewModel(new WorkSpaceViewModel());
            Kernel.BindViewModel(new MemoListViewModel());
            Kernel.BindViewModel(new StatisticViewModel());
            Kernel.BindViewModel(new TagReviewViewModel());
            Kernel.BindViewModel(new ApplicationViewModel());
        }
    }
}