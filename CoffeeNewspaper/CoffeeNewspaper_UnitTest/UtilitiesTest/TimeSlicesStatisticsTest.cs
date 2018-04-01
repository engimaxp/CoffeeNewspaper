using System;
using System.Collections.Generic;
using CN_Model;
using NUnit.Framework;

namespace CoffeeNewspaper_UnitTest.UtilitiesTest
{
    [TestFixture]
    public class TimeSlicesStatisticsTest
    {
        [Test]
        public void GetTimeSliceDuration()
        {
            //arrange
            var time = Convert.ToDateTime("2018-4-1 12:00:00");
            var timeslice1 = new CNTimeSlice(time.AddHours(-1), time.AddHours(1));
            //act
            var result = timeslice1.GetTimeSliceDuration(time);
            //arrange
            Assert.AreEqual(time.AddHours(1) - time.AddHours(-1), result);
        }

        [Test]
        public void GetTimeSliceDuration_EndTimeIsNull()
        {
            //arrange
            var time = Convert.ToDateTime("2018-4-1 12:00:00");
            var timeslice1 = new CNTimeSlice(time.AddHours(-1));
            //act
            var result = timeslice1.GetTimeSliceDuration(time);
            //arrange
            Assert.AreEqual(time - time.AddHours(-1), result);
        }

        [Test]
        public void GetTimeSliceDuration_StartTimeOverNow()
        {
            //arrange
            var time = Convert.ToDateTime("2018-4-1 12:00:00");
            var timeslice1 = new CNTimeSlice(time.AddHours(1));
            //act
            var result = timeslice1.GetTimeSliceDuration(time);
            //arrange
            Assert.AreEqual(TimeSpan.Zero, result);
        }

        [Test]
        public void GetTotalHours()
        {
            //arrange
            var time = Convert.ToDateTime("2018-4-1 12:00:00");
            var timeslice1 = new CNTimeSlice(time.AddHours(-1), time.AddHours(1));
            var timeslice2 = new CNTimeSlice(time.AddDays(-1).AddHours(-1), time.AddDays(-1).AddHours(1));
            var timeslice3 = new CNTimeSlice(time.AddDays(-1).AddHours(-3), time.AddDays(-1).AddHours(-2));
            var testList = new List<CNTimeSlice>() {timeslice1, timeslice2, timeslice3 };
            //act
            double result = testList.GetTotalHours();
            //assert
            Assert.AreEqual(5, result);
        }
        [Test]
        public void GetTotalDays()
        {
            //arrange
            var time = Convert.ToDateTime("2018-4-1 12:00:00");
            var timeslice1 = new CNTimeSlice(time.AddHours(-1), time.AddHours(1));
            var timeslice2 = new CNTimeSlice(time.AddDays(-1).AddHours(-1), time.AddDays(-1).AddHours(1));
            var timeslice3 = new CNTimeSlice(time.AddDays(-1).AddHours(-3), time.AddDays(-1).AddHours(-2));
            var testList = new List<CNTimeSlice>() { timeslice1, timeslice2, timeslice3 };
            //act
            double result = testList.GetTotalDays();
            //assert
            Assert.GreaterOrEqual(1e-5,Math.Abs(5.0/24.0-result));
        }
        [Test]
        public void GetWorkDays_SimpleTwoDays()
        {
            //arrange
            var time = Convert.ToDateTime("2018-4-1 12:00:00");
            var timeslice1 = new CNTimeSlice(time.AddHours(-1), time.AddHours(1));
            var timeslice2 = new CNTimeSlice(time.AddDays(-1).AddHours(-1), time.AddDays(-1).AddHours(1));
            var timeslice3 = new CNTimeSlice(time.AddDays(-1).AddHours(-3), time.AddDays(-1).AddHours(-2));
            var testList = new List<CNTimeSlice>() { timeslice1, timeslice2, timeslice3 };
            //act
            double result = testList.GetWorkDays();
            //assert
            Assert.AreEqual(2, result);
        }
        [Test]
        public void GetWorkDays_MutipleDaysWithColide()
        {
            //arrange
            var time = Convert.ToDateTime("2018-4-1 12:00:00");
            var timeslice1 = new CNTimeSlice(time.AddHours(-1), time.AddHours(1));
            //one day
            Assert.AreEqual(1,new List<CNTimeSlice>(){timeslice1}.GetWorkDays());

            var timeslice2 = new CNTimeSlice(time.AddDays(-1).AddHours(-1), time.AddDays(-1).AddHours(1));
            var timeslice3 = new CNTimeSlice(time.AddDays(-1).AddHours(1), time.AddDays(-1).AddHours(2));
            //two same day
            Assert.AreEqual(1, new List<CNTimeSlice>() { timeslice2, timeslice3 }.GetWorkDays());

            //one contain two other day
            var timeslice4 = new CNTimeSlice(time.AddDays(-3).AddHours(1), time.AddDays(-1).AddHours(-2));
            Assert.AreEqual(3, new List<CNTimeSlice>() { timeslice2, timeslice3, timeslice4 }.GetWorkDays());

            //discrete distribution
            var timeslice5 = new CNTimeSlice(time.AddDays(-6).AddHours(1), time.AddDays(-5).AddHours(-2));
            Assert.AreEqual(5, new List<CNTimeSlice>() { timeslice2, timeslice3, timeslice4 , timeslice5 }.GetWorkDays());
        }
    }
}