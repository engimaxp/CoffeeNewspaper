using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CN_Repository;

namespace CoffeeNewspaper_UnitTest.DomainTest
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
            Assert.AreEqual(tsProvider.Today,DateTime.Now.ToString("yyyy-MM-dd"));
        }

        [Test]
        public void Time

    }
}
