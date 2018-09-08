using System;
using System.Threading.Tasks;

namespace CN_Core.Interfaces
{

    public interface IUnitOfWork 
    {
        Task<bool> Commit();
    }
}