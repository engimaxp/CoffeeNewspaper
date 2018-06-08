using System;
using System.Collections.Generic;

namespace CN_Core
{
    /// <summary>
    ///     Relation between pretask and suftask is many-to-many
    /// </summary>
    public class CNTaskConnector
    {
        #region LazyLoading

        private CNTask _preTask;

        private CNTask _sufTask;

        private CNTaskConnector(Action<object, string> lazyLoader)
        {
            LazyLoader = lazyLoader;
        }

        private Action<object, string> LazyLoader { get; set; }

        #endregion

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
        public CNTask PreTask
        {
            get => LazyLoader == null ? _preTask : LazyLoader?.Load(this, ref _preTask);
            set => _preTask = value;
        }

        public int SufTaskId { get; set; }
        public CNTask SufTask
        {
            get => LazyLoader == null ? _sufTask : LazyLoader?.Load(this, ref _sufTask);
            set => _sufTask = value;
        }

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