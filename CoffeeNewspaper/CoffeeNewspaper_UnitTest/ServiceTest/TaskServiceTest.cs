﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
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
            await mockTaskDataStore.Received()
                .ExpandTaskTime(Arg.Is<CNTask>(
                    x => x.TaskId == assesTask.TaskId), now.AddHours(1), null);
            await mockTimesliceDataStore.Received()
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
            await mockTaskDataStore.Received()
                .ExpandTaskTime(Arg.Is<CNTask>(
                    x => x.TaskId == assesTask.TaskId), now.AddHours(1), null);
            await mockTimesliceDataStore.Received()
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
            await mockTimesliceDataStore.DidNotReceive().AddTimeSlice(Arg.Any<CNTimeSlice>());
        }

        [Test]
        public async Task AddATimeSlice_TimeSliceIntercept_Fail()
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
            await mockTimesliceDataStore.DidNotReceive().AddTimeSlice(Arg.Any<CNTimeSlice>());
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs()
                .UpdateTask(assesTask);
            await mockTimesliceDataStore.DidNotReceiveWithAnyArgs().DeleteTimeSliceByTask(0);
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
            await mockTaskDataStore.Received()
                .UpdateTask(Arg.Is<CNTask>(x=>x.StartTime == null && x.EndTime == null));
            await mockTimesliceDataStore.Received().DeleteTimeSliceByTask(assesTask.TaskId);
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
            await mockTaskDataStore.Received()
                .UpdateStartTaskTime(Arg.Is<CNTask>(x => x.TaskId == assesTask.TaskId), null);
            await mockTaskDataStore.Received()
                .UpdateStartTaskTime(Arg.Is<CNTask>(x => x.TaskId == assesTask.TaskId), null);
            await mockTimesliceDataStore.Received().DeleteTimeSlice(addedTimeslice);
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
//            await mockTaskDataStore.Received().UpdateStartTaskTime(Arg.Is<CNTask>(x=>x.TaskId == assesTask.TaskId), null);
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateStartTaskTime(assesTask, null);
            await mockTaskDataStore.Received()
                .UpdateEndTaskTime(Arg.Is<CNTask>(x => x.TaskId == assesTask.TaskId), now.AddHours(2));
            await mockTimesliceDataStore.Received().DeleteTimeSlice(addedTimeslice);
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateStartTaskTime(assesTask, null);
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateEndTaskTime(assesTask, null);
            await mockTimesliceDataStore.DidNotReceiveWithAnyArgs().DeleteTimeSlice(addedTimeslice);
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
            //            await mockTaskDataStore.Received().UpdateStartTaskTime(Arg.Is<CNTask>(x=>x.TaskId == assesTask.TaskId), null);
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateEndTaskTime(assesTask, null);
            await mockTaskDataStore.Received()
                .UpdateStartTaskTime(Arg.Is<CNTask>(x => x.TaskId == assesTask.TaskId), now.AddHours(2));
            await mockTimesliceDataStore.Received().DeleteTimeSlice(addedTimeslice);
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
            await mockTaskDataStore.Received()
                .UpdateEndTaskTime(Arg.Is<CNTask>(x => x.TaskId == assesTask.TaskId), now.AddHours(4));
            addedTimeslice.EndDateTime = now.AddHours(4);
            await mockTimesliceDataStore.Received().UpdateTimeSlice(addedTimeslice);
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs()
                .UpdateEndTaskTime(assesTask, now.AddHours(4));
            addedTimeslice.EndDateTime = now.AddHours(4);
            await mockTimesliceDataStore.DidNotReceiveWithAnyArgs().UpdateTimeSlice(addedTimeslice);
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs()
                .UpdateEndTaskTime(assesTask, now.AddHours(4));
            addedTimeslice.EndDateTime = now.AddHours(4);
            await mockTimesliceDataStore.DidNotReceiveWithAnyArgs().UpdateTimeSlice(addedTimeslice);
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
            await mockTaskDataStore.Received()
                .UpdateEndTaskTime(Arg.Is<CNTask>(x => x.TaskId == assesTask.TaskId), now.AddHours(4));
            addedTimeslice.EndDateTime = now.AddHours(4);
            await mockTimesliceDataStore.Received().UpdateTimeSlice(addedTimeslice);
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(null);
        }
        [Test]
        public async Task EditATask_TaskContentEmpty_Fail()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var task = DomainTestHelper.GetARandomTask(1);
            task.Content = String.Empty;

            //Act
            Assert.ThrowsAsync<ArgumentException>(async()=>await targetService.EditATask(task));

            //Assert
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(null);
        }
        [Test]
        public async Task EditATask_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);

            //Assess
            mockTaskDataStore.UpdateTask(mockTask).Returns(Task.FromResult(true));

            //Act
            var result = await targetService.EditATask(mockTask);

            //Assert
            Assert.IsTrue(result);
            await mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x=>x.Content.Equals(mockTask.Content)));
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
            await mockTaskDataStore.Received().AddTask(Arg.Is<CNTask>(x=>x.Content.Equals(mockTask.Content)));
        }

        [Test]
        public async Task CreateATask_TaskContentNotExists_ThrowException()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask();
            mockTask.Content = string.Empty;
            //Assess
            //Act

            Assert.ThrowsAsync<ArgumentException>(async()=>await targetService.CreateATask(mockTask));

            await mockTaskDataStore.DidNotReceiveWithAnyArgs().AddTask(mockTask);
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }
        [Test]
        public async Task DeleteTask_TaskHasChildTasks_ThrowException()
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }
        [Test]
        public async Task DeleteTask_TaskHasSufTasks_ThrowException()
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
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
            await mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => (x.TaskId == mockTask.TaskId) && x.IsDeleted));
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
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
            await mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => (x.TaskId == mockTask.TaskId) && !x.IsDeleted));
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }
        [Test]
        public async Task RemoveTask_TaskHasChildTasks_ThrowException()
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
        }
        [Test]
        public async Task RemoveTask_TaskHasSufTasks_ThrowException()
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().RemoveTask(mockTask);
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
            await mockTaskDataStore.Received().RemoveTask(Arg.Is<CNTask>(x => x.TaskId == mockTask.TaskId));
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public async Task StartATask_TaskStatusNotValid_ThrowsException()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.DONE;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            Assert.ThrowsAsync<TaskStatusException>(async () => await targetService.StartATask(mockTask.TaskId));

            //Assert
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public async Task StartATask_Success()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.TODO;
            //Assess
            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            var result = await targetService.StartATask(mockTask.TaskId);

            //Assert
            Assert.IsTrue(result);
            await mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.Status == CNTaskStatus.DOING && x.TaskId == mockTask.TaskId));
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public async Task PauseATask_TaskStatusNotValid_ThrowsException()
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
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
            await mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.Status == CNTaskStatus.TODO && x.TaskId == mockTask.TaskId));
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public async Task PendingATask_TaskStatusNotValid_ThrowsException()
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
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
            await mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => 
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
        [Test]
        public async Task FinishATask_TaskStatusNotValid_ThrowsException()
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
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
            await mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.Status == CNTaskStatus.DONE && x.TaskId == mockTask.TaskId));
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
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
            await mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x=>x.FailReason.Equals("reason") && x.TaskId == mockTask.TaskId));
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
            await mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.FailReason.Equals("reason") && x.IsFail && x.TaskId == mockTask.TaskId));
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
            await targetService.SetParentTask(null, mockParentTask);

            //Assert
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
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

            //Act
            await targetService.SetParentTask(mockTask, mockParentTask);

            //Assert
            await mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x => x.ParentTask!=null && x.ParentTask.TaskId == mockParentTask.TaskId && x.TaskId == mockTask.TaskId));
        }


        [Test]
        public async Task AddPreTask_TargetTaskIsNull_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.DONE;
            //Assess
