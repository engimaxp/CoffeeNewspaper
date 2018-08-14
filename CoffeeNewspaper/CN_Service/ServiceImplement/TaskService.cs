using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Repository;
using CN_Core.Interfaces.Service;
using CN_Core.Utilities;

namespace CN_Service
{
    public class TaskService : ITaskService
    {
        #region Construct

        public TaskService(ITimeSliceDataStore TimeSliceDataStore, ITaskDataStore TaskDataStore)
        {
            timeSliceDataStore = TimeSliceDataStore;
            taskDataStore = TaskDataStore;
        }

        #endregion

        #region Private Properties

        private readonly ITaskDataStore taskDataStore;
        private readonly ITimeSliceDataStore timeSliceDataStore;

        #endregion

        #region Task relevent

        public Task<bool> EditATask(CNTask task)
        {
            if (task == null || task.TaskId <= 0) return Task.FromResult(false);
            if (string.IsNullOrEmpty(task.Content) || string.IsNullOrEmpty(task.Content.Trim())) throw new ArgumentException("The Content Field is needed");
            return EditATaskAsync(task);
        }

        private async Task<bool> EditATaskAsync(CNTask task)
        {
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

        public Task<CNTask> CreateATask(CNTask task)
        {
            if (string.IsNullOrEmpty(task.Content) || string.IsNullOrEmpty(task.Content.Trim())) throw new ArgumentException("The Content Field is needed");
            return CreateATaskAsync(task);
        }

        private async Task<CNTask> CreateATaskAsync(CNTask task)
        {
            var parentTaskId = task.ParentTask?.TaskId ?? task.ParentTaskID;
            task.Sort = await taskDataStore.GetMaxSort(parentTaskId) + 1;
            return await taskDataStore.AddTask(task);
        }

        public async Task<bool> DeleteTask(int taskId, bool force = false)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) return false;
            if (targetTask.IsDeleted) return true;
            if (targetTask.ChildTasks.FilterDeletedAndOrderBySortTasks().Any() && !force) throw new TaskHasChildTasksException();
            if (targetTask.SufTaskConnectors.Select(y=>y.SufTask).FilterDeletedAndOrderBySortTasks().Any() && !force) throw new TaskHasSufTasksException();
            //Delete the task itself
            targetTask.IsDeleted = true;
            //Delete the tasks children and suffix task
            foreach (var task in targetTask.ChildTasks.FilterDeletedAndOrderBySortTasks())
            {
                await DeleteTask(task.TaskId, force);
            }
            foreach (var task in targetTask.SufTaskConnectors.Select(y => y.SufTask).FilterDeletedAndOrderBySortTasks())
            {
                await DeleteTask(task.TaskId, force);
            }
            return await taskDataStore.UpdateTask(targetTask);
        }

