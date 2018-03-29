﻿using System;
using System.Collections.Generic;
using System.Linq;
using CN_Model;
using CN_Repository;

namespace CN_BLL
{
    public class TaskService : ITaskService
    {
        private readonly ITimeSliceService timeSliceService;
        private readonly IRootDataProvider rootDataProvider;

        public TaskService()
        {
            timeSliceService = new TimeSliceService();
            rootDataProvider = RootDataProvider.GetProvider();
        }
        public TaskService(IRootDataProvider rootDataProvider)
        {
            this.timeSliceService = new TimeSliceService(); ;
            this.rootDataProvider = rootDataProvider;
        }
        public TaskService(ITimeSliceService timeSliceService, IRootDataProvider rootDataProvider)
        {
            this.timeSliceService = timeSliceService;
            this.rootDataProvider = rootDataProvider;
        }

        public int CreateATask(CNTask task)
        {
            var root = rootDataProvider.GetRootData();
            task.TaskId = root.GetNextTaskID();
            root.AddOrUpdateTask(task);
            rootDataProvider.Persistence(root);
            return task.TaskId;
        }

        public bool DeleteTask(int taskId, bool force = false)
        {
            var root = rootDataProvider.GetRootData();
            var targetTask = root.GetTaskById(taskId);
            if (string.IsNullOrEmpty(targetTask?.Content))
            {
                throw new ArgumentException("TaskID does not exist!");
            }
            if (targetTask.IsDeleted)
            {
                return true;
            }
            if (root.HasChildTasks(taskId) && !force)
            {
                throw new TaskHasChildTasksException();
            }
            if (root.HasSufTasks(taskId) && !force){
                throw new TaskHasSufTasksException();
            }

            root.DeleteTaskById(taskId);
            rootDataProvider.Persistence(root);
            return true;
        }

        public bool RecoverTask(int taskId)
        {
            var root = rootDataProvider.GetRootData();
            var targetTask = root.GetTaskById(taskId);
            if (string.IsNullOrEmpty(targetTask?.Content))
            {
                throw new ArgumentException("TaskID does not exist!");
            }
            if (!targetTask.IsDeleted)
            {
                return true;
            }
            root.RecoverTaskById(taskId);
            rootDataProvider.Persistence(root);
            return true;
        }

        public bool RemoveTask(int taskId, bool force = false)
        {
            var root = rootDataProvider.GetRootData();
            var targetTask = root.GetTaskById(taskId);
            if (string.IsNullOrEmpty(targetTask?.Content))
            {
                throw new ArgumentException("TaskID does not exist!");
            }
            if (!targetTask.IsDeleted)
            {
                throw new ArgumentException("Task is not deleted");
            }
            if (root.HasChildTasks(taskId) && !force)
            {
                throw new TaskHasChildTasksException();
            }
            if (root.HasSufTasks(taskId) && !force)
            {
                throw new TaskHasSufTasksException();
            }
            root.RemoveTaskById(taskId);
            rootDataProvider.Persistence(root);
            return true;
        }
    }
}