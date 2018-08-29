using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CN_Core.Specification;

namespace CN_Core.Interfaces.Repository
{
    public interface ITagDataStore
    {
        /// <summary>
        /// Get function by filter
        /// </summary>
        /// <param name="spec">spec contain Criteria</param>
        /// <returns></returns>
        Task<ICollection<CNTag>> GetAllTagsBySpecification(ISpecification<CNTag> spec);
    }
}