﻿using NUnit.Framework;
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
            CNMemo testMemo = DomainTestHelper.GetARandomMemo("1");
            root.AddOrUpdateGlobalMemo(testMemo);
            Assert.AreEqual(root.GetMemoById("1"), testMemo);
        }

        [Test]
        public void AddTwoGlobalMemoWithSameID_ReturnTheFirstOne()
        {
            CNRoot root = new CNRoot();
            CNMemo testMemo1 = DomainTestHelper.GetARandomMemo("1");
            root.AddOrUpdateGlobalMemo(testMemo1);
            CNMemo testMemo2 = DomainTestHelper.GetARandomMemo("1");
            root.AddOrUpdateGlobalMemo(testMemo2);
            Assert.AreEqual(root.GetMemoById("1"), testMemo2);
        }

        [Test]
        public void AddAMemoOfTask()
        {
            CNRoot root = new CNRoot();
            CNTask testTask = DomainTestHelper.GetARandomTask(1);
            CNMemo testMemo1 = DomainTestHelper.GetARandomMemo("2");
            testTask.AddOrUpdateMemo(testMemo1);
            root.AddOrUpdateTask(testTask);
            Assert.AreEqual(root.GetMemoById("2"), testMemo1);
        }

        [Test]
        public void GetAllUniqueMemo()
        {
            CNRoot root = new CNRoot();
            CNTask testTask = DomainTestHelper.GetARandomTask(1);
            CNMemo testMemo1 = DomainTestHelper.GetARandomMemo("2");
            CNMemo testMemo2 = DomainTestHelper.GetARandomMemo("2");
            testTask.AddOrUpdateMemo(testMemo1).AddOrUpdateMemo(testMemo2);
            root.AddOrUpdateTask(testTask);
            CNMemo testMemo3 = DomainTestHelper.GetARandomMemo("1");
            CNMemo testMemo4 = DomainTestHelper.GetARandomMemo("1");
            root.AddOrUpdateGlobalMemo(testMemo1);
            root.AddOrUpdateGlobalMemo(testMemo3);
            root.AddOrUpdateGlobalMemo(testMemo4);
            var result = root.GetAllUniqueMemo().ToList();
            Assert.IsNotNull(result);
            Assert.AreEqual(3,result.Count());
            Assert.AreEqual(new List<CNMemo>(){testMemo2,testMemo4}.Except(result,CNMemo.CnMemoComparer).Count(),0);
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
            Assert.AreEqual(new List<CNTask>() { testTask2, testTask3 }.Except(root.GetAllRootTasks(), CNTask.CnTaskComparer).ToList().Count, 0);
        }

        [Test]
        public void GetSpecificTaskMemos()
        {
            CNRoot root = new CNRoot();
            CNTask testTask1 = DomainTestHelper.GetARandomTask(1);
            CNMemo testMemo1 = DomainTestHelper.GetARandomMemo("2");
            CNMemo testMemo2 = DomainTestHelper.GetARandomMemo("2");
            testTask1.AddOrUpdateMemo(testMemo1).AddOrUpdateMemo(testMemo2);
            root.AddOrUpdateTask(testTask1);
            Assert.AreEqual( new List<CNMemo>() {testMemo2}.Except(root.GetTaskMemo(1),CNMemo.CnMemoComparer).ToList().Count , 0);
        }

        [Test]
        public void SearchMemoByContent()
        {
            string searchcontent = "keyv";

            CNRoot root = new CNRoot();
            CNTask testTask = DomainTestHelper.GetARandomTask(1);
            CNMemo testMemo1 = DomainTestHelper.GetARandomMemo("2");
            CNMemo testMemo2 = DomainTestHelper.GetARandomMemo("2");
            testMemo2.Content += searchcontent;
            testTask.AddOrUpdateMemo(testMemo1).AddOrUpdateMemo(testMemo2);
            root.AddOrUpdateTask(testTask);
            CNMemo testMemo3 = DomainTestHelper.GetARandomMemo("1");
            testMemo3.Content += searchcontent;
            CNMemo testMemo4 = DomainTestHelper.GetARandomMemo("1");
            root.AddOrUpdateGlobalMemo(testMemo3);
            root.AddOrUpdateGlobalMemo(testMemo4);

            CNMemo testMemo5 = DomainTestHelper.GetARandomMemo("3");
            testMemo5.Content += searchcontent;
            root.AddOrUpdateGlobalMemo(testMemo5);
            Assert.AreEqual(new List<CNMemo>() { testMemo2,testMemo5 }.Except(root.SearchMemoByContent(searchcontent),CNMemo.CnMemoComparer).ToList().Count, 0);
        }

        [Test]
        public void GetOrderedTaskList()
        {
        }

        [Test]
        public void StartATask()
        {
            CNRoot root = new CNRoot();
            root.AddOrUpdateTask(DomainTestHelper.GetARandomTask(1));
            root.StartTask(1);
            Assert.AreEqual(root.GetTaskById(1).Status,CNTaskStatus.DOING);
        }

        [Test]
        public void EndATask()
        {
            CNRoot root = new CNRoot();
            root.AddOrUpdateTask(DomainTestHelper.GetARandomTask(1));
            root.EndTask(1);
            Assert.AreEqual(root.GetTaskById(1).Status, CNTaskStatus.DONE);
        }

        [Test]
        public void UpdateAMemo_UpdateBothGlobalAndTasksMemo()
        {

            CNRoot root = new CNRoot();
            CNTask testTask1 = DomainTestHelper.GetARandomTask(1);
            CNMemo testMemo1 = DomainTestHelper.GetARandomMemo("2");
            testTask1.AddOrUpdateMemo(testMemo1);
            root.AddOrUpdateTask(testTask1);
            root.AddOrUpdateGlobalMemo(testMemo1);
            
            CNMemo testMemo2 = DomainTestHelper.GetARandomMemo("2");
            root.UpdateMemo(testMemo2);

            Assert.AreEqual(testMemo2,root.GetMemoById("2"));
        }


        [Test]
        public void ReplaceAWordOfATaskMemos()
        {

            CNRoot root = new CNRoot();
            CNTask testTask1 = DomainTestHelper.GetARandomTask(1);
            CNMemo testMemo1 = DomainTestHelper.GetARandomMemo("2");
            testTask1.AddOrUpdateMemo(testMemo1);
            root.AddOrUpdateTask(testTask1);
            root.AddOrUpdateGlobalMemo(testMemo1);

            CNMemo copy = testMemo1.Clone() as CNMemo;
            if (copy != null)
            {
                copy.Content = copy.Content.Replace("Start with", "End with");
            }

            root.ReplaceAWordOfATaskMemos("Start with", "End with");

            Assert.AreEqual(copy, root.GetMemoById("2"));
        }

        [Test]
        public void RemoveTest()
        {
            var root  = DomainTestHelper.GetRandomRoot();
            var testTask = DomainTestHelper.GetARandomTask(2);
            var testMemo = DomainTestHelper.GetARandomMemo("2");
            testTask.AddOrUpdateMemo(testMemo);
            root.AddOrUpdateTask(testTask);

            Assert.IsTrue(root.TaskList.Contains(testTask));

            //act
            int index = root.TaskList.FindIndex(x=>x.TaskId == testTask.TaskId);
            root.TaskList.RemoveAt(index);
            Assert.IsFalse(root.TaskList.Contains(testTask));
        }

        [Test]
        public void GetTaskAndChildSufTasksById()
        {
            var root = DomainTestHelper.GetRandomRoot();
            var testtask1 = DomainTestHelper.GetARandomTask(1);
            var testtask2 = DomainTestHelper.GetARandomTask(2);
            var testtask3 = DomainTestHelper.GetARandomTask(3);
            var testtask4 = DomainTestHelper.GetARandomTask(4);
            testtask2.ParentTaskId = testtask1.TaskId;
            testtask3.ParentTaskId = testtask1.TaskId;
            testtask4.ParentTaskId = testtask2.TaskId;
            testtask4.PreTaskIds = new List<int>(){ testtask3.TaskId };
            root.AddOrUpdateTask(testtask1);
            root.AddOrUpdateTask(testtask2);
            root.AddOrUpdateTask(testtask3);
            root.AddOrUpdateTask(testtask4);
            var result = root.GetTaskAndChildSufTasksById(1);

            Assert.AreEqual(4,result.Count);
            Assert.AreEqual(0,result.Except(new List<CNTask>(){testtask1, testtask2 , testtask3 , testtask4 },CNTask.CnTaskComparer).Count());
        }
    }
}
