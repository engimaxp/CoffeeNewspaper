using System.Collections.Generic;
using System.Threading.Tasks;

namespace CN_Core.Interfaces.Repository
{
    public interface ITimeSliceDataStore
    {
        #region Add Methods

        /// <summary>
        ///     Add a timeslice
        /// </summary>
        /// <param name="timeSlice"></param>
        /// <returns></returns>
        CNTimeSlice AddTimeSlice(CNTimeSlice timeSlice);

        #endregion

        #region Select Method

        /// <summary>
        ///     Get a TimeSlice by id
        /// </summary>
        /// <param name="timeSliceTimeSliceId"></param>
        /// <returns></returns>
        Task<CNTimeSlice> GetTimeSliceById(string timeSliceTimeSliceId);

        /// <summary>
        /// Get a task's timeslice list
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        Task<ICollection<CNTimeSlice>> GetTimeSliceByTaskID(int taskid);
        #endregion

        #region Update Methods

        /// <summary>
        ///     Update a TimeSlice
        /// </summary>
        /// <param name="lastSlice"></param>
        /// <returns></returns>
        void UpdateTimeSlice(CNTimeSlice lastSlice);

        #endregion

        #region Delete Methods

        /// <summary>
        ///     Delet a TimeSlice
        /// </summary>
        /// <param name="originSlice"></param>
        /// <returns></returns>
        void DeleteTimeSlice(CNTimeSlice originSlice);

        /// <summary>
        ///     Delet TimeSlices By taskid
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        void DeleteTimeSliceByTask(int taskid);

        #endregion
    }
}