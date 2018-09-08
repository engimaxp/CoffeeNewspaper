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
    public class TaskTimeSliceRelationTest : RepositarySetupAndTearDown
    {
        /// <summary>
        /// Get a timeslice-already-bind task
        /// </summary>
        /// <param name="bindTimeSliceCount">how many timeslice is bind to this task</param>
        /// <returns></returns>
        private CNTask GetTimeSliceBindTestTask(int bindTimeSliceCount)
        {
            var assesTask = DomainTestHelper.GetARandomTask();
            Enumerable.Range(0, bindTimeSliceCount).ToList().ForEach(x =>
            {
                var assesTimeSlice = DomainTestHelper.GetARandomTimeSlice();
                assesTask.UsedTimeSlices.Add(assesTimeSlice); 
            });
            return assesTask;
        }
        [Test]
        public async Task DeleteATaskTimeSliceRelation_TaskExist_TimeSliceDeleted()
        {
            await UseMemoryContextRun(async (dbcontext,unitOfWork) =>
            {
                //initiate the datastore
                var assesTask = GetTimeSliceBindTestTask(1);
                var taskDataStore = new TaskDataStore(dbcontext);
                taskDataStore.AddTask(assesTask);
                await unitOfWork.Commit();
                assesTask.UsedTimeSlices.Remove(assesTask.UsedTimeSlices.First());
                taskDataStore.UpdateTask(assesTask);

                await unitOfWork.Commit();
                Assert.AreEqual(0,dbcontext.TimeSlices.Count());
                Assert.AreEqual(1, dbcontext.Tasks.Count());
                Assert.AreEqual(0,dbcontext.Tasks.First().UsedTimeSlices.Count());
            });
        }
        [Test]
        public async Task DeleteTask_RelatedTimeSliceAlsoDeleted()
        {
            await UseMemoryContextRun(async (dbcontext,unitOfWork) =>
            {
                //initiate the datastore
                var assesTask = GetTimeSliceBindTestTask(3);
                var taskDataStore = new TaskDataStore(dbcontext);
                taskDataStore.AddTask(assesTask);

                await unitOfWork.Commit();
                 taskDataStore.RemoveTask(assesTask);

                var result = await unitOfWork.Commit();
                Assert.IsTrue(result);
                Assert.AreEqual(0, dbcontext.TimeSlices.Count());
                Assert.AreEqual(0, dbcontext.Tasks.Count());
            });
        }
    }
}