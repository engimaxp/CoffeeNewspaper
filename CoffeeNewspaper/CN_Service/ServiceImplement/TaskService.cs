using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Repository;
using CN_Core.Interfaces.Service;
using CN_Core.Utilities;

namespace CN_Service
{
    public class TaskService : ITaskService
    {
        #region Construct

        public TaskService(IUnitOfWork UnitOfWork, ITimeSliceDataStore TimeSliceDataStore, ITaskDataStore TaskDataStore)
        {
            unitOfWork = UnitOfWork;
            timeSliceDataStore = TimeSliceDataStore;
            taskDataStore = TaskDataStore;
        }

        #endregion

        #region Private Properties

        private readonly IUnitOfWork unitOfWork;
        private readonly ITaskDataStore taskDataStore;
        private readonly ITimeSliceDataStore timeSliceDataStore;

        #endregion

        #region Task relevent

        public Task<bool> EditATask(CNTask task)
        {
            if (task == null || task.TaskId <= 0) return Task.FromResult(false);
            if (string.IsNullOrEmpty(task.Content) || string.IsNullOrEmpty(task.Content.Trim()))
                throw new ArgumentException("The Content Field is needed");
            return EditATaskAsync(task);
        }

        private async Task<bool> EditATaskAsync(CNTask task)
        {
                taskDataStore.UpdateTask(task);
                return await unitOfWork.Commit();
        }

        public async Task<ICollection<CNTask>> GetAllTasks()
        {
                return await taskDataStore.GetAllTask();
        }

        public async Task<CNTask> GetTaskById(int taskId)
        {
                return await taskDataStore.GetTask(taskId);
        }

        public async Task<CNTask> GetTaskByIdNoTracking(int taskId)
        {
                return await taskDataStore.GetTaskNoTracking(taskId);
        }

        public async Task<int> GetTaskRootParentId(int taskId)
        {
                var task = await taskDataStore.GetTask(taskId);
                while (task.HasParentTask()) task = task.ParentTask;
                return task.TaskId;
        }

        public Task<CNTask> CreateATask(CNTask task)
        {
            if (string.IsNullOrEmpty(task.Content) || string.IsNullOrEmpty(task.Content.Trim()))
                throw new ArgumentException("The Content Field is needed");
            return CreateATaskAsync(task);
        }

        private async Task<CNTask> CreateATaskAsync(CNTask task)
        {
                var parentTaskId = task.ParentTask?.TaskId ?? task.ParentTaskID;
                task.Sort = await taskDataStore.GetMaxSort(parentTaskId) + 1;
                var result = taskDataStore.AddTask(task);
                return await unitOfWork.Commit() ? result : null;
        }

        private async Task DeleteTaskWithoutCommit(int taskId, bool force)
        {
                var targetTask = await taskDataStore.GetTask(taskId);
                if (targetTask == null) return;
                if (targetTask.IsDeleted) return;
                if (targetTask.ChildTasks.FilterDeletedAndOrderBySortTasks().Any() && !force)
                    throw new TaskHasChildTasksException();
                if (targetTask.SufTaskConnectors.Select(y => y.SufTask).FilterDeletedAndOrderBySortTasks().Any() &&
                    !force) throw new TaskHasSufTasksException();
                //Delete the task itself
                targetTask.IsDeleted = true;
                //Delete the tasks children and suffix task
                foreach (var task in targetTask.ChildTasks.FilterDeletedAndOrderBySortTasks())
                    await DeleteTaskWithoutCommit(task.TaskId, force);
                foreach (var task in targetTask.SufTaskConnectors.Select(y => y.SufTask)
                    .FilterDeletedAndOrderBySortTasks())
                    await DeleteTaskWithoutCommit(task.TaskId, force);
                taskDataStore.UpdateTask(targetTask);
        }

