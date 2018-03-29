using System;
using System.Collections.Generic;
using CN_BLL;
using CN_Model;
using CN_Repository;
using CoffeeNewspaper_UnitTest.DomainTest;
using NUnit.Framework;
using NSubstitute;
namespace CoffeeNewspaper_UnitTest.ServiceTest
{
    [TestFixture]
    public class TaskServiceTest
    {
        #region Create Test
        [Test]
        public void CreateATask_ParameterWrong_parentTaskNotExist_throwsArgException()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            rootDataProvider.GetRootData().Returns(DomainTestHelper.GetRandomRoot());
            var targetService = new TaskService(rootDataProvider);
            var testTask = DomainTestHelper.GetARandomTask(2);
            //parentTask not exist
            testTask.ParentTaskId = 2;
            Assert.Throws<ArgumentException>(() =>
            {
                targetService.CreateATask(testTask);
            });
        }

        [Test]
        public void CreateATask_ParameterWrong_pretaskNotExist_throwsArgException()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            rootDataProvider.GetRootData().Returns(DomainTestHelper.GetRandomRoot());
            TaskService targetService = new TaskService(rootDataProvider);
            var testTask = DomainTestHelper.GetARandomTask(2);
            //pretask not exist
            testTask.PreTaskIds = new List<int>(){2};
            Assert.Throws<ArgumentException>(() =>
            {
                targetService.CreateATask(testTask);
            });
        }

        [Test]
        public void CreateATask_ParameterWrong_ContentEmpty_throwsArgException()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            rootDataProvider.GetRootData().Returns(DomainTestHelper.GetRandomRoot());
            TaskService targetService = new TaskService(rootDataProvider);
            var testTask = DomainTestHelper.GetARandomTask(2);
            //pretask not exist
            testTask.Content = string.Empty;
            Assert.Throws<ArgumentException>(() =>
            {
                targetService.CreateATask(testTask);
            });
        }

        [Test]
        public void CreateATask_ParameterCorrect()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            rootDataProvider.GetRootData().Returns(DomainTestHelper.GetRandomRoot());
            TaskService targetService = new TaskService(rootDataProvider);
            var testTask = DomainTestHelper.GetARandomTask(2);
            int taskid = targetService.CreateATask(testTask);
            testTask.TaskId = taskid;
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x=>Equals(x.GetTaskById(taskid), testTask)));
        }
        #endregion

        #region Remove Test
        [Test]
        public void RemoveATask_HasNoChildTask_TaskDidNotExist_throwsException()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            rootDataProvider.GetRootData().Returns(DomainTestHelper.GetRandomRoot());
            TaskService targetService = new TaskService(rootDataProvider);
            var testTask = DomainTestHelper.GetARandomTask(2);
            Assert.Throws<ArgumentException>(() => {
                bool result = targetService.RemoveTask(testTask.TaskId);
                Assert.IsFalse(result);
            });
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }
        [Test]
        public void RemoveATask_HasNoChildTask_StatusNotDelete_throwsException()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.Throws<ArgumentException>(() => {
                bool result = targetService.RemoveTask(1);
                Assert.IsFalse(result);
            });
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }
        [Test]
        public void RemoveATask_HasNoChildTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeDeleted = DomainTestHelper.GetARandomTask(2);
            taskToBeDeleted.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(taskToBeDeleted);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeDeleted));

            //Act
            bool result = targetService.RemoveTask(taskToBeDeleted.TaskId);
            
            //Assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x=>x.TaskList.Count == 1 && !taskToBeDeleted.Equals(x.GetTaskById(taskToBeDeleted.TaskId)) ));
        }
        [Test]
        public void RemoveATask_HasChildTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeDeleted = DomainTestHelper.GetARandomTask(2);
            var childTask = DomainTestHelper.GetARandomTask(3);
            childTask.ParentTaskId = taskToBeDeleted.TaskId;
            taskToBeDeleted.IsDeleted = true;
            childTask.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(taskToBeDeleted);
            arrangeRoot.AddOrUpdateTask(childTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeDeleted));
            Assert.IsTrue(arrangeRoot.TaskList.Contains(childTask));

            //Act Assert
            Assert.Throws<TaskHasChildTasksException>(() => {
                bool result = targetService.RemoveTask(taskToBeDeleted.TaskId);
                Assert.IsFalse(result);
            });
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
            
        }
        [Test]
        public void RemoveATask_HasSufTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeDeleted = DomainTestHelper.GetARandomTask(2);
            var sufTask = DomainTestHelper.GetARandomTask(3);
            sufTask.PreTaskIds = new List<int>(){ taskToBeDeleted.TaskId };
            taskToBeDeleted.IsDeleted = true;
            sufTask.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(taskToBeDeleted);
            arrangeRoot.AddOrUpdateTask(sufTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeDeleted));
            Assert.IsTrue(arrangeRoot.TaskList.Contains(sufTask));

            //Act Assert
            Assert.Throws<TaskHasSufTasksException>(() => {
                bool result = targetService.RemoveTask(taskToBeDeleted.TaskId);
                Assert.IsFalse(result);
            });
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());

        }
        [Test]
        public void ForceRemoveATask_HasChildTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeDeleted = DomainTestHelper.GetARandomTask(2);
            var childTask = DomainTestHelper.GetARandomTask(3);
            childTask.ParentTaskId = taskToBeDeleted.TaskId;
            taskToBeDeleted.IsDeleted = true;
            childTask.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(taskToBeDeleted);
            arrangeRoot.AddOrUpdateTask(childTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeDeleted));
            Assert.IsTrue(arrangeRoot.TaskList.Contains(childTask));

            //Act
            bool result = targetService.RemoveTask(taskToBeDeleted.TaskId,true);

            //Assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => x.TaskList.Count == 1 && !taskToBeDeleted.Equals(x.GetTaskById(taskToBeDeleted.TaskId)) && !childTask.Equals(x.GetTaskById(childTask.TaskId))));

        }
        [Test]
        public void ForceRemoveATask_HasSufTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeDeleted = DomainTestHelper.GetARandomTask(2);
            var sufTask = DomainTestHelper.GetARandomTask(3);
            sufTask.PreTaskIds = new List<int>() { taskToBeDeleted.TaskId };
            taskToBeDeleted.IsDeleted = true;
            sufTask.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(taskToBeDeleted);
            arrangeRoot.AddOrUpdateTask(sufTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeDeleted));
            Assert.IsTrue(arrangeRoot.TaskList.Contains(sufTask));

            //Act
            bool result = targetService.RemoveTask(taskToBeDeleted.TaskId, true);

            //Assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => x.TaskList.Count == 1 && !taskToBeDeleted.Equals(x.GetTaskById(taskToBeDeleted.TaskId)) && !sufTask.Equals(x.GetTaskById(sufTask.TaskId))));

        }
        [Test]
        public void RemoveATaskWithMemo_MemoMoveToGlobal()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeDeleted = DomainTestHelper.GetARandomTask(2);
            taskToBeDeleted.IsDeleted = true;
            var testMemo = DomainTestHelper.GetARandomMemo("1");
            taskToBeDeleted.AddOrUpdateMemo(testMemo);

            arrangeRoot.AddOrUpdateTask(taskToBeDeleted);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeDeleted));

            //Act
            bool result = targetService.RemoveTask(taskToBeDeleted.TaskId);

            //Assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => x.TaskList.Count == 1 && x.GetMemoById(testMemo.MemoId).Equals(testMemo) ));
        }
        [Test]
        public void ForceRemoveATaskWithMemo_ChildTaskAlsoHasMemo_MemoMoveToGlobal()
        {

            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeDeleted = DomainTestHelper.GetARandomTask(2);
            var childTask = DomainTestHelper.GetARandomTask(3);
            var testMemo = DomainTestHelper.GetARandomMemo("1");
            var testchildMemo = DomainTestHelper.GetARandomMemo("2");
            taskToBeDeleted.IsDeleted = true;
            childTask.IsDeleted = true;
            taskToBeDeleted.AddOrUpdateMemo(testMemo);
            childTask.AddOrUpdateMemo(testchildMemo);
            childTask.ParentTaskId = taskToBeDeleted.TaskId;
            arrangeRoot.AddOrUpdateTask(taskToBeDeleted);
            arrangeRoot.AddOrUpdateTask(childTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeDeleted));
            Assert.IsTrue(arrangeRoot.TaskList.Contains(childTask));

            //Act
            bool result = targetService.RemoveTask(taskToBeDeleted.TaskId, true);

            //Assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => x.TaskList.Count == 1 && x.GetMemoById(testMemo.MemoId).Equals(testMemo) && x.GetMemoById(testchildMemo.MemoId).Equals(testchildMemo)));
        }

        #endregion

        #region Delete Test
        [Test]
        public void DeleteATask_HasNoChildTask_TaskDidNotExist_throwsException()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            rootDataProvider.GetRootData().Returns(DomainTestHelper.GetRandomRoot());
            TaskService targetService = new TaskService(rootDataProvider);
            var testTask = DomainTestHelper.GetARandomTask(2);
            Assert.Throws<ArgumentException>(() => {
                bool result = targetService.DeleteTask(testTask.TaskId);
                Assert.IsFalse(result);
            });
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }
        [Test]
        public void DeleteATask_HasNoChildTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeDeleted = DomainTestHelper.GetARandomTask(2);
            arrangeRoot.AddOrUpdateTask(taskToBeDeleted);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeDeleted));

            //Act
            bool result = targetService.DeleteTask(taskToBeDeleted.TaskId);

            //Assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => x.GetTaskById(taskToBeDeleted.TaskId).IsDeleted));
        }
        [Test]
        public void DeleteATask_HasChildTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeDeleted = DomainTestHelper.GetARandomTask(2);
            var childTask = DomainTestHelper.GetARandomTask(3);
            childTask.ParentTaskId = taskToBeDeleted.TaskId;
            arrangeRoot.AddOrUpdateTask(taskToBeDeleted);
            arrangeRoot.AddOrUpdateTask(childTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeDeleted));
            Assert.IsTrue(arrangeRoot.TaskList.Contains(childTask));

            //Act Assert
            Assert.Throws<TaskHasChildTasksException>(() => {
                bool result = targetService.DeleteTask(taskToBeDeleted.TaskId);
                Assert.IsFalse(result);
            });
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());

        }
        [Test]
        public void DeleteATask_HasSufTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeDeleted = DomainTestHelper.GetARandomTask(2);
            var sufTask = DomainTestHelper.GetARandomTask(3);
            sufTask.PreTaskIds = new List<int>() { taskToBeDeleted.TaskId };
            arrangeRoot.AddOrUpdateTask(taskToBeDeleted);
            arrangeRoot.AddOrUpdateTask(sufTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeDeleted));
            Assert.IsTrue(arrangeRoot.TaskList.Contains(sufTask));

            //Act Assert
            Assert.Throws<TaskHasSufTasksException>(() => {
                bool result = targetService.DeleteTask(taskToBeDeleted.TaskId);
                Assert.IsFalse(result);
            });
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());

        }
        [Test]
        public void ForceDeleteATask_HasChildTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeDeleted = DomainTestHelper.GetARandomTask(2);
            var childTask = DomainTestHelper.GetARandomTask(3);
            childTask.ParentTaskId = taskToBeDeleted.TaskId;
            arrangeRoot.AddOrUpdateTask(taskToBeDeleted);
            arrangeRoot.AddOrUpdateTask(childTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeDeleted));
            Assert.IsTrue(arrangeRoot.TaskList.Contains(childTask));

            //Act
            bool result = targetService.DeleteTask(taskToBeDeleted.TaskId, true);

            //Assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => x.GetTaskById(taskToBeDeleted.TaskId).IsDeleted && x.GetTaskById(childTask.TaskId).IsDeleted));

        }
        [Test]
        public void ForceDeleteATask_HasSufTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeDeleted = DomainTestHelper.GetARandomTask(2);
            var sufTask = DomainTestHelper.GetARandomTask(3);
            sufTask.PreTaskIds = new List<int>() { taskToBeDeleted.TaskId };
            arrangeRoot.AddOrUpdateTask(taskToBeDeleted);
            arrangeRoot.AddOrUpdateTask(sufTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeDeleted));
            Assert.IsTrue(arrangeRoot.TaskList.Contains(sufTask));

            //Act
            bool result = targetService.DeleteTask(taskToBeDeleted.TaskId, true);

            //Assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => x.GetTaskById(taskToBeDeleted.TaskId).IsDeleted && x.GetTaskById(sufTask.TaskId).IsDeleted));

        }
        #endregion

        #region Recover Test
        [Test]
        public void RecoverATask_HasNoChildTask_TaskDidNotExist_throwsException()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            rootDataProvider.GetRootData().Returns(DomainTestHelper.GetRandomRoot());
            TaskService targetService = new TaskService(rootDataProvider);
            var testTask = DomainTestHelper.GetARandomTask(2);
            Assert.Throws<ArgumentException>(() => {
                bool result = targetService.RecoverTask(testTask.TaskId);
                Assert.IsFalse(result);
            });
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
        }
        [Test]
        public void RecoverATask_HasNoChildTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeRecovered = DomainTestHelper.GetARandomTask(2);
            taskToBeRecovered.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(taskToBeRecovered);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeRecovered));

            //Act
            bool result = targetService.RecoverTask(taskToBeRecovered.TaskId);

            //Assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => !x.GetTaskById(taskToBeRecovered.TaskId).IsDeleted));
        }
        [Test]
        public void RecoverATask_HasChildTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeRecovered = DomainTestHelper.GetARandomTask(2);
            var childTask = DomainTestHelper.GetARandomTask(3);
            childTask.ParentTaskId = taskToBeRecovered.TaskId;
            taskToBeRecovered.IsDeleted = true;
            childTask.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(taskToBeRecovered);
            arrangeRoot.AddOrUpdateTask(childTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeRecovered));
            Assert.IsTrue(arrangeRoot.TaskList.Contains(childTask));

            //Act Assert
                bool result = targetService.RecoverTask(taskToBeRecovered.TaskId);
                Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => !x.GetTaskById(taskToBeRecovered.TaskId).IsDeleted && !x.GetTaskById(childTask.TaskId).IsDeleted) );

        }
        [Test]
        public void RecoverATask_HasSufTask()
        {
            //Arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var taskToBeRecovered = DomainTestHelper.GetARandomTask(2);
            var sufTask = DomainTestHelper.GetARandomTask(3);
            taskToBeRecovered.IsDeleted = true;
            sufTask.IsDeleted = true;
            sufTask.PreTaskIds = new List<int>() { taskToBeRecovered.TaskId };
            arrangeRoot.AddOrUpdateTask(taskToBeRecovered);
            arrangeRoot.AddOrUpdateTask(sufTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            TaskService targetService = new TaskService(rootDataProvider);
            Assert.IsTrue(arrangeRoot.TaskList.Contains(taskToBeRecovered));
            Assert.IsTrue(arrangeRoot.TaskList.Contains(sufTask));

            //Act Assert
            bool result = targetService.RecoverTask(taskToBeRecovered.TaskId);
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => !x.GetTaskById(taskToBeRecovered.TaskId).IsDeleted && !x.GetTaskById(sufTask.TaskId).IsDeleted));

        }
        #endregion
        [Test]
        public void StartATaskWithPreTaskNotDone_throwException()
        {
            Assert.Fail();
        }

        [Test]
        public void StartATaskWithPreTaskDone()
        {
            Assert.Fail();
        }

        [Test]
        public void StartAndStopTaskMutipleTimes_HasCorrectDurationCount()
        {
            Assert.Fail();
        }

        [Test]
        public void StartAndStopTaskMutipleTimes_HasCorrectWorkDaysCount()
        {
            Assert.Fail();
        }

    }
}
