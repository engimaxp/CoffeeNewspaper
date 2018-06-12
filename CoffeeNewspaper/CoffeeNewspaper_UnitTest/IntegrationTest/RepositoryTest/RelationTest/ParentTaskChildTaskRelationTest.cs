﻿using System.Linq;
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
    public class ParentTaskChildTaskRelationTest : RepositarySetupAndTearDown
    {
        /// <summary>
        /// Get a childtask-already-bind task
        /// </summary>
        /// <param name="bindTaskCount">how many childtask is bind to this task</param>
        /// <returns></returns>
        private CNTask GetChildTaskBindTestTask(int bindTaskCount)
        {
            var assesTask = DomainTestHelper.GetARandomTask();
            Enumerable.Range(0, bindTaskCount).ToList().ForEach(x =>
            {
                var assesChildTask = DomainTestHelper.GetARandomTask();
                assesTask.ChildTasks.Add(assesChildTask); 
            });
            return assesTask;
        }
        [Test]
        public async Task DeleteAParentChildTaskRelation_BothTaskExist()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //initiate the datastore
                var assesTask = GetChildTaskBindTestTask(1);
                var taskDataStore = new TaskDataStore(dbcontext);
                await taskDataStore.AddTask(assesTask);

                assesTask.ChildTasks.Remove(assesTask.ChildTasks.First());
                await taskDataStore.UpdateTask(assesTask);

                Assert.AreEqual(2,dbcontext.Tasks.Count());
                Assert.AreEqual(0,dbcontext.Tasks.First().ChildTasks.Count());
            });
        }
        [Test]
        public async Task AddDupplicateTaskTaskRelation_ReturnFalse()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //initiate the datastore
                var assesTask = GetChildTaskBindTestTask(1);
                var taskDataStore = new TaskDataStore(dbcontext);
                await taskDataStore.AddTask(assesTask);

                assesTask.ChildTasks.Add(assesTask.ChildTasks.First());
                var result = await taskDataStore.UpdateTask(assesTask);

                Assert.IsTrue(result);


                Assert.AreEqual(2, dbcontext.Tasks.Count());
                Assert.AreEqual(1, assesTask.ChildTasks.Count());
            });
        }
        [Test]
        public async Task DeleteParentTask_RelatedChildTaskExist_ParentChangeToNull()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //initiate the datastore
                var assesTask = GetChildTaskBindTestTask(1);
                var taskDataStore = new TaskDataStore(dbcontext);
                await taskDataStore.AddTask(assesTask);
                
                var result = await taskDataStore.RemoveTask(assesTask);

                Assert.IsTrue(result);
                Assert.AreEqual(1, dbcontext.Tasks.Count());
                Assert.IsNull(dbcontext.Tasks.First().ParentTask);
            });
        }
        [Test]
        public async Task DeleteChildTask_RelatedParentTaskExis()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //initiate the datastore
                var assesTask = GetChildTaskBindTestTask(1);
                var taskDataStore = new TaskDataStore(dbcontext);
                await taskDataStore.AddTask(assesTask);

                var result = await taskDataStore.RemoveTask(assesTask.ChildTasks.First());

                Assert.IsTrue(result);
                var testresult = await taskDataStore.GetAllTask();
                Assert.AreEqual(assesTask.TaskId, testresult.First().TaskId);
            });
        }
    }
}