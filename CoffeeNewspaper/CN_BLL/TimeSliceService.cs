using System;
using System.Collections.Generic;
using System.Linq;
using CN_Model;
using CN_Repository;

namespace CN_BLL
{
    public class TimeSliceService : ITimeSliceService
    {
        private readonly ITimeSliceProvider timeSliceProvider;
        private readonly IRootDataProvider rootDataProvider;

        public TimeSliceService()
        {
            timeSliceProvider = TimeSliceProvider.GetProvider();
            rootDataProvider = RootDataProvider.GetProvider();
        }
        public TimeSliceService(ITimeSliceProvider timeSliceProvider)
        {
            this.timeSliceProvider = timeSliceProvider;
            rootDataProvider = RootDataProvider.GetProvider();
        }
        public TimeSliceService(ITimeSliceProvider timeSliceProvider, IRootDataProvider rootDataProvider)
        {
            this.timeSliceProvider = timeSliceProvider;
            this.rootDataProvider = rootDataProvider;
        }

        /// <summary>
        /// Get Task's All TimeSlices
        /// </summary>
        /// <param name="task"></param>
        /// <returns>not null</returns>
        public IEnumerable<CNTimeSlice> GetTaskTimeSlices(CNTask task)
        {
            if (task?.StartTime == null) return new List<CNTimeSlice>();

            var result = new List<CNTimeSlice>();
            DateTime endPointer = DateTime.Now.Date;
            if (task.EndTime != null)
            {
                endPointer = task.EndTime.Value.Date;
            }
            for (var currentPointer = task.StartTime.Value.Date;
                    currentPointer <= endPointer;
                    currentPointer = currentPointer.AddDays(1))
            {
                    var datasourceDictionary = timeSliceProvider.GetOriginalDataByDate(
                        currentPointer.ToString(CNConstants.DIRECTORY_DATEFORMAT));
                if (datasourceDictionary != null &&
                    datasourceDictionary.ContainsKey(task.TaskId) && 
                    datasourceDictionary[task.TaskId]!=null && 
                    datasourceDictionary[task.TaskId].Count>0)
                {
                    datasourceDictionary[task.TaskId].Sort();
                    var last = datasourceDictionary[task.TaskId].Last();
                    if (last.EndDateTime != null && last.EndDateTime.Value.Date >= last.StartDateTime.Date.AddDays(2))
                    {
                        currentPointer = last.EndDateTime.Value.Date.AddDays(-1);
                    }

                    result.AddRange(datasourceDictionary[task.TaskId]);
                }
            }
            return result;
        }


        public void AddATimeSlice(CNTask task, CNTimeSlice timeSlice)
        {
            var originDatas = GetTaskTimeSlices(task).ToList();
            if (originDatas.Exists(r => r.InterceptWith(timeSlice)))
                throw new ArgumentException("Time slice intervene with exist");
            var source = timeSliceProvider.GetOriginalDataByDate(timeSlice.StartDate);

            if (source.ContainsKey(task.TaskId))
            {
                source[task.TaskId].Add(timeSlice);
                source[task.TaskId].Sort();
                ExpandTaskTime(task.TaskId, source[task.TaskId].First().StartDateTime, source[task.TaskId].Last().EndDateTime);
            }
            else
            {
                source.Add(task.TaskId, new List<CNTimeSlice> {timeSlice});
                ExpandTaskTime(task.TaskId, timeSlice.StartDateTime, timeSlice.EndDateTime);
            }

            timeSliceProvider.OverWriteToDataSourceByDate(timeSlice.StartDate, source);
        }


