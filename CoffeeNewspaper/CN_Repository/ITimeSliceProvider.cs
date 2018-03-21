using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CN_Model;

namespace CN_Repository
{
    public interface ITimeSliceProvider
    {
        /// <summary>
        /// write
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="serializeObject"></param>
        void OverWriteToDataSourceByDate(string fileName, Dictionary<int, List<CNTimeSlice>> serializeObject);
        /// <summary>
        /// read
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Dictionary<int, List<CNTimeSlice>> GetOriginalDataByDate(string fileName);
    }
}
