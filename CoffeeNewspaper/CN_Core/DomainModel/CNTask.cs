using System;
using System.Collections.Generic;
using System.Linq;

namespace CN_Core
{
    public class CNTask : IComparable<CNTask>
    {
        

        #region Constructor

        public CNTask()
        {
            
        }
        #endregion

        #region Public Properties

        /// <summary>
        ///     The id of this task
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        ///     The detail of this task most likely plain text
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     CreatedTime of this task
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     StartTime of this task
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        ///     The importance of this task
        /// </summary>
        public CNPriority Priority { get; set; }

        /// <summary>
        ///     How urgent is this task
        /// </summary>
        public CNUrgency Urgency { get; set; }

        /// <summary>
        ///     Estimated duration of this task
        ///     count by minute
        ///     -1 means unkonwn
        ///     -2 means never will end
        ///     0 means user doest fill this property
        /// </summary>
        public int EstimatedDuration { get; set; }

        /// <summary>
        ///     The EndTime of this task
        ///     EndTime must greater than StartTime
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        ///     Time before which this task must be finished
        /// </summary>
        public DateTime? DeadLine { get; set; }

        /// <summary>
        ///     the status of this task
        /// </summary>
        public CNTaskStatus Status { get; set; } = CNTaskStatus.TODO;

        /// <summary>
        ///     Define this task is deleted or not
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        ///     Define this task is failed or not
        /// </summary>
        public bool IsFail { get; set; }

        /// <summary>
        ///     The fail reason of this task
        ///     if this task's <see cref="IsFail" /> is true ,this properties shall store the fail reason
        /// </summary>
        public string FailReason { get; set; }

        /// <summary>
        ///     the tasks memos relation entites
        /// </summary>
        public virtual ICollection<CNTaskMemo> TaskMemos { get; set; } = new HashSet<CNTaskMemo>();

        /// <summary>
        ///     Tags of this task
        /// </summary>
        public virtual ICollection<CNTaskTagger> TaskTaggers { get; set; } = new HashSet<CNTaskTagger>();

        /// <summary>
        ///     ParentTaskId
        /// </summary>
        public int? ParentTaskID { get; set; }

        /// <summary>
        ///     ParentTask of this task
        ///     one task only have one parent ,so its a tree structure
        /// </summary>
        public virtual CNTask ParentTask { get; set; } 

        /// <summary>
        ///     PreTasks of this task
        ///     one task may have many pretasks , only when these tasks is done when this task can start
        /// </summary>
        public virtual ICollection<CNTaskConnector> PreTaskConnectors { get; set; } = new HashSet<CNTaskConnector>();


        /// <summary>
        ///     SufTasks of this task
        ///     one task may have many Suftasks , only this task is done when these tasks can start
        /// </summary>
        public virtual ICollection<CNTaskConnector> SufTaskConnectors { get; set; } = new HashSet<CNTaskConnector>();

        /// <summary>
        ///     ChildTasks of this task
        /// </summary>
        public virtual ICollection<CNTask> ChildTasks { get; set; } = new HashSet<CNTask>();

        /// <summary>
        ///     The timeslices this task has used
        /// </summary>
        public virtual ICollection<CNTimeSlice> UsedTimeSlices { get; set; } = new HashSet<CNTimeSlice>();

        #endregion

        #region Interface Implementation


        #region Formatting implement

        public override string ToString()
        {
            return $"{nameof(TaskId)}: {TaskId}, {nameof(Content)}: {Content}";
        }

        #endregion

        #region Comparable Implement

        /// <summary>
        ///     Compare two tasks,usable on default sorting
        ///     1. compare urgency
        ///     2. compare priority
        ///     3. compare deadline the earlier is greater
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(CNTask other)
        {
            if (Urgency > other.Urgency) return 1;
            if (Urgency < other.Urgency) return -1;
            if (Priority > other.Priority) return 1;
            if (Priority < other.Priority) return -1;
            if (DeadLine == null && other.DeadLine == null) return 0;
            if (DeadLine == null && other.DeadLine != null) return -1;
            if (DeadLine != null && other.DeadLine == null) return 1;
            if (DeadLine > other.DeadLine) return -1;
            if (DeadLine < other.DeadLine) return 1;
            return 0;
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        ///     Defines this task has or not memos
        /// </summary>
        /// <returns></returns>
        public bool HasMemo()
        {
            return TaskMemos.Any();
        }

        /// <summary>
        ///     Defines this task has specified id's memo
        /// </summary>
        /// <param name="memoId">the id to find</param>
        /// <returns>true if it has this memo</returns>
        public bool HasMemo(string memoId)
        {
            return TaskMemos.Any(x => x.MemoId.Equals(memoId));
        }

        /// <summary>
        ///     Get all Memos
        /// </summary>
        /// <returns>a list of this task's memo</returns>
        public List<CNMemo> GetAllMemos()
        {
            return TaskMemos.Select(x => x.Memo).ToList();
        }

        /// <summary>
        ///     Defines this Task has parent Task
        /// </summary>
        /// <returns>true if it has</returns>
        public bool HasParentTask()
        {
            return ParentTask != null;
        }

        #endregion
    }
}