using System;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces;

namespace CN_Repository
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly CNDbContext _databaseContext;
        private bool _disposed = false;

        public UnitOfWork(CNDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> Commit()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            return await IoC.Task.Run(
                  async () => await _databaseContext.SaveChangesAsync()>0,false);

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _databaseContext?.Dispose();
            }

            _disposed = true;
        }
    }
}