using CN_Presentation.ViewModel.Application;
using CN_Presentation.ViewModel.Base;
using CN_Presentation.ViewModel.Controls;
using Ninject;

namespace CN_Presentation
{
    public static class ViewModelForIoC
    {
        /// <summary>
        /// Helper Method: if a ViewModel is already binded then return directly else bind it
        /// </summary>
        /// <param name="Kernel"></param>
        /// <param name="model"></param>
        private static void BindViewModel<TViewModel>(this IKernel Kernel, TViewModel model) where TViewModel : BaseViewModel
        {
            if (Kernel == null) return;
            var origindbContext = Kernel.TryGet<TViewModel>();
            // if the originDBContext exists then jump to exit
            if (origindbContext != null) return;

            // create a default dbContext object
            Kernel.Bind<TViewModel>().ToConstant(model);
        }

        /// <summary>
        ///     Bind the ViewModel
        ///     shall do this at the beginning of app start
        /// </summary>
        public static void BindInitialViewModel(this IKernel Kernel)
        {
            Kernel.BindViewModel(new HeadMenuViewModel());
            Kernel.BindViewModel(new TasksListViewModel());
            Kernel.BindViewModel(new WorkSpaceViewModel());
            Kernel.BindViewModel(new MemoListViewModel());
            Kernel.BindViewModel(new StatisticViewModel());
            Kernel.BindViewModel(new TagReviewViewModel());
            Kernel.BindViewModel(new ApplicationViewModel());
        }
    }
}
