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

    public class TaskTagSpecification : BaseSpecification<CNTag>
    {
        public TaskTagSpecification(int taskid)
            : base(x => x.TaskTaggers != null && x.TaskTaggers.Any(y=>y.TaskId == taskid))
        {
        }
    }
    public class TagAndTaskSpecification : BaseSpecification<CNTag>
    {
        public TagAndTaskSpecification(int taskid,string tagStart)
    : base(b => b.Title.ToLower().Contains(tagStart.ToLower()) && b.TaskTaggers != null && b.TaskTaggers.Any(y => y.TaskId == taskid))
    {
        }
    }


}