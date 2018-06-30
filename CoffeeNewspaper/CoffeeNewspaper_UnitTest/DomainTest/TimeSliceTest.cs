using System;
using System.Collections.Generic;
using System.Linq;
using CN_Core;
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
            Assert.Throws<ArgumentException>(() => {
                CNTimeSlice a = new CNTimeSlice(DateTime.Now, DateTime.Now.AddMinutes(-1));});
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
        public void InterceptWithAnother_nullCondition_false()
        {
            CNTimeSlice a = new CNTimeSlice(DateTime.Now.AddMinutes(-3), DateTime.Now);
            Assert.IsFalse(a.InterceptWith(null));
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

        [Test]
        public void InterceptWithAnother_BothEndTime_TimeLineNotIntercept3()
        {
            var now = DateTime.Now;
            var a = new CNTimeSlice(now.AddHours(1), now.AddHours(2));
            var b = new CNTimeSlice(now.AddHours(3));
            var c = new CNTimeSlice(now.AddHours(2), now.AddHours(3));

            Assert.IsFalse(a.InterceptWith(b));
            Assert.IsFalse(b.InterceptWith(a));
            Assert.IsFalse(a.InterceptWith(c));
            Assert.IsFalse(c.InterceptWith(a));
            Assert.IsTrue(c.InterceptWith(b));
            Assert.IsTrue(b.InterceptWith(c));
        }

        [Test]
        public void GetDayDuration_SingleDay()
        {
            var currentTime = DateTime.Now;
            CNTimeSlice a = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            CNTimeSlice b = new CNTimeSlice(Convert.ToDateTime(currentTime.AddDays(-1).ToShortDateString()), Convert.ToDateTime(currentTime.ToShortDateString()).AddMilliseconds(-1));
            Assert.AreEqual(b,a.GetDayDuration());
        }
        [Test]
        public void GetDayDuration_MutipleDay()
        {
            var currentTime = DateTime.Now;
            CNTimeSlice a = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddMinutes(2));
            CNTimeSlice b = new CNTimeSlice(Convert.ToDateTime(currentTime.AddDays(-1).ToShortDateString()), Convert.ToDateTime(currentTime.AddDays(1).ToShortDateString()).AddMilliseconds(-1));
            Assert.AreEqual(b, a.GetDayDuration());
        }
        [Test]
        public void GetDayDuration_StartTimeBiggerThanDateTimeNow()
        {
            var currentTime = DateTime.Now;
            CNTimeSlice a = new CNTimeSlice(currentTime.AddDays(1).AddMinutes(-1), currentTime.AddDays(1).AddMinutes(4));
            Assert.IsNull(a.GetDayDuration());
        }

        [Test]
        public void IsContaining_NoBoundary()
        {
            var currentTime = DateTime.Now;
            CNTimeSlice a = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1));
            CNTimeSlice b = new CNTimeSlice(Convert.ToDateTime(currentTime.AddDays(-1).ToShortDateString()), Convert.ToDateTime(currentTime.ToShortDateString()));
            Assert.IsFalse(b.IsContaining(a));
        }
        [Test]
        public void IsContaining_BWrapA()
        {
            var currentTime = DateTime.Now;
            CNTimeSlice a = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            CNTimeSlice b = new CNTimeSlice(Convert.ToDateTime(currentTime.AddDays(-1).ToShortDateString()), Convert.ToDateTime(currentTime.ToShortDateString()));
            Assert.IsTrue(b.IsContaining(a));
        }
        [Test]
        public void IsContaining_CEqualD()
        {
            var currentTime = DateTime.Now;
            CNTimeSlice c = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            CNTimeSlice d = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            Assert.IsTrue(c.IsContaining(d));
            Assert.IsTrue(d.IsContaining(c));
        }
        [Test]
        public void IsContaining_EColideF()
        {
            var currentTime = DateTime.Now;
            CNTimeSlice e = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-2), currentTime.AddDays(-1).AddMinutes(1));
            CNTimeSlice f = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            Assert.IsFalse(e.IsContaining(f));
            Assert.IsFalse(f.IsContaining(e));
        }
        [Test]
        public void IsContaining_GWithinHWithBoundaries()
        {
            var currentTime = DateTime.Now;
            CNTimeSlice g = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-2), currentTime.AddDays(-1).AddMinutes(1));
            CNTimeSlice h = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-2), currentTime.AddDays(-1).AddMinutes(2));
            Assert.IsFalse(g.IsContaining(h));
            Assert.IsTrue(h.IsContaining(g));
            g = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            h = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-2), currentTime.AddDays(-1).AddMinutes(2));
            Assert.IsFalse(g.IsContaining(h));
            Assert.IsTrue(h.IsContaining(g));
        }

        [Test]
        public void GetWorkDays()
        {
            var currentTime = DateTime.Now;
            CNTimeSlice a = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            CNTimeSlice b = a.GetDayDuration();
            Assert.AreEqual(1,b.GetWorkDays());
            a = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(1).AddMinutes(2));
            b = a.GetDayDuration();
            Assert.AreEqual(3, b.GetWorkDays());
            a = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), DateTime.Now.Date);
            b = a.GetDayDuration();
            Assert.AreEqual(2, b.GetWorkDays());
        }
        [Test]
        public void GetWorkDays_EndTimeZero()
        {
            var currentTime = DateTime.Now;
            CNTimeSlice a = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1));
            ;
            Assert.AreEqual(0, a.GetWorkDays());
        }

        [Test]
        public void SortTest()
        {
            var currentTime = DateTime.Now;
            CNTimeSlice a = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-1), currentTime.AddDays(-1).AddMinutes(2));
            CNTimeSlice b = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-2), currentTime.AddDays(-1).AddMinutes(2)); 
            CNTimeSlice c = new CNTimeSlice(currentTime.AddDays(-1).AddMinutes(-4), currentTime.AddDays(-1).AddMinutes(2));

            var result = new List<CNTimeSlice>() {a, b, c};
            result.Sort();
            Assert.AreEqual(c, result.First());
            Assert.AreEqual(a, result.Last());
        }
    }
}
