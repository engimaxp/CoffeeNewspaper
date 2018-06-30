using System.Threading.Tasks;
using Ninject;

namespace CN_Repository
{
    /// <summary>
    ///     Extension method to inject CNDBContext to Container
    /// </summary>
    public static class DataContextForIoC
    {
        /// <summary>
        ///     Bind the DbContext
        ///     shall do this at the beginning of app start
        /// </summary>
        /// <param name="Kernel"></param>
        /// <param name="InMemoryMode">true if used in unittest</param>
        public static async Task BindCNDBContext(this IKernel Kernel, bool InMemoryMode = false)
        {
            if (Kernel == null) return;

            // create a default dbContext object
            var dbContext = InMemoryMode ? CNDbContext.GetMemorySqlDatabase() : CNDbContext.GetFileSqlDatabase();
            if(!InMemoryMode)
                await dbContext.Database.EnsureCreatedAsync();
            // bind a default DBContext to IoC
            Kernel.Bind<CNDbContext>().ToConstant(dbContext);
        }
        
    }
}