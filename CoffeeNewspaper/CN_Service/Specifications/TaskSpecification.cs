using CN_Core;
using CN_Core.Specification;

namespace CN_Service
{
    public class UndeletedCompleteTopLevelTaskSpecification: BaseSpecification<CNTask>
    {
        public UndeletedCompleteTopLevelTaskSpecification()
            : base(b => b.Status == CNTaskStatus.DONE && !b.IsDeleted && b.ParentTask== null)
        {
        }
    }
}