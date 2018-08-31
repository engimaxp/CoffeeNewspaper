using System;
using System.Linq;
using CN_Core;
using CN_Core.Specification;

namespace CN_Service
{
    public class TagSpecification : BaseSpecification<CNTag>
    {
        public TagSpecification(string tagStart)
            : base(b => b.Title.ToLower().Contains(tagStart.ToLower()))
        {
        }
    }

    public class TaskBoundedTagsSpecification : BaseSpecification<CNTag>
    {
        public TaskBoundedTagsSpecification()
            : base(x => x.TaskTaggers != null && x.TaskTaggers.Any(y => !y.Task.IsDeleted))
        {
        }
    }
    
    public class TagTitleSpecification : BaseSpecification<CNTag>
    {
        public TagTitleSpecification(string tagTitle)
            : base(x => x.Title == tagTitle)
        {
        }
    }
}