        public void DeleteTimeSlices(CNTask task, CNTimeSlice timeSlice)
        {
            var originDatas = GetTaskTimeSlices(task).ToList();
            if (originDatas.Contains(timeSlice))
            {
                var source = timeSliceProvider.GetOriginalDataByDate(timeSlice.StartDate);
                if (source.ContainsKey(task.TaskId))
                {
                    source[task.TaskId].Remove(timeSlice);
                    source[task.TaskId].Sort();
                    if (Equals(originDatas.First(), timeSlice) )
                    {
                        UpdateStartTaskTime(task.TaskId, source[task.TaskId].First().StartDateTime);
                    }
                    else if (Equals(originDatas.Last(), timeSlice))
                    {
                        UpdateEndTaskTime(task.TaskId, source[task.TaskId].Last().EndDateTime);
                    }
                    else
                    {
                        ExpandTaskTime(task.TaskId, source[task.TaskId].First().StartDateTime, source[task.TaskId].Last().EndDateTime);
                    }
                    timeSliceProvider.OverWriteToDataSourceByDate(timeSlice.StartDate, source);
                }
            }
        }

        public void EndTimeSlice(int taskId, DateTime endTime)
        {
            var originRoot = rootDataProvider.GetRootData();
            var tasks = originRoot.GetTaskAndChildSufTasksById(taskId);
            foreach (var task in tasks)
            {
                var originDatas = GetTaskTimeSlices(task).ToList();
                originDatas.Sort();
                if (!(originDatas.Last().Clone() is CNTimeSlice lastSlice) || lastSlice.EndDateTime != null) return;
                lastSlice.EndDateTime = endTime;
                var source = timeSliceProvider.GetOriginalDataByDate(lastSlice.StartDate);
                source[task.TaskId].Remove(source[task.TaskId].Last());
                source[task.TaskId].Add(lastSlice);
                source[task.TaskId].Sort();
                timeSliceProvider.OverWriteToDataSourceByDate(lastSlice.StartDate, source);
                UpdateEndTaskTime(task.TaskId, source[task.TaskId].Last().EndDateTime);
            }
        }

        public void DeleteAllTimeSlices(int taskId)
        {
            var originRoot = rootDataProvider.GetRootData();
            var tasks = originRoot.GetTaskAndChildSufTasksById(taskId);
            foreach (var task in tasks)
            {
                var originDatas = GetTaskTimeSlices(task).ToList();
                var dates = originDatas.Select(x => x.StartDate).Distinct();
                foreach (var date in dates)
                {
                    var source = timeSliceProvider.GetOriginalDataByDate(date);
                    source.Remove(task.TaskId);
                    timeSliceProvider.OverWriteToDataSourceByDate(date, source);
                }
            }
        }

        private void ExpandTaskTime(int taskid, DateTime? targetStartTime, DateTime? targetEndTime)
        {
            var originRoot = rootDataProvider.GetRootData();
            var originDataTask = originRoot.GetTaskById(taskid);
            if (string.IsNullOrEmpty(originDataTask?.Content)) return;
            if(originDataTask.StartTime ==null || originDataTask.EndTime == null || originDataTask.StartTime > targetStartTime || originDataTask.EndTime < targetEndTime || targetEndTime == null)
            {
                if (originDataTask.StartTime == null || originDataTask.StartTime > targetStartTime)
                {
                    originDataTask.StartTime = targetStartTime;
                }
                if ((originDataTask.EndTime == null && targetEndTime != null) ||//Pause A Task
                    (originDataTask.EndTime != null && targetEndTime == null) ||//Start A Task
                    (originDataTask.EndTime != null && targetEndTime != null && originDataTask.EndTime < targetEndTime))//Update A Task timeslice
                {
                    originDataTask.EndTime = targetEndTime;
                }
                originRoot.AddOrUpdateTask(originDataTask);
                rootDataProvider.Persistence(originRoot);
            }
        }

        private void UpdateStartTaskTime(int taskid, DateTime? targetStartTime)
        {
            var originRoot = rootDataProvider.GetRootData();
            var originDataTask = originRoot.GetTaskById(taskid);
                    originDataTask.StartTime = targetStartTime;
                originRoot.AddOrUpdateTask(originDataTask);
                rootDataProvider.Persistence(originRoot);
        }
        private void UpdateEndTaskTime(int taskid, DateTime? targetEndTime)
        {
            var originRoot = rootDataProvider.GetRootData();
            var originDataTask = originRoot.GetTaskById(taskid);
            originDataTask.EndTime = targetEndTime;
            originRoot.AddOrUpdateTask(originDataTask);
            rootDataProvider.Persistence(originRoot);
        }
    }
}