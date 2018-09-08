using System.Collections.ObjectModel;

namespace CN_Presentation.ViewModel.Controls.Design
{
    public class TaskTreeDesignModel:TaskTreeViewModel
    {
        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static TaskTreeDesignModel Instance => new TaskTreeDesignModel();
        public TaskTreeDesignModel():base(null)
        {
            Items = new ObservableCollection<TaskTreeItemViewModel>()
            {
                new TaskTreeItemViewModel(this)
                {
                    Title = "我能用滴答清单做什么？",
                    ChildItems = new ObservableCollection<TaskTreeItemViewModel>()
                    {
                        new TaskTreeItemViewModel(this)
                        {
                            Title = "记录任务、添加提醒，绝不错过重要事项",
                            Urgency = TaskUrgency.NotUrgent,
                            IsSelected = true,
                            IsCompleted = true,
                            CurrentStatus = TaskCurrentStatus.STOP
                        },
                        new TaskTreeItemViewModel(this)
                        {
                            Title = "为重复任务设置灵活的任务周期",
                            Urgency = TaskUrgency.Urgent,
                            CurrentStatus = TaskCurrentStatus.STOP
                        },
                        new TaskTreeItemViewModel(this)
                        {
                            Title = "集成第三方日历，轻松管理所有事项",
                            Urgency = TaskUrgency.NotUrgent,
                            CurrentStatus = TaskCurrentStatus.STOP
                        },
                        new TaskTreeItemViewModel(this)
                        {
                            Title = "使用文件夹、清单、优先级、标签，让任务清晰有条理",
                            Urgency = TaskUrgency.Urgent,
                            CurrentStatus = TaskCurrentStatus.PENDING
                        },
                        new TaskTreeItemViewModel(this)
                        {
                            Title = "使用列表模式，逐条添加子任务",
                            Urgency = TaskUrgency.VeryUrgent,
                            CurrentStatus = TaskCurrentStatus.STOP
                        },
                        new TaskTreeItemViewModel(this)
                        {
                            Title = "与家人、朋友、同事共享清单进行协作",
                            Urgency = TaskUrgency.NotUrgent,
                            CurrentStatus = TaskCurrentStatus.STOP
                        },
                    }
                },
                new TaskTreeItemViewModel(this)
                {
                    Title = "欢迎加入滴答清单",
                }
            };
        }
    }
}