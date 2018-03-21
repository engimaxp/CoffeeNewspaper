using System;
using System.Collections.Generic;
using System.Linq;
using CN_Model;
using Newtonsoft.Json;
using SimpleTxtDB;

namespace CN_Repository
{
    public class TimeSliceProvider: ITimeSliceProvider
    {
        private static TimeSliceProvider _timeSliceProvider;

        private static readonly object lockobject = new object();
        private TimeSliceProvider()
        {
            Today = DateTime.Now.ToString(CNConstants.DIRECTORY_DATEFORMAT);
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

        public const string parentDirectoryName = "TimeSlices";


        public void OverWriteToDataSourceByDate(string fileName, Dictionary<int, List<CNTimeSlice>> serializeObject)
        {
            TxtDB fileDb = new TxtDB(parentDirectoryName + "\\" + fileName);
            fileDb.OverWrite(JsonConvert.SerializeObject(serializeObject, Formatting.Indented));
        }

        public Dictionary<int, List<CNTimeSlice>> GetOriginalDataByDate(string fileName)
        {
            TxtDB fileDb = new TxtDB(parentDirectoryName + "\\" + fileName);
            Dictionary<int, List<CNTimeSlice>> processData = null;
            try
            {
                string datasource = fileDb.ReadAll();
                processData = string.IsNullOrEmpty(datasource)
                    ? new Dictionary<int, List<CNTimeSlice>>()
                    : JsonConvert.DeserializeObject<Dictionary<int, List<CNTimeSlice>>>(datasource);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return processData;
        }

        
        
    }
}
