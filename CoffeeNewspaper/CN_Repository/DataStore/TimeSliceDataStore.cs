using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CN_Repository
{
    public class TimeSliceDataStore : BaseDataStore, ITimeSliceDataStore
    {
        public TimeSliceDataStore(CNDbContext dbContext) : base(dbContext)
        {
        }

        #region Add Methods

        public CNTimeSlice AddTimeSlice(CNTimeSlice timeSlice)
        {
            mDbContext.TimeSlices.Add(timeSlice);
            return timeSlice;
        }

        #endregion

        #region Update Methods

        public void UpdateTimeSlice(CNTimeSlice lastSlice)
        {
            mDbContext.TimeSlices.Update(lastSlice);
        }

        #endregion

        #region Select Methods

        public async Task<CNTimeSlice> GetTimeSliceById(string timeSliceTimeSliceId)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    return await mDbContext.TimeSlices.Include(x=>x.Task).FirstOrDefaultAsync(r =>
                        string.Equals(r.TimeSliceId, timeSliceTimeSliceId, StringComparison.Ordinal));
                });
        }

        public async Task<ICollection<CNTimeSlice>> GetTimeSliceByTaskID(int taskid)
        {
            return await IoC.Task.Run(
                async () => { return await mDbContext.TimeSlices.Where(r => r.TaskId == taskid).ToListAsync(); });
        }

        #endregion

        #region Delete Methods

        public void DeleteTimeSlice(CNTimeSlice originSlice)
        {
            mDbContext.TimeSlices.Remove(originSlice);
        }

        public void DeleteTimeSliceByTask(int taskid)
        {
            mDbContext.TimeSlices.RemoveRange(mDbContext.TimeSlices.Where(r => r.TaskId == taskid));
        }

        #endregion
    }
}