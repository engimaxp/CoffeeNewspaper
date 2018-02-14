using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CN_Model;

namespace CoffeeNewspaper_UnitTest.DomainTest
{
    [TestFixture]
    public class RootTest
    {
        [Test]
        public void AddATask()
        {
            CNRoot root =  new CNRoot();
            CNTask testTask = DomainTestHelper.GetARandomTask(1);
            root.AddOrUpdateTask(testTask);
            Assert.AreEqual(root.GetFirstTask(), testTask);
        }

        [Test]
        public void AddAGlobalMemo()
        {
            CNRoot root = new CNRoot();
            CNMemo testMemo = DomainTestHelper.GetARandomMemo(1);
            root.AddOrUpdateGlobalMemo(testMemo);
            Assert.AreEqual(root.GetMemoById(1), testMemo);
        }

        [Test]
        public void AddTwoGlobalMemoWithSameID_ReturnTheFirstOne()
        {
            CNRoot root = new CNRoot();
            CNMemo testMemo1 = DomainTestHelper.GetARandomMemo(1);
            root.AddOrUpdateGlobalMemo(testMemo1);
            CNMemo testMemo2 = DomainTestHelper.GetARandomMemo(1);
            root.AddOrUpdateGlobalMemo(testMemo2);
            Assert.AreEqual(root.GetMemoById(1), testMemo2);
        }

        [Test]
        public void AddAMemoOfTask()
        {
            CNRoot root = new CNRoot();
            CNTask testTask = DomainTestHelper.GetARandomTask(1);
            CNMemo testMemo1 = DomainTestHelper.GetARandomMemo(2);
            testTask.AddOrUpdateMemo(testMemo1);
            root.AddOrUpdateTask(testTask);
            Assert.AreEqual(root.GetMemoById(2), testMemo1);
        }

        [Test]
        public void GetAllUniqueMemo()
        {
            CNRoot root = new CNRoot();
            CNTask testTask = DomainTestHelper.GetARandomTask(1);
            CNMemo testMemo1 = DomainTestHelper.GetARandomMemo(2);
            CNMemo testMemo2 = DomainTestHelper.GetARandomMemo(2);
            testTask.AddOrUpdateMemo(testMemo1).AddOrUpdateMemo(testMemo2);
            root.AddOrUpdateTask(testTask);
            CNMemo testMemo3 = DomainTestHelper.GetARandomMemo(1);
            CNMemo testMemo4 = DomainTestHelper.GetARandomMemo(1);
            root.AddOrUpdateGlobalMemo(testMemo3);
            root.AddOrUpdateGlobalMemo(testMemo4);
            Assert.AreEqual(new List<CNMemo>(){testMemo2,testMemo4}.Except(root.GetAllUniqueMemo()).ToList().Count,0);
        }

        [Test]
        public void GetAllRootTasks()
        {
            CNRoot root = new CNRoot();
            CNTask testTask1 = DomainTestHelper.GetARandomTask(1);
            CNTask testTask2 = DomainTestHelper.GetARandomTask(1);
            CNTask testTask3 = DomainTestHelper.GetARandomTask(2);
            CNTask testTask4 = DomainTestHelper.GetARandomTask(3);
            testTask4.SetParentTask(testTask3);

            root.AddOrUpdateTask(testTask1);
            root.AddOrUpdateTask(testTask2);
            root.AddOrUpdateTask(testTask3);
            root.AddOrUpdateTask(testTask4);
            Assert.AreEqual(new List<CNTask>() { testTask2, testTask3 }.Except(root.GetAllRootTasks()).ToList().Count, 0);
        }

        [Test]
        public void GetSpecificTaskMemos()
        {
            CNRoot root = new CNRoot();
            CNTask testTask1 = DomainTestHelper.GetARandomTask(1);
            CNMemo testMemo1 = DomainTestHelper.GetARandomMemo(2);
            CNMemo testMemo2 = DomainTestHelper.GetARandomMemo(2);
            testTask1.AddOrUpdateMemo(testMemo1).AddOrUpdateMemo(testMemo2);
            root.AddOrUpdateTask(testTask1);
            Assert.AreEqual( new List<CNMemo>() {testMemo2}.Except(root.GetTaskMemo(1)).ToList().Count , 0);
        }

        [Test]
        public void SearchMemoByContent()
        {
            string searchcontent = "keyv";

            CNRoot root = new CNRoot();
            CNTask testTask = DomainTestHelper.GetARandomTask(1);
            CNMemo testMemo1 = DomainTestHelper.GetARandomMemo(2);
            CNMemo testMemo2 = DomainTestHelper.GetARandomMemo(2);
            testMemo2.Content += searchcontent;
            testTask.AddOrUpdateMemo(testMemo1).AddOrUpdateMemo(testMemo2);
            root.AddOrUpdateTask(testTask);
            CNMemo testMemo3 = DomainTestHelper.GetARandomMemo(1);
            testMemo3.Content += searchcontent;
            CNMemo testMemo4 = DomainTestHelper.GetARandomMemo(1);
            root.AddOrUpdateGlobalMemo(testMemo3);
            root.AddOrUpdateGlobalMemo(testMemo4);

            CNMemo testMemo5 = DomainTestHelper.GetARandomMemo(3);
            testMemo5.Content += searchcontent;
            root.AddOrUpdateGlobalMemo(testMemo5);
            Assert.AreEqual(new List<CNMemo>() { testMemo2,testMemo5 }.Except(root.SearchMemoByContent(searchcontent)).ToList().Count, 0);
        }

        [Test]
        public void GetOrderedTaskList()
        {
        }

        [Test]
        public void StartATask()
        {
        }

        [Test]
        public void EndATask()
        {
        }

        [Test]
        public void UpdateAMemoOfTask()
        {
        }

        [Test]
        public void UpdateAMemoOfGlobal()
        {
        }

        [Test]
        public void ReplaceAWordOfATaskMemos()
        {
        }
        
    }
}
