using System;
using System.Threading.Tasks;

namespace CN_Core.Interfaces
{

    public interface IUnitOfWork : IDisposable
    {
        Task<bool> Commit();
    }
}