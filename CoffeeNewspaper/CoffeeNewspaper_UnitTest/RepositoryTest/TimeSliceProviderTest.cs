using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CN_Model;
using CN_Repository;
using Newtonsoft.Json;
using NUnit.Framework;
using SimpleTxtDB;

namespace CoffeeNewspaper_UnitTest.RepositoryTest
{
    [TestFixture]
    public class TimeSliceProviderTest
    {
        [Test]
        public void TimeSliceProviderSingletonTest()
        {
            TimeSliceProvider tsProvider1 = null;
            TimeSliceProvider tsProvider2 = null;
            for (int i = 0; i < 1000; i++)
            {
                Parallel.Invoke(() =>
                {
                    tsProvider1 = TimeSliceProvider.GetProvider();
                }, () =>
                {
                    tsProvider2 = TimeSliceProvider.GetProvider();
                });
                Assert.AreEqual(tsProvider1, tsProvider2);
            }
        }
        [Test]
        public void TimeSliceProviderCreate()
        {
            var tsProvider = TimeSliceProvider.GetProvider();
            Assert.IsNotNull(tsProvider);
            Assert.AreEqual(tsProvider.Today,DateTime.Now.ToString(CNConstants.DIRECTORY_DATEFORMAT));
        }
        
        [Test]
        public void OverWriteToDataSourceByDate()
        {
            var tsProvider = TimeSliceProvider.GetProvider();
            var currentTime = DateTime.Now;
            var testslice = new CNTimeSlice(currentTime.AddMinutes(-3), currentTime);
            TxtDB fileDb = new TxtDB(TimeSliceProvider.parentDirectoryName + "\\" + testslice.StartDate);
            fileDb.DumpFile();

            tsProvider.OverWriteToDataSourceByDate(testslice.StartDate, new Dictionary<int, List<CNTimeSlice>>{{ 1, new List<CNTimeSlice>(){ testslice } }});

            var result = fileDb.ReadAll().Trim();//Write Operation Add a breakline
            Assert.IsFalse(string.IsNullOrEmpty(result));

            Assert.AreEqual(result,JsonConvert.SerializeObject(new Dictionary<int, List<CNTimeSlice>> { { 1, new List<CNTimeSlice>() { testslice } } },Formatting.Indented));
            fileDb.DumpFile();
        }

        [Test]
        public void GetOriginalDataByDate()
        {
            var tsProvider = TimeSliceProvider.GetProvider();
            var currentTime = DateTime.Now;
            var testslice = new CNTimeSlice(currentTime.AddMinutes(-3), currentTime);
            TxtDB fileDb = new TxtDB(TimeSliceProvider.parentDirectoryName + "\\" + testslice.StartDate);
            fileDb.DumpFile();

            tsProvider.OverWriteToDataSourceByDate(testslice.StartDate, new Dictionary<int, List<CNTimeSlice>> { { 1, new List<CNTimeSlice>() { testslice } } });

            var result = tsProvider.GetOriginalDataByDate(testslice.StartDate);

            Assert.AreEqual(result, new Dictionary<int, List<CNTimeSlice>> { { 1, new List<CNTimeSlice>() { testslice } } });
            fileDb.DumpFile();
        }


    }
}
