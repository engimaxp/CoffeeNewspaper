using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using CN_Core;

namespace CoffeeNewspaper_UnitTest.DomainTest
{
    [TestFixture]
    public class TaskTest
    {
        [Test]
        public void TaskCompare_SameUrgent_SameImportance_OrderByDeadLine()
        {
            var task1 = DomainTestHelper.GetARandomTask();
            var task2 = DomainTestHelper.GetARandomTask();
            var task3 = DomainTestHelper.GetARandomTask();
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
            var task1 = DomainTestHelper.GetARandomTask();
            var task2 = DomainTestHelper.GetARandomTask();
            var task3 = DomainTestHelper.GetARandomTask();
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
            var task1 = DomainTestHelper.GetARandomTask();
            var task2 = DomainTestHelper.GetARandomTask();
            var task3 = DomainTestHelper.GetARandomTask();
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
            var task1 = DomainTestHelper.GetARandomTask();
            var task2 = DomainTestHelper.GetARandomTask();
            var task3 = DomainTestHelper.GetARandomTask();
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
