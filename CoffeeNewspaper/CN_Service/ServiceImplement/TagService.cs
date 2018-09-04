using System.Collections.Generic;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Repository;
using CN_Core.Interfaces.Service;

namespace CN_Service
{
    public class TagService : ITagService
    {
        private readonly ITagDataStore _tagDataStore;
        private readonly IUnitOfWork unitOfWork;

        public TagService(IUnitOfWork UnitOfWork, ITagDataStore tagDataStore)
        {
            unitOfWork = UnitOfWork;
            _tagDataStore = tagDataStore;
        }

        public async Task<ICollection<CNTag>> GetAllTaskTags()
        {
                return await _tagDataStore.GetAllTagsBySpecification(new TaskBoundedTagsSpecification());
        }


        public async Task<ICollection<CNTag>> GetStartStringTag(string tagSubstring)
        {
                return await _tagDataStore.GetAllTagsBySpecification(new TagSpecification(tagSubstring));
        }

        public async Task<CNTag> GetTagByTitle(string tagTitle)
        {
                return await _tagDataStore.GetFirstTagBySpecification(new TagTitleSpecification(tagTitle));
        }
        public async Task<ICollection<CNTag>> GetAllTagByTaskId(int taskid)
        {
            if(taskid<=0) return new List<CNTag>();
            return await _tagDataStore.GetAllTagsBySpecification(new TaskSpecification(taskid));
        }

        public async Task<ICollection<CNTag>> GetAllTagByTaskIdAndTagTitle(int taskid, string tagSubstring)
        {
            return await _tagDataStore.GetAllTagsBySpecification(new TagAndTaskSpecification(taskid,tagSubstring));
        }
    }
}