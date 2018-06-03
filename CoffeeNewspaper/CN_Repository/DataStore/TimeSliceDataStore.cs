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

        public async Task<CNTimeSlice> AddTimeSlice(CNTimeSlice timeSlice)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    mDbContext.TimeSlices.Add(timeSlice);
                    await mDbContext.SaveChangesAsync();
                    return timeSlice;
                },(CNTimeSlice)null);
        }

        #endregion

        #region Update Methods

        public async Task<bool> UpdateTimeSlice(CNTimeSlice lastSlice)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    mDbContext.TimeSlices.Update(lastSlice);
                    return await mDbContext.SaveChangesAsync() > 0;
                }, false);
        }

        #endregion

        #region Select Methods

        public async Task<CNTimeSlice> GetTimeSliceById(string timeSliceTimeSliceId)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    return await mDbContext.TimeSlices.FirstOrDefaultAsync(r =>
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

        public async Task<bool> DeleteTimeSlice(CNTimeSlice originSlice)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    mDbContext.TimeSlices.Remove(originSlice);
                    return await mDbContext.SaveChangesAsync() > 0;
                }, false
            );
        }

        public async Task<bool> DeleteTimeSliceByTask(int taskid)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    mDbContext.TimeSlices.RemoveRange(mDbContext.TimeSlices.Where(r => r.TaskId == taskid));
                    return await mDbContext.SaveChangesAsync() > 0;
                }, false);
        }

        #endregion
    }
}