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
    public class MemoTagRelationTest : RepositarySetupAndTearDown
    {
        /// <summary>
        /// Get a tag-already-bind memo
        /// </summary>
        /// <param name="bindTagCount">how many tag is bind to this memo</param>
        /// <returns></returns>
        private CNMemo GetTagBindTestMemo(int bindTagCount)
        {
            var assesMemo = DomainTestHelper.GetARandomMemo();
            Enumerable.Range(0, bindTagCount).ToList().ForEach(x =>
            {
                var assesTag = DomainTestHelper.GetARandomTag();
                assesMemo.MemoTaggers.Add(new CNMemoTagger() { Tag = assesTag, Memo = assesMemo }); 
            });
            return assesMemo;
        }
        [Test]
        public async Task DeleteAMemoTagRelation_BothMemoAndTagStillExist()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //initiate the datastore
                var assesMemo = GetTagBindTestMemo(1);
                var memoDataStore = new MemoDataStore(dbcontext);
                await memoDataStore.AddMemo(assesMemo);

                assesMemo.MemoTaggers.Remove(assesMemo.MemoTaggers.First());
                await memoDataStore.UpdateMemo(assesMemo);

                Assert.AreEqual(1,dbcontext.Tags.Count());
                Assert.AreEqual(1, dbcontext.Memos.Count());
                Assert.AreEqual(0,dbcontext.MemoTaggers.Count());
            });
        }
        [Test]
        public async Task AddDupplicateMemoTagRelation_ReturnFalse()
        {
            await UseMemoryContextRun(async dbcontext =>
            {
                //initiate the datastore
                var assesMemo = GetTagBindTestMemo(1);
                var memoDataStore = new MemoDataStore(dbcontext);
                await memoDataStore.AddMemo(assesMemo);

                assesMemo.MemoTaggers.Add(new CNMemoTagger(){TagId = assesMemo.MemoTaggers.First().TagId,MemoId = assesMemo.MemoId});
                var result = await memoDataStore.UpdateMemo(assesMemo);

                Assert.IsFalse(result);
                Assert.AreEqual(1, dbcontext.Tags.Count());
                Assert.AreEqual(1, dbcontext.Memos.Count());
                Assert.AreEqual(1, dbcontext.MemoTaggers.Count());
            });
        }
    }
}