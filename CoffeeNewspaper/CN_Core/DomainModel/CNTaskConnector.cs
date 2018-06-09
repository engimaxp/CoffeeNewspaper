using System;
using System.Collections.Generic;

namespace CN_Core
{
    /// <summary>
    ///     Relation between pretask and suftask is many-to-many
    /// </summary>
    public class CNTaskConnector
    {
        

        #region Constructor

        public CNTaskConnector()
        {
            
        }
        #endregion

        #region Public Properties

        /// <summary>
        ///     Id of this relation
        /// </summary>
        public string TaskConnectorId { get; set; }

        public int PreTaskId { get; set; }
        public virtual CNTask PreTask { get; set; }
        public int SufTaskId { get; set; }
        public virtual CNTask SufTask { get; set; }
        #endregion

        private sealed class PreTaskIdSufTaskIdEqualityComparer : IEqualityComparer<CNTaskConnector>
        {
            public bool Equals(CNTaskConnector x, CNTaskConnector y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.PreTaskId == y.PreTaskId && x.SufTaskId == y.SufTaskId;
            }

            public int GetHashCode(CNTaskConnector obj)
            {
                unchecked
                {
                    return (obj.PreTaskId * 397) ^ obj.SufTaskId;
                }
            }
        }

        public static IEqualityComparer<CNTaskConnector> PreTaskIdSufTaskIdComparer { get; } = new PreTaskIdSufTaskIdEqualityComparer();

        #region Formatting Implement

        public override string ToString()
        {
            return
                $"{nameof(TaskConnectorId)}: {TaskConnectorId}, {nameof(PreTaskId)}: {PreTaskId}, {nameof(SufTaskId)}: {SufTaskId}";
        }

        #endregion
    }
}