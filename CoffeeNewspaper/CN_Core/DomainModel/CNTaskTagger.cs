using System;
using System.Collections.Generic;

namespace CN_Core
{
    /// <summary>
    ///     Relation between tag and task is many-to-one
    /// </summary>
    public class CNTaskTagger
    {
        #region LazyLoading

        private CNTag _tag;

        private CNTask _task;

        private CNTaskTagger(Action<object, string> lazyLoader)
        {
            LazyLoader = lazyLoader;
        }

        private Action<object, string> LazyLoader { get; set; }

        #endregion

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
        public CNTag Tag
        {
            get => LazyLoader == null ? _tag : LazyLoader?.Load(this, ref _tag);
            set => _tag = value;
        }
        public int TaskId { get; set; }
        public CNTask Task
        {
            get => LazyLoader == null ? _task : LazyLoader?.Load(this, ref _task);
            set => _task = value;
        }

        #endregion
        
    }
}