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
    public class TaskMemoRelationTest : RepositarySetupAndTearDown
    {
        /// <summary>
        /// Get a task-already-bind memo
        /// </summary>
        /// <param name="bindTaskCount">how many task is bind to this memo</param>
        /// <returns></returns>
        private CNMemo GetTaskBindTestMemo(int bindTaskCount)
        {
            var assesMemo = DomainTestHelper.GetARandomMemo();
            Enumerable.Range(0,bindTaskCount).ToList().ForEach(x =>
            {
                var assesTask = DomainTestHelper.GetARandomTask();
                assesMemo.TaskMemos.Add(new CNTaskMemo() { Memo = assesMemo, Task = assesTask }); 
            });
            return assesMemo;
        }
        [Test]
        public async Task DeleteATaskMemoRelation_BothTaskAndMemoStillExist()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //initiate the datastore
                var assesMemo = GetTaskBindTestMemo(1);
                var MemoDataStore = new MemoDataStore(dbcontext);
                await MemoDataStore.AddMemo(assesMemo);

                assesMemo.TaskMemos.Remove(assesMemo.TaskMemos.First());
                await MemoDataStore.UpdateMemo(assesMemo);

                Assert.AreEqual(1,dbcontext.Memos.Count());
                Assert.AreEqual(1, dbcontext.Tasks.Count());
                Assert.AreEqual(0,dbcontext.TaskMemos.Count());
            });
        }
        [Test]
        public async Task AddDupplicateTaskMemoRelation_ReturnFalse()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //initiate the datastore
                var assesMemo = GetTaskBindTestMemo(1);
                var MemoDataStore = new MemoDataStore(dbcontext);
                await MemoDataStore.AddMemo(assesMemo);

                assesMemo.TaskMemos.Add(new CNTaskMemo(){MemoId = assesMemo.MemoId,TaskId = assesMemo.TaskMemos.First().TaskId});
                var result = await MemoDataStore.UpdateMemo(assesMemo);

                Assert.IsFalse(result);
                Assert.AreEqual(1, dbcontext.Memos.Count());
                Assert.AreEqual(1, dbcontext.Tasks.Count());
                Assert.AreEqual(1, dbcontext.TaskMemos.Count());
            });
        }
    }
}