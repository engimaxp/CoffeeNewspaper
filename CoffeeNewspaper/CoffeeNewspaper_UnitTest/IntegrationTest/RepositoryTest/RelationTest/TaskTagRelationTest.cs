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
    public class TaskTagRelationTest : RepositarySetupAndTearDown
    {
        /// <summary>
        /// Get a tag-already-bind task
        /// </summary>
        /// <param name="bindTagCount">how many tag is bind to this task</param>
        /// <returns></returns>
        private CNTask GetTagBindTestTask(int bindTagCount)
        {
            var assesTask = DomainTestHelper.GetARandomTask();
            Enumerable.Range(0, bindTagCount).ToList().ForEach(x =>
            {
                var assesTag = DomainTestHelper.GetARandomTag();
                assesTask.TaskTaggers.Add(new CNTaskTagger() { Tag = assesTag, Task = assesTask }); 
            });
            return assesTask;
        }
        [Test]
        public async Task DeleteATaskTagRelation_BothTaskAndTagStillExist()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //initiate the datastore
                var assesTask = GetTagBindTestTask(1);
                var taskDataStore = new TaskDataStore(dbcontext);
                await taskDataStore.AddTask(assesTask);

                assesTask.TaskTaggers.Remove(assesTask.TaskTaggers.First());
                await taskDataStore.UpdateTask(assesTask);

                Assert.AreEqual(1,dbcontext.Tags.Count());
                Assert.AreEqual(1, dbcontext.Tasks.Count());
                Assert.AreEqual(0,dbcontext.TaskTaggers.Count());
            });
        }
        [Test]
        public async Task AddDupplicateTaskTagRelation_ReturnFalse()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //initiate the datastore
                var assesTask = GetTagBindTestTask(1);
                var taskDataStore = new TaskDataStore(dbcontext);
                await taskDataStore.AddTask(assesTask);

                assesTask.TaskTaggers.Add(new CNTaskTagger(){TagId = assesTask.TaskTaggers.First().TagId,TaskId = assesTask.TaskId});
                var result = await taskDataStore.UpdateTask(assesTask);

                Assert.IsFalse(result);
                Assert.AreEqual(1, dbcontext.Tags.Count());
                Assert.AreEqual(1, dbcontext.Tasks.Count());
                Assert.AreEqual(1, dbcontext.TaskTaggers.Count());
            });
        }
    }
}