using System.Collections.ObjectModel;

namespace CN_Presentation.ViewModel.Controls.Design
{
    public class TaskTreeDesignModel:TaskTreeViewModel
    {
        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static TaskTreeDesignModel Instance => new TaskTreeDesignModel();
        public TaskTreeDesignModel()
        {
            Items = new ObservableCollection<TaskTreeItemViewModel>()
            {
                new TaskTreeItemViewModel()
                {
                    Title = "我能用滴答清单做什么？",
                    ChildItems = new ObservableCollection<TaskTreeItemViewModel>()
                    {
                        new TaskTreeItemViewModel(){Title = "记录任务、添加提醒，绝不错过重要事项"},
                        new TaskTreeItemViewModel(){Title = "为重复任务设置灵活的任务周期"},
                        new TaskTreeItemViewModel(){Title = "集成第三方日历，轻松管理所有事项",},
                        new TaskTreeItemViewModel(){Title = "使用文件夹、清单、优先级、标签，让任务清晰有条理"},
                        new TaskTreeItemViewModel(){Title = "使用列表模式，逐条添加子任务"},
                        new TaskTreeItemViewModel(){Title = "与家人、朋友、同事共享清单进行协作"},
                    }
                },
                new TaskTreeItemViewModel()
                {
                    Title = "欢迎加入滴答清单",
                }
            };
        }
    }
}