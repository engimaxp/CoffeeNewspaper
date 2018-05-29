using System;
using System.Linq;
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
        public void AddTask()
        {
            var task = UseMemoryContextRun(async dbcontext =>
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

                //assert it
                Assert.AreEqual(tasks.FirstOrDefault(), assesTask);
            });
            task.Wait();
        }
    }
}