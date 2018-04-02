using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CN_Model;
using CN_Repository;
using CoffeeNewspaper_UnitTest.DomainTest;
using Newtonsoft.Json;
using NUnit.Framework;
using SimpleTxtDB;

namespace CoffeeNewspaper_UnitTest.RepositoryTest
{
    [TestFixture]
    public class RootDataProviderTest
    {
        
        [Test]
        public void Persistence()
        {
            var tsProvider = RootDataProvider.GetProvider();
            TxtDB fileDb = new TxtDB(RootDataProvider.persistenceFileName);
            fileDb.DumpFile();
            var testTask = DomainTestHelper.GetARandomTask(10);
            var mockRoot = new CNRoot();
            mockRoot.AddOrUpdateTask(testTask);
            tsProvider.Persistence(mockRoot);

            var result = fileDb.ReadAll().Trim();//Write Operation Add a breakline
            Assert.IsFalse(string.IsNullOrEmpty(result));

            Assert.AreEqual(result,JsonConvert.SerializeObject(mockRoot, Formatting.Indented));
            fileDb.DumpFile();
        }

        [Test]
        public void GetRootData()
        {
            var tsProvider = RootDataProvider.GetProvider();
            TxtDB fileDb = new TxtDB(RootDataProvider.persistenceFileName);
            fileDb.DumpFile();
            var testTask = DomainTestHelper.GetARandomTask(10);
            var mockRoot = new CNRoot();
            mockRoot.AddOrUpdateTask(testTask);
            tsProvider.Persistence(mockRoot);
            var result = tsProvider.GetRootData();

            Assert.AreEqual(result.GetTaskById(10), mockRoot.GetTaskById(10));
            fileDb.DumpFile();
        }


    }
}
