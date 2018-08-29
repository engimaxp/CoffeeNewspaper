using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Repository;
using CN_Core.Specification;
using Microsoft.EntityFrameworkCore;

namespace CN_Repository
{
    public class TagDataStore: BaseDataStore, ITagDataStore
    {
        public TagDataStore(CNDbContext dbContext) : base(dbContext)
        {
        }

        #region Select Methods

        public async Task<ICollection<CNTag>> GetAllTagsBySpecification(ISpecification<CNTag> spec)
        {
            return await IoC.Task.Run(
                async () =>
                {
                    // fetch a Queryable that includes all expression-based includes
                    var queryableResultWithIncludes = spec.Includes
                        .Aggregate(mDbContext.Set<CNTag>().AsQueryable(),
                            (current, include) => current.Include(include));

                    // modify the IQueryable to include any string-based include statements
                    var secondaryResult = spec.IncludeStrings
                        .Aggregate(queryableResultWithIncludes,
                            (current, include) => current.Include(include));

                    // return the result of the query using the specification's criteria expression
                    return await secondaryResult
                        .Where(spec.Criteria).ToListAsync();
                });
        }
        
        #endregion
    }
}