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
                if (datasourceDictionary != null && datasourceDictionary.ContainsKey(task.TaskId))
                {
                    result.AddRange(datasourceDictionary[task.TaskId]);
                }
            }
            return result.OrderBy(r=>r.StartDateTime);
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
                source[task.TaskId] = source[task.TaskId].OrderBy(r => r.StartDateTime).ToList();
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
                    source[task.TaskId] = source[task.TaskId].OrderBy(r => r.StartDateTime).ToList();
                    if (Equals(originDatas.First(), timeSlice) )
                    {
                        ShrinkStartTaskTime(task.TaskId, source[task.TaskId].First().StartDateTime);
                    }
                    else if (Equals(originDatas.Last(), timeSlice))
                    {
                        ShrinkEndTaskTime(task.TaskId, source[task.TaskId].Last().EndDateTime);
                    }
                    else
                    {
                        ExpandTaskTime(task.TaskId, source[task.TaskId].First().StartDateTime, source[task.TaskId].Last().EndDateTime);
                    }
                    timeSliceProvider.OverWriteToDataSourceByDate(timeSlice.StartDate, source);
                }
            }
        }

        private void ExpandTaskTime(int taskid, DateTime? targetStartTime, DateTime? targetEndTime)
        {
            var originRoot = rootDataProvider.GetRootData();
            var originDataTask = originRoot.GetTaskById(taskid);
            if(originDataTask.StartTime ==null || originDataTask.EndTime == null || originDataTask.StartTime > targetStartTime || originDataTask.EndTime < targetEndTime)
            {
                if (originDataTask.StartTime == null || originDataTask.StartTime > targetStartTime)
                {
                    originDataTask.StartTime = targetStartTime;
                }
                if (originDataTask.EndTime == null || originDataTask.EndTime < targetEndTime)
                {
                    originDataTask.EndTime = targetEndTime;
                }
                originRoot.AddOrUpdateTask(originDataTask);
                rootDataProvider.Persistence(originRoot);
            }
        }

        private void ShrinkStartTaskTime(int taskid, DateTime? targetStartTime)
        {
            var originRoot = rootDataProvider.GetRootData();
            var originDataTask = originRoot.GetTaskById(taskid);
                    originDataTask.StartTime = targetStartTime;
                originRoot.AddOrUpdateTask(originDataTask);
                rootDataProvider.Persistence(originRoot);
        }
        private void ShrinkEndTaskTime(int taskid, DateTime? targetEndTime)
        {
            var originRoot = rootDataProvider.GetRootData();
            var originDataTask = originRoot.GetTaskById(taskid);
            originDataTask.EndTime = targetEndTime;
            originRoot.AddOrUpdateTask(originDataTask);
            rootDataProvider.Persistence(originRoot);
        }
    }
}