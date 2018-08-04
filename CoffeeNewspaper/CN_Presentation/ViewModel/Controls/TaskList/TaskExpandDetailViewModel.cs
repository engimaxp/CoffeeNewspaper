using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CN_Core;
using CN_Core.Utilities;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Controls
{
    public class TaskExpandDetailViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Child tasks viewModel to represent child task tree
        /// </summary>
        public TaskTreeViewModel ChildTasksModel { get; set; }

        /// <summary>
        /// Task Content Detail
        /// </summary>
        public string TaskDetailContent { get; set; }

        /// <summary>
        /// Created Time of this task
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public string CreatedTimeLongForm => $"({CreatedTime.ToString(CNConstants.DIRECTORY_DATETIMEFORMAT)})";

        public string CreatedTimeShortForm => CreatedTime.EarlyTimeFormat();

        #endregion

        #region Private Methods

        /// <summary>
        /// DFS to Add all child tasks
        /// </summary>
        /// <param name="taskInfoChildTasks"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        private ObservableCollection<TaskTreeItemViewModel> MapToViewModel(ICollection<CNTask> taskInfoChildTasks,
            ITreeNodeSubscribe container)
        {
            var result = new ObservableCollection<TaskTreeItemViewModel>();
            foreach (var a in taskInfoChildTasks)
            {
                var viewModel =
                    new TaskTreeItemViewModel(container, a) {ChildItems = MapToViewModel(a.ChildTasks, container)};
                result.Add(viewModel);
            }

            return result;
        }

        #endregion

        #region Constructor

        public TaskExpandDetailViewModel()
        {
        }

        public TaskExpandDetailViewModel(CNTask TaskInfo)
        {
            var childTaskModel = new TaskTreeViewModel(TaskInfo);
            childTaskModel.Items = MapToViewModel(TaskInfo.ChildTasks, childTaskModel);
            ChildTasksModel = childTaskModel;
        }

        #endregion
    }
}