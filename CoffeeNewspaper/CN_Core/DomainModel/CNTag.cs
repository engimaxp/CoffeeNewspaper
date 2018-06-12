﻿using System;
using System.Collections.Generic;

namespace CN_Core
{
    public class CNTag : IEquatable<CNTag>
    {
        

        #region Constructor

        public CNTag()
        {
            
        }
        #endregion

        #region Public Properties

        /// <summary>
        ///     identity of this tag
        /// </summary>
        public string TagId { get; set; }

        /// <summary>
        ///     Title of this tag
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Task relations this tag has appended to
        /// </summary>
        public virtual ICollection<CNTaskTagger> TaskTaggers { get; set; } = new HashSet<CNTaskTagger>();

        /// <summary>
        ///     Memo relations this tag has appended to
        /// </summary>
        public virtual ICollection<CNMemoTagger> MemoTaggers { get; set; } = new HashSet<CNMemoTagger>();

        #endregion

        #region Interface Implementation

        private sealed class TitleEqualityComparer : IEqualityComparer<CNTag>
        {
            public bool Equals(CNTag x, CNTag y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                return string.Equals(x.Title, y.Title);
            }

            public int GetHashCode(CNTag obj)
            {
                return (obj.Title != null ? obj.Title.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<CNTag> TitleComparer { get; } = new TitleEqualityComparer();

        #region Equatable implementation
        
        public bool Equals(CNTag other)
        {
            return string.Equals(TagId, other?.TagId) && string.Equals(Title, other?.Title);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CNTag) obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #endregion

        #region Formatting Implement

        public override string ToString()
        {
            return $"{TagId}---{Title}";
        }

        #endregion

        #endregion
    }
}