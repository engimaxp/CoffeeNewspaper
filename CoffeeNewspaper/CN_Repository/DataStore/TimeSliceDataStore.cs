using System;
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

        public async Task<CNTimeSlice> AddTimeSlice(CNTimeSlice timeSlice)
        {
            mDbContext.TimeSlices.Add(timeSlice);
            await mDbContext.SaveChangesAsync();
            return timeSlice;
        }

        #endregion

        #region Update Methods

        public async Task<bool> UpdateTimeSlice(CNTimeSlice lastSlice)
        {
            mDbContext.TimeSlices.Update(lastSlice);
            return await mDbContext.SaveChangesAsync() > 0;
        }

        #endregion

        #region Select Methods

        public async Task<CNTimeSlice> GetTimeSliceById(string timeSliceTimeSliceId)
        {
            return await mDbContext.TimeSlices.FirstOrDefaultAsync(r =>
                string.Equals(r.TimeSliceId, timeSliceTimeSliceId, StringComparison.Ordinal));
        }

        #endregion

        #region Delete Methods

        public async Task<bool> DeleteTimeSlice(CNTimeSlice originSlice)
        {
            mDbContext.TimeSlices.Remove(originSlice);
            return await mDbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTimeSliceByTask(int taskid)
        {
            mDbContext.TimeSlices.RemoveRange(mDbContext.TimeSlices.Where(r => r.TaskId == taskid));
            return await mDbContext.SaveChangesAsync() > 0;
        }

        #endregion
    }
}