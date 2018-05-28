using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ninject;

namespace CN_Repository
{
    /// <summary>
    /// Extension method to inject CNDBContext to Container
    /// </summary>
    public static class DataContextForIoC
    {
        /// <summary>
        /// Bind the DbContext
        /// </summary>
        /// <param name="Kernel"></param>
        public static void BindCNDBContext(this IKernel Kernel)
        {
            if (Kernel == null) return;
            var origindbContext = Kernel.TryGet<CNDbContext>();
            // if the originDBContext exists then jump to exit
            if (origindbContext != null) return;

            // create a default dbContext object
            var options = new DbContextOptions<CNDbContext>();
            var dbContext = new CNDbContext(options);

            // bind a default DBContext to IoC
            Kernel.Bind<CNDbContext>().ToConstant(dbContext);
        }

        /// <summary>
        /// Unbind the DbContext
        /// </summary>
        /// <param name="Kernel"></param>
        public static void UnBindCNDBContext(this IKernel Kernel)
        {
            if (Kernel == null) return;
            var origindbContext = Kernel.TryGet<CNDbContext>();
            // if the originDBContext exists then jump to exit
            if (origindbContext != null) return;
            Kernel.Unbind<CNDbContext>();
        }
    }
}



