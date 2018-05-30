using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Repository;
using CN_Core.Interfaces.Service;

namespace CN_Service
{
    public class TaskService : ITaskService
    {
        private readonly ITaskDataStore taskDataStore;
        private readonly ITimeSliceDataStore timeSliceDataStore;

        public TaskService(ITimeSliceDataStore TimeSliceDataStore, ITaskDataStore TaskDataStore)
        {
            timeSliceDataStore = TimeSliceDataStore;
            taskDataStore = TaskDataStore;
        }

        #region Task relevent

        public async Task<bool> EditATask(CNTask task)
        {
            if (task.TaskId <= 0) return false;
            return await taskDataStore.UpdateTask(task);
        }

        public async Task<ICollection<CNTask>> GetAllTasks()
        {
            return await taskDataStore.GetAllTask();
        }

        public async Task<CNTask> GetTaskById(int taskId)
        {
            return await taskDataStore.GetTask(taskId);
        }

        public async Task<CNTask> CreateATask(CNTask task)
        {
            return await taskDataStore.AddTask(task);
        }

        public async Task<bool> DeleteTask(int taskId, bool force = false)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) throw new ArgumentException("TaskID does not exist!");
            if (targetTask.IsDeleted) return true;
            if (targetTask.ChildTasks.Count > 0 && !force) throw new TaskHasChildTasksException();
            if (targetTask.SufTaskConnectors.Count > 0 && !force) throw new TaskHasSufTasksException();
            //Delete the task itself
            targetTask.IsDeleted = true;
            //Delete the tasks children and suffix task
            targetTask.ChildTasks.ToList()
                .ForEach(x => Task.Run(async () => await DeleteTask(x.TaskId, force)));
            targetTask.SufTaskConnectors.Select(y => y.SufTask).ToList()
                .ForEach(x => Task.Run(async () => await DeleteTask(x.TaskId, force)));
            return await taskDataStore.UpdateTask(targetTask);
        }

        public async Task<bool> RecoverATask(int taskId)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) throw new ArgumentException("TaskID does not exist!");
            if (!targetTask.IsDeleted) return true;

            //Recover the task itself
            targetTask.IsDeleted = false;
            //Recover the tasks children and suffix task
            targetTask.ChildTasks.ToList()
                .ForEach(x => Task.Run(async () => await RecoverATask(x.TaskId)));
            targetTask.SufTaskConnectors.Select(y => y.SufTask).ToList()
                .ForEach(x => Task.Run(async () => await RecoverATask(x.TaskId)));
            return await taskDataStore.UpdateTask(targetTask);
        }

        public async Task<bool> StartATask(int taskId)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) throw new ArgumentException("TaskID does not exist!");
            if (targetTask.IsDeleted) throw new ArgumentException("Task is deleted");
            if (targetTask.Status != CNTaskStatus.TODO && targetTask.Status != CNTaskStatus.DONE)
                throw new TaskStatusException(new List<CNTaskStatus> {CNTaskStatus.TODO, CNTaskStatus.DONE},
                    targetTask.Status);
            //set status to doing
            targetTask.Status = CNTaskStatus.DOING;
            await taskDataStore.UpdateTask(targetTask);
            //if this is the first start then update targetTask's startTime
            //add a timeslice to this task
            await AddATimeSlice(targetTask.TaskId, new CNTimeSlice(DateTime.Now));
            return true;
        }

        public async Task<bool> PauseATask(int taskId)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) throw new ArgumentException("TaskID does not exist!");
            if (targetTask.IsDeleted) throw new ArgumentException("Task is deleted");
            if (targetTask.Status != CNTaskStatus.DOING)
                throw new TaskStatusException(new List<CNTaskStatus> {CNTaskStatus.DOING}, targetTask.Status);
            targetTask.Status = CNTaskStatus.TODO;
            await taskDataStore.UpdateTask(targetTask);

            //EndTimeSlices of this task's children and suffix task
            return await EndTimeSlice(targetTask.TaskId, DateTime.Now);
        }

        public async Task<bool> RemoveATask(int taskId, bool force = false)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) throw new ArgumentException("TaskID does not exist!");
            if (!targetTask.IsDeleted) throw new ArgumentException("Task is not deleted");
            if (targetTask.ChildTasks.Count > 0 && !force) throw new TaskHasChildTasksException();
            if (targetTask.SufTaskConnectors.Count > 0 && !force) throw new TaskHasSufTasksException();

            return await taskDataStore.RemoveTask(targetTask);
        }

        public async Task<bool> FinishATask(int taskId)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) throw new ArgumentException("TaskID does not exist!");
            if (targetTask.IsDeleted) throw new ArgumentException("Task is deleted");
            if (targetTask.Status == CNTaskStatus.DONE)
                throw new TaskStatusException(new List<CNTaskStatus> {CNTaskStatus.DOING, CNTaskStatus.TODO},
                    targetTask.Status);
            targetTask.Status = CNTaskStatus.DONE;
            await taskDataStore.UpdateTask(targetTask);

            //EndTimeSlices of this task's children and suffix task
            return await EndTimeSlice(targetTask.TaskId, DateTime.Now);
        }

        public async Task<bool> FailATask(int taskId, string reason)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) throw new ArgumentException("TaskID does not exist!");

            if (targetTask.IsFail)
            {
                targetTask.FailReason = reason;
                return await taskDataStore.UpdateTask(targetTask);
            }

            //tag this as filed
            targetTask.IsFail = true;
            targetTask.FailReason = reason;
            await taskDataStore.UpdateTask(targetTask);

            //EndTimeSlices of this task's children and suffix task
            return await EndTimeSlice(targetTask.TaskId, DateTime.Now);
        }

        public async Task<bool> SetParentTask(CNTask targetTask, CNTask parentTask)
        {
            targetTask.ParentTask = parentTask;
            return await taskDataStore.UpdateTask(targetTask);
        }

        #endregion

        #region TimeSlices relevent

        public async Task<bool> DeleteAllTimeSlicesOfTask(int taskId)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) throw new ArgumentException("TaskID does not exist!");
            targetTask.ChildTasks.ToList()
                .ForEach(x => Task.Run(async () => await DeleteAllTimeSlicesOfTask(x.TaskId)));
            targetTask.SufTaskConnectors.Select(y => y.SufTask).ToList()
                .ForEach(x => Task.Run(async () => await DeleteAllTimeSlicesOfTask(x.TaskId)));
            targetTask.StartTime = null;
            targetTask.EndTime = null;
            await taskDataStore.UpdateTask(targetTask);
            return await timeSliceDataStore.DeleteTimeSliceByTask(targetTask.TaskId);
        }

        public async Task<ICollection<CNTimeSlice>> GetTaskTimeSlices(int taskid)
        {
            var task = await taskDataStore.GetTask(taskid);
            if (task == null) return new List<CNTimeSlice>();
            return task.UsedTimeSlices.ToList();
        }

        public async Task<CNTimeSlice> AddATimeSlice(int taskid, CNTimeSlice timeSlice)
        {
            //get original task
            var task = await taskDataStore.GetTask(taskid);
            if (task == null) return null;

            //detect conflict
            if (task.UsedTimeSlices.Any(r => r.InterceptWith(timeSlice)))
                throw new ArgumentException("Time slice intervene with exist");

            //Update Task's StartTime or EndTime
            var source = new List<CNTimeSlice>(task.UsedTimeSlices) {timeSlice};
            source.Sort();
            await ExpandTaskTime(task.TaskId, source.First().StartDateTime, source.Last().EndDateTime);

            //Save TimeSlice
            timeSlice.Task = task;
            return await timeSliceDataStore.AddTimeSlice(timeSlice);
        }


        public async Task<bool> DeleteTimeSlices(CNTimeSlice timeSlice)
        {
            //Get original slice by id
            var originSlice = await timeSliceDataStore.GetTimeSliceById(timeSlice.TimeSliceId);

            //Update Task's StartTime or EndTime
            var originDatas = originSlice.Task.UsedTimeSlices.ToList();
            if (Equals(originDatas.First(), timeSlice))
                await UpdateStartTaskTime(originSlice.Task.TaskId, originDatas.First().StartDateTime);
            else if (Equals(originDatas.Last(), timeSlice))
                await UpdateEndTaskTime(originSlice.Task.TaskId, originDatas.Last().EndDateTime);
            else
                await ExpandTaskTime(originSlice.Task.TaskId, originDatas.First().StartDateTime,
                    originDatas.Last().EndDateTime);
            //delete the timeslice
            return await timeSliceDataStore.DeleteTimeSlice(originSlice);
        }

        public async Task<bool> EndTimeSlice(int taskId, DateTime endTime)
        {
            var originDataTask = await taskDataStore.GetTask(taskId);

            var originDatas = originDataTask.UsedTimeSlices.ToList();
            originDatas.Sort();
            if (!(originDatas.Last().Clone() is CNTimeSlice lastSlice) || lastSlice.EndDateTime != null) return true;
            lastSlice.EndDateTime = endTime;

            //EndTimeSlices of this task's children and suffix task
            originDataTask.ChildTasks.ToList()
                .ForEach(x => Task.Run(async () => await EndTimeSlice(x.TaskId, endTime)));
            originDataTask.SufTaskConnectors.Select(y => y.SufTask).ToList()
                .ForEach(x => Task.Run(async () => await EndTimeSlice(x.TaskId, endTime)));

            await UpdateEndTaskTime(originDataTask.TaskId, endTime);
            return await timeSliceDataStore.UpdateTimeSlice(lastSlice);
        }
        //
        //        public void DeleteAllTimeSlices(int taskId)
        //        {
        //            var originRoot = rootDataProvider.GetRootData();
        //            var tasks = originRoot.GetTaskAndChildSufTasksById(taskId);
        //            foreach (var task in tasks)
        //            {
        //                var originDatas = GetTaskTimeSlices(task).ToList();
        //                var dates = originDatas.Select(x => x.StartDate).Distinct();
        //                foreach (var date in dates)
        //                {
        //                    var source = timeSliceProvider.GetOriginalDataByDate(date);
        //                    source.Remove(task.TaskId);
        //                    timeSliceProvider.OverWriteToDataSourceByDate(date, source);
        //                }
        //            }
        //        }

        private async Task ExpandTaskTime(int taskid, DateTime? targetStartTime, DateTime? targetEndTime)
        {
            var originDataTask = await taskDataStore.GetTask(taskid);
            if (originDataTask == null) return;
            if (originDataTask.StartTime == null || originDataTask.EndTime == null ||
                originDataTask.StartTime > targetStartTime || originDataTask.EndTime < targetEndTime ||
                targetEndTime == null)
            {
                if (originDataTask.StartTime == null || originDataTask.StartTime > targetStartTime)
                    originDataTask.StartTime = targetStartTime;
                if (originDataTask.EndTime == null && targetEndTime != null || //Pause A Task
                    originDataTask.EndTime != null && targetEndTime == null || //Start A Task
                    originDataTask.EndTime != null && targetEndTime != null && originDataTask.EndTime < targetEndTime
                ) //Update A Task timeslice
                    originDataTask.EndTime = targetEndTime;

                await taskDataStore.UpdateTask(originDataTask);
            }
        }

        private async Task UpdateStartTaskTime(int taskid, DateTime? targetStartTime)
        {
            var originDataTask = await taskDataStore.GetTask(taskid);
            if (originDataTask == null) return;
            originDataTask.StartTime = targetStartTime;
            await taskDataStore.UpdateTask(originDataTask);
        }

        private async Task UpdateEndTaskTime(int taskid, DateTime? targetEndTime)
        {
            var originDataTask = await taskDataStore.GetTask(taskid);
            if (originDataTask == null) return;
            originDataTask.EndTime = targetEndTime;
            await taskDataStore.UpdateTask(originDataTask);
        }

        #endregion
    }
}