using System;
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
    }
}