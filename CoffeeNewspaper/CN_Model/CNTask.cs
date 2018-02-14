using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CN_Model
{
    public class CNTask : IEquatable<CNTask>
    {
        public CNTask()
        {
            _memos = new List<CNMemo>();
        }

        public bool Equals(CNTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TaskId == other.TaskId && 
                string.Equals(Content, other.Content) && 
                CreateTime.Equals(other.CreateTime) && 
                StartTime.Equals(other.StartTime) &&
                Priority == other.Priority && 
                Urgency == other.Urgency && 
                EstimatedDuration == other.EstimatedDuration &&
                EndTime.Equals(other.EndTime) &&
                (Memos == null && other.Memos == Memos) || (Memos != null && other.Memos != null && Memos.Count == other.Memos.Count && !Memos.Except(other.Memos).Any()) &&
                (Tags == null && other.Tags == Tags) || (Tags != null && other.Tags != null && Tags.Count == other.Tags.Count && !Tags.Except(other.Tags).Any()) &&
                Equals(ParentTaskId, other.ParentTaskId) &&
                (PreTaskIds == null && other.PreTaskIds == PreTaskIds) || (PreTaskIds != null && other.PreTaskIds != null && PreTaskIds.Count == other.PreTaskIds.Count && !PreTaskIds.Except(other.PreTaskIds).Any());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as CNTask;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = TaskId;
                hashCode = (hashCode*397) ^ (Content != null ? Content.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ CreateTime.GetHashCode();
                hashCode = (hashCode*397) ^ StartTime.GetHashCode();
                hashCode = (hashCode*397) ^ (int) Priority;
                hashCode = (hashCode*397) ^ (int) Urgency;
                hashCode = (hashCode*397) ^ EstimatedDuration;
                hashCode = (hashCode*397) ^ EndTime.GetHashCode();
                hashCode = (hashCode*397) ^ (Memos != null ? Memos.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Tags != null ? Tags.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ParentTaskId != null ? ParentTaskId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PreTaskIds != null ? PreTaskIds.GetHashCode() : 0);
                return hashCode;
            }
        }

        public int TaskId { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? StartTime { get; set; }
        public CNPriority Priority { get; set; }
        public CNUrgency Urgency { get; set; }
        /// <summary>
        /// 预计耗时，-1代表不知道，-2代表永远不会完成
        /// </summary>
        public int EstimatedDuration { get; set; }

        public DateTime? EndTime { get; set; }
        private List<CNMemo> _memos;
        private List<CNMemo> Memos {
            get {
                if (_memos != null && _memos.Count > 0)
                {
                    var distcount = _memos.Select(r => r.MemoId).Distinct().ToList();
                    if (distcount.Count() != _memos.Count)
                    {
                        List<CNMemo> distinctMemos = new List<CNMemo>();
                        distcount.ForEach(x => distinctMemos.Add(_memos.FirstOrDefault(y => y.MemoId == x)));
                        _memos = distinctMemos;
                    }
                }
                return _memos;
            }
            set { _memos = value; }
        }
        public List<string> Tags { get; set; }
        private int ParentTaskId { get; set; }
        private List<int> PreTaskIds { get; set; }

        public CNTask AddOrUpdateMemo(CNMemo newMemo)
        {
            if (Memos.Exists(r => r.MemoId == newMemo.MemoId))
            {
                Memos[Memos.FindIndex(x => x.MemoId == newMemo.MemoId)] = newMemo;
            }else Memos.Add(newMemo);
            return this;
        }

        public bool HasMemo()
        {
            return Memos.Any();
        }

        public List<CNMemo> GetAllMemos()
        {
            return Memos.ToList();
        }

        public void SetParentTask(CNTask parentTask)
        {
            this.ParentTaskId = parentTask.TaskId;
        }

        public bool HasParentTask()
        {
            return this.ParentTaskId != 0;
        }
    }
}
