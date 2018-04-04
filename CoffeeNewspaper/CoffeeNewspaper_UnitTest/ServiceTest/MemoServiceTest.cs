using System;
using System.Linq;
using CN_BLL;
using CN_Model;
using CN_Repository;
using CoffeeNewspaper_UnitTest.DomainTest;
using NSubstitute;
using NUnit.Framework;

namespace CoffeeNewspaper_UnitTest.ServiceTest
{
    [TestFixture]
    public class MemoServiceTest
    {
        [Test]
        public void AddGlobalMemo()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));

            //act
            var guid = targetService.AddAMemoToGlobal(testMemo);

            //assert
            testMemo.MemoId=guid;
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x=>x.MemoList.Contains(testMemo)));
        }
        [Test]
        public void AddTaskMemo_taskExisit()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            var targetService = new MemoService(rootDataProvider);
            var targetTask = arrangeRoot.GetFirstTask();
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));

            //act
            var guid = targetService.AddAMemoToTask(testMemo, targetTask.TaskId);

            //assert
            testMemo.MemoId = guid;
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => !x.MemoList.Contains(testMemo) && x.GetTaskMemo(targetTask.TaskId).Contains(testMemo)));
        }
        [Test]
        public void AddTaskMemo_taskNotExisit_returnEmptyMemoID()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));

            //act
            var guid = targetService.AddAMemoToTask(testMemo, 2);

            //assert
            Assert.IsTrue(string.IsNullOrEmpty(guid));
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }

        [Test]
        public void UpdateMemo_MemoNotExists_throwsException()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));

            //act
            Assert.Throws<ArgumentException>(() =>
            {
                var result = targetService.UpdateAMemo(testMemo);
                Assert.IsFalse(result);
            });

            //assert
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }
        [Test]
        public void UpdateMemo_MemoExistsInGlobalOnly()
        {
            var rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));
            arrangeRoot.AddOrUpdateGlobalMemo(testMemo);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            //act
            if (testMemo.Clone() is CNMemo newMemo)
            {
                newMemo.Content = Guid.NewGuid().ToString("N");
                var result = targetService.UpdateAMemo(newMemo);
                //assert
                Assert.IsTrue(result);
                rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x=>x.MemoList.Contains(newMemo)));
            }
        }
        [Test]
        public void UpdateMemo_MemoExistsInTaskOnly()
        {
            var rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = DomainTestHelper.GetARandomTask(1);
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));
            targetTask.AddOrUpdateMemo(testMemo);
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            //act
            if (testMemo.Clone() is CNMemo newMemo)
            {
                newMemo.Content = Guid.NewGuid().ToString("N");
                var result = targetService.UpdateAMemo(newMemo);
                //assert
                Assert.IsTrue(result);
                rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => x.GetTaskMemo(targetTask.TaskId).Contains(newMemo) && !x.MemoList.Contains(newMemo)));
            }
        }
        [Test]
        public void UpdateMemo_MemoExistsInTaskAndGlobal()
        {
            var rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask1 = DomainTestHelper.GetARandomTask(1);
            var targetTask2 = DomainTestHelper.GetARandomTask(2);
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));
            targetTask1.AddOrUpdateMemo(testMemo);
            arrangeRoot.AddOrUpdateTask(targetTask1);
            targetTask2.AddOrUpdateMemo(testMemo);
            arrangeRoot.AddOrUpdateTask(targetTask2);
            arrangeRoot.AddOrUpdateGlobalMemo(testMemo);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            //act
            if (testMemo.Clone() is CNMemo newMemo)
            {
                newMemo.Content = Guid.NewGuid().ToString("N");
                var result = targetService.UpdateAMemo(newMemo);
                //assert
                Assert.IsTrue(result);
                rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x =>
                    x.GetTaskMemo(targetTask1.TaskId).Contains(newMemo)&&
                    x.GetTaskMemo(targetTask2.TaskId).Contains(newMemo)&&
                    x.MemoList.Contains(newMemo)));
            }
        }
        [Test]
        public void DeleteGlobalMemo_MemoExists()
        {
            var rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));
            arrangeRoot.AddOrUpdateGlobalMemo(testMemo);
            rootDataProvider.GetRootData().Returns(arrangeRoot);

            Assert.IsTrue(arrangeRoot.MemoList.Contains(testMemo));

            //act
            var result = targetService.DeleteAMemo(testMemo.MemoId, 0);
            //assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => !x.MemoList.Contains(testMemo)));
        }

        [Test]
        public void DeleteGlobalMemo_MemoNotExists_ReturnFalse()
        {
            var rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));
            //arrangeRoot.AddOrUpdateGlobalMemo(testMemo);
            rootDataProvider.GetRootData().Returns(arrangeRoot);

            Assert.IsFalse(arrangeRoot.MemoList.Contains(testMemo));
            
            //act
            var result = targetService.DeleteAMemo(testMemo.MemoId, 0);
            //assert
            Assert.IsFalse(result);

            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }

        [Test]
        public void DeleteGlobalMemo_MemoNotExists_MemoExistsInTasks_ReturnFalse()
        {
            var rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));
            var task = arrangeRoot.GetFirstTask();
            task.AddOrUpdateMemo(testMemo);
            arrangeRoot.AddOrUpdateTask(task);

            Assert.IsFalse(string.IsNullOrEmpty(arrangeRoot.GetMemoById(testMemo.MemoId)?.MemoId));
            Assert.IsFalse(arrangeRoot.MemoList.Contains(testMemo));

            rootDataProvider.GetRootData().Returns(arrangeRoot);
            //act
            var result = targetService.DeleteAMemo(testMemo.MemoId, 0);
            //assert
            Assert.IsFalse(result);

            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }

        [Test]
        public void DeleteTaskMemo_TaskExists_MemoNotExists_MemoExistsInGlobal_ReturnFalse()
        {
            var rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));
            var task = arrangeRoot.GetFirstTask();
            arrangeRoot.AddOrUpdateTask(task);
            arrangeRoot.AddOrUpdateGlobalMemo(testMemo);

            Assert.IsFalse(string.IsNullOrEmpty(arrangeRoot.GetMemoById(testMemo.MemoId)?.MemoId));
            Assert.IsTrue(arrangeRoot.MemoList.Contains(testMemo));
            Assert.IsFalse(task.Memos.Contains(testMemo));
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            //act
            var result = targetService.DeleteAMemo(testMemo.MemoId, task.TaskId);
            //assert
            Assert.IsFalse(result);

            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }

        [Test]
        public void DeleteTaskMemo_TaskNotExists_ReturnFalse()
        {
            var rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));
            var task = DomainTestHelper.GetARandomTask(2);
            arrangeRoot.AddOrUpdateGlobalMemo(testMemo);

            Assert.IsFalse(string.IsNullOrEmpty(arrangeRoot.GetMemoById(testMemo.MemoId)?.MemoId));
            Assert.IsTrue(arrangeRoot.MemoList.Contains(testMemo));
            Assert.IsTrue(string.IsNullOrEmpty(arrangeRoot.GetTaskById(task.TaskId)?.Content));
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            //act
            var result = targetService.DeleteAMemo(testMemo.MemoId, task.TaskId);
            //assert
            Assert.IsFalse(result);

            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }

        [Test]
        public void DeleteTaskMemo_TaskExists_MemoNotExists_MemoExistsInOtherTask_ReturnFalse()
        {
            var rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));
            var task = arrangeRoot.GetFirstTask();
            var othertask = DomainTestHelper.GetARandomTask(2);
            othertask.AddOrUpdateMemo(testMemo);
            arrangeRoot.AddOrUpdateTask(othertask);

            Assert.IsFalse(task.TaskId.Equals(othertask.TaskId));
            Assert.IsFalse(string.IsNullOrEmpty(arrangeRoot.GetMemoById(testMemo.MemoId)?.MemoId));
            Assert.IsFalse(arrangeRoot.MemoList.Contains(testMemo));
            Assert.IsFalse(string.IsNullOrEmpty(arrangeRoot.GetTaskById(task.TaskId)?.Content));
            Assert.IsTrue(arrangeRoot.GetTaskMemo(othertask.TaskId).Contains(testMemo));
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            //act
            var result = targetService.DeleteAMemo(testMemo.MemoId, task.TaskId);
            //assert
            Assert.IsFalse(result);

            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }
        [Test]
        public void DeleteTaskMemo_TaskExists_MemoExists()
        {
            var rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetService = new MemoService(rootDataProvider);
            var testMemo = DomainTestHelper.GetARandomMemo(Guid.NewGuid().ToString("N"));
            var task = arrangeRoot.GetFirstTask();
            var othertask = DomainTestHelper.GetARandomTask(2);
            othertask.AddOrUpdateMemo(testMemo);
            task.AddOrUpdateMemo(testMemo);
            arrangeRoot.AddOrUpdateTask(task);
            arrangeRoot.AddOrUpdateTask(othertask);
            arrangeRoot.AddOrUpdateGlobalMemo(testMemo);

            rootDataProvider.GetRootData().Returns(arrangeRoot);
            //act
            var result = targetService.DeleteAMemo(testMemo.MemoId, task.TaskId);
            //assert
            Assert.IsTrue(result);

            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => 
                x.MemoList.Contains(testMemo)&&
                !x.GetTaskMemo(task.TaskId).Contains(testMemo)&&
                x.GetTaskMemo(othertask.TaskId).Contains(testMemo)
                ));
        }
    }
}