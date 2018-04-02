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
        public void a_CreateATask_ParameterWrong_parentTaskNotExist_throwsArgException()
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
        public void a_CreateATask_ParameterWrong_pretaskNotExist_throwsArgException()
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
        public void a_CreateATask_ParameterWrong_ContentEmpty_throwsArgException()
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
        public void a_CreateATask_ParameterCorrect()
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
        public void b_RemoveATask_HasNoChildTask_TaskDidNotExist_throwsException()
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
        public void b_RemoveATask_HasNoChildTask_StatusNotDelete_throwsException()
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
        public void b_RemoveATask_HasNoChildTask()
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
        public void b_RemoveATask_HasChildTask()
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
        public void b_RemoveATask_HasSufTask()
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
        public void b_ForceRemoveATask_HasChildTask()
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
        public void b_ForceRemoveATask_HasSufTask()
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
        public void b_RemoveATaskWithMemo_MemoMoveToGlobal()
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
        public void b_ForceRemoveATaskWithMemo_ChildTaskAlsoHasMemo_MemoMoveToGlobal()
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
        public void c_DeleteATask_HasNoChildTask_TaskDidNotExist_throwsException()
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
        public void c_DeleteATask_HasNoChildTask()
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
        public void c_DeleteATask_HasChildTask()
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
        public void c_DeleteATask_HasSufTask()
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
        public void c_ForceDeleteATask_HasChildTask()
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
        public void c_ForceDeleteATask_HasSufTask()
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
        public void d_RecoverATask_HasNoChildTask_TaskDidNotExist_throwsException()
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
        public void d_RecoverATask_HasNoChildTask()
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
        public void d_RecoverATask_HasChildTask()
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
        public void d_RecoverATask_HasSufTask()
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

        #region Start Test

        [Test]
        public void e_StartATask()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = arrangeRoot.GetFirstTask();
            targetTask.Status = CNTaskStatus.TODO;
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act
            var result = targetService.StartATask(targetTask.TaskId);

            //assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => x.GetTaskById(targetTask.TaskId).Status == CNTaskStatus.DOING));
            timeSliceService.Received().AddATimeSlice(Arg.Is<CNTask>(x => x.TaskId == targetTask.TaskId), Arg.Is<CNTimeSlice>(x => x.EndDateTime == null && DateTime.Now - x.StartDateTime <= TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void e_StartATask_TaskStateInvalid_throwsException()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = arrangeRoot.GetFirstTask();
            targetTask.Status = CNTaskStatus.DOING;
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act assert
            Assert.Throws<TaskStatusException>(() => {
                var result = targetService.StartATask(targetTask.TaskId);
                Assert.IsFalse(result);
            });

            //assert
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
            timeSliceService.DidNotReceive().AddATimeSlice(Arg.Any<CNTask>(),Arg.Any<CNTimeSlice>());
        }

        [Test]
        public void e_StartATask_TaskIsDeleted_throwsException()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = arrangeRoot.GetFirstTask();
            targetTask.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act assert
            Assert.Throws<ArgumentException>(() => {
                var result = targetService.StartATask(targetTask.TaskId);
                Assert.IsFalse(result);
            });

            //assert
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
            timeSliceService.DidNotReceive().AddATimeSlice(Arg.Any<CNTask>(), Arg.Any<CNTimeSlice>());
        }

        [Test]
        public void e_StartATask_TaskNotExisit_throwsException()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = arrangeRoot.GetFirstTask();
            targetTask.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act assert
            Assert.Throws<ArgumentException>(() => {
                var result = targetService.StartATask(2);
                Assert.IsFalse(result);
            });

            //assert
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
            timeSliceService.DidNotReceive().AddATimeSlice(Arg.Any<CNTask>(), Arg.Any<CNTimeSlice>());
        }
        #endregion

        #region Pause Test

        [Test]
        public void f_PauseATask()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = arrangeRoot.GetFirstTask();
            targetTask.Status = CNTaskStatus.DOING;
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act
            var result = targetService.PauseATask(targetTask.TaskId);

            //assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => x.GetTaskById(targetTask.TaskId).Status == CNTaskStatus.TODO));
            timeSliceService.Received().EndTimeSlice(Arg.Is<CNTask>(x => x.TaskId == targetTask.TaskId), Arg.Is<DateTime>(x => DateTime.Now - x <= TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void f_PauseATask_TaskStateInvalid_throwsException()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = arrangeRoot.GetFirstTask();
            targetTask.Status = CNTaskStatus.DONE;
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act assert
            Assert.Throws<TaskStatusException>(() => {
                var result = targetService.PauseATask(targetTask.TaskId);
                Assert.IsFalse(result);
            });

            //assert
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
            timeSliceService.DidNotReceive().AddATimeSlice(Arg.Any<CNTask>(), Arg.Any<CNTimeSlice>());
        }

        [Test]
        public void f_PauseATask_TaskIsDeleted_throwsException()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = arrangeRoot.GetFirstTask();
            targetTask.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act assert
            Assert.Throws<ArgumentException>(() => {
                var result = targetService.PauseATask(targetTask.TaskId);
                Assert.IsFalse(result);
            });

            //assert
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
            timeSliceService.DidNotReceive().AddATimeSlice(Arg.Any<CNTask>(), Arg.Any<CNTimeSlice>());
        }

        [Test]
        public void f_PauseATask_TaskNotExisit_throwsException()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = arrangeRoot.GetFirstTask();
            targetTask.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act assert
            Assert.Throws<ArgumentException>(() => {
                var result = targetService.PauseATask(2);
                Assert.IsFalse(result);
            });

            //assert
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
            timeSliceService.DidNotReceive().AddATimeSlice(Arg.Any<CNTask>(), Arg.Any<CNTimeSlice>());
        }
        #endregion

        #region Finish Test

        [Test]
        public void g_FinishATaskWithPreTaskNotFinish_throwException()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var preTask = DomainTestHelper.GetARandomTask(2);
            var sufTask = DomainTestHelper.GetARandomTask(3);
            sufTask.PreTaskIds = new List<int>(){ preTask.TaskId };
            preTask.Status = CNTaskStatus.DOING;
            sufTask.Status = CNTaskStatus.DOING;
            arrangeRoot.AddOrUpdateTask(preTask);
            arrangeRoot.AddOrUpdateTask(sufTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act
            var exception = Assert.Throws<PreTaskNotEndedException>(() =>
            {
                var result = targetService.FinishATask(sufTask.TaskId);
                Assert.IsFalse(result);
            }).ToString();
            //assert
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
            timeSliceService.DidNotReceive().EndTimeSlice(Arg.Any<CNTask>(), Arg.Any<DateTime>());
        }

        [Test]
        public void g_FinishATaskWithPreTaskFinish()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var preTask = DomainTestHelper.GetARandomTask(2);
            var sufTask = DomainTestHelper.GetARandomTask(3);
            sufTask.PreTaskIds = new List<int>() { preTask.TaskId };
            preTask.Status = CNTaskStatus.DONE;
            sufTask.Status = CNTaskStatus.DOING;
            arrangeRoot.AddOrUpdateTask(preTask);
            arrangeRoot.AddOrUpdateTask(sufTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act
            var result = targetService.FinishATask(sufTask.TaskId);

            //assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => x.GetTaskById(sufTask.TaskId).Status == CNTaskStatus.DONE));
            timeSliceService.Received().EndTimeSlice(Arg.Is<CNTask>(x => x.TaskId == sufTask.TaskId), Arg.Is<DateTime>(x => DateTime.Now - x <= TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void g_FinishATaskWithNoPreTask()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = arrangeRoot.GetFirstTask();
            targetTask.Status = CNTaskStatus.DOING;
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act
            var result = targetService.FinishATask(targetTask.TaskId);

            //assert
            Assert.IsTrue(result);
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x => x.GetTaskById(targetTask.TaskId).Status == CNTaskStatus.DONE));
            timeSliceService.Received().EndTimeSlice(Arg.Is<CNTask>(x => x.TaskId == targetTask.TaskId), Arg.Is<DateTime>(x => DateTime.Now - x <= TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void g_FinishATask_TaskStateInvalid_throwsException()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = arrangeRoot.GetFirstTask();
            targetTask.Status = CNTaskStatus.DONE;
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act assert
            Assert.Throws<TaskStatusException>(() => {
                var result = targetService.FinishATask(targetTask.TaskId);
                Assert.IsFalse(result);
            });

            //assert
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
            timeSliceService.DidNotReceive().AddATimeSlice(Arg.Any<CNTask>(), Arg.Any<CNTimeSlice>());
        }

        [Test]
        public void g_FinishATask_TaskIsDeleted_throwsException()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = arrangeRoot.GetFirstTask();
            targetTask.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act assert
            Assert.Throws<ArgumentException>(() => {
                var result = targetService.FinishATask(targetTask.TaskId);
                Assert.IsFalse(result);
            });

            //assert
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
            timeSliceService.DidNotReceive().AddATimeSlice(Arg.Any<CNTask>(), Arg.Any<CNTimeSlice>());
        }

        [Test]
        public void g_FinishATask_TaskNotExisit_throwsException()
        {
            //arrange
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            var arrangeRoot = DomainTestHelper.GetRandomRoot();
            var targetTask = arrangeRoot.GetFirstTask();
            targetTask.IsDeleted = true;
            arrangeRoot.AddOrUpdateTask(targetTask);
            rootDataProvider.GetRootData().Returns(arrangeRoot);
            ITimeSliceService timeSliceService = Substitute.For<ITimeSliceService>();
            TaskService targetService = new TaskService(timeSliceService, rootDataProvider);

            //act assert
            Assert.Throws<ArgumentException>(() => {
                var result = targetService.FinishATask(2);
                Assert.IsFalse(result);
            });

            //assert
            rootDataProvider.DidNotReceive().Persistence(Arg.Any<CNRoot>());
            timeSliceService.DidNotReceive().AddATimeSlice(Arg.Any<CNTask>(), Arg.Any<CNTimeSlice>());
        }
        #endregion

    }
}