        public async Task<bool> DeleteTask(int taskId, bool force = false)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) return false;
            if (targetTask.IsDeleted) return true;
            await DeleteTaskWithoutCommit(taskId, force);
            return await unitOfWork.Commit();
        }

        private async Task RecoverATaskWithoutCommit(int taskId)
        {
                var targetTask = await taskDataStore.GetTask(taskId);
                if (targetTask == null) return;
                if (!targetTask.IsDeleted) return;

                //Recover the task itself
                targetTask.IsDeleted = false;
                //Recover the tasks children and suffix task
                foreach (var task in targetTask.ChildTasks)
                    await RecoverATaskWithoutCommit(task.TaskId);
                foreach (var task in targetTask.SufTaskConnectors.Select(y => y.SufTask))
                    await RecoverATaskWithoutCommit(task.TaskId);
                taskDataStore.UpdateTask(targetTask);
                
        }

        public async Task<bool> RecoverATask(int taskId)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) return false;
            if (!targetTask.IsDeleted) return true;
            await RecoverATaskWithoutCommit(taskId);
            return await unitOfWork.Commit();
        }

        public async Task<bool> StartATask(int taskId)
        {
                var targetTask = await taskDataStore.GetTask(taskId);
                if (targetTask == null) return false;
                if (targetTask.IsDeleted) return false;
                if (targetTask.Status != CNTaskStatus.TODO && targetTask.Status != CNTaskStatus.PENDING &&
                    targetTask.Status != CNTaskStatus.DONE)
                    throw new TaskStatusException(
                        new List<CNTaskStatus> {CNTaskStatus.TODO, CNTaskStatus.PENDING, CNTaskStatus.DONE},
                        targetTask.Status);
                //set status to doing
                if (targetTask.Status == CNTaskStatus.PENDING) targetTask.PendingReason = null;
                targetTask.Status = CNTaskStatus.DOING;
                if (targetTask.IsFail)
                {
                    targetTask.FailReason = null;
                    targetTask.IsFail = false;
                }

                taskDataStore.UpdateTask(targetTask);
                //if this is the first start then update targetTask's startTime
                //add a timeslice to this task
                await AddATimeSlice(targetTask.TaskId, new CNTimeSlice(DateTime.Now));
                return await unitOfWork.Commit();
        }

        public async Task<bool> PauseATask(int taskId)
        {
                var targetTask = await taskDataStore.GetTask(taskId);
                if (targetTask == null) return false;
                if (targetTask.IsDeleted) return false;
                if (targetTask.Status != CNTaskStatus.DOING)
                    throw new TaskStatusException(new List<CNTaskStatus> {CNTaskStatus.DOING}, targetTask.Status);
                targetTask.Status = CNTaskStatus.TODO;
                taskDataStore.UpdateTask(targetTask);

            //EndTimeSlices of this task's children and suffix task
            return await EndTimeSlice(targetTask.TaskId, DateTime.Now);
        }

        public async Task<bool> PendingATask(int taskId, string reason)
        {
                var targetTask = await taskDataStore.GetTask(taskId);
                if (targetTask == null) return false;
                if (targetTask.IsDeleted) return false;
                if (targetTask.Status != CNTaskStatus.DOING
                    && targetTask.Status != CNTaskStatus.TODO
                    && targetTask.Status != CNTaskStatus.PENDING)
                    throw new TaskStatusException(
                        new List<CNTaskStatus> {CNTaskStatus.DOING, CNTaskStatus.TODO, CNTaskStatus.PENDING},
                        targetTask.Status);
                //EndTimeSlices of this task's children and suffix task
                if (targetTask.Status == CNTaskStatus.DOING) await EndTimeSlice(targetTask.TaskId, DateTime.Now);
                targetTask.Status = CNTaskStatus.PENDING;
                targetTask.PendingReason = reason;
                taskDataStore.UpdateTask(targetTask);
                return await unitOfWork.Commit();
        }
        
        public async Task<ICollection<CNTask>> GetChildTasksNoTracking(int taskId)
        {
                return await taskDataStore.GetChildTasksNoTracking(taskId);
        }

        private async Task RemoveATaskWithoutCommit(int taskId, bool force)
        {
                var targetTask = await taskDataStore.GetTask(taskId);
                if (targetTask == null) return;
                if (!targetTask.IsDeleted) return;
                if (targetTask.ChildTasks.Any() && !force) throw new TaskHasChildTasksException();
                if (targetTask.SufTaskConnectors.Count > 0 && !force) throw new TaskHasSufTasksException();

                //Remove the tasks children and suffix task
                foreach (var task in targetTask.ChildTasks)
                    await RemoveATaskWithoutCommit(task.TaskId, force);
                foreach (var task in targetTask.SufTaskConnectors.Select(y => y.SufTask))
                    await RemoveATaskWithoutCommit(task.TaskId, force);
                taskDataStore.RemoveTask(targetTask);
        }

        public async Task<bool> RemoveATask(int taskId, bool force = false)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) return false;
            if (!targetTask.IsDeleted) return false;
            await RemoveATaskWithoutCommit(taskId, force);
            return await unitOfWork.Commit();
        }


        public async Task<bool> FinishATask(int taskId)
        {
                var targetTask = await taskDataStore.GetTask(taskId);
                if (targetTask == null) return false;
                if (targetTask.IsDeleted) return false;
                if (targetTask.Status == CNTaskStatus.DONE)
                    throw new TaskStatusException(
                        new List<CNTaskStatus> {CNTaskStatus.DOING, CNTaskStatus.TODO, CNTaskStatus.PENDING},
                        targetTask.Status);
                if (targetTask.Status == CNTaskStatus.PENDING) targetTask.PendingReason = null;
                targetTask.Status = CNTaskStatus.DONE;
                if (targetTask.IsFail)
                {
                    targetTask.FailReason = null;
                    targetTask.IsFail = false;
                }

                taskDataStore.UpdateTask(targetTask);

            //EndTimeSlices of this task's children and suffix task
            return await EndTimeSlice(targetTask.TaskId, DateTime.Now);
        }

        public async Task<bool> FailATask(int taskId, string reason)
        {
                var targetTask = await taskDataStore.GetTask(taskId);
                if (targetTask == null) return false;

                if (targetTask.IsFail)
                {
                    targetTask.FailReason = reason;
                    taskDataStore.UpdateTask(targetTask);
                    return await unitOfWork.Commit();
            }
                else
                {
                    //tag this as filed
                    targetTask.IsFail = true;
                    targetTask.FailReason = reason;
                    targetTask.Status = CNTaskStatus.TODO;
                taskDataStore.UpdateTask(targetTask);

                    //EndTimeSlices of this task's children and suffix task
                    return await EndTimeSlice(targetTask.TaskId, DateTime.Now);
                }

        }

        public async Task<bool> SetParentTask(int targetTaskId, int parentTaskId, int pos)
        {
                if (targetTaskId<=0) return false;
            var targetTask =await taskDataStore.GetTask(targetTaskId);
            var parentTask = parentTaskId>0?await taskDataStore.GetTask(parentTaskId):null;
                //directly add to the rear of tasklist
                if (pos < 0)
                {
                    targetTask.Sort = await taskDataStore.GetMaxSort(parentTaskId) + 1;
                }
                //add to a pos
                else
                {
                    var originOrder = parentTask == null
                        ? (await GetAllTasks()).ToList()
                        : parentTask.ChildTasks.FilterDeletedAndOrderBySortTasks().ToList();
                    if (pos >= originOrder.Count) pos = originOrder.Count - 1;
                    targetTask.Sort = originOrder[pos].Sort + 1;
                    for (int i = pos + 1, currentSort = targetTask.Sort; i < originOrder.Count; i++)
                    {
                        originOrder[i].Sort = ++currentSort;
                        await EditATaskAsync(originOrder[i]);
                    }
                }

                targetTask.ParentTaskID = parentTaskId;
                taskDataStore.UpdateTask(targetTask);
                return await unitOfWork.Commit();
        }

        public async Task<bool> AddPreTask(CNTask targetTask, CNTask preTask)
        {
                if (targetTask == null || preTask == null) return false;
                if (targetTask.PreTaskConnectors.Any(r => r.PreTask.TaskId == preTask.TaskId)) return false;
                targetTask.PreTaskConnectors.Add(new CNTaskConnector {PreTask = preTask, SufTask = targetTask});

                //if pretask not complete update current task status to pending
                if (preTask.Status != CNTaskStatus.DONE)
                    await PendingATask(targetTask.TaskId, CNConstants.PENDINGREASON_PreTaskNotComplete);
                taskDataStore.UpdateTask(targetTask);
                return await unitOfWork.Commit();
        }

        public async Task<bool> DelPreTask(CNTask targetTask, CNTask preTask)
        {
                if (targetTask == null || preTask == null) return false;

                var preTasks = targetTask.PreTaskConnectors.ToList();
                var connector = preTasks.FirstOrDefault(r => r.PreTask.TaskId == preTask.TaskId);
                if (connector == null) return false;

                taskDataStore.RemoveTaskConnector(connector);

                var otherPreTasks = preTasks.Where(r => r.PreTask.TaskId != preTask.TaskId);
                if (otherPreTasks.Select(y => y.PreTask).All(x => x.Status == CNTaskStatus.DONE)
                    && targetTask.Status == CNTaskStatus.PENDING)
                {
                    targetTask.Status = CNTaskStatus.TODO;
                    targetTask.PendingReason = null;
                }

                taskDataStore.UpdateTask(targetTask);
                return await unitOfWork.Commit();
        }

        #endregion

        #region TimeSlices relevent

        private async Task DeleteAllTimeSlicesOfTaskWithoutCommit(int taskId)
        {
                var targetTask = await taskDataStore.GetTask(taskId);
                if (targetTask == null) return;

                foreach (var task in targetTask.ChildTasks ?? new List<CNTask>())
                    await DeleteAllTimeSlicesOfTaskWithoutCommit(task.TaskId);
                foreach (var task in targetTask.SufTaskConnectors.Select(y => y.SufTask))
                    await DeleteAllTimeSlicesOfTaskWithoutCommit(task.TaskId);
                targetTask.StartTime = null;
                targetTask.EndTime = null;
                taskDataStore.UpdateTask(targetTask);
                timeSliceDataStore.DeleteTimeSliceByTask(targetTask.TaskId);
        }

        public async Task<bool> DeleteAllTimeSlicesOfTask(int taskId)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) return false;
            await DeleteAllTimeSlicesOfTaskWithoutCommit(taskId);
            return await unitOfWork.Commit();
        }

        public async Task<ICollection<CNTimeSlice>> GetTaskTimeSlices(int taskid)
        {
                var task = await taskDataStore.GetTask(taskid);
                if (task == null) return new List<CNTimeSlice>();
                return await timeSliceDataStore.GetTimeSliceByTaskID(taskid);
        }

        public async Task<CNTimeSlice> AddATimeSlice(int taskid, CNTimeSlice timeSlice)
        {
                //get original task
                var task = await taskDataStore.GetTask(taskid);
                if (task == null) return null;

                //detect conflict
                if (task.UsedTimeSlices.Any(r => r.InterceptWith(timeSlice)))
                    throw new TimeSliceInterveneException();

                //Update Task's StartTime or EndTime
                var source = new List<CNTimeSlice>(task.UsedTimeSlices) {timeSlice};
                source.Sort();
                taskDataStore.ExpandTaskTime(task, source.First().StartDateTime, source.Last().EndDateTime);

                //Save TimeSlice
                timeSlice.Task = task;
                var result = timeSliceDataStore.AddTimeSlice(timeSlice);
                return await unitOfWork.Commit()?result:null;
        }


        public async Task<bool> DeleteTimeSlices(CNTimeSlice timeSlice)
        {
                //Get original slice by id
                var originSlice = await timeSliceDataStore.GetTimeSliceById(timeSlice.TimeSliceId);
                if (originSlice == null) return false;
                //Update Task's StartTime or EndTime
                var originDatas = originSlice.Task.UsedTimeSlices.ToList();

                //if only contain one slice
                if (originDatas.Count == 1)
                {
                    taskDataStore.UpdateStartTaskTime(originSlice.Task, null);
                    taskDataStore.UpdateEndTaskTime(originSlice.Task, null);
                }
                else
                {
                    //if have mutiple timeslices
                    originDatas.Sort();
                    if (Equals(originDatas.First(), timeSlice))
                    {
                        originDatas.Remove(timeSlice);
                        taskDataStore.UpdateStartTaskTime(originSlice.Task, originDatas.First().StartDateTime);
                    }
                    else if (Equals(originDatas.Last(), timeSlice))
                    {
                        originDatas.Remove(timeSlice);
                        taskDataStore.UpdateEndTaskTime(originSlice.Task, originDatas.Last().EndDateTime);
                    }
                }


                //delete the timeslice
                timeSliceDataStore.DeleteTimeSlice(originSlice);
                return await unitOfWork.Commit();
        }

        public async Task EndTimeSliceWithoutCommit(int taskId, DateTime endTime)
        {
                var originDataTask = await taskDataStore.GetTask(taskId);
                if (originDataTask?.UsedTimeSlices == null || originDataTask.UsedTimeSlices.Count == 0) return;
                var originDatas = originDataTask.UsedTimeSlices.ToList();
                originDatas.Sort();
                // if the last timeslice is ended then return true
                var lastSlice = originDatas.Last();
                if (lastSlice.EndDateTime != null) return;
                // if the last timeslice's starttime is bigger than endtime return false
                if (lastSlice.StartDateTime > endTime) return;
                lastSlice.EndDateTime = endTime;

                //EndTimeSlices of this task's children and suffix task
                foreach (var task in originDataTask.ChildTasks ?? new List<CNTask>())
                    await EndTimeSliceWithoutCommit(task.TaskId, endTime);
                foreach (var task in originDataTask.SufTaskConnectors.Select(y => y.SufTask))
                    await EndTimeSliceWithoutCommit(task.TaskId, endTime);
                taskDataStore.UpdateEndTaskTime(originDataTask, endTime);
                timeSliceDataStore.UpdateTimeSlice(lastSlice);
        }

        public async Task<bool> EndTimeSlice(int taskId, DateTime endTime)
        {
            var originDataTask = await taskDataStore.GetTask(taskId);
            if (originDataTask?.UsedTimeSlices == null || originDataTask.UsedTimeSlices.Count == 0) return false;
            var originDatas = originDataTask.UsedTimeSlices.ToList();
            originDatas.Sort();
            // if the last timeslice is ended then return true
            var lastSlice = originDatas.Last();
            if (lastSlice.EndDateTime != null) return true;
            // if the last timeslice's starttime is bigger than endtime return false
            if (lastSlice.StartDateTime > endTime) return false;
            await EndTimeSliceWithoutCommit(taskId, endTime);

            return await unitOfWork.Commit();
        }

        #endregion
        }
}