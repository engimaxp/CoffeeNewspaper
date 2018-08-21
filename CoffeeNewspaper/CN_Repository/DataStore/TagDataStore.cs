using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace CN_Repository
{
    public class TagDataStore: BaseDataStore, ITagDataStore
    {
        public TagDataStore(CNDbContext dbContext) : base(dbContext)
        {
        }

        #region Select Methods

        public async Task<ICollection<CNTag>> GetAllTaskTags()
        {
            return await IoC.Task.Run(
                async () => await mDbContext.Tags.Where(x=> x.TaskTaggers!=null && x.TaskTaggers.Any(y=>!y.Task.IsDeleted)).ToListAsync());
        }
        #endregion
    }
}