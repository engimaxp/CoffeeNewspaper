using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Repository;
using CoffeeNewspaper_UnitTest.DomainTest;
using NUnit.Framework;

namespace CoffeeNewspaper_UnitTest.RepositoryTest.RelationTest
{

    /// <summary>
    ///     In-Memory integration data persistent test
    /// </summary>
    [TestFixture]
    public class TaskConnectorRelationTest : RepositarySetupAndTearDown
    {
        /// <summary>
        /// Get a suftask-already-bind task
        /// </summary>
        /// <param name="bindTaskCount">how many suftasks is bind to this task</param>
        /// <returns></returns>
        private CNTask GetSufTaskBindTestTask(int bindTaskCount)
        {
            var assesTask = DomainTestHelper.GetARandomTask();
            Enumerable.Range(0, bindTaskCount).ToList().ForEach(x =>
            {
                var assesSufTask = DomainTestHelper.GetARandomTask();
                assesTask.SufTaskConnectors.Add(new CNTaskConnector(){PreTask = assesTask,SufTask = assesSufTask });
            });
            return assesTask;
        }
        [Test]
        public async Task DeleteASufTaskPreTaskRelation_BothTasksStillExist()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //initiate the datastore
                var assesTask = GetSufTaskBindTestTask(1);
                var taskDataStore = new TaskDataStore(dbcontext);
                await taskDataStore.AddTask(assesTask);

                assesTask.SufTaskConnectors.Remove(assesTask.SufTaskConnectors.First());
                await taskDataStore.UpdateTask(assesTask);

                Assert.AreEqual(2, dbcontext.Tasks.Count());
                Assert.AreEqual(0, dbcontext.TaskConnectors.Count());
            });
        }
        [Test]
        public async Task AddDupplicateSufTaskPreTaskRelationRelation_ReturnFalse()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //initiate the datastore
                var assesTask = GetSufTaskBindTestTask(1);
                var taskDataStore = new TaskDataStore(dbcontext);
                await taskDataStore.AddTask(assesTask);

                assesTask.SufTaskConnectors.Add(new CNTaskConnector() { PreTaskId = assesTask.TaskId, SufTaskId = assesTask.SufTaskConnectors.First().SufTaskId });
                var result = await taskDataStore.UpdateTask(assesTask);

                Assert.IsFalse(result);
                Assert.AreEqual(2, dbcontext.Tasks.Count());
                Assert.AreEqual(1, dbcontext.TaskConnectors.Distinct().Count());
                Assert.AreEqual(0, dbcontext.Tasks.First().PreTaskConnectors.Distinct(CNTaskConnector.PreTaskIdSufTaskIdComparer).Count());
                Assert.AreEqual(1, dbcontext.Tasks.Last().PreTaskConnectors.Distinct(CNTaskConnector.PreTaskIdSufTaskIdComparer).Count());
                Assert.AreEqual(1, dbcontext.Tasks.First().SufTaskConnectors.Distinct(CNTaskConnector.PreTaskIdSufTaskIdComparer).Count());
                Assert.AreEqual(0, dbcontext.Tasks.Last().SufTaskConnectors.Distinct(CNTaskConnector.PreTaskIdSufTaskIdComparer).Count());
            });
        }
    }
}