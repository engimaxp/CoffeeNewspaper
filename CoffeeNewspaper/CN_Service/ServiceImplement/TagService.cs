using System.Collections.Generic;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Repository;
using CN_Core.Interfaces.Service;

namespace CN_Service
{
    public class TagService:ITagService
    {
        private readonly ITagDataStore _tagDataStore;

        public TagService(ITagDataStore tagDataStore)
        {
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
    }
}