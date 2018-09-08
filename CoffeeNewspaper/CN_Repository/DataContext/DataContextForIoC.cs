using System.Threading.Tasks;
using CN_Core;
using Ninject;
using Ninject.Extensions.NamedScope;

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
        public static void BindCNDBContext(this IKernel Kernel, bool InMemoryMode = false)
        {
            if (Kernel == null) return;
            if (InMemoryMode)
            {
                Kernel.Bind<CNDbContext>().ToConstant(CNDbContext.GetMemorySqlDatabase()).InNamedScope(CNConstants.NinjectNamedScopeForService);
            }
            else
            {
                Kernel.Bind<CNDbContext>().ToMethod(context => CNDbContext.GetFileSqlDatabase()).InNamedScope(CNConstants.NinjectNamedScopeForService);
            }
        }
        
    }
}