using System;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces;

namespace CN_Repository
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly CNDbContext _databaseContext;

        public UnitOfWork(CNDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<bool> Commit()
        {
            return await IoC.Task.Run(
                  async () => await _databaseContext.SaveChangesAsync()>0,false);

        }
        
    }
}