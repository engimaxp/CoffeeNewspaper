using System;
using System.Collections.Generic;
using CN_Model;

namespace CN_BLL
{
    public interface ITimeSliceService
    {
        /// <summary>
        /// Get all timeSlices from startdate to endDate
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        IEnumerable<CNTimeSlice> GetTaskTimeSlices(CNTask task);

        /// <summary>
        /// add a timeslice to task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="timeSlice"></param>
        void AddATimeSlice(CNTask task, CNTimeSlice timeSlice);

        /// <summary>
        /// Delete timeslices
        /// </summary>
        /// <param name="task"></param>
        /// <param name="timeSlice"></param>
        void DeleteTimeSlices(CNTask task, CNTimeSlice timeSlice);

        /// <summary>
        /// End all null endTime of a task and its child tasks or suf tasks
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="endTime"></param>
        void EndTimeSlice(int taskId, DateTime endTime);

        /// <summary>
        /// Remove all timeslices of a task and its child tasks or suf tasks permanently
        /// </summary>
        /// <param name="taskId"></param>
        void DeleteAllTimeSlices(int taskId);
    }
}
