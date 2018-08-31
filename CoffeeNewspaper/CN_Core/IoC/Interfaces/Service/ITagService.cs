using System.Collections.Generic;
using System.Threading.Tasks;

namespace CN_Core.Interfaces.Service
{
    public interface ITagService
    {

        /// <summary>
        /// Get all existing task tags from db
        /// </summary>
        /// <returns></returns>
        Task<ICollection<CNTag>> GetAllTaskTags();

        /// <summary>
        /// Get all tag starting with substring <see cref="tagSubstring"/>
        /// </summary>
        /// <param name="tagSubstring"></param>
        /// <returns></returns>
        Task<ICollection<CNTag>> GetStartStringTag(string tagSubstring);

        /// <summary>
        /// Find tag by title
        /// </summary>
        /// <param name="tagTitle"></param>
        /// <returns></returns>
        Task<CNTag> GetTagByTitle(string tagTitle);
    }
}