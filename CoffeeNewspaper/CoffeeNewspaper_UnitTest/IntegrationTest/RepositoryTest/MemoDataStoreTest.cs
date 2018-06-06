using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
    public class MemoDataStoreTest : RepositarySetupAndTearDown
    {
        [Test]
        public async Task AddMemo_QueryAllMemo_QuerySpecifiedMemo_AllPass()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //Arrange argument to be tested
                var assesMemo = DomainTestHelper.GetARandomMemo();

                //initiate the datastore
                var MemoDataStore = new MemoDataStore(dbcontext);

                //do testing action
                await MemoDataStore.AddMemo(assesMemo);

                //query the result from db for assert
                var Memos = await MemoDataStore.GetAllGlobalMemos();
                Memos.ToList().ForEach(Console.WriteLine);

                var firstMemo = await MemoDataStore.GetMemoById(Memos.First().MemoId);

                Assert.AreEqual(Memos.First(), firstMemo);
                Assert.AreEqual(Memos.FirstOrDefault(), assesMemo);
            });
        }

        [Test]
        public async Task DeleteAMemo_Success()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var MemoDataStore = new MemoDataStore(dbcontext);
                var assesMemo = DomainTestHelper.GetARandomMemo();
                var addedMemo = await MemoDataStore.AddMemo(assesMemo);


                var beforeDeleteResult = await MemoDataStore.DeleteMemo(addedMemo);
                Assert.IsTrue(beforeDeleteResult);

                var afterDeleteResult = await MemoDataStore.GetMemoById(addedMemo.MemoId);
                Assert.IsNull(afterDeleteResult);
            });
        }

        [Test]
        public async Task DeleteAMemo_MemoIdNotExist_Fail()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var MemoDataStore = new MemoDataStore(dbcontext);
                var assesMemo = DomainTestHelper.GetARandomMemo(true);
                var beforeDeleteResult = await MemoDataStore.DeleteMemo(assesMemo);
                Assert.IsFalse(beforeDeleteResult);
            });
        }

        [Test]
        public async Task QuerySpecifiedMemoDoesntExist_ReturnNull()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var MemoDataStore = new MemoDataStore(dbcontext);
                var firstMemo = await MemoDataStore.GetMemoById(Guid.NewGuid().ToString("D"));

                Assert.IsNull(firstMemo);
            });
        }

        [Test]
        public async Task UpdateMemo()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var MemoDataStore = new MemoDataStore(dbcontext);
                var assesMemo = DomainTestHelper.GetARandomMemo();
                var addedMemo = await MemoDataStore.AddMemo(assesMemo);

                //update the added Memo content,make sure its not equal to original Memo
                addedMemo.Content = "testing update";

                //update to database ,make sure what ef return is equal to modified Memo
                var updatedResult = await MemoDataStore.UpdateMemo(addedMemo);

                Assert.IsTrue(updatedResult);
                //select from db again ,make sure the Memo is updated
                var selectedMemo = await MemoDataStore.GetMemoById(addedMemo.MemoId);
                Assert.AreEqual(selectedMemo, addedMemo);
            });
        }

        [Test]
        public async Task UpdateMemo_MemoIdNotExist_Fail()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var MemoDataStore = new MemoDataStore(dbcontext);
                var assesMemo = DomainTestHelper.GetARandomMemo(true);

                //update the added Memo content,make sure its not equal to original Memo
                assesMemo.Content = "testing update";

                //update to database ,make sure what ef return is equal to modified Memo
                var updatedResult = await MemoDataStore.UpdateMemo(assesMemo);

                Assert.False(updatedResult);
            });
        }

        [Test]
        public async Task Clone_SimpleGlobalMemo()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var MemoDataStore = new MemoDataStore(dbcontext);
                var assesMemo = DomainTestHelper.GetARandomMemo();
                var addedMemo = await MemoDataStore.AddMemo(assesMemo);
                var cloneMemo = await MemoDataStore.CloneAMemo(addedMemo.MemoId);

                var allMemos = await MemoDataStore.GetAllGlobalMemos();
                Assert.AreEqual(2,allMemos.Count);

                Assert.IsNotNull(cloneMemo);
                Assert.AreNotEqual(addedMemo.MemoId,cloneMemo.MemoId);
            });
        }

        /// <summary>
        /// Some Memo Contain Relations
        /// this test will assert those relations are cloned properly
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Clone_ComplicatedMemo()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                var MemoDataStore = new MemoDataStore(dbcontext);

                //arrange initial entity
                var assesMemo = DomainTestHelper.GetARandomMemo();
                var task = DomainTestHelper.GetARandomTask();
                var tags = new[] {DomainTestHelper.GetARandomTag(), DomainTestHelper.GetARandomTag()}.ToList();

                //add relations
                assesMemo.TaskMemos = new List<CNTaskMemo>(){new CNTaskMemo(){Memo = assesMemo,Task = task}};
                task.TaskTaggers = tags.Select(x=>new CNTaskTagger(){Tag =x,Task = task}).ToList();
                assesMemo.MemoTaggers = tags.Select(x => new CNMemoTagger() { Tag = x, Memo = assesMemo }).ToList();

                var addedMemo = await MemoDataStore.AddMemo(assesMemo);
                var cloneMemo = await MemoDataStore.CloneAMemo(addedMemo.MemoId);

                Assert.AreEqual(addedMemo.TaskMemos.First().TaskId, cloneMemo.TaskMemos.First().TaskId);
                Assert.AreEqual(addedMemo.MemoTaggers.First().TagId, cloneMemo.MemoTaggers.First().TagId);

            });
        }
    }
}