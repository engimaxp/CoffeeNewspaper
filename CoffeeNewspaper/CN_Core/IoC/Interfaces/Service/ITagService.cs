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
    }
}