        public async Task<bool> RecoverATask(int taskId)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) return false; 
            if (!targetTask.IsDeleted) return true;

            //Recover the task itself
            targetTask.IsDeleted = false;
            //Recover the tasks children and suffix task
            foreach (var task in targetTask.ChildTasks)
            {
                await RecoverATask(task.TaskId);
            }
            foreach (var task in targetTask.SufTaskConnectors.Select(y => y.SufTask))
            {
                await RecoverATask(task.TaskId);
            }
            return await taskDataStore.UpdateTask(targetTask);
        }

        public async Task<bool> StartATask(int taskId)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) return false;
            if (targetTask.IsDeleted) return false; 
            if (targetTask.Status != CNTaskStatus.TODO && targetTask.Status != CNTaskStatus.PENDING)
                throw new TaskStatusException(new List<CNTaskStatus> {CNTaskStatus.TODO, CNTaskStatus.PENDING},
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
            if (targetTask == null) return false;
            if (targetTask.IsDeleted) return false;
            if (targetTask.Status != CNTaskStatus.DOING)
                throw new TaskStatusException(new List<CNTaskStatus> {CNTaskStatus.DOING}, targetTask.Status);
            targetTask.Status = CNTaskStatus.TODO;
            await taskDataStore.UpdateTask(targetTask);

            //EndTimeSlices of this task's children and suffix task
            await EndTimeSlice(targetTask.TaskId, DateTime.Now);
            return true;
        }

        public async Task<bool> PendingATask(int TaskId,string reason)
        {
            var targetTask = await taskDataStore.GetTask(TaskId);
            if (targetTask == null) return false;
            if (targetTask.IsDeleted) return false;
            if (targetTask.Status != CNTaskStatus.DOING
                && targetTask.Status != CNTaskStatus.TODO
                && targetTask.Status != CNTaskStatus.PENDING)
                throw new TaskStatusException(new List<CNTaskStatus> { CNTaskStatus.DOING, CNTaskStatus.TODO, CNTaskStatus.PENDING }, targetTask.Status);
            //EndTimeSlices of this task's children and suffix task
            if (targetTask.Status == CNTaskStatus.DOING)
            {
                await EndTimeSlice(targetTask.TaskId, DateTime.Now);
            }
            targetTask.Status = CNTaskStatus.PENDING;
            targetTask.PendingReason = reason;
            return await taskDataStore.UpdateTask(targetTask);
        }

        public async Task<bool> RemoveATask(int taskId, bool force = false)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) return false;
            if (!targetTask.IsDeleted) return false;
            if (targetTask.ChildTasks.Any() && !force) throw new TaskHasChildTasksException();
            if (targetTask.SufTaskConnectors.Count > 0 && !force) throw new TaskHasSufTasksException();

            //Remove the tasks children and suffix task
            foreach (var task in targetTask.ChildTasks)
            {
                await RemoveATask(task.TaskId, force);
            }
            foreach (var task in targetTask.SufTaskConnectors.Select(y => y.SufTask))
            {
                await RemoveATask(task.TaskId, force);
            }
            return await taskDataStore.RemoveTask(targetTask);
        }

        public async Task<bool> FinishATask(int taskId)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) return false;
            if (targetTask.IsDeleted) return false;
            if (targetTask.Status == CNTaskStatus.DONE)
                throw new TaskStatusException(new List<CNTaskStatus> {CNTaskStatus.DOING, CNTaskStatus.TODO, CNTaskStatus.PENDING },
                    targetTask.Status);
            targetTask.Status = CNTaskStatus.DONE;
            await taskDataStore.UpdateTask(targetTask);

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
                return await taskDataStore.UpdateTask(targetTask);
            }

            //tag this as filed
            targetTask.IsFail = true;
            targetTask.FailReason = reason;
            await taskDataStore.UpdateTask(targetTask);

            //EndTimeSlices of this task's children and suffix task
            await EndTimeSlice(targetTask.TaskId, DateTime.Now);
            return true;
        }

        public async Task<bool> SetParentTask(CNTask targetTask, CNTask parentTask,int pos)
        {
            if (targetTask == null) return false;
            //directly add to the rear of tasklist
            if (pos < 0)
            {
                targetTask.Sort = await taskDataStore.GetMaxSort(parentTask?.TaskId) + 1;
            }
            //add to a pos
            else
            {
                var originOrder = parentTask == null ? (await GetAllTasks()).ToList() : parentTask.ChildTasks.FilterDeletedAndOrderBySortTasks().ToList();
                if (pos >= originOrder.Count) pos = originOrder.Count - 1;
                targetTask.Sort = originOrder[pos].Sort+1;
                for (int i = pos+1, currentSort = targetTask.Sort; i < originOrder.Count; i++)
                {
                    originOrder[i].Sort = ++currentSort;
                    await EditATaskAsync(originOrder[i]);
                }
            }
            targetTask.ParentTask = parentTask;
            return await taskDataStore.UpdateTask(targetTask);
        }

        public async Task<bool> AddPreTask(CNTask targetTask, CNTask preTask)
        {
            if (targetTask == null || preTask == null)
            {
                return false;
            }
            if (targetTask.PreTaskConnectors.Any(r => r.PreTask.TaskId == preTask.TaskId))
            {
                return false;
            }
            targetTask.PreTaskConnectors.Add(new CNTaskConnector(){PreTask = preTask,SufTask = targetTask});

            //if pretask not complete update current task status to pending
            if (preTask.Status != CNTaskStatus.DONE)
            {
                await PendingATask(targetTask.TaskId, CNConstants.PENDINGREASON_PreTaskNotComplete);
            }
            return await taskDataStore.UpdateTask(targetTask);
        }

        public async Task<bool> DelPreTask(CNTask targetTask, CNTask preTask)
        {
            if (targetTask == null || preTask == null)
            {
                return false;
            }

            var preTasks = targetTask.PreTaskConnectors.ToList();
            var  connector = preTasks.FirstOrDefault(r => r.PreTask.TaskId == preTask.TaskId);
            if (connector == null) return false;

            if (await taskDataStore.RemoveTaskConnector(connector))
            {
                var otherPreTasks = preTasks.Where(r => r.PreTask.TaskId != preTask.TaskId);
                if (otherPreTasks.Select(y=>y.PreTask).All(x=>x.Status == CNTaskStatus.DONE)
                    && targetTask.Status == CNTaskStatus.PENDING)
                {
                    targetTask.Status = CNTaskStatus.TODO;
                    targetTask.PendingReason = null;
                }
                return await taskDataStore.UpdateTask(targetTask);
            }

            return false;
        }
        #endregion

        #region TimeSlices relevent

        public async Task<bool> DeleteAllTimeSlicesOfTask(int taskId)
        {
            var targetTask = await taskDataStore.GetTask(taskId);
            if (targetTask == null) return false;

            foreach (var task in targetTask.ChildTasks??new List<CNTask>())
            {
                await DeleteAllTimeSlicesOfTask(task.TaskId);
            }
            foreach (var task in targetTask.SufTaskConnectors.Select(y => y.SufTask))
            {
                await DeleteAllTimeSlicesOfTask(task.TaskId);
            }
            targetTask.StartTime = null;
            targetTask.EndTime = null;
            await taskDataStore.UpdateTask(targetTask);
            return await timeSliceDataStore.DeleteTimeSliceByTask(targetTask.TaskId);
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
            await taskDataStore.ExpandTaskTime(task, source.First().StartDateTime, source.Last().EndDateTime);

            //Save TimeSlice
            timeSlice.Task = task;
            return await timeSliceDataStore.AddTimeSlice(timeSlice);
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
                await taskDataStore.UpdateStartTaskTime(originSlice.Task, null);
                await taskDataStore.UpdateEndTaskTime(originSlice.Task, null);
            }
            else
            {
                //if have mutiple timeslices
                originDatas.Sort();
                if (Equals(originDatas.First(), timeSlice))
                {
                    originDatas.Remove(timeSlice);
                    await taskDataStore.UpdateStartTaskTime(originSlice.Task, originDatas.First().StartDateTime);
                }
                else if (Equals(originDatas.Last(), timeSlice))
                {
                    originDatas.Remove(timeSlice);
                    await taskDataStore.UpdateEndTaskTime(originSlice.Task, originDatas.Last().EndDateTime);
                }
            }
            

            //delete the timeslice
            return await timeSliceDataStore.DeleteTimeSlice(originSlice);
        }

        public async Task<bool> EndTimeSlice(int taskId, DateTime endTime)
        {
            var originDataTask = await taskDataStore.GetTask(taskId);
            if (originDataTask?.UsedTimeSlices == null || originDataTask.UsedTimeSlices.Count == 0) return false;
            var originDatas = originDataTask.UsedTimeSlices.ToList();
            originDatas.Sort();
            // if the last timeslice is ended then return true
            if (!(originDatas.Last().Clone() is CNTimeSlice lastSlice) || lastSlice.EndDateTime != null) return true;
            // if the last timeslice's starttime is bigger than endtime return false
            if (lastSlice.StartDateTime > endTime) return false;
            lastSlice.EndDateTime = endTime;

            //EndTimeSlices of this task's children and suffix task
            foreach (var task in originDataTask.ChildTasks ?? new List<CNTask>())
            {
                await EndTimeSlice(task.TaskId, endTime);
            }
            foreach (var task in originDataTask.SufTaskConnectors.Select(y => y.SufTask))
            {
                await EndTimeSlice(task.TaskId, endTime);
            }
            await taskDataStore.UpdateEndTaskTime(originDataTask, endTime);
            return await timeSliceDataStore.UpdateTimeSlice(lastSlice);
        }

        #endregion
    }
}