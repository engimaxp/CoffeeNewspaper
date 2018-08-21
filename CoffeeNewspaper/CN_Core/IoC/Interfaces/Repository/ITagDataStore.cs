using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CN_Core.Interfaces.Repository
{
    public interface ITagDataStore
    {
        /// <summary>
        /// Get all existing task tags from db
        /// </summary>
        /// <returns></returns>
        Task<ICollection<CNTag>> GetAllTaskTags();
    }
}