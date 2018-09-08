﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CN_Repository;
using CoffeeNewspaper_UnitTest.DomainTest;
using NUnit.Framework;

namespace CoffeeNewspaper_UnitTest.RepositoryTest
{
    /// <summary>
    ///     In-TimeSlicery integration data persistent test
    /// </summary>
    [TestFixture]
    public class TimeSliceDataStoreTest : RepositarySetupAndTearDown
    {
        [Test]
        public async Task AddTimeSlice_QueryAllTimeSlice_QuerySpecifiedTimeSlice_AllPass()
        {
            await UseMemoryContextRun(async (dbcontext,unitOfWork) =>
            {
                //Arrange argument to be tested
                var assesTimeSlice = DomainTestHelper.GetARandomTimeSlice();
                var task = DomainTestHelper.GetARandomTask();

                //add this timeslice to task
                assesTimeSlice.Task = task;

                //initiate the datastore
                var TimeSliceDataStore = new TimeSliceDataStore(dbcontext);

                //do testing action
                var addedTimeSlice = TimeSliceDataStore.AddTimeSlice(assesTimeSlice);

                await unitOfWork.Commit();
                //query the result from db for assert
                var TimeSlices = await TimeSliceDataStore.GetTimeSliceByTaskID(addedTimeSlice.TaskId);

                var firstTimeSlice = await TimeSliceDataStore.GetTimeSliceById(TimeSlices.First().TimeSliceId);

                Assert.AreEqual(TimeSlices.First(), firstTimeSlice);
                Assert.AreEqual(TimeSlices.FirstOrDefault(), assesTimeSlice);
            });
        }

        [Test]
        public async Task AddTimeSlice_WithoutTask_Fail()
        {
            await UseMemoryContextRun(async (dbcontext,unitOfWork) =>
            {
                //Arrange argument to be tested
                var assesTimeSlice = DomainTestHelper.GetARandomTimeSlice();

                //initiate the datastore
                var TimeSliceDataStore = new TimeSliceDataStore(dbcontext);

                //do testing action
                 TimeSliceDataStore.AddTimeSlice(assesTimeSlice);

                var result = await unitOfWork.Commit();
                Assert.IsFalse(result);
            });
        }

        [Test]
        public async Task DeleteATimeSlice_Success()
        {
            await UseMemoryContextRun(async (dbcontext,unitOfWork) =>
            {
                var TimeSliceDataStore = new TimeSliceDataStore(dbcontext);
                //Arrange argument to be tested
                var assesTimeSlice = DomainTestHelper.GetARandomTimeSlice();
                var task = DomainTestHelper.GetARandomTask();

                //add this timeslice to task
                assesTimeSlice.Task = task;

                var addedTimeSlice = TimeSliceDataStore.AddTimeSlice(assesTimeSlice);

                await unitOfWork.Commit();

                 TimeSliceDataStore.DeleteTimeSlice(addedTimeSlice);

                var beforeDeleteResult = await unitOfWork.Commit();
                Assert.IsTrue(beforeDeleteResult);

                var afterDeleteResult = await TimeSliceDataStore.GetTimeSliceById(addedTimeSlice.TimeSliceId);
                Assert.IsNull(afterDeleteResult);
            });
        }

        [Test]
        public async Task DeleteATimeSlice_TimeSliceIdNotExist_Fail()
        {
            await UseMemoryContextRun(async (dbcontext,unitOfWork) =>
            {
                var TimeSliceDataStore = new TimeSliceDataStore(dbcontext);
                var assesTimeSlice = DomainTestHelper.GetARandomTimeSlice();
                assesTimeSlice.TimeSliceId = Guid.NewGuid().ToString("D");
                TimeSliceDataStore.DeleteTimeSlice(assesTimeSlice);
                var beforeDeleteResult = await unitOfWork.Commit();
                Assert.IsFalse(beforeDeleteResult);
            });
        }

        [Test]
        public async Task QuerySpecifiedTimeSliceDoesntExist_ReturnNull()
        {
            await UseMemoryContextRun(async (dbcontext,unitOfWork) =>
            {
                var TimeSliceDataStore = new TimeSliceDataStore(dbcontext);
                var firstTimeSlice = await TimeSliceDataStore.GetTimeSliceById(Guid.NewGuid().ToString("D"));

                Assert.IsNull(firstTimeSlice);
            });
        }

