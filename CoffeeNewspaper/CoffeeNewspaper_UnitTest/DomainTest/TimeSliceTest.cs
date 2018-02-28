using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeNewspaper_UnitTest.DomainTest
{
    [TestFixture]
    public class TimeSliceTest
    {
        [Test]
        public void StartATimeSlice()
        {
//            TimeSliceProvider tsProvider = new TimeSliceProvider();
//            tsProvider.StartATimeSliceNow(1);
//            tsProvider.StartATimeSliceFrom(1, DateTime.Now.AddHours(-1));
        }

        [Test]
        public void EndATimeSlice()
        {
            // TODO: Add your test code here
            Assert.Pass("Your first passing test");
        }

        [Test]
        public void StartATimeSliceEndInNextDay()
        {
            // TODO: Add your test code here
            Assert.Pass("Your first passing test");
        }

        [Test]
        public void GetTimeSlicesWithTaskID()
        {
            // TODO: Add your test code here
            Assert.Pass("Your first passing test");
        }

        [Test]
        public void GetTotalTimeInfoWithTaskID()
        {
            // TODO: Add your test code here
            Assert.Pass("Your first passing test");
        }
    }
}
