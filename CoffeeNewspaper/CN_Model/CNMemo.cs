using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CN_Model
{
    public class CNMemo : IEquatable<CNMemo>,ICloneable
    {
        public string MemoId { get; set; }

        private string _content;
        public string Content { get { if (_content == null) { _content = "memo content here"; }return _content; } set { _content = value; } }

        public override string ToString()
        {
            return $"{nameof(MemoId)}: {MemoId}, {nameof(Title)}: {Title}";
        }

        private string _title;
        public string Title { get { if (_title == null) { _title = "NewTitle"; } return _title; } set { _title = value; } }
        public string Tag { get; set; }
        public bool Equals(CNMemo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(MemoId, other.MemoId) && string.Equals(Content, other.Content) && string.Equals(Title, other.Title) && string.Equals(Tag, other.Tag);
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
                var hashCode = (MemoId != null ? MemoId.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Content != null ? Content.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Tag != null ? Tag.GetHashCode() : 0);
                return hashCode;
            }
        }

        public object Clone()
        {
            return new CNMemo()
            {
                Content = string.Copy(Content),
                Title = string.Copy(Title),
                MemoId = MemoId,
                Tag = string.Copy(Tag)
            };
        }
    }
}
