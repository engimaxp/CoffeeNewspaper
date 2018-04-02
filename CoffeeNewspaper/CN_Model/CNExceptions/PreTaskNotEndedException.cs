using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CN_Model
{
    public class PreTaskNotEndedException : Exception
    {
        private readonly List<CNTask> _preTasks;
        private readonly CNTask _curTask;

        public PreTaskNotEndedException(List<CNTask> preTaskses, CNTask curTask)
        {
            _preTasks = preTaskses??new List<CNTask>();
            _curTask = curTask;
        }

        public override string ToString()
        {
            return $"targetTask:{_curTask?.TaskId}:{_curTask?.Content} preTask not ended:{string.Join(";\r\n",_preTasks.Select(x=>x.ToString()))}";
        }
    }
}
