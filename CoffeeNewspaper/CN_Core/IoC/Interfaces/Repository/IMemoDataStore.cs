using System.Threading.Tasks;

namespace CN_Core.IoC.Interfaces.Repository
{
    public interface IMemoDataStore
    {
        /// <summary>
        /// Deep copy of this memo
        /// </summary>
        /// <param name="originMemo">origin Memo being cloned</param>
        /// <returns>cloned Memo</returns>
        Task<CNMemo> CloneAMemo(CNMemo originMemo);
    }
}