        [Test]
        public async Task UpdateTimeSlice()
        {
            await UseMemoryContextRun(async (dbcontext,unitOfWork) =>
            {
                var TimeSliceDataStore = new TimeSliceDataStore(dbcontext);
                //Arrange argument to be tested
                var assesTimeSlice = DomainTestHelper.GetARandomTimeSlice();
                var task = DomainTestHelper.GetARandomTask();

                //add this timeslice to task
                assesTimeSlice.Task = task;
                var addedTimeSlice = TimeSliceDataStore.AddTimeSlice(assesTimeSlice);
                await unitOfWork.Commit();
                //update the added TimeSlice content,make sure its not equal to original TimeSlice
                addedTimeSlice.EndDateTime = DateTime.Now.AddDays(1);

                //update to database ,make sure what ef return is equal to modified TimeSlice
                TimeSliceDataStore.UpdateTimeSlice(addedTimeSlice);
                var updatedResult = await unitOfWork.Commit();
                Assert.IsTrue(updatedResult);
                //select from db again ,make sure the TimeSlice is updated
                var selectedTimeSlice = await TimeSliceDataStore.GetTimeSliceById(addedTimeSlice.TimeSliceId);
                Assert.AreEqual(selectedTimeSlice, addedTimeSlice);
            });
        }

        [Test]
        public async Task UpdateTimeSlice_TimeSliceIdNotExist_Fail()
        {
            await UseMemoryContextRun(async (dbcontext,unitOfWork) =>
            {
                var TimeSliceDataStore = new TimeSliceDataStore(dbcontext);
                var assesTimeSlice = DomainTestHelper.GetARandomTimeSlice();
                assesTimeSlice.TimeSliceId = Guid.NewGuid().ToString("D");

                //update the added TimeSlice content,make sure its not equal to original TimeSlice
                assesTimeSlice.EndDateTime = DateTime.Now.AddDays(1);

                //update to database ,make sure what ef return is equal to modified TimeSlice
                 TimeSliceDataStore.UpdateTimeSlice(assesTimeSlice);
                var updatedResult = await unitOfWork.Commit();
                Assert.False(updatedResult);
            });
        }

        [Test]
        public async Task UpdateTimeSlice_TryDeleteTheTaskId_Fail()
        {
            await UseMemoryContextRun(async (dbcontext,unitOfWork) =>
            {
                var TimeSliceDataStore = new TimeSliceDataStore(dbcontext);
                //Arrange argument to be tested
                var assesTimeSlice = DomainTestHelper.GetARandomTimeSlice();
                var task = DomainTestHelper.GetARandomTask();

                //add this timeslice to task
                assesTimeSlice.Task = task;
                var addedTimeSlice = TimeSliceDataStore.AddTimeSlice(assesTimeSlice);
                await unitOfWork.Commit();
                //try delete the task relation
                addedTimeSlice.Task = null;
                addedTimeSlice.TaskId = 0;

                //update to database ,make sure what ef return is equal to modified TimeSlice
                TimeSliceDataStore.UpdateTimeSlice(addedTimeSlice);
                var updatedResult = await unitOfWork.Commit();
                Assert.IsFalse(updatedResult);
            });
        }

        [Test] 
        public async Task DeleteTimeSliceByTask_Success()
        {
            await UseMemoryContextRun(async (dbcontext,unitOfWork) =>
            {
                var TimeSliceDataStore = new TimeSliceDataStore(dbcontext);
                //Arrange argument to be tested
                var assesTimeSlice = DomainTestHelper.GetARandomTimeSlice();
                var task = DomainTestHelper.GetARandomTask();
                //add this timeslice to task
                task.UsedTimeSlices.Add(assesTimeSlice);
                var TaskDataStore = new TaskDataStore(dbcontext);
                TaskDataStore.AddTask(task);
                await unitOfWork.Commit();
                //action
                TimeSliceDataStore.DeleteTimeSliceByTask(task.TaskId);
                var result = await unitOfWork.Commit();
                Assert.IsTrue(result);

                var finalTask = await TaskDataStore.GetTask(task.TaskId);
                Assert.IsTrue(finalTask.UsedTimeSlices== null|| finalTask.UsedTimeSlices.Count == 0);
            });
        }
    }
}