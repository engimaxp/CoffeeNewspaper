using System;
using CN_Model;
using NUnit.Framework;

namespace CoffeeNewspaper_UnitTest.DomainTest
{
    [TestFixture]
    public class TimeSliceTest
    {
        [Test]
        public void CreateATimeSlice()
        {
            CNTimeSlice a = new CNTimeSlice(DateTime.Now, DateTime.Now.AddMinutes(1));
            CNTimeSlice b = new CNTimeSlice(DateTime.Now);
            Assert.IsNotNull(a);
            Assert.IsNotNull(b);
        }
        [Test]
        public void CreateATimeSlice_EndTimeGreaterThanStartTime()
        {
            Assert.Catch<ArgumentException>(() => {
                CNTimeSlice a = new CNTimeSlice(DateTime.Now, DateTime.Now.AddMinutes(-1));
            });
        }
        [Test]
        public void CloneATimeSlice()
        {
            CNTimeSlice a = new CNTimeSlice(DateTime.Now,DateTime.Now.AddMinutes(1));
            CNTimeSlice b = a.Clone() as CNTimeSlice;

            Assert.IsFalse(ReferenceEquals(a, b));

            Assert.AreEqual(a,b);
        }

        [Test]
        public void InterceptWithAnother_BothNoEndTime()
        {
            CNTimeSlice a = new CNTimeSlice(DateTime.Now);
            CNTimeSlice b = new CNTimeSlice(DateTime.Now.AddMinutes(-1));
            Assert.IsTrue(a.InterceptWith(b));
            Assert.IsTrue(b.InterceptWith(a));
        }

        [Test]
        public void InterceptWithAnother_OneNoEndTime_TimeLineIntercept1()
        {
            CNTimeSlice a = new CNTimeSlice(DateTime.Now);
            CNTimeSlice b = new CNTimeSlice(DateTime.Now.AddMinutes(-1),DateTime.Now.AddMinutes(1));

            Assert.IsTrue(a.InterceptWith(b));
            Assert.IsTrue(b.InterceptWith(a));
        }

        [Test]
        public void InterceptWithAnother_OneNoEndTime_TimeLineIntercept2()
        {
            CNTimeSlice a = new CNTimeSlice(DateTime.Now);
            CNTimeSlice b = new CNTimeSlice(DateTime.Now.AddMinutes(1), DateTime.Now.AddMinutes(2));

            Assert.IsTrue(a.InterceptWith(b));
            Assert.IsTrue(b.InterceptWith(a));
        }
        [Test]
        public void InterceptWithAnother_OneNoEndTime_TimeLineNotIntercept()
        {
            CNTimeSlice a = new CNTimeSlice(DateTime.Now);
            CNTimeSlice b = new CNTimeSlice(DateTime.Now.AddMinutes(-2), DateTime.Now.AddMinutes(-1));

            Assert.IsFalse(a.InterceptWith(b));
            Assert.IsFalse(b.InterceptWith(a));
        }


        [Test]
        public void InterceptWithAnother_BothEndTime_TimeLineIntercept1()
        {
            CNTimeSlice a = new CNTimeSlice(DateTime.Now.AddMinutes(-3),DateTime.Now);
            CNTimeSlice b = new CNTimeSlice(DateTime.Now.AddMinutes(-2), DateTime.Now.AddMinutes(-1));

            Assert.IsTrue(a.InterceptWith(b));
            Assert.IsTrue(b.InterceptWith(a));
        }
        [Test]
        public void InterceptWithAnother_BothEndTime_TimeLineIntercept2()
        {
            CNTimeSlice a = new CNTimeSlice(DateTime.Now.AddMinutes(-3), DateTime.Now);
            CNTimeSlice b = new CNTimeSlice(DateTime.Now.AddMinutes(-4), DateTime.Now.AddMinutes(-2));

            Assert.IsTrue(a.InterceptWith(b));
            Assert.IsTrue(b.InterceptWith(a));
        }
        [Test]
        public void InterceptWithAnother_BothEndTime_TimeLineIntercept3()
        {
            CNTimeSlice a = new CNTimeSlice(DateTime.Now.AddMinutes(-3), DateTime.Now);
            CNTimeSlice b = new CNTimeSlice(DateTime.Now.AddMinutes(-2), DateTime.Now.AddMinutes(1));

            Assert.IsTrue(a.InterceptWith(b));
            Assert.IsTrue(b.InterceptWith(a));
        }

        [Test]
        public void InterceptWithAnother_BothEndTime_TimeLineIntercept5()
        {
            var currentTime = DateTime.Now;
            CNTimeSlice a = new CNTimeSlice(currentTime.AddDays(-2).AddMinutes(-3), currentTime.AddDays(-1).AddMinutes(-2));
            CNTimeSlice b = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-3), currentTime.AddDays(-1).AddMinutes(-1));

            Assert.IsTrue(a.InterceptWith(b));
            Assert.IsTrue(b.InterceptWith(a));
        }

        [Test]
        public void InterceptWithAnother_BothEndTime_TimeLineNotIntercept1()
        {
            CNTimeSlice a = new CNTimeSlice(DateTime.Now.AddMinutes(-3), DateTime.Now);
            CNTimeSlice b = new CNTimeSlice(DateTime.Now.AddMinutes(1), DateTime.Now.AddMinutes(3));

            Assert.IsFalse(a.InterceptWith(b));
            Assert.IsFalse(b.InterceptWith(a));
        }

        [Test]
        public void InterceptWithAnother_BothEndTime_TimeLineNotIntercept2()
        {
            var currentTime = DateTime.Now;
               CNTimeSlice a = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            CNTimeSlice b = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-2), currentTime.AddDays(-1).AddMinutes(-1));

            Assert.IsFalse(a.InterceptWith(b));
            Assert.IsFalse(b.InterceptWith(a));
        }
    }
}
