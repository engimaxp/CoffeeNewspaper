using System;
using System.Linq;
using System.Threading.Tasks;
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
                var assesTask = DomainTestHelper.GetARandomTask();
                assesTask.TaskId = 1;
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
                var updatedResult = await taskDataStore.UpdateTask(addedTask);

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
                var assesTask = DomainTestHelper.GetARandomTask();
                assesTask.TaskId = 1;

                //update the added task content,make sure its not equal to original task
                assesTask.Content = "testing update";

                //update to database ,make sure what ef return is equal to modified task
                var updatedResult = await taskDataStore.UpdateTask(assesTask);

                Assert.False(updatedResult);
            });
        }
    }
}