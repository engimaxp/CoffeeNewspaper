using System;
using System.Collections.Generic;
using System.Linq;
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
        [Test]
        public void CreateATask_ParameterWrong_parentTaskNotExist_throwsArgException()
        {
            IRootDataProvider rootDataProvider = Substitute.For<IRootDataProvider>();
            rootDataProvider.GetRootData().Returns(DomainTestHelper.GetRandomRoot());
;            TaskService targetService = new TaskService(rootDataProvider);
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
            ; TaskService targetService = new TaskService(rootDataProvider);
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
            ; TaskService targetService = new TaskService(rootDataProvider);
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
            ; TaskService targetService = new TaskService(rootDataProvider);
            var testTask = DomainTestHelper.GetARandomTask(2);
            int taskid = targetService.CreateATask(testTask);
            testTask.TaskId = taskid;
            rootDataProvider.Received().Persistence(Arg.Is<CNRoot>(x=>Equals(x.GetTaskById(taskid), testTask)));
        }

        [Test]
        public void DeleteATask()
        {
            Assert.Fail();
        }

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
