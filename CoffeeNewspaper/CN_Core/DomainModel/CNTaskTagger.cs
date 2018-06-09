using System;
using System.Collections.Generic;

namespace CN_Core
{
    /// <summary>
    ///     Relation between tag and task is many-to-one
    /// </summary>
    public class CNTaskTagger
    {
        
        #region Constructor

        public CNTaskTagger()
        {
            
        }
        #endregion

        #region Formatting Implement

        public override string ToString()
        {
            return $"{nameof(TagId)}: {TagId}, {nameof(TaskId)}: {TaskId}";
        }

        #endregion
        

        #region Public Properties

        /// <summary>
        ///     Id of this relation
        /// </summary>
        public string TaskTaggerId { get; set; }

        public string TagId { get; set; }
        public virtual CNTag Tag { get; set; }
        public int TaskId { get; set; }
        public virtual CNTask Task { get; set; }

        #endregion
        
    }
}