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
                ID = 1,
                Content = "Write A Program",
                CreateTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                Priority = CNPriority.High,
                Urgency = CNUrgency.High,
                EstimatedDuration = 3600,
                Memos = new List<CNMemo>() {new CNMemo()
                {
                    ID = 1,
                    Content = "Start with writing tests!",
                    Tag = ""
                },
                new CNMemo()
                {
                    ID = 1,
                    Content = "Start with writing tests!",
                    Tag = ""
                }},
                Tags = new List<string>() { "Work" },
                Parent = null,
                PreTask = null,
            }, new CNTask()
            {
                ID = 1,
                Content = "Write A Program",
                CreateTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                Priority = CNPriority.High,
                Urgency = CNUrgency.High,
                EstimatedDuration = 3600,
                Memos = new List<CNMemo>() {new CNMemo()
                {
                    ID = 1,
                    Content = "Start with writing tests!",
                    Tag = ""
                },
                new CNMemo()
                {
                    ID = 1,
                    Content = "Start with writing tests!",
                    Tag = ""
                }},
                Tags = new List<string>() { "Work" },
                Parent = null,
                PreTask = null,
            });
        }
        [Test]
        public void TaskAreNotEqual()
        {
            Assert.AreNotEqual(new CNTask()
            {
                ID = 1,
                Content = "Write A Program",
                CreateTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                Priority = CNPriority.High,
                Urgency = CNUrgency.High,
                EstimatedDuration = 3600,
                Memos = new List<CNMemo>() {new CNMemo()
                {
                    ID = 1,
                    Content = "Start with writing tests!",
                    Tag = ""
                },
                new CNMemo()
                {
                    ID = 1,
                    Content = "Start with writing tests!",
                    Tag = ""
                }},
                Tags = new List<string>() { "Work" },
                Parent = null,
                PreTask = null,
            }, new CNTask()
            {
                ID = 1,
                Content = "Write A Program",
                CreateTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                Priority = CNPriority.High,
                Urgency = CNUrgency.High,
                EstimatedDuration = 3600,
                Memos = new List<CNMemo>() {new CNMemo()
                {
                    ID = 1,
                    Content = "Start with writing tests!",
                    Tag = ""
                },
                new CNMemo()
                {
                    ID = 1,
                    Content = "Start with writing tests!",
                    Tag = ""
                }},
                Tags = new List<string>() { "Work" },
                Parent = null,
                PreTask = new List<CNTask>(),
            });
        }

        [Test]
        public void CreateTask()
        {
            var task = new CNTask()
            {
                ID = 1,
                Content = "Write A Program",
                CreateTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                Priority = CNPriority.High,
                Urgency = CNUrgency.High,
                EstimatedDuration = 3600,
                Memos = new List<CNMemo>() {new CNMemo()
                {
                    ID = 1,
                    Content = "Start with writing tests!",
                    Tag = ""
                },
                new CNMemo()
                {
                    ID = 1,
                    Content = "Start with writing tests!",
                    Tag = ""
                }},
                Tags = new List<string>() { "Work" },
                Parent = null,
                PreTask = null,
            };
            Assert.IsNotNull(task);
        }
    }
}
