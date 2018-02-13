using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CN_Model
{
    public class CNTask : IEquatable<CNTask>
    {
        public bool Equals(CNTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID == other.ID && 
                string.Equals(Content, other.Content) && 
                CreateTime.Equals(other.CreateTime) && 
                StartTime.Equals(other.StartTime) &&
                Priority == other.Priority && 
                Urgency == other.Urgency && 
                EstimatedDuration == other.EstimatedDuration &&
                EndTime.Equals(other.EndTime) &&
                (Memos == null && other.Memos == Memos) || (Memos != null && other.Memos != null && Memos.Count == other.Memos.Count && !Memos.Except(other.Memos).Any()) &&
                (Tags == null && other.Tags == Tags) || (Tags != null && other.Tags != null && Tags.Count == other.Tags.Count && !Tags.Except(other.Tags).Any()) &&
                Equals(Parent, other.Parent) &&
                (PreTask == null && other.PreTask == PreTask) || (PreTask != null && other.PreTask != null && PreTask.Count == other.PreTask.Count && !PreTask.Except(other.PreTask).Any());
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
                var hashCode = ID;
                hashCode = (hashCode*397) ^ (Content != null ? Content.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ CreateTime.GetHashCode();
                hashCode = (hashCode*397) ^ StartTime.GetHashCode();
                hashCode = (hashCode*397) ^ (int) Priority;
                hashCode = (hashCode*397) ^ (int) Urgency;
                hashCode = (hashCode*397) ^ EstimatedDuration;
                hashCode = (hashCode*397) ^ EndTime.GetHashCode();
                hashCode = (hashCode*397) ^ (Memos != null ? Memos.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Tags != null ? Tags.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Parent != null ? Parent.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (PreTask != null ? PreTask.GetHashCode() : 0);
                return hashCode;
            }
        }

        public int ID { get; set; }
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
        public List<CNMemo> Memos { get; set; }
        public List<string> Tags { get; set; }
        public CNTask Parent { get; set; }
        public List<CNTask> PreTask { get; set; }
    }
}
