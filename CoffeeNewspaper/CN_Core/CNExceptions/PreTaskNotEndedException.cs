using System;
using System.Collections.Generic;
using System.Linq;

namespace CN_Core
{
    /// <summary>
    ///     The pretask of this <see cref="CNTask" /> is not yet finished
    ///     throw this exception while try to finish a task
    /// </summary>
    public class PreTaskNotEndedException : Exception
    {
        #region Construction

        public PreTaskNotEndedException(List<CNTask> preTaskses, CNTask curTask)
        {
            _preTasks = preTaskses ?? new List<CNTask>();
            _curTask = curTask;
        }

        #endregion

        #region Formatting implement

        public override string ToString()
        {
            return
                $"targetTask:{_curTask?.TaskId}:{_curTask?.Content} preTask not ended:{string.Join(";\r\n", _preTasks.Select(x => x.ToString()))}";
        }

        #endregion

        #region Private Properties

        /// <summary>
        ///     The <see cref="_preTasks" /> of the exception <see cref="CNTask" />
        /// </summary>
        private readonly List<CNTask> _preTasks;

        /// <summary>
        ///     The exception triggered <see cref="CNTask" />
        /// </summary>
        private readonly CNTask _curTask;

        #endregion
    }
}