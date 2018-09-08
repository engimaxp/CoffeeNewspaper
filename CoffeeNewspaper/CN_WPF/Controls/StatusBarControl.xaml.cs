using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using CN_Presentation.ViewModel.Controls.Design;

namespace CN_WPF
{
    /// <summary>
    /// StatusBarControl.xaml 的交互逻辑
    /// </summary>
    public partial class StatusBarControl : UserControl
    {
        public StatusBarControl()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = ViewModelLocator.StatusBarViewModel;
                this.Loaded += (s, e) =>
                {
                    //  DispatcherTimer setup
                    var dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                    dispatcherTimer.Start();
                };
            }
            else
            {
                DataContext = StatusBarDesignModel.Instance;
            }

        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Task.Run(async () => await ViewModelLocator.StatusBarViewModel.TimeSpanIncrement());
        }
    }
}
