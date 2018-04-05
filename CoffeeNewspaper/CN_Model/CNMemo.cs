using System;
using System.Collections.Generic;
using System.Linq;

namespace CN_Model
{
    public class CNMemo : IEquatable<CNMemo>,ICloneable
    {
        private sealed class CnMemoEqualityComparer : IEqualityComparer<CNMemo>
        {
            public bool Equals(CNMemo x, CNMemo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.MemoId, y.MemoId) && string.Equals(x.Content, y.Content) && string.Equals(x.Title, y.Title) && ((x.Tags == null && y.Tags == x.Tags) ||
                    (x.Tags != null && y.Tags != null && x.Tags.Count == y.Tags.Count &&
                     !x.Tags.Except(y.Tags).Any()));
            }

            public int GetHashCode(CNMemo obj)
            {
                return obj.ToString().GetHashCode();
            }
        }

        public static IEqualityComparer<CNMemo> CnMemoComparer { get; } = new CnMemoEqualityComparer();

        public CNMemo()
        {
            Tags = new List<string>();
        }

        public string MemoId { get; set; }
        
        public string Content { get; set; }

        public override string ToString()
        {
            return $"{nameof(MemoId)}: {MemoId}, {nameof(Title)}: {Title}";
        }
        
        public string Title { get; set; }
        public List<string> Tags { get; set; }
        public bool Equals(CNMemo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(MemoId, other.MemoId) 
                   && string.Equals(Content, other.Content) 
                   && string.Equals(Title, other.Title) 
                   && ((Tags == null && other.Tags == Tags) ||
                    (Tags != null && other.Tags != null && Tags.Count == other.Tags.Count &&
                     !Tags.Except(other.Tags).Any())) ;
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
                hashCode = (hashCode*397) ^ (Tags != null ? Tags.GetHashCode() : 0);
                return hashCode;
            }
        }

        public object Clone()
        {
            string[] tagarrays = new string[Tags.Count];
            Tags.CopyTo(tagarrays);
            return new CNMemo()
            {
                Content = string.Copy(Content),
                Title = string.Copy(Title),
                MemoId = MemoId,
                Tags = tagarrays.ToList()
            };
        }
    }
}
