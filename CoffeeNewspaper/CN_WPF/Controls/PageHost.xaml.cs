﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CN_Core;
using CN_Presentation;
using CN_Presentation.ViewModel.Application;
using CN_Presentation.ViewModel.Base;

namespace CN_WPF
{
    /// <summary>
    ///     Interaction logic for PageHost.xaml
    /// </summary>
    public partial class PageHost : UserControl
    {
        #region Constructor

        /// <summary>
        ///     Default constructor
        /// </summary>
        public PageHost()
        {
            InitializeComponent();

            // If we are in DesignMode, show the current page
            // as the dependency property does not fire
            if (DesignerProperties.GetIsInDesignMode(this))
                NewPage.Content = IoC.Get<ApplicationViewModel>().CurrentPage.ToBasePage();
        }

        #endregion

        #region Property Changed Events

        /// <summary>
        ///     Called when the <see cref="CurrentPage" /> value has changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        private static object CurrentPagePropertyChanged(DependencyObject d, object value)
        {
            // Get current values
            var currentPage = (ApplicationPage) value;
            var currentPageViewModel = d.GetValue(CurrentPageViewModelProperty);

            // Get the frames
            var newPageFrame = (d as PageHost)?.NewPage;
            var oldPageFrame = (d as PageHost)?.OldPage;

            // If the current page hasn't changed
            // just update the view model
            if (newPageFrame?.Content is BasePage page &&
                page.ToApplicationPage() == currentPage)
            {
                // Just update the view model
                page.ViewModelObject = currentPageViewModel;

                return value;
            }

            // Store the current page content as the old page
            var oldPageContent = newPageFrame?.Content;

            // Remove current page from new page frame
            if (newPageFrame != null)
            {
                newPageFrame.Content = null;

                // Move the previous page into the old page frame
                if (oldPageFrame != null)
                {
                    oldPageFrame.Content = oldPageContent;

                    // Animate out previous page when the Loaded event fires
                    // right after this call due to moving frames
                    //            if (oldPageContent is BasePage oldPage)
                    //            {
                    //                // Tell old page to animate out
                    //                oldPage.ShouldAnimateOut = true;
                    //
                    //                // Once it is done, remove it
                    //                Task.Delay((int)(oldPage.SlideSeconds * 1000)).ContinueWith((t) =>
                    //                {
                    //                    // Remove old page
                    //                    Application.Current.Dispatcher.Invoke(() => oldPageFrame.Content = null);
                    //                });
                    //            }
                    if (oldPageContent is BasePage)
                        Application.Current.Dispatcher.Invoke(() => oldPageFrame.Content = null);

                    // Set the new page content
                    newPageFrame.Content = currentPage.ToBasePage(currentPageViewModel);
                }
            }

            return value;
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        ///     The current page to show in the page host
        /// </summary>
        public ApplicationPage CurrentPage
        {
            get => (ApplicationPage) GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        /// <summary>
        ///     Registers <see cref="CurrentPage" /> as a dependency property
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(ApplicationPage), typeof(PageHost),
                new UIPropertyMetadata(default(ApplicationPage), null, CurrentPagePropertyChanged));


        /// <summary>
        ///     The current page to show in the page host
        /// </summary>
        public BaseViewModel CurrentPageViewModel
        {
            get => (BaseViewModel) GetValue(CurrentPageViewModelProperty);
            set => SetValue(CurrentPageViewModelProperty, value);
        }

        /// <summary>
        ///     Registers <see cref="CurrentPageViewModel" /> as a dependency property
        /// </summary>
        public static readonly DependencyProperty CurrentPageViewModelProperty =
            DependencyProperty.Register(nameof(CurrentPageViewModel),
                typeof(BaseViewModel), typeof(PageHost),
                new UIPropertyMetadata());

        #endregion
    }
}