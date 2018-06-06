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
    public class MemoServiceTest : ServiceSetupTearDown
    {
        [Test]
        public async Task AddAMemoToTask_TaskExist_Success()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            mockTaskDataStore.GetTask(1).Returns(Task.FromResult(DomainTestHelper.GetARandomTask()));
            mockMemoDataStore.AddMemo(Arg.Any<CNMemo>())
                .Returns(Task.FromResult(DomainTestHelper.GetARandomMemo(true)));

            //Act
            var result = await targetService.AddAMemoToTask(DomainTestHelper.GetARandomMemo(), 1);

            //Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));
            await mockMemoDataStore.Received().AddMemo(Arg.Is<CNMemo>(x => x.TaskMemos.Count > 0));
        }

        [Test]
        public async Task AddAMemoToTask_TaskExistMemoExist_Success()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            mockTaskDataStore.GetTask(1).Returns(Task.FromResult(DomainTestHelper.GetARandomTask()));
            mockMemoDataStore.UpdateMemo(Arg.Any<CNMemo>())
                .Returns(Task.FromResult(true));

            //Act
            var result = await targetService.AddAMemoToTask(DomainTestHelper.GetARandomMemo(true), 1);

            //Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));
            await mockMemoDataStore.Received().UpdateMemo(Arg.Is<CNMemo>(x => x.TaskMemos.Count > 0));
        }

        [Test]
        public async Task AddAMemoToTask_TaskNotExist_Fail()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            mockTaskDataStore.GetTask(1).Returns(Task.FromResult((CNTask) null));

            //Act
            var result = await targetService.AddAMemoToTask(DomainTestHelper.GetARandomMemo(), 1);

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(result));
            await mockMemoDataStore.DidNotReceiveWithAnyArgs().AddMemo(Arg.Any<CNMemo>());
        }

        [Test]
        public async Task DeleteAMemo_MemoExist_Success()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            var testMemo = DomainTestHelper.GetARandomMemo(true);
            mockMemoDataStore.GetMemoById(testMemo.MemoId).Returns(Task.FromResult(testMemo));

            //Act
            var result = await targetService.DeleteAMemo(testMemo.MemoId);

            //Assert
            Assert.IsFalse(result);
            await mockMemoDataStore.Received().DeleteMemo(Arg.Is<CNMemo>(x => x.MemoId == testMemo.MemoId));
        }

        [Test]
        public async Task DeleteAMemo_MemoNotExist_Fail()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            var testMemo = DomainTestHelper.GetARandomMemo(true);
            mockMemoDataStore.GetMemoById(testMemo.MemoId).Returns(Task.FromResult((CNMemo) null));

            //Act
            var result = await targetService.DeleteAMemo(testMemo.MemoId);

            //Assert
            Assert.IsFalse(result);
            await mockMemoDataStore.DidNotReceiveWithAnyArgs().DeleteMemo(Arg.Any<CNMemo>());
        }


        [Test]
        public async Task RemoveAMemoFromTask_MemoNotExist_Fail()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            var assesMemo = DomainTestHelper.GetARandomMemo(true);
            mockMemoDataStore.GetMemoById(assesMemo.MemoId).Returns(Task.FromResult((CNMemo) null));

            //Act
            var result = await targetService.RemoveAMemoFromTask(assesMemo.MemoId, 1);

            //Assert
            Assert.False(result);
            await mockMemoDataStore.DidNotReceiveWithAnyArgs().UpdateMemo(Arg.Any<CNMemo>());
        }

        [Test]
        public async Task RemoveAMemoFromTask_Success()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            var assesMemo = DomainTestHelper.GetARandomMemo(true);
            var assesTask = DomainTestHelper.GetARandomTask();
            assesMemo.TaskMemos.Add(new CNTaskMemo
            {
                Memo = assesMemo,
                MemoId = assesMemo.MemoId,
                Task = assesTask,
                TaskId = assesTask.TaskId
            });
            mockMemoDataStore.GetMemoById(assesMemo.MemoId).Returns(Task.FromResult(assesMemo));
            mockMemoDataStore.UpdateMemo(Arg.Any<CNMemo>()).Returns(Task.FromResult(true));
            //Act
            var result = await targetService.RemoveAMemoFromTask(assesMemo.MemoId, assesTask.TaskId);

            //Assert
            Assert.True(result);
            await mockMemoDataStore.Received().UpdateMemo(Arg.Is<CNMemo>(x => x.TaskMemos.Count == 0));
        }

        [Test]
        public async Task RemoveAMemoFromTask_TaskMemoRelationNotExist_Fail()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            var assesMemo = DomainTestHelper.GetARandomMemo(true);
            mockMemoDataStore.GetMemoById(assesMemo.MemoId).Returns(Task.FromResult(assesMemo));

            //Act
            var result = await targetService.RemoveAMemoFromTask(assesMemo.MemoId, 1);

            //Assert
            Assert.False(result);
            await mockMemoDataStore.DidNotReceiveWithAnyArgs().UpdateMemo(Arg.Any<CNMemo>());
        }
    }
}