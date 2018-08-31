using System.Collections.Generic;
using System.Threading.Tasks;
using CN_Core;

namespace CN_Presentation.ViewModel.Input
{
    public interface IAddNewTag
    {
        /// <summary>
        /// Notify parent control a tag has been added
        /// </summary>
        /// <param name="newTag"></param>
        /// <returns></returns>
        Task NotifyAddNewTag(string newTag);

        /// <summary>
        /// Get Exists titles
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetExistsTagsTitle();
    }
}