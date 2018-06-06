using System;
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
        
    }
}