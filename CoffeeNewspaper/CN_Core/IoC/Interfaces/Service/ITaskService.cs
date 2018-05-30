using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CN_Core.Interfaces.Service
{
    public interface ITaskService
    {
        /// <summary>
        ///     Create a Task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<CNTask> CreateATask(CNTask task);

        /// <summary>
        ///     Delete a Task logical delete
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="force">true if ignores suftasks and childtasks</param>
        /// <returns></returns>
        Task<bool> DeleteTask(int taskId, bool force = false);

        /// <summary>
        ///     Remove a Task from db physical delete
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="force">true if ignores suftasks and childtasks</param>
        /// <returns></returns>
        Task<bool> RemoveATask(int taskId, bool force = false);

        /// <summary>
        ///     Recover a Task from delete
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<bool> RecoverATask(int taskId);

        /// <summary>
        ///     Start doing a task
        ///     means adding a timeslice with no endtime to task
        ///     if its the first time to do this task ,then add a StartTime to this task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<bool> StartATask(int taskId);

        /// <summary>
        ///     Pause a undergoing task
        ///     means updating most recent timeslice's endtime to now
        ///     and update EndTime to this Task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<bool> PauseATask(int taskId);

        /// <summary>
        ///     Finishi a task
        ///     if it's not end then updating most recent timeslice's endtime to now
        ///     and update EndTime to this Task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<bool> FinishATask(int taskId);

        /// <summary>
        ///     Fail to finishi a Task
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="reason">the reason of why failed</param>
        /// <returns></returns>
        Task<bool> FailATask(int taskId, string reason);

        /// <summary>
        ///     Update a Task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<bool> EditATask(CNTask task);

        /// <summary>
        ///     Get all tasks
        /// </summary>
        /// <returns></returns>
        Task<ICollection<CNTask>> GetAllTasks();

        /// <summary>
        ///     Get task by id
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        Task<CNTask> GetTaskById(int taskId);

        /// <summary>
        ///     Set Parent task of this task
        /// </summary>
        /// <param name="targetTask"></param>
        /// <param name="parentTask"></param>
        /// <returns></returns>
        Task<bool> SetParentTask(CNTask targetTask, CNTask parentTask);

        #region TimeSlices relevent

        /// <summary>
        ///     End all null endTime of a task and its child tasks or suf tasks
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="endTime"></param>
        Task<bool> EndTimeSlice(int taskId, DateTime endTime);

        /// <summary>
        ///     Remove all timeslices of a task and its child tasks or suf tasks permanently
        /// </summary>
        /// <param name="taskId"></param>
        Task<bool> DeleteAllTimeSlicesOfTask(int taskId);

        /// <summary>
        ///     Get all timeSlices from startdate to endDate
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        Task<ICollection<CNTimeSlice>> GetTaskTimeSlices(int taskid);

        /// <summary>
        ///     add a timeslice to task
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="timeSlice"></param>
        /// <returns>timeslice just added to the database</returns>
        Task<CNTimeSlice> AddATimeSlice(int taskid, CNTimeSlice timeSlice);

        /// <summary>
        ///     Delete timeslices
        ///     if the task of this timeslice delete effect the start-endtime
        ///     then need to update the task
        /// </summary>
        /// <param name="timeSlice"></param>
        Task<bool> DeleteTimeSlices(CNTimeSlice timeSlice);

        #endregion
    }
}