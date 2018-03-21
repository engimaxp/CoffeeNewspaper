using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CN_Model;

namespace CN_Repository
{
    public interface IRootDataProvider
    {
        /// <summary>
        /// Write To Persistence Layer
        /// </summary>
        /// <param name="rootInfo"></param>
        void Persistence(CNRoot rootInfo);
        /// <summary>
        /// Get from Persistence Layer
        /// </summary>
        /// <returns></returns>
        CNRoot GetRootData();
    }
}