//            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            var result = await targetService.AddPreTask(null, null);

            //Assert
            Assert.IsFalse(result);
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
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
            //            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            var result = await targetService.AddPreTask(mockTask, mockPreTask);

            //Assert
            Assert.IsFalse(result);
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
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
            //            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            await targetService.AddPreTask(mockTask, mockPreTask);

            //Assert
            await mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x=>
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
            //            mockTaskDataStore.GetTask(mockTask.TaskId).Returns(Task.FromResult(mockTask));

            //Act
            var result = await targetService.DelPreTask(null, null);

            //Assert
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
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
                x.PreTaskId == mockPreTask.TaskId && x.SufTaskId == mockTask.TaskId)).Returns(Task.FromResult(true));

            //Act
            await targetService.DelPreTask(mockTask, mockPreTask);

            //Assert
            await mockTaskDataStore.Received().RemoveTaskConnector(Arg.Is<CNTaskConnector>(x =>
                x.PreTaskId == mockPreTask.TaskId && x.SufTaskId == mockTask.TaskId));
            await mockTaskDataStore.Received().UpdateTask(Arg.Is<CNTask>(x =>
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
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }


        [Test]
        public async Task DelPreTask_DBFail_ReturnFalse()
        {
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<ITaskService>();
            var mockTask = DomainTestHelper.GetARandomTask(1);
            mockTask.Status = CNTaskStatus.PENDING;
            mockTask.PendingReason = CNConstants.PENDINGREASON_PreTaskNotComplete;
            var mockPreTask = DomainTestHelper.GetARandomTask(2);
            mockPreTask.Status = CNTaskStatus.TODO;
            mockTask.PreTaskConnectors.Add(new CNTaskConnector()
            {
                PreTask = mockPreTask,
                PreTaskId = mockPreTask.TaskId,
                SufTask = mockTask,
                SufTaskId = mockTask.TaskId
            });
            //Assess
            mockTaskDataStore.RemoveTaskConnector(Arg.Is<CNTaskConnector>(x =>
                x.PreTaskId == mockPreTask.TaskId && x.SufTaskId == mockTask.TaskId)).Returns(Task.FromResult(false));

            //Act
            var result = await targetService.DelPreTask(mockTask, mockPreTask);

            //Assert
            Assert.IsFalse(result);
            await mockTaskDataStore.Received().RemoveTaskConnector(Arg.Is<CNTaskConnector>(x =>
                x.PreTaskId == mockPreTask.TaskId && x.SufTaskId == mockTask.TaskId));
            await mockTaskDataStore.DidNotReceiveWithAnyArgs().UpdateTask(mockTask);
        }
    }
}