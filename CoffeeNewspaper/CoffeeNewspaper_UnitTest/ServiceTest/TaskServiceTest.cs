using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Repository;
using CN_Core.Interfaces.Service;
using CoffeeNewspaper_UnitTest.DomainTest;
using Ninject;
using NSubstitute;
using NUnit.Framework;

namespace CoffeeNewspaper_UnitTest.ServiceTest
{
    [TestFixture]
    public class TaskServiceTest : ServiceSetupTearDown
    {
        [Test]
        public async Task AddATimeSlice_AddaBiggerSlice_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);
            var addedTimeslice =
                new CNTimeSlice(now.AddHours(3));
            assesTask.UsedTimeSlices =
                new[]
                {
                    new CNTimeSlice(now.AddHours(1), now.AddHours(2)),
                    new CNTimeSlice(now.AddHours(2), now.AddHours(3).AddMilliseconds(-1))
                }.ToList();
            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));

            //Act
            await targetService.AddATimeSlice(assesTask.TaskId, addedTimeslice);

            //Assert
            mockTaskDataStore.Received()
                .ExpandTaskTime(Arg.Is<CNTask>(
                    x => x.TaskId == assesTask.TaskId), now.AddHours(1), null);
            mockTimesliceDataStore.Received()
                .AddTimeSlice(Arg.Is<CNTimeSlice>(x =>
                    x.Equals(addedTimeslice) && x.Task.TaskId == assesTask.TaskId));
        }

        [Test]
        public async Task AddATimeSlice_AddaSmallerSlice_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);
            var addedTimeslice = new CNTimeSlice(now.AddHours(2), now.AddHours(3).AddMilliseconds(-1));
            assesTask.UsedTimeSlices =
                new[]
                {
                    new CNTimeSlice(now.AddHours(1), now.AddHours(2)),
                    new CNTimeSlice(now.AddHours(3))
                }.ToList();
            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));

            //Act
            await targetService.AddATimeSlice(assesTask.TaskId, addedTimeslice);

            //Assert
            mockTaskDataStore.Received()
                .ExpandTaskTime(Arg.Is<CNTask>(
                    x => x.TaskId == assesTask.TaskId), now.AddHours(1), null);
            mockTimesliceDataStore.Received()
                .AddTimeSlice(Arg.Is<CNTimeSlice>(x =>
                    x.Equals(addedTimeslice) && x.Task.TaskId == assesTask.TaskId));
        }

        [Test]
        public async Task AddATimeSlice_TaskNotExist_Fail()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();

            //Assess
            mockTaskDataStore.GetTask(1).Returns(Task.FromResult((CNTask) null));

            //Act
            var result = await targetService.AddATimeSlice(1, DomainTestHelper.GetARandomTimeSlice());

            //Assert
            Assert.IsNull(result);
            mockTimesliceDataStore.DidNotReceive().AddTimeSlice(Arg.Any<CNTimeSlice>());
        }

        [Test]
        public void AddATimeSlice_TimeSliceIntercept_Fail()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);
            assesTask.UsedTimeSlices =
                new[]
                {
                    new CNTimeSlice(now.AddHours(1), now.AddHours(2)),
                    new CNTimeSlice(now.AddHours(3))
                }.ToList();
            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));

            //Act
            Assert.ThrowsAsync<TimeSliceInterveneException>(async () =>
                await targetService.AddATimeSlice(assesTask.TaskId, new CNTimeSlice(now.AddHours(4))));

            //Assert
             mockTimesliceDataStore.DidNotReceive().AddTimeSlice(Arg.Any<CNTimeSlice>());
        }

        [Test]
        public async Task GetTaskTimeSlices_TaskNull_ReturnNewEmptyList()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);

            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult((CNTask)null));

            //Act
            var result = await targetService.GetTaskTimeSlices(assesTask.TaskId);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0,result.Count);
            await mockTimesliceDataStore.DidNotReceiveWithAnyArgs().GetTimeSliceByTaskID(0);
        }
        [Test]
        public async Task GetTaskTimeSlices_Traverse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);

            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));

            //Act
            await targetService.GetTaskTimeSlices(assesTask.TaskId);

            //Assert
            await mockTimesliceDataStore.Received().GetTimeSliceByTaskID(assesTask.TaskId);
        }
        [Test]
        public async Task DeleteAllTimeSlicesOfTask_TaskIsNull_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);

            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult((CNTask)null));
            
            //Act
            await targetService.DeleteAllTimeSlicesOfTask(assesTask.TaskId);

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs()
                .UpdateTask(assesTask);
             mockTimesliceDataStore.DidNotReceiveWithAnyArgs().DeleteTimeSliceByTask(0);
        }
        [Test]
        public async Task DeleteAllTimeSlicesOfTask_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);
            var assesChildTask = DomainTestHelper.GetARandomTask(2);
            var assesSufTask = DomainTestHelper.GetARandomTask(3);
            assesTask.ChildTasks.Add(assesChildTask);
            assesTask.SufTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = assesTask,
                PreTaskId = assesTask.TaskId,
                SufTask = assesSufTask,
                SufTaskId = assesSufTask.TaskId
            });
            assesTask.StartTime = now.AddDays(-1);
            assesTask.EndTime = now.AddDays(1);

            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));

            //Act
            await targetService.DeleteAllTimeSlicesOfTask(assesTask.TaskId);

            //Assert
             mockTaskDataStore.Received()
                .UpdateTask(Arg.Is<CNTask>(x=>x.StartTime == null && x.EndTime == null));
             mockTimesliceDataStore.Received().DeleteTimeSliceByTask(assesTask.TaskId);
        }
        [Test]
        public async Task DeleteTimeSlices_ContainOnlyOneSlice_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);
            assesTask.StartTime = now.AddHours(1);

            assesTask.EndTime = now.AddHours(2);
            var addedTimeslice =
                new CNTimeSlice(now.AddHours(1), now.AddHours(2))
                {
                    Task = assesTask,
                    TimeSliceId = Guid.NewGuid().ToString("D")
                };
            assesTask.UsedTimeSlices =
                new[]
                {
                    addedTimeslice
                }.ToList();
            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));
            mockTimesliceDataStore.GetTimeSliceById(addedTimeslice.TimeSliceId)
                .Returns(Task.FromResult(addedTimeslice));
            //Act
            await targetService.DeleteTimeSlices(addedTimeslice);

            //Assert
             mockTaskDataStore.Received()
                .UpdateStartTaskTime(Arg.Is<CNTask>(x => x.TaskId == assesTask.TaskId), null);
             mockTaskDataStore.Received()
                .UpdateStartTaskTime(Arg.Is<CNTask>(x => x.TaskId == assesTask.TaskId), null);
             mockTimesliceDataStore.Received().DeleteTimeSlice(addedTimeslice);
        }

        [Test]
        public async Task DeleteTimeSlices_EndOfTwoSlice_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);
            assesTask.StartTime = now.AddHours(1);

            assesTask.EndTime = now.AddHours(3).AddMilliseconds(-1);
            var addedTimeslice =
                new CNTimeSlice(now.AddHours(2), now.AddHours(3).AddMilliseconds(-1))
                {
                    Task = assesTask,
                    TimeSliceId = Guid.NewGuid().ToString("D")
                };
            assesTask.UsedTimeSlices =
                new[]
                {
                    new CNTimeSlice(now.AddHours(1), now.AddHours(2)),
                    addedTimeslice
                }.ToList();
            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));
            mockTimesliceDataStore.GetTimeSliceById(addedTimeslice.TimeSliceId)
                .Returns(Task.FromResult(addedTimeslice));
            //Act
            await targetService.DeleteTimeSlices(addedTimeslice);

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateStartTaskTime(assesTask, null);
             mockTaskDataStore.Received()
                .UpdateEndTaskTime(Arg.Is<CNTask>(x => x.TaskId == assesTask.TaskId), now.AddHours(2));
             mockTimesliceDataStore.Received().DeleteTimeSlice(addedTimeslice);
        }
        
        [Test]
        public async Task DeleteTimeSlices_SliceDontExist_Fail()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);
            assesTask.StartTime = now.AddHours(1);

            assesTask.EndTime = now.AddHours(3).AddMilliseconds(-1);
            var addedTimeslice =
                new CNTimeSlice(now.AddHours(2), now.AddHours(3).AddMilliseconds(-1))
                {
                    Task = assesTask,
                    TimeSliceId = Guid.NewGuid().ToString("D")
                };
            assesTask.UsedTimeSlices =
                new[]
                {
                    new CNTimeSlice(now.AddHours(1), now.AddHours(2)),
                    addedTimeslice
                }.ToList();
            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));
            mockTimesliceDataStore.GetTimeSliceById(addedTimeslice.TimeSliceId)
                .Returns(Task.FromResult((CNTimeSlice) null));
            //Act
            var result = await targetService.DeleteTimeSlices(addedTimeslice);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateStartTaskTime(assesTask, null);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateEndTaskTime(assesTask, null);
             mockTimesliceDataStore.DidNotReceiveWithAnyArgs().DeleteTimeSlice(addedTimeslice);
        }

        [Test]
        public async Task DeleteTimeSlices_StartOfTwoSlice_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);
            assesTask.StartTime = now.AddHours(1);

            assesTask.EndTime = now.AddHours(3).AddMilliseconds(-1);
            var addedTimeslice =
                new CNTimeSlice(now.AddHours(1), now.AddHours(2))
                {
                    Task = assesTask,
                    TimeSliceId = Guid.NewGuid().ToString("D")
                };
            assesTask.UsedTimeSlices =
                new[]
                {
                    new CNTimeSlice(now.AddHours(2), now.AddHours(3).AddMilliseconds(-1)),
                    addedTimeslice
                }.ToList();
            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));
            mockTimesliceDataStore.GetTimeSliceById(addedTimeslice.TimeSliceId)
                .Returns(Task.FromResult(addedTimeslice));
            //Act
            await targetService.DeleteTimeSlices(addedTimeslice);

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateEndTaskTime(assesTask, null);
             mockTaskDataStore.Received()
                .UpdateStartTaskTime(Arg.Is<CNTask>(x => x.TaskId == assesTask.TaskId), now.AddHours(2));
             mockTimesliceDataStore.Received().DeleteTimeSlice(addedTimeslice);
        }
        
        [Test]
        public async Task EndTimeSlice_EndOfTwoSlice_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);
            var assesChildTask = DomainTestHelper.GetARandomTask(2);
            var assesSufTask = DomainTestHelper.GetARandomTask(3);
            assesTask.ChildTasks.Add(assesChildTask);
            assesTask.SufTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = assesTask,
                PreTaskId = assesTask.TaskId,
                SufTask = assesSufTask,
                SufTaskId = assesSufTask.TaskId
            });
            assesTask.StartTime = now.AddHours(1);

            assesTask.EndTime = now.AddHours(3).AddMilliseconds(-1);
            var addedTimeslice =
                new CNTimeSlice(now.AddHours(2)) {Task = assesTask, TimeSliceId = Guid.NewGuid().ToString("D")};
            assesTask.UsedTimeSlices =
                new[]
                {
                    new CNTimeSlice(now.AddHours(1), now.AddHours(2)),
                    addedTimeslice
                }.ToList();
            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));
            mockTimesliceDataStore.GetTimeSliceById(addedTimeslice.TimeSliceId)
                .Returns(Task.FromResult(addedTimeslice));
            //Act
            await targetService.EndTimeSlice(assesTask.TaskId, now.AddHours(4));

            //Assert
             mockTaskDataStore.Received()
                .UpdateEndTaskTime(Arg.Is<CNTask>(x => x.TaskId == assesTask.TaskId), now.AddHours(4));
            addedTimeslice.EndDateTime = now.AddHours(4);
             mockTimesliceDataStore.Received().UpdateTimeSlice(addedTimeslice);
        }

        [Test]
        public async Task EndTimeSlice_LastSliceStartTimeBiggerThanSetEndTime_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);
            var assesChildTask = DomainTestHelper.GetARandomTask(2);
            var assesSufTask = DomainTestHelper.GetARandomTask(3);
            assesTask.ChildTasks.Add(assesChildTask);
            assesTask.SufTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = assesTask,
                PreTaskId = assesTask.TaskId,
                SufTask = assesSufTask,
                SufTaskId = assesSufTask.TaskId
            });
            assesTask.StartTime = now.AddHours(1);

            assesTask.EndTime = now.AddHours(3).AddMilliseconds(-1);
            var addedTimeslice =
                new CNTimeSlice(now.AddHours(2)) { Task = assesTask, TimeSliceId = Guid.NewGuid().ToString("D") };
            assesTask.UsedTimeSlices =
                new[]
                {
                    new CNTimeSlice(now.AddHours(1), now.AddHours(2)),
                    addedTimeslice
                }.ToList();
            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));
            mockTimesliceDataStore.GetTimeSliceById(addedTimeslice.TimeSliceId)
                .Returns(Task.FromResult(addedTimeslice));
            //Act
            await targetService.EndTimeSlice(assesTask.TaskId, now.AddHours(1));

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs()
                .UpdateEndTaskTime(assesTask, now.AddHours(4));
            addedTimeslice.EndDateTime = now.AddHours(4);
             mockTimesliceDataStore.DidNotReceiveWithAnyArgs().UpdateTimeSlice(addedTimeslice);
        }

        [Test]
        public async Task EndTimeSlice_LastSliceEndTimeIsAlreadySet_DoNotingAndReturnTrue()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);
            var assesChildTask = DomainTestHelper.GetARandomTask(2);
            var assesSufTask = DomainTestHelper.GetARandomTask(3);
            assesTask.ChildTasks.Add(assesChildTask);
            assesTask.SufTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = assesTask,
                PreTaskId = assesTask.TaskId,
                SufTask = assesSufTask,
                SufTaskId = assesSufTask.TaskId
            });
            assesTask.StartTime = now.AddHours(1);

            assesTask.EndTime = now.AddHours(3).AddMilliseconds(-1);
            var addedTimeslice =
                new CNTimeSlice(now.AddHours(2),now.AddHours(5)) { Task = assesTask, TimeSliceId = Guid.NewGuid().ToString("D") };
            assesTask.UsedTimeSlices =
                new[]
                {
                    new CNTimeSlice(now.AddHours(1), now.AddHours(2)),
                    addedTimeslice
                }.ToList();
            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));
            mockTimesliceDataStore.GetTimeSliceById(addedTimeslice.TimeSliceId)
                .Returns(Task.FromResult(addedTimeslice));
            //Act
            await targetService.EndTimeSlice(assesTask.TaskId, now.AddHours(4));

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs()
                .UpdateEndTaskTime(assesTask, now.AddHours(4));
            addedTimeslice.EndDateTime = now.AddHours(4);
             mockTimesliceDataStore.DidNotReceiveWithAnyArgs().UpdateTimeSlice(addedTimeslice);
        }
        [Test]
        public async Task DeleteTask_EndOfTwoSlice_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var mockTimesliceDataStore = _kernel.Get<ITimeSliceDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var now = DateTime.Now;

            //Assess
            var assesTask = DomainTestHelper.GetARandomTask(1);
            assesTask.StartTime = now.AddHours(1);

            assesTask.EndTime = now.AddHours(3).AddMilliseconds(-1);
            var addedTimeslice =
                new CNTimeSlice(now.AddHours(2)) { Task = assesTask, TimeSliceId = Guid.NewGuid().ToString("D") };
            assesTask.UsedTimeSlices =
                new[]
                {
                    new CNTimeSlice(now.AddHours(1), now.AddHours(2)),
                    addedTimeslice
                }.ToList();
            mockTaskDataStore.GetTask(assesTask.TaskId).Returns(Task.FromResult(assesTask));
            mockTimesliceDataStore.GetTimeSliceById(addedTimeslice.TimeSliceId)
                .Returns(Task.FromResult(addedTimeslice));
            //Act
            await targetService.EndTimeSlice(assesTask.TaskId, now.AddHours(4));

            //Assert
             mockTaskDataStore.Received()
                .UpdateEndTaskTime(Arg.Is<CNTask>(x => x.TaskId == assesTask.TaskId), now.AddHours(4));
            addedTimeslice.EndDateTime = now.AddHours(4);
             mockTimesliceDataStore.Received().UpdateTimeSlice(addedTimeslice);
        }

        [Test]
        public async Task EditATask_TaskNull_Fail()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            

            //Act
            var result = await targetService.EditATask(null);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(null);
        }
        [Test]
        public void EditATask_TaskContentEmpty_Fail()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var task = DomainTestHelper.GetARandomTask(1);
            task.Content = String.Empty;

            //Act
            Assert.ThrowsAsync<ArgumentException>(async()=>await targetService.EditATask(task));

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(null);
        }
        [Test]
        public async Task EditATask_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var unitOfWork = _kernel.Get<IUnitOfWork>();
            var mockTask = DomainTestHelper.GetARandomTask(1);

            //Assess
            unitOfWork.Commit().Returns(Task.FromResult(true));

            //Act
            var result = await targetService.EditATask(mockTask);

            //Assert
            Assert.IsTrue(result);
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x=>x.Content.Equals(mockTask.Content)));
        }
        [Test]
        public async Task GetAllTasks_Traverse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();

            //Assess
            //Act
            await targetService.GetAllTasks();

            //Assert
            await mockTaskDataStore.Received().GetAllTask();
        }
        [Test]
        public async Task GetTaskById_Traverse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();

            //Assess
            //Act
            await targetService.GetTaskById(1);

            //Assert
            await mockTaskDataStore.Received().GetTask(1);
        }
        [Test]
        public async Task CreateATask_Traverse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask();
            //Assess
            //Act
            await targetService.CreateATask(mockTask);

            //Assert
             mockTaskDataStore.Received().AddTask(Arg.Is<CNTask>(x=>x.Content.Equals(mockTask.Content)));
        }

        [Test]
        public void CreateATask_TaskContentNotExists_ThrowException()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask();
            mockTask.Content = string.Empty;
            //Assess
            //Act

            Assert.ThrowsAsync<ArgumentException>(async()=>await targetService.CreateATask(mockTask));

             mockTaskDataStore.DidNotReceiveWithAnyArgs().AddTask(mockTask);
        }

        [Test]
        public async Task DeleteTask_TaskIsNull_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult((CNTask)null));

            //Act
            var result = await targetService.DeleteTask(mockTask.TaskId);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }
        [Test]
        public async Task DeleteTask_TaskIsDeleted_ReturnTrue()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.IsDeleted = true;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            var result = await targetService.DeleteTask(mockTask.TaskId);

            //Assert
            Assert.IsTrue(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }
        [Test]
        public void DeleteTask_TaskHasChildTasks_ThrowException()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            var mockChildTask = DomainTestHelper.GetARandomTask(2);
            mockTask.ChildTasks.Add(mockChildTask);
            mockTask.IsDeleted = false;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            Assert.ThrowsAsync<TaskHasChildTasksException>(async ()=> await targetService.DeleteTask(mockTask.TaskId)); 

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }
        [Test]
        public void DeleteTask_TaskHasSufTasks_ThrowException()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            var mockSufTask = DomainTestHelper.GetARandomTask(3);
            mockTask.SufTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = mockTask,
                SufTask = mockSufTask
            });
            mockTask.IsDeleted = false;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            Assert.ThrowsAsync<TaskHasSufTasksException>(async () => await targetService.DeleteTask(mockTask.TaskId));

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }

        [Test]
        public async Task DeleteTask_TaskHasSufTasksHasChildTasksForceDelete_ReturnTrue()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            var mockChildTask = DomainTestHelper.GetARandomTask(2);
            mockTask.ChildTasks.Add(mockChildTask);
            var mockSufTask = DomainTestHelper.GetARandomTask(3);
            mockTask.SufTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = mockTask,
                SufTask = mockSufTask
            });
            mockTask.IsDeleted = false;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            await targetService.DeleteTask(mockTask.TaskId,true);
            
            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => (x.TaskId == mockTask.TaskId) && x.IsDeleted));
        }



        [Test]
        public async Task RecoverATask_TaskIsNull_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult((CNTask)null));

            //Act
            var result = await targetService.RecoverATask(mockTask.TaskId);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }
        [Test]
        public async Task RecoverATask_TaskIsRecoverAd_ReturnTrue()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.IsDeleted = false;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            var result = await targetService.RecoverATask(mockTask.TaskId);

            //Assert
            Assert.IsTrue(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }
        [Test]
        public async Task RecoverATask_TaskHasSufTasksHasChildTasksForceRecoverA_ReturnTrue()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            var mockChildTask = DomainTestHelper.GetARandomTask(2);
            mockTask.ChildTasks.Add(mockChildTask);
            var mockSufTask = DomainTestHelper.GetARandomTask(3);
            mockTask.SufTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = mockTask,
                SufTask = mockSufTask
            });
            mockTask.IsDeleted = true;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            await targetService.RecoverATask(mockTask.TaskId);

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => (x.TaskId == mockTask.TaskId) && !x.IsDeleted));
        }



        [Test]
        public async Task RemoveTask_TaskIsNull_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult((CNTask)null));

            //Act
            var result = await targetService.RemoveATask(mockTask.TaskId);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }
        [Test]
        public async Task RemoveTask_TaskIsRemoved_ReturnTrue()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.IsDeleted = false;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            var result = await targetService.RemoveATask(mockTask.TaskId);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }
        [Test]
        public void RemoveTask_TaskHasChildTasks_ThrowException()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            var mockChildTask = DomainTestHelper.GetARandomTask(2);
            mockTask.ChildTasks.Add(mockChildTask);
            mockTask.IsDeleted = true;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            Assert.ThrowsAsync<TaskHasChildTasksException>(async () => await targetService.RemoveATask(mockTask.TaskId));

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }
        [Test]
        public void RemoveTask_TaskHasSufTasks_ThrowException()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            var mockSufTask = DomainTestHelper.GetARandomTask(3);
            mockTask.IsDeleted = true;
            mockTask.SufTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = mockTask,
                SufTask = mockSufTask
            });
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            Assert.ThrowsAsync<TaskHasSufTasksException>(async () => await targetService.RemoveATask(mockTask.TaskId));

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }

        [Test]
        public async Task RemoveTask_TaskHasSufTasksHasChildTasksForceRemove_ReturnTrue()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            var mockChildTask = DomainTestHelper.GetARandomTask(2);
            mockTask.ChildTasks.Add(mockChildTask);
            mockTask.IsDeleted = true;
            var mockSufTask = DomainTestHelper.GetARandomTask(3);
            mockTask.SufTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = mockTask,
                SufTask = mockSufTask
            });
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            await targetService.RemoveATask(mockTask.TaskId, true);

            //Assert
             mockTaskDataStore.Received().RemoveTask(Arg.Is<CNTask>(x => x.TaskId == mockTask.TaskId));
        }

        [Test]
        public async Task StartATask_TaskNotExist_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult((CNTask)null));

            //Act
            var result = await targetService.StartATask(mockTask.TaskId);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }

        [Test]
        public async Task StartATask_TaskIsDeleted_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.IsDeleted = true;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            var result = await targetService.StartATask(mockTask.TaskId);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public void StartATask_TaskStatusNotValid_ThrowsException()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.DOING;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            Assert.ThrowsAsync<TaskStatusException>(async () => await targetService.StartATask(mockTask.TaskId));

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public async Task StartATask_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var unitOfWork = _kernel.Get<IUnitOfWork>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.TODO;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));
            unitOfWork.Commit().Returns(Task.FromResult(true));
            //Act
            var result = await targetService.StartATask(mockTask.TaskId);

            //Assert
            Assert.IsTrue(result);
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.Status == CNTaskStatus.DOING && x.TaskId == mockTask.TaskId));
        }



        [Test]
        public async Task PauseATask_TaskNotExist_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult((CNTask)null));

            //Act
            var result = await targetService.PauseATask(mockTask.TaskId);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }

        [Test]
        public async Task PauseATask_TaskIsDeleted_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.IsDeleted = true;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            var result = await targetService.PauseATask(mockTask.TaskId);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public void PauseATask_TaskStatusNotValid_ThrowsException()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.PENDING;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            Assert.ThrowsAsync<TaskStatusException>(async () => await targetService.PauseATask(mockTask.TaskId));

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public async Task PauseATask_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.DOING;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            await targetService.PauseATask(mockTask.TaskId);

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.Status == CNTaskStatus.TODO && x.TaskId == mockTask.TaskId));
        }


        [Test]
        public async Task PendingATask_TaskNotExist_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult((CNTask)null));

            //Act
            var result = await targetService.PendingATask(mockTask.TaskId,null);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }

        [Test]
        public async Task PendingATask_TaskIsDeleted_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.IsDeleted = true;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            var result = await targetService.PendingATask(mockTask.TaskId, null);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public void PendingATask_TaskStatusNotValid_ThrowsException()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.DONE;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            Assert.ThrowsAsync<TaskStatusException>(async () => await targetService.PendingATask(mockTask.TaskId, null));

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public async Task PendingATask_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.DOING;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            await targetService.PendingATask(mockTask.TaskId, "pending reason");

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => 
                x.Status == CNTaskStatus.PENDING 
                && "pending reason".Equals(x.PendingReason)
                && x.TaskId == mockTask.TaskId));
        }


        [Test]
        public async Task FinishATask_TaskNotExist_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult((CNTask)null));

            //Act
            var result = await targetService.FinishATask(mockTask.TaskId);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }

        [Test]
        public async Task FinishATask_TaskIsDeleted_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.IsDeleted = true;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            var result = await targetService.FinishATask(mockTask.TaskId);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public void FinishATask_TaskStatusNotValid_ThrowsException()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.DONE;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            Assert.ThrowsAsync<TaskStatusException>(async () => await targetService.FinishATask(mockTask.TaskId));

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public async Task FinishATask_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.DOING;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            await targetService.FinishATask(mockTask.TaskId);

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.Status == CNTaskStatus.DONE && x.TaskId == mockTask.TaskId));
        }


        [Test]
        public async Task FailATask_TaskNotExist_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult((CNTask)null));

            //Act
            var result = await targetService.FailATask(mockTask.TaskId,null);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }


        [Test]
        public async Task FailATask_TaskAlreadyFail_UpdateReason()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.IsFail = true;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            await targetService.FailATask(mockTask.TaskId, "reason");

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x=>x.FailReason.Equals("reason") && x.TaskId == mockTask.TaskId));
        }
        [Test]
        public async Task FailATask_TaskNotAlreadyFail_UpdateBothFields()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.IsFail = false;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            await targetService.FailATask(mockTask.TaskId, "reason");

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.FailReason.Equals("reason") && x.IsFail && x.TaskId == mockTask.TaskId));
        }


        [Test]
        public async Task SetParentTask_TaskIsNull_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            var mockParentTask = DomainTestHelper.GetARandomTask(2);
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            await targetService.SetParentTask(0, mockParentTask.TaskId,-1);

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public async Task SetParentTask_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            var mockParentTask = DomainTestHelper.GetARandomTask(2);
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            mockTaskDataStore.GetTask(mockParentTask.TaskId).Returns(Task.FromResult(mockParentTask));
            //Act
            await targetService.SetParentTask(mockTask.TaskId, mockParentTask.TaskId,-1);

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.ParentTaskID == mockParentTask.TaskId && x.TaskId == mockTask.TaskId));
        }

        [Test]
        public async Task SetParentTask_SetParentTaskNull_WithNoPos()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(4);
            var mockSort = 10;
            var mockTask1 = DomainTestHelper.GetARandomTask(1);
            var mockTask2 = DomainTestHelper.GetARandomTask(2);
            var mockTask3 = DomainTestHelper.GetARandomTask(3);
            mockTask1.Sort = 1;
            mockTask2.Sort = 2;
            mockTask3.Sort = 3;
            //Assess
            mockTaskDataStore.GetAllTask().Returns(new List<CNTask>() { mockTask1, mockTask2, mockTask3 });
            mockTaskDataStore.GetMaxSort(0).Returns(Task.FromResult(mockSort));

            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));
            //Act
            await targetService.SetParentTask(mockTask.TaskId, 0, -1);

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.ParentTaskID == 0
                                                                              && x.TaskId == mockTask.TaskId
                                                                              && x.Sort == mockSort+1));
        }

        [Test]
        public async Task SetParentTask_SetParentTaskNull_WithLastPos()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask1 = DomainTestHelper.GetARandomTask(1);
            var mockTask2 = DomainTestHelper.GetARandomTask(2);
            var mockTask3 = DomainTestHelper.GetARandomTask(3);
            var targetTask = DomainTestHelper.GetARandomTask(4);
            mockTask1.Sort = 1;
            mockTask2.Sort = 2;
            mockTask3.Sort = 3;
            //Assess
            mockTaskDataStore.GetAllTask().Returns(new List<CNTask>(){mockTask1,mockTask2,mockTask3});

            mockTaskDataStore.GetTask(targetTask.TaskId).Returns(Task.FromResult(targetTask));
            //Act
            await targetService.SetParentTask(targetTask.TaskId, 0, 2);

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.ParentTaskID == 0
                                                                              && x.TaskId == targetTask.TaskId
                                                                              && x.Sort == mockTask3.Sort + 1));
        }

        [Test]
        public async Task SetParentTask_SetParentTaskNull_WithMiddlePos()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask1 = DomainTestHelper.GetARandomTask(1);
            var mockTask2 = DomainTestHelper.GetARandomTask(2);
            var mockTask3 = DomainTestHelper.GetARandomTask(3);
            var targetTask = DomainTestHelper.GetARandomTask(4);
            mockTask1.Sort = 1;
            mockTask2.Sort = 2;
            mockTask3.Sort = 3;
            //Assess
            mockTaskDataStore.GetAllTask().Returns(new List<CNTask>() { mockTask1, mockTask2, mockTask3 });

            mockTaskDataStore.GetTask(targetTask.TaskId).Returns(Task.FromResult(targetTask));
            //Act
            await targetService.SetParentTask(targetTask.TaskId, 0, 1);

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.ParentTaskID == 0
                                                                              && x.TaskId == targetTask.TaskId
                                                                              && x.Sort == mockTask2.Sort + 1));
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.TaskId == mockTask3.TaskId
                                                                              && x.Sort == mockTask2.Sort + 2));
        }

        [Test]
        public async Task SetParentTask_SetParentTaskNotNull_WithNoPos()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockParent = DomainTestHelper.GetARandomTask(10);
            var mockTask = DomainTestHelper.GetARandomTask(1);
            var mockSort = 10;
            //Assess
            mockTaskDataStore.GetMaxSort(mockParent.TaskId).Returns(Task.FromResult(mockSort));

            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));
            mockTaskDataStore.GetTask(mockParent.TaskId).Returns(Task.FromResult(mockParent));
            //Act
            await targetService.SetParentTask(mockTask.TaskId, mockParent.TaskId, -1);

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.ParentTaskID == mockParent.TaskId
                                                                              && x.TaskId == mockTask.TaskId
                                                                              && x.Sort == mockSort + 1));
        }

        [Test]
        public async Task SetParentTask_SetParentTaskNotNull_WithLastPos()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockParent = DomainTestHelper.GetARandomTask(10);
            var mockTask1 = DomainTestHelper.GetARandomTask(1);
            var mockTask2 = DomainTestHelper.GetARandomTask(2);
            var mockTask3 = DomainTestHelper.GetARandomTask(3);
            var targetTask = DomainTestHelper.GetARandomTask(4);
            mockTask1.Sort = 1;
            mockTask2.Sort = 2;
            mockTask3.Sort = 3;
            mockParent.ChildTasks.Add(mockTask1);
            mockParent.ChildTasks.Add(mockTask2);
            mockParent.ChildTasks.Add(mockTask3);
            //Assess
            mockTaskDataStore.GetTask(targetTask.TaskId).Returns(Task.FromResult(targetTask));
            mockTaskDataStore.GetTask(mockParent.TaskId).Returns(Task.FromResult(mockParent));

            //Act
            await targetService.SetParentTask(targetTask.TaskId, mockParent.TaskId, 5);

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.ParentTaskID == mockParent.TaskId
                                                                              && x.TaskId == targetTask.TaskId
                                                                              && x.Sort == mockTask3.Sort + 1));
        }

        [Test]
        public async Task SetParentTask_SetParentTaskNotNull_WithMiddlePos()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockParent = DomainTestHelper.GetARandomTask(10);
            var mockTask1 = DomainTestHelper.GetARandomTask(1);
            var mockTask2 = DomainTestHelper.GetARandomTask(2);
            var mockTask3 = DomainTestHelper.GetARandomTask(3);
            var targetTask = DomainTestHelper.GetARandomTask(4);
            mockTask1.Sort = 1;
            mockTask2.Sort = 2;
            mockTask3.Sort = 3;
            mockParent.ChildTasks.Add(mockTask1);
            mockParent.ChildTasks.Add(mockTask2);
            mockParent.ChildTasks.Add(mockTask3);
            //Assess
            mockTaskDataStore.GetTask(targetTask.TaskId).Returns(Task.FromResult(targetTask));
            mockTaskDataStore.GetTask(mockParent.TaskId).Returns(Task.FromResult(mockParent));

            //Act
            await targetService.SetParentTask(targetTask.TaskId, mockParent.TaskId, 1);

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.ParentTaskID == mockParent.TaskId
                                                                              && x.TaskId == targetTask.TaskId
                                                                              && x.Sort == mockTask2.Sort + 1));
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.TaskId == mockTask3.TaskId
                                                                              && x.Sort == mockTask2.Sort + 2));
        }
        [Test]
        public async Task AddPreTask_TargetTaskIsNull_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.DONE;
            //Assess

            //Act
            var result = await targetService.AddPreTask(null, null);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }


        [Test]
        public async Task AddPreTask_PreTaskExists_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            var mockPreTask = DomainTestHelper.GetARandomTask(2);
            mockTask.PreTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = mockPreTask,
                PreTaskId = mockPreTask.TaskId,
                SufTask = mockTask,
                SufTaskId = mockTask.TaskId
            });
            //Assess

            //Act
            var result = await targetService.AddPreTask(mockTask, mockPreTask);

            //Assert
            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }


        [Test]
        public async Task AddPreTask_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.TODO;
            var mockPreTask = DomainTestHelper.GetARandomTask(2);
            mockPreTask.Status = CNTaskStatus.TODO;
            mockPreTask.Status = CNTaskStatus.TODO;
            //Assess

            //Act
            await targetService.AddPreTask(mockTask, mockPreTask);

            //Assert
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x=>
                x.TaskId == mockTask.TaskId
                && x.PreTaskConnectors!=null
                && x.PreTaskConnectors.Count>0
                && x.PreTaskConnectors.Any(y=>y.PreTask.TaskId == mockPreTask.TaskId)));
        }


        [Test]
        public async Task DelPreTask_TargetTaskIsNull_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.DONE;
            //Assess
            //Act
            var result = await targetService.DelPreTask(null, null);

            //Assert
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }


        [Test]
        public async Task DelPreTask_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.PENDING;
            mockTask.PendingReason = CNConstants.PENDINGREASON_PreTaskNotComplete;
            var mockPreTask = DomainTestHelper.GetARandomTask(2);
            var mockPreTaskDone = DomainTestHelper.GetARandomTask(3);
            mockPreTaskDone.Status = CNTaskStatus.DONE;
            mockPreTask.Status = CNTaskStatus.TODO;
            mockTask.PreTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = mockPreTask,
                PreTaskId = mockPreTask.TaskId,
                SufTask = mockTask,
                SufTaskId = mockTask.TaskId
            });
            mockTask.PreTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = mockPreTaskDone,
                PreTaskId = mockPreTaskDone.TaskId,
                SufTask = mockTask,
                SufTaskId = mockTask.TaskId
            });
            //Assess
            mockTaskDataStore.RemoveTaskConnector(Arg.Is<CNTaskConnector>(x =>
                x.PreTaskId == mockPreTask.TaskId && x.SufTaskId == mockTask.TaskId));

            //Act
            await targetService.DelPreTask(mockTask, mockPreTask);

            //Assert
             mockTaskDataStore.Received().RemoveTaskConnector(Arg.Is<CNTaskConnector>(x =>
                x.PreTaskId == mockPreTask.TaskId && x.SufTaskId == mockTask.TaskId));
             mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x =>
                x.TaskId == mockTask.TaskId
                && x.PreTaskConnectors != null
                && x.PreTaskConnectors.Count > 0
                && x.PreTaskConnectors.Any(y => y.PreTask.TaskId == mockPreTask.TaskId)));
        }


        [Test]
        public async Task DelPreTask_PreTaskNotExist_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            var mockPreTask = DomainTestHelper.GetARandomTask(2);
            mockPreTask.Status = CNTaskStatus.TODO;
            //Assess
            //            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            var result = await targetService.DelPreTask(mockTask, mockPreTask);

            //Assert

            Assert.IsFalse(result);
             mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }


    }
}