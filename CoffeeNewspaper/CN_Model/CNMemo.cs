using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CN_Model
{
    public class CNMemo : IEquatable<CNMemo>,ICloneable
    {
        public int MemoId { get; set; }
        public string Content { get; set; }
        public string Tag { get; set; }
        public bool Equals(CNMemo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return MemoId == other.MemoId && string.Equals(Content, other.Content) && string.Equals(Tag, other.Tag);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as CNMemo;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MemoId;
                hashCode = (hashCode*397) ^ (Content != null ? Content.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Tag != null ? Tag.GetHashCode() : 0);
                return hashCode;
            }
        }

        public object Clone()
        {
            return new CNMemo()
            {
                Content = string.Copy(this.Content),
                MemoId = this.MemoId,
                Tag = string.Copy(this.Tag)
            };
        }
    }
}
