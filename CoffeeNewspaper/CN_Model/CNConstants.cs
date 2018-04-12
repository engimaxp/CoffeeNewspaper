using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CN_Model
{
    public class CNConstants
    {
        public const string DIRECTORY_DATEFORMAT = "yyyy-MM-dd";

        //预计耗时 -1代表不知道
        public const int ESTIMATED_DURATION_NOTKNOWN = -1;
        /// <summary>
        /// 预计耗时 -2代表永远不会完成
        /// </summary>
        public const int ESTIMATED_DURATION_NEVERDONE = -2;
        /// <summary>
        /// 预计耗时 0代表没有填写预估值
        /// </summary>
        public const int ESTIMATED_DURATION_NOTWRITE = 0;

    }
}
