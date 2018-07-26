using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Repository;
using CoffeeNewspaper_UnitTest.DomainTest;
using NUnit.Framework;

namespace CoffeeNewspaper_UnitTest.RepositoryTest
{
    /// <summary>
    ///     In-Memory integration data persistent test
    /// </summary>
    [TestFixture]
    public class TaskDataStoreTest : RepositarySetupAndTearDown
    {
        [Test]
        public async Task AddTask_QueryAllTask_QuerySpecifiedTask_AllPass()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //Arrange argument to be tested
                var assesTask = DomainTestHelper.GetARandomTask();

                //initiate the datastore
                var taskDataStore = new TaskDataStore(dbcontext);

                //do testing action
                await taskDataStore.AddTask(assesTask);

                //query the result from db for assert
                var tasks = await taskDataStore.GetAllTask();
                tasks.ToList().ForEach(Console.WriteLine);

                var firstTask = await taskDataStore.GetTask(tasks.First().TaskId);

                Assert.AreEqual(tasks.First(), firstTask);
                Assert.AreEqual(tasks.FirstOrDefault(), assesTask);
            });
        }

        [Test]
        public async Task DeleteATask_Success()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);


                var beforeDeleteResult = await taskDataStore.RemoveTask(addedTask);
                Assert.IsTrue(beforeDeleteResult);

                var afterDeleteResult = await taskDataStore.GetTask(addedTask.TaskId);
                Assert.IsNull(afterDeleteResult);
            });
        }

        [Test]
        public async Task DeleteATask_TaskIdNotExist_Fail()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask(1);
                var beforeDeleteResult = await taskDataStore.RemoveTask(assesTask);
                Assert.IsFalse(beforeDeleteResult);
            });
        }

        [Test]
        public async Task QuerySpecifiedTaskDoesntExist_ReturnNull()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var firstTask = await taskDataStore.GetTask(1);

                Assert.IsNull(firstTask);
            });
        }

        [Test]
        public async Task UpdateTask()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);

                //update the added task content,make sure its not equal to original task
                addedTask.Content = "testing update";

                //update to database ,make sure what ef return is equal to modified task
                var updatedResult = await taskDataStore.UpdateTask(addedTask, addedTask);

                Assert.IsTrue(updatedResult);
                //select from db again ,make sure the task is updated
                var selectedTask = await taskDataStore.GetTask(addedTask.TaskId);
                Assert.AreEqual(selectedTask, addedTask);
            });
        }

        [Test]
        public async Task UpdateTask_TaskIdNotExist_Fail()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask(1);

                //update the added task content,make sure its not equal to original task
                assesTask.Content = "testing update";

                //update to database ,make sure what ef return is equal to modified task
                var updatedResult = await taskDataStore.UpdateTask(assesTask);

                Assert.False(updatedResult);
            });
        }
        [Test]
        public async Task UpdateEndTaskTime_Success()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);

                //update to database ,make sure what ef return is equal to modified task
                var nowtime = DateTime.Now;
                await taskDataStore.UpdateEndTaskTime(assesTask, nowtime);

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.IsNotNull(finalTask.EndTime);
                Assert.AreEqual(finalTask.EndTime, nowtime);
            });
        }
        [Test]
        public async Task UpdateEndTaskTime_OriginalTaskNull_ReturnFalse()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);

                //update to database ,make sure what ef return is equal to modified task
                var nowtime = DateTime.Now;
                await taskDataStore.UpdateEndTaskTime(null, nowtime);

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.IsNull(finalTask.EndTime);
            });
        }
        [Test]
        public async Task UpdateEndTaskTime_OriginalTaskEndTimeNull_TargetTimeNull_ReturnFalse()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);

                //update to database ,make sure what ef return is equal to modified task
                var nowtime = DateTime.Now;
                await taskDataStore.UpdateEndTaskTime(assesTask, null);

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.IsNull(finalTask.EndTime);
            });
        }
        [Test]
        public async Task UpdateEndTaskTime_OriginalTaskEndTimeEqualsTargetTime_ReturnFalse()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var nowtime = DateTime.Now;
                assesTask.EndTime = nowtime;
                var addedTask = await taskDataStore.AddTask(assesTask);

                //update to database ,make sure what ef return is equal to modified task
                await taskDataStore.UpdateEndTaskTime(assesTask, nowtime);

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.IsNotNull(finalTask.EndTime);
                Assert.AreEqual(finalTask.EndTime, nowtime);
            });
        }

        [Test]
        public async Task UpdateStartTaskTime_Success()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);

                //update to database ,make sure what ef return is equal to modified task
                var nowtime = DateTime.Now;
                await taskDataStore.UpdateStartTaskTime(assesTask, nowtime);

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.IsNotNull(finalTask.StartTime);
                Assert.AreEqual(finalTask.StartTime, nowtime);
            });
        }
        [Test]
        public async Task UpdateStartTaskTime_OriginalTaskNull_ReturnFalse()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);

                //update to database ,make sure what ef return is equal to modified task
                var nowtime = DateTime.Now;
                await taskDataStore.UpdateStartTaskTime(null, nowtime);

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.IsNull(finalTask.StartTime);
            });
        }
        [Test]
        public async Task UpdateStartTaskTime_OriginalTaskStartTimeNull_TargetTimeNull_ReturnFalse()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);

                //update to database ,make sure what ef return is equal to modified task
                var nowtime = DateTime.Now;
                await taskDataStore.UpdateStartTaskTime(assesTask, null);

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.IsNull(finalTask.StartTime);
            });
        }

        [Test]
        public async Task UpdateStartTaskTime_OriginalTaskStartTimeEqualsTargetTime_ReturnFalse()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var nowtime = DateTime.Now;
                assesTask.StartTime = nowtime;
                var addedTask = await taskDataStore.AddTask(assesTask);

                //update to database ,make sure what ef return is equal to modified task
                await taskDataStore.UpdateStartTaskTime(assesTask, nowtime);

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.IsNotNull(finalTask.StartTime);
                Assert.AreEqual(finalTask.StartTime, nowtime);
            });
        }
        [Test]
        public async Task ExpandTaskTime_TaskisNull_DirectReturn()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);

                //update to database ,make sure what ef return is equal to modified task
                var nowtime = DateTime.Now;
                await taskDataStore.ExpandTaskTime(null, nowtime,nowtime.AddDays(1));

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.IsNull(finalTask.StartTime);
                Assert.IsNull(finalTask.EndTime);
            });
        }
        [Test]
        public async Task ExpandTaskTime_SmallerOrEqual_DirectReturn()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);
                //update to database ,make sure what ef return is equal to modified task

                var nowtime = DateTime.Now;
                addedTask.StartTime = nowtime;
                addedTask.EndTime = nowtime.AddDays(1);
                await taskDataStore.ExpandTaskTime(addedTask, addedTask.StartTime, addedTask.EndTime);

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.AreEqual(finalTask.StartTime, addedTask.StartTime);
                Assert.AreEqual(finalTask.EndTime, addedTask.EndTime);
                
                await taskDataStore.ExpandTaskTime(addedTask, nowtime.AddMinutes(1), nowtime.AddDays(1).AddMinutes(-1));

                finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.AreEqual(addedTask.StartTime,finalTask.StartTime);
                Assert.AreEqual(addedTask.EndTime,finalTask.EndTime);
            });
        }
        [Test]
        public async Task ExpandTaskTime_StartTimeSmaller_UpdateStartTime()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);
                //update to database ,make sure what ef return is equal to modified task

                var nowtime = DateTime.Now;
                addedTask.StartTime = nowtime;
                addedTask.EndTime = nowtime.AddDays(1);
                await taskDataStore.ExpandTaskTime(addedTask, nowtime.AddMilliseconds(-1), addedTask.EndTime);

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.AreEqual(nowtime.AddMilliseconds(-1), finalTask.StartTime);
                Assert.AreEqual(addedTask.EndTime,finalTask.EndTime);
            });
        }
        [Test]
        public async Task ExpandTaskTime_EndTimeBigger_UpdateEndTime()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);
                //update to database ,make sure what ef return is equal to modified task

                var nowtime = DateTime.Now;
                addedTask.StartTime = nowtime;
                addedTask.EndTime = nowtime.AddDays(1);
                await taskDataStore.ExpandTaskTime(addedTask, addedTask.StartTime, nowtime.AddDays(1).AddMilliseconds(1));

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.AreEqual(addedTask.StartTime, finalTask.StartTime);
                Assert.AreEqual(nowtime.AddDays(1).AddMilliseconds(1), finalTask.EndTime);
            });
        }
        [Test]
        public async Task ExpandTaskTime_OriginEndTimeNull_UpdateEndTime()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);
                //update to database ,make sure what ef return is equal to modified task

                var nowtime = DateTime.Now;
                addedTask.StartTime = nowtime;
                addedTask.EndTime = null;
                await taskDataStore.ExpandTaskTime(addedTask, addedTask.StartTime, addedTask.EndTime?.AddMilliseconds(1));

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.AreEqual(addedTask.StartTime, finalTask.StartTime);
                Assert.AreEqual(addedTask.EndTime?.AddMilliseconds(1), finalTask.EndTime);
            });
        }
        [Test]
        public async Task ExpandTaskTime_TargetEndTimeNull_UpdateEndTime()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);
                //update to database ,make sure what ef return is equal to modified task

                var nowtime = DateTime.Now;
                addedTask.StartTime = nowtime;
                addedTask.EndTime = nowtime.AddDays(1);
                await taskDataStore.ExpandTaskTime(addedTask, addedTask.StartTime, null);

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.AreEqual(addedTask.StartTime, finalTask.StartTime);
                Assert.AreEqual(null, finalTask.EndTime);
            });
        }
        [Test]
        public async Task ExpandTaskTime_BothEndTimeNull_Return()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask = DomainTestHelper.GetARandomTask();
                var addedTask = await taskDataStore.AddTask(assesTask);
                //update to database ,make sure what ef return is equal to modified task

                var nowtime = DateTime.Now;
                addedTask.StartTime = nowtime;
                addedTask.EndTime = null;
                await taskDataStore.ExpandTaskTime(addedTask, addedTask.StartTime, null);

                var finalTask = await taskDataStore.GetTask(assesTask.TaskId);
                Assert.AreEqual(addedTask.StartTime, finalTask.StartTime);
                Assert.AreEqual(null, finalTask.EndTime);
            });
        }
        [Test]
        public async Task RemoveTaskConnector_Success()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var taskDataStore = new TaskDataStore(dbcontext);
                var assesTask1 = DomainTestHelper.GetARandomTask();
                var assesTask2 = DomainTestHelper.GetARandomTask();
                var connector = new CNTaskConnector() {PreTask = assesTask2, SufTask = assesTask1};
                assesTask1.PreTaskConnectors = new List<CNTaskConnector>(){ connector };
                var addedTask = await taskDataStore.AddTask(assesTask1);
                //update to database ,make sure what ef return is equal to modified task
                
                await taskDataStore.RemoveTaskConnector(connector);

                var finalTask1 = await taskDataStore.GetTask(assesTask1.TaskId);
                Assert.IsNotNull(finalTask1);
                Assert.IsTrue(assesTask1.PreTaskConnectors == null || assesTask1.PreTaskConnectors.Count == 0);
                var finalTask2 = await taskDataStore.GetTask(assesTask2.TaskId);
                Assert.IsNotNull(finalTask2);
                Assert.IsTrue(assesTask2.SufTaskConnectors == null || assesTask2.SufTaskConnectors.Count == 0);
            });
        }
    }
}