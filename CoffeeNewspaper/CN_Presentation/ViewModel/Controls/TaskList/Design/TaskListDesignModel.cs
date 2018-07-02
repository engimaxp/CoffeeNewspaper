﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CN_Presentation.ViewModel.Controls.Design
{
    public class TaskListDesignModel : TaskListViewModel
    {
        #region Constructor

        /// <summary>
        ///     Default constructor
        /// </summary>
        public TaskListDesignModel()
        {
            Items = new ObservableCollection<TaskListItemViewModel>
            {
                new TaskListItemViewModel
                {
                    Urgency = TaskUrgency.NotUrgent,
                    IsExpanded = true,
                    Status = TaskCurrentStatus.COMPLETE,
                    TaskTitle =
                        "Each control in WPF has a DataContext property. It's meant to be bound to an object that contains the data to be displayed. The DataContext property is inherited along the logical tree."

                },
                new TaskListItemViewModel
                {
                    Urgency = TaskUrgency.Urgent,
                    IsExpanded = false,
                    Status = TaskCurrentStatus.COMPLETE,
                    TaskTitle =
                        "Each control in WPF has a DataContext property. It's meant to be bound to an object that contains the data to be displayed. The DataContext property is inherited along the logical tree."
                },new TaskListItemViewModel
                {
                    Urgency = TaskUrgency.NotUrgent,
                    IsExpanded = false,
                    Status = TaskCurrentStatus.UNDERGOING,
                    TaskTitle =
                        "Each control in WPF has a DataContext property. It's meant to be bound to an object that contains the data to be displayed. The DataContext property is inherited along the logical tree."
                },new TaskListItemViewModel
                {
                    Urgency = TaskUrgency.Urgent,
                    IsExpanded = false,
                    Status = TaskCurrentStatus.PENDING,
                    TaskTitle =
                        "Each control in WPF has a DataContext property. It's meant to be bound to an object that contains the data to be displayed. The DataContext property is inherited along the logical tree."
                },new TaskListItemViewModel
                {
                    Urgency = TaskUrgency.VeryUrgent,
                    IsExpanded = false,
                    Status = TaskCurrentStatus.DELETE,
                    TaskTitle =
                        "Each control in WPF has a DataContext property. It's meant to be bound to an object that contains the data to be displayed. The DataContext property is inherited along the logical tree."
                },new TaskListItemViewModel
                {
                    Urgency = TaskUrgency.NotUrgent,
                    IsExpanded = false,
                    Status = TaskCurrentStatus.STOP,
                    TaskTitle =
                        "Each control in WPF has a DataContext property. It's meant to be bound to an object that contains the data to be displayed. The DataContext property is inherited along the logical tree."
                }
            };

            ActivatedSearchTxts = new ObservableCollection<string>()
            {
                "Hello","My","您好","Boun"
            };
        }

        #endregion

        #region Singleton

        /// <summary>
        ///     A single instance of the design model
        /// </summary>
        public static TaskListDesignModel Instance => new TaskListDesignModel();

        #endregion
    }
}