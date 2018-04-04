using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CN_Model;

namespace CoffeeNewspaper_UnitTest.DomainTest
{
    [TestFixture]
    public class TaskTest
    {
        [Test]
        public void TaskAreEqual()
        {
            var time = DateTime.Now;
            var task1 = new CNTask()
            {
                TaskId = 1,
                Content = "Write A Program",
                CreateTime = time,
                StartTime = null,
                EndTime = null,
                Priority = CNPriority.High,
                Urgency = CNUrgency.High,
                EstimatedDuration = 3600,
                Tags = new List<string>() {"Work"},
            }.AddOrUpdateMemo(new CNMemo()
            {
                MemoId = "1",
                Content = "Start with writing tests!",
                Tags = new List<string>() {""}
            }).AddOrUpdateMemo(new CNMemo()
            {
                MemoId = "1",
                Content = "Start with writing tests!",
                Tags = new List<string>() {""}
            });
            var task2 = new CNTask()
            {
                TaskId = 1,
                Content = "Write A Program",
                CreateTime = time,
                StartTime = null,
                EndTime = null,
                Priority = CNPriority.High,
                Urgency = CNUrgency.High,
                EstimatedDuration = 3600,
                Tags = new List<string>() {"Work"},
            }.AddOrUpdateMemo(new CNMemo()
            {
                MemoId = "1",
                Content = "Start with writing tests!",
                Tags = new List<string>() {""}
            });
            Assert.AreEqual(1, new List<CNTask>() { task1 }.Intersect(new List<CNTask>() { task2 }, CNTask.CnTaskComparer).Count());
            Assert.AreEqual(task1,task2);
        }
        [Test]
        public void TaskAreNotEqual()
        {
            Assert.AreNotEqual(new CNTask()
            {
                TaskId = 1,
                Content = "Write A Program",
                CreateTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                Priority = CNPriority.High,
                Urgency = CNUrgency.High,
                EstimatedDuration = 3600,
                Tags = new List<string>() { "Work" },
            }.AddOrUpdateMemo(new CNMemo()
            {
                MemoId = "1",
                Content = "Start with writing tests!",
                Tags = new List<string>() { "" }
            }), new CNTask()
            {
                TaskId = 1,
                Content = "Write A Program",
                CreateTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                Priority = CNPriority.High,
                Urgency = CNUrgency.High,
                EstimatedDuration = 3600,
                Tags = new List<string>() { "Work2" },
            }.AddOrUpdateMemo(new CNMemo()
            {
                MemoId = "1",
                Content = "Start with writing tests!",
                Tags = new List<string>() { "" }
            }));
        }

        [Test]
        public void TaskCompare_SameUrgent_SameImportance_OrderByDeadLine()
        {
            var task1 = DomainTestHelper.GetARandomTask(1);
            var task2 = DomainTestHelper.GetARandomTask(2);
            var task3 = DomainTestHelper.GetARandomTask(3);
            task1.Urgency = CNUrgency.Normal;
            task2.Urgency = CNUrgency.Normal;
            task3.Urgency = CNUrgency.Normal;
            task1.Priority = CNPriority.Normal;
            task2.Priority = CNPriority.Normal;
            task3.Priority = CNPriority.Normal;
            task1.DeadLine = DateTime.Now.AddDays(3);
            task2.DeadLine = DateTime.Now.AddDays(1);
            task3.DeadLine = DateTime.Now.AddDays(2);

            var list = new List<CNTask>() { task1,task2,task3};
            list.Sort();
            list.Reverse();
            Assert.AreEqual(task2,list.First());
            Assert.AreEqual(task1, list.Last());
        }

        [Test]
        public void TaskCompare_SameUrgent_OrderByImportance()
        {
            var task1 = DomainTestHelper.GetARandomTask(1);
            var task2 = DomainTestHelper.GetARandomTask(2);
            var task3 = DomainTestHelper.GetARandomTask(3);
            task1.Urgency = CNUrgency.Normal;
            task2.Urgency = CNUrgency.Normal;
            task3.Urgency = CNUrgency.Normal;
            task1.Priority = CNPriority.High;
            task2.Priority = CNPriority.Low;
            task3.Priority = CNPriority.Normal;
            task1.DeadLine = DateTime.Now.AddDays(3);
            task2.DeadLine = DateTime.Now.AddDays(1);
            task3.DeadLine = DateTime.Now.AddDays(2);

            var list = new List<CNTask>() { task1, task2, task3 };
            list.Sort();
            list.Reverse();
            Assert.AreEqual(task1, list.First());
            Assert.AreEqual(task2, list.Last());
        }
        [Test]
        public void TaskCompare_OrderByUrgent()
        {
            var task1 = DomainTestHelper.GetARandomTask(1);
            var task2 = DomainTestHelper.GetARandomTask(2);
            var task3 = DomainTestHelper.GetARandomTask(3);
            task1.Urgency = CNUrgency.Normal;
            task2.Urgency = CNUrgency.High;
            task3.Urgency = CNUrgency.Low;
            task1.Priority = CNPriority.High;
            task2.Priority = CNPriority.Low;
            task3.Priority = CNPriority.Normal;
            task1.DeadLine = DateTime.Now.AddDays(3);
            task2.DeadLine = DateTime.Now.AddDays(1);
            task3.DeadLine = DateTime.Now.AddDays(2);

            var list = new List<CNTask>() { task1, task2, task3 };
            list.Sort();
            list.Reverse();
            Assert.AreEqual(task2, list.First());
            Assert.AreEqual(task3, list.Last());
        }

        [Test]
        public void TaskCompare_OrderByMixed()
        {
            var task1 = DomainTestHelper.GetARandomTask(1);
            var task2 = DomainTestHelper.GetARandomTask(2);
            var task3 = DomainTestHelper.GetARandomTask(3);
            task1.Urgency = CNUrgency.Normal;
            task2.Urgency = CNUrgency.Normal;
            task3.Urgency = CNUrgency.Low;
            task1.Priority = CNPriority.High;
            task2.Priority = CNPriority.Normal;
            task3.Priority = CNPriority.Normal;
            task1.DeadLine = DateTime.Now.AddDays(3);
            task2.DeadLine = DateTime.Now.AddDays(1);
            task3.DeadLine = DateTime.Now.AddDays(2);

            var list = new List<CNTask>() { task1, task2, task3 };
            list.Sort();
            list.Reverse();
            Assert.AreEqual(task1, list.First());
            Assert.AreEqual(task3, list.Last());
        }

    }
}
