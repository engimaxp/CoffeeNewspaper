using System.Collections.Generic;
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
        public async Task AddAMemoToTask_MemoNull_Fail()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            mockTaskDataStore.GetTask(1).Returns(Task.FromResult(DomainTestHelper.GetARandomTask()));
            mockMemoDataStore.AddMemo(Arg.Any<CNMemo>())
                .Returns(DomainTestHelper.GetARandomMemo(true));

            //Act
            var result = await targetService.AddAMemoToTask(null, 1);

            //Assert
            Assert.True(string.IsNullOrEmpty(result));
            mockMemoDataStore.DidNotReceiveWithAnyArgs().AddMemo(Arg.Any<CNMemo>());
        }
        [Test]
        public async Task AddAMemoToTask_TaskExist_Success()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<IMemoService>();
            var mockTask = DomainTestHelper.GetARandomTask();
            var mockTag = DomainTestHelper.GetARandomTag();
            mockTask.TaskTaggers = new List<CNTaskTagger>(){new CNTaskTagger(){Tag = mockTag,Task = mockTask}};
            //Assess
            mockTaskDataStore.GetTask(1).Returns(Task.FromResult(mockTask));
            mockMemoDataStore.AddMemo(Arg.Any<CNMemo>())
                .Returns(DomainTestHelper.GetARandomMemo(true));

            //Act
            var result = await targetService.AddAMemoToTask(DomainTestHelper.GetARandomMemo(), 1);

            //Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));
            mockMemoDataStore.Received().AddMemo(Arg.Is<CNMemo>(x => x.TaskMemos.Count > 0 && x.MemoTaggers.Count>0));
        }

        [Test]
        public async Task AddAMemoToTask_TaskExistMemoExist_Success()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            mockTaskDataStore.GetTask(1).Returns(Task.FromResult(DomainTestHelper.GetARandomTask()));
            mockMemoDataStore.UpdateMemo(Arg.Any<CNMemo>());

            //Act
            var result = await targetService.AddAMemoToTask(DomainTestHelper.GetARandomMemo(true), 1);

            //Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));
            mockMemoDataStore.Received().UpdateMemo(Arg.Is<CNMemo>(x => x.TaskMemos.Count > 0));
        }

        [Test]
        public async Task AddAMemoToTask_DBFail_False()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var mockTaskDataStore = _kernel.Get<ITaskDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            mockTaskDataStore.GetTask(1).Returns(Task.FromResult(DomainTestHelper.GetARandomTask()));
            mockMemoDataStore.UpdateMemo(Arg.Any<CNMemo>());

            //Act
            await targetService.AddAMemoToTask(DomainTestHelper.GetARandomMemo(true), 1);

            //Assert
            mockMemoDataStore.Received().UpdateMemo(Arg.Is<CNMemo>(x => x.TaskMemos.Count > 0));
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
            mockMemoDataStore.DidNotReceiveWithAnyArgs().AddMemo(Arg.Any<CNMemo>());
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
            mockMemoDataStore.Received().DeleteMemo(Arg.Is<CNMemo>(x => x.MemoId == testMemo.MemoId));
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
            mockMemoDataStore.DidNotReceiveWithAnyArgs().DeleteMemo(Arg.Any<CNMemo>());
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
            mockMemoDataStore.DidNotReceiveWithAnyArgs().UpdateMemo(Arg.Any<CNMemo>());
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
            mockMemoDataStore.UpdateMemo(Arg.Any<CNMemo>());
            //Act
            await targetService.RemoveAMemoFromTask(assesMemo.MemoId, assesTask.TaskId);

            //Assert
            mockMemoDataStore.Received().UpdateMemo(Arg.Is<CNMemo>(x => x.TaskMemos.Count == 0));
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
            mockMemoDataStore.DidNotReceiveWithAnyArgs().UpdateMemo(Arg.Any<CNMemo>());
        }
        [Test]
        public async Task AddAMemoToGlobal_Success()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            var assesMemo = DomainTestHelper.GetARandomMemo(true);
            mockMemoDataStore.AddMemo(assesMemo).Returns(assesMemo);

            //Act
            await targetService.AddAMemoToGlobal(assesMemo);

            //Assert
            mockMemoDataStore.Received().AddMemo(Arg.Is<CNMemo>(x=>x.MemoId == assesMemo.MemoId));
        }
        [Test]
        public async Task AddAMemoToGlobal_Null_Fail()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            var assesMemo = DomainTestHelper.GetARandomMemo(true);
            //Act
            var result = await targetService.AddAMemoToGlobal(null);

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(result));
            mockMemoDataStore.DidNotReceiveWithAnyArgs().AddMemo(assesMemo);
        }

        [Test]
        public async Task UpdateAMemo_Success()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            var assesMemo = DomainTestHelper.GetARandomMemo(true);
            mockMemoDataStore.UpdateMemo(assesMemo);

            //Act
            await targetService.UpdateAMemo(assesMemo);

            //Assert
            mockMemoDataStore.Received().UpdateMemo(Arg.Is<CNMemo>(x => x.MemoId == assesMemo.MemoId));
        }
        [Test]
        public async Task UpdateAMemo_Null_Fail()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Assess
            var assesMemo = DomainTestHelper.GetARandomMemo(true);
            //Act
            var result = await targetService.UpdateAMemo(null);

            //Assert
            Assert.IsFalse(result);
            mockMemoDataStore.DidNotReceiveWithAnyArgs().UpdateMemo(assesMemo);
        }
        [Test]
        public async Task GetAllGlobalMemos_Traverse()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Act
            await targetService.GetAllGlobalMemos();

            //Assert
            await mockMemoDataStore.Received().GetAllGlobalMemos();
        }
        [Test]
        public async Task GetAllTaskMemos_Traverse()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Act
            await targetService.GetAllTaskMemos(1);

            //Assert
            await mockMemoDataStore.Received().GetAllTaskMemos(1);
        }
        [Test]
        public async Task GetMemoById_Traverse()
        {
            var mockMemoDataStore = _kernel.Get<IMemoDataStore>();
            var targetService = _kernel.Get<IMemoService>();

            //Act
            await targetService.GetMemoById("1");

            //Assert
            await mockMemoDataStore.Received().GetMemoById("1");
        }
    }
}