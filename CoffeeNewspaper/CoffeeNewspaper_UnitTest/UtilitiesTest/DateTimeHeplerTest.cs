using System;
using CN_Core.Utilities;
using NUnit.Framework;

namespace CoffeeNewspaper_UnitTest.UtilitiesTest
{
    [TestFixture]
    public class DateTimeHeplerTest
    {
        [Test]
        public void SecondsAgoTest()
        {
            DateTime target = DateTime.Now.AddSeconds(-8);
            Assert.AreEqual("Just Now",target.EarlyTimeFormat());

            target = DateTime.Now.AddSeconds(-18);
            Assert.AreEqual("Just Now", target.EarlyTimeFormat());

            target = DateTime.Now.AddSeconds(-59);
            Assert.AreEqual("Just Now", target.EarlyTimeFormat());
        }

        [Test]
        public void MiniutesAgoTest()
        {
            DateTime target = DateTime.Now.AddMinutes(-1);
            Assert.AreEqual("1 Minute Ago", target.EarlyTimeFormat());

            target = DateTime.Now.AddMinutes(-9);
            Assert.AreEqual("9 Minutes Ago", target.EarlyTimeFormat());

            target = DateTime.Now.AddMinutes(-18);
            Assert.AreEqual("18 Minutes Ago", target.EarlyTimeFormat());

            target = DateTime.Now.AddMinutes(-59).AddSeconds(-59);
            Assert.AreEqual("59 Minutes Ago", target.EarlyTimeFormat());
        }

        [Test]
        public void HoursAgoTest()
        {
            DateTime target = DateTime.Now.AddHours(-1);
            Assert.AreEqual("1 Hour Ago", target.EarlyTimeFormat());

            target = DateTime.Now.AddHours(-6).AddMinutes(-59);
            Assert.AreEqual("6 Hours Ago", target.EarlyTimeFormat());
        }


        [Test]
        public void EarlyThisDayTest()
        {
            DateTime now = new DateTime(2018,8,3,12,0,0);
            DateTime target = now.AddHours(-11).AddMinutes(-59).AddSeconds(-59);
            Assert.AreEqual("Early Today", target.EarlyTimeFormat(now));

            target = now.AddHours(-7);
            Assert.AreEqual("Early Today", target.EarlyTimeFormat(now));
        }

        [Test]
        public void YesterDayTest()
        {
            DateTime now = new DateTime(2018, 8, 3, 12, 0, 0);
            DateTime target = now.AddHours(-12).AddSeconds(-1);
            Assert.AreEqual("Yesterday", target.EarlyTimeFormat(now));

            target = now.AddHours(-35).AddMinutes(-59).AddSeconds(-59);
            Assert.AreEqual("Yesterday", target.EarlyTimeFormat(now));
        }

        [Test]
        public void DayAgoTest()
        {
            DateTime now = new DateTime(2018, 8, 3, 12, 0, 0);
            DateTime target = now.AddHours(-36).AddSeconds(-1);
            Assert.AreEqual("2 Days Ago", target.EarlyTimeFormat(now));

            target = now.AddDays(-6).AddSeconds(1);
            Assert.AreEqual("6 Days Ago", target.EarlyTimeFormat(now));
        }


        [Test]
        public void WeekAgoTest()
        {
            DateTime now = new DateTime(2018, 8, 3, 12, 0, 0);
            DateTime target = now.AddDays(-7);
            Assert.AreEqual("1 Week Ago", target.EarlyTimeFormat(now));
            
            target = now.AddMonths(-1).AddSeconds(1);
            Assert.AreEqual("4 Weeks Ago", target.EarlyTimeFormat(now));
        }
        
        [Test]
        public void MonthAgoTest()
        {
            DateTime now = new DateTime(2018, 8, 3, 12, 0, 0);
            DateTime target = now.AddMonths(-1).AddSeconds(-1);
            Assert.AreEqual("1 Month Ago", target.EarlyTimeFormat(now));

            target = now.AddMonths(-12).AddSeconds(1);
            Assert.AreEqual("12 Months Ago", target.EarlyTimeFormat(now));
        }


        [Test]
        public void YearAgoTest()
        {
            DateTime now = new DateTime(2018, 8, 3, 12, 0, 0);
            DateTime target = now.AddYears(-1).AddSeconds(-1);
            Assert.AreEqual("1 Year Ago", target.EarlyTimeFormat(now));

            target = now.AddYears(-12);
            Assert.AreEqual("12 Years Ago", target.EarlyTimeFormat(now));
        }
    }
}