using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CN_Repository
{
    public class TimeSliceProvider
    {
        private static TimeSliceProvider _timeSliceProvider = null;

        private static readonly object lockobject = new object();
        private TimeSliceProvider()
        {
            Today = DateTime.Now.ToString("yyyy-MM-dd");
        }

        public string Today { get; set; }

        public static TimeSliceProvider GetProvider()
        {
            if (_timeSliceProvider == null)
            {
                lock (lockobject)
                {
                    if (_timeSliceProvider == null)
                    {

                        lock (lockobject)
                        {
                            _timeSliceProvider = new TimeSliceProvider();
                        }
                    }
                }
            }
            return _timeSliceProvider;
        }
    }
}
