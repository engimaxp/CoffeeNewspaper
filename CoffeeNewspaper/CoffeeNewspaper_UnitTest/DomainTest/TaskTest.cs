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
            Assert.AreEqual(new CNTask()
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
                MemoId = 1,
                Content = "Start with writing tests!",
                Tag = ""
            }).AddOrUpdateMemo(new CNMemo()
            {
                MemoId = 1,
                Content = "Start with writing tests!",
                Tag = ""
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
                Tags = new List<string>() { "Work" },
            }.AddOrUpdateMemo(new CNMemo()
            {
                MemoId = 1,
                Content = "Start with writing tests!",
                Tag = ""
            }));
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
                MemoId = 1,
                Content = "Start with writing tests!",
                Tag = ""
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
                MemoId = 1,
                Content = "Start with writing tests!",
                Tag = ""
            }));
